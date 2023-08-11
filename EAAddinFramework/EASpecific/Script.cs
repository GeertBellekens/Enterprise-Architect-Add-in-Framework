/*
 * Created by SharpDevelop.
 * User: Geert
 * Date: 7/10/2014
 * Time: 19:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EAAddinFramework.Utilities;
using System.Text.RegularExpressions;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Windows.Forms;

namespace EAAddinFramework.EASpecific
{
    /// <summary>
    /// Description of Script.
    /// </summary>
    public class Script
    {
        public static string scriptLanguageIndicator = "Language=\"";
        public static string scriptNameIndicator = "Script Name=\"";
        public static string eaMaticScriptIndicator = "EA-Matic";

        private readonly Model model;
        private readonly ScriptRepository scriptRepository;
        private readonly string scriptID;
        public string code { get; private set; }
        private string _saveCode;
        private string saveCode
        {
            get
            {
                if (String.IsNullOrEmpty(this._saveCode))
                {
                    this._saveCode = this.code;
                }
                return this._saveCode;
            }
            set => this._saveCode = value;
        }
        public string scriptkey { get => $"!INC {this.fullyQualifiedName}"; }
        public string errorMessage { get; set; }
        private ScriptCtrl scriptController;
        public List<ScriptFunction> functions { get; set; }
        public List<ScriptFunction> addinFunctions => this.functions.Where(x => x.isAddinFunction).ToList();
        private ScriptLanguage language;
        public string fullyQualifiedName
        {
            get => $"{this.groupName}.{this.name}";
        }
        public string name { get; set; }
        public string groupName { get => this.group.name; }
        private ScriptGroup _group;
        public ScriptGroup group
        {
            get => this._group;
            set
            {
                // TODO: Check that changing a scripts group doesn't leave a reference in the old ScriptGroup
                this._group = value;
                this._group.addScript(this);
            }
        }
        public string displayName
        {
            get
            {
                return this.name + " - " + this.scriptController.Language;
            }
        }
        public bool isStatic { get; private set; }

        public bool isEAMaticScript
        {
            get => code.Contains(Script.eaMaticScriptIndicator);

        }

        public override string ToString()
        {
            return this.fullyQualifiedName;
        }

        /// <summary>
        /// creaates a new script
        /// </summary>
        /// <param name="scriptID">the id of the script</param>
        /// <param name="scriptName">the name of the script</param>
        /// <param name="groupName">the name of the scriptgroup</param>
        /// <param name="code">the code</param>
        /// <param name="language">the language the code is written in</param>
        /// <param name="model">the model this script resides in</param>
        public Script(ScriptRepository scriptRepository, string scriptID, string scriptName, string groupName, string code, string language, Model model, bool isStatic = false)
        {
            this.scriptRepository = scriptRepository;
            this.model = model;
            this.scriptID = scriptID;
            this.name = scriptName;
            this.group = ScriptGroup.getScriptGroup(groupName, this.model);
            this.functions = new List<ScriptFunction>();
            this.code = code;
            this.setLanguage(language);
            this.isStatic = isStatic;
        }
        /// <summary>
        /// reload the code into the controller to refresh the functions
        /// </summary>
        public void reloadCode()
        {
            //remove all functions
            this.functions.Clear();
            //create new scriptcontroller
            if (Environment.Is64BitProcess)
            {
                this.scriptController = new ScriptCtrl64();
            }
            else
            {
                this.scriptController = new ScriptCtrl32();
            }

            this.scriptController.Language = this.language.name;
            this.scriptController.AddObject("Repository", model.wrappedModel);
            //Add the actual code. This must be done in a try/catch because a syntax error in the script will result in an exception from AddCode
            try
            {
                //first add the included code
                string processedCode = this.InlineIncludeStatements();
                //remove "as xxx" statements
                processedCode = removeAsTypeStatements(processedCode);
                processedCode = replaceSessionOuputWithRepositoryWriteOutput(processedCode);
                Logger.logDebug($"script={this.fullyQualifiedName}\n" + processedCode);

                //then add the included code to the scriptcontroller
                this.scriptController.AddCode(processedCode);
                //set the functions
                this.functions = this.scriptController.GetScriptFunctions(this);
            }
            catch (Exception e)
            {
                //the addCode didn't work, probably because of a syntax error, or unsupported syntax in the code
                this.errorMessage = e.Message;
                if (this.scriptController.Error != null && !String.IsNullOrEmpty(this.scriptController.Error.Description))
                {
                    this.errorMessage += " ERROR : " + this.scriptController.Error.Description + " | Line of error: " + this.scriptController.Error.Line + " | Code error: " + this.scriptController.Error.Text;
                }
                var errorMessageToLog = $"Error in loading code for script '{this.fullyQualifiedName}': {this.errorMessage}";
                Logger.logError(errorMessageToLog);
                EAOutputLogger.log(this.model, "EA-Matic", errorMessageToLog, 0, LogTypeEnum.error);
                MessageBox.Show(errorMessageToLog, "EA-Matic Failed to Load Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal static string getLanguageFromPath(string path)
        {
            string extension = Path.GetExtension(path);
            string foundLanguage = "VBScript";
            if (extension.Equals("vbs", StringComparison.InvariantCultureIgnoreCase))
            {
                foundLanguage = "VBScript";
            }
            else if (extension.Equals("js", StringComparison.InvariantCultureIgnoreCase))
            {
                if (path.Contains("Jscript"))
                {
                    foundLanguage = "Jscript";
                }
                else
                {
                    foundLanguage = "JavaScript";
                }
            }
            return foundLanguage;
        }

        /// <summary>
        /// remove any "as XXX" from the scriptcode as in "dim element as EA.Element" as this won't compile
        /// </summary>
        /// <param name="scriptCode">the code of the script</param>
        /// <returns>the fixed code</returns>
        private static string removeAsTypeStatements(string scriptCode)
        {
            var typeRegex = new Regex(@"(?<=dim|var)( .+)( as EA\..+)", RegexOptions.IgnoreCase);
            return typeRegex.Replace(scriptCode, "$1");
        }

        /// <summary>
        /// replace Session.output, handling balanced parenthesis and line continutations, with Repository.WriteOutput
        /// </summary>
        /// <param name="scriptCode">the code of the script</param>
        /// <returns>the fixed code</returns>
        private static string replaceSessionOuputWithRepositoryWriteOutput(string scriptCode)
        {
            Regex systemOutputRegex = new Regex(@"(?<indentation>[ \t]+)(?<sessionoutput>Session\.output)(?<open>\()?(?<parameters>(?:_\r?\n|[^\r\n])*)(?<-open>\))?(?(open)(?!))", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return systemOutputRegex.Replace(scriptCode, "${indentation}Repository.WriteOutput \"Script\", ${parameters}, 0");
        }

        /// <summary>
        /// replaces the !INC statements with the actual code
        /// </summary>
        /// <param name="includedScriptsAsINCstatements">a list of !INC statements already processed so duplicates are not inlined</param>
        /// <returns>the code with all !INC statements inlined</returns>
        /// <exception cref="ArgumentOutOfRangeException">when a script matching the !INC statement can not be found in the script repository</exception>
        private string InlineIncludeStatements(List<string> includedScriptsAsINCstatements = null)
        {
            string processedCode = $"'/////////////////////////////" + Environment.NewLine 
                + $"'/// BEGIN: {this.fullyQualifiedName}" + Environment.NewLine
                + $"'/////////////////////////////" + Environment.NewLine 
                + this.code + Environment.NewLine
                + $"'/////////////////////////////" + Environment.NewLine
                + $"'/// END: {this.fullyQualifiedName}" + Environment.NewLine
                + $"'/////////////////////////////" + Environment.NewLine;
            if (includedScriptsAsINCstatements == null)
            {
                includedScriptsAsINCstatements = new List<string>();
            }
            //find all lines starting with !INC
            foreach (string incStatement in this.getINCStatements())
            {
                if (!includedScriptsAsINCstatements.Contains(incStatement)) //prevent including code twice
                {
                    includedScriptsAsINCstatements.Add(incStatement);
                    //then replace with the contents of the included script
                    string[] groupAndName = parseGroupAndNameFromINC(incStatement);
                    string group = groupAndName[0];
                    string name = groupAndName[1];
                    try
                    {
                        Script includedScript = scriptRepository.getScriptByGroupAndName(group, name);
                        if (includedScript == null)
                        {
                            throw new IncludeNotFoundException(incStatement);
                        }
                        string codeToBeIncluded = includedScript.InlineIncludeStatements(includedScriptsAsINCstatements) + Environment.NewLine;
                        processedCode = processedCode.Replace(incStatement + Environment.NewLine, codeToBeIncluded);
                        processedCode = processedCode.Replace(incStatement + "\n", codeToBeIncluded); //for some reason, since v16, the code seems to be using \n instead of \r\n as newlines. So try a second time to make sure.
                    }
                    catch (IncludeNotFoundException causedBy)
                    {
                        throw new IncludeNotFoundException(this.fullyQualifiedName, causedBy);
                    }
                }
                else
                {
                    //remove the included string because the script was already included
                    processedCode = processedCode.Replace(incStatement + Environment.NewLine, Environment.NewLine);
                    processedCode = processedCode.Replace(incStatement + "\n", Environment.NewLine);
                }
            }

            return processedCode;

        }
        
        /// <summary>
        /// From the scripts code, return a list of the !INC statements
        /// </summary>
        /// <returns>the code's !INC statements</returns>
        public List<string> getINCStatements()
        {
            List<string> includes = new List<string>();
            using (StringReader reader = new StringReader(code))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("!INC"))
                    {
                        // used solely for side-affect of throwing exception if malformed
                        parseGroupAndNameFromINC(line);
                        includes.Add(line);
                    }
                }
            }
            return includes;
        }

        private void setLanguage(string language)
        {
            switch (language)
            {
                case "VBScript":
                    this.language = new VBScriptLanguage();
                    break;
                case "JScript":
                    this.language = new JScriptLanguage();
                    break;
                case "JavaScript":
                    this.language = new JavaScriptLanguage();
                    break;
            }
        }

        internal void save(string scriptPath)
        {
            //add groupName to savecode
            addGroupNameToSaveCode();
            var path = getPath();
            var fullPath = scriptPath + path + this.name + this.language.extension;
            //check if the script has changed 
            if (File.Exists(fullPath))
            {
                var existingCode = File.ReadAllText(fullPath);
                if (!existingCode.Equals(this.saveCode))
                {
                    //write to file
                    File.WriteAllText(fullPath, this.saveCode);
                }
            }
            else
            {
                //make sure the directory exists
                Directory.CreateDirectory(scriptPath + path);
                //write to file
                File.WriteAllText(fullPath, this.saveCode);
            }
        }
        private string getPath()
        {
            var regex = new Regex(@"(\[path=)(.+)\]", RegexOptions.IgnoreCase);
            var match = regex.Match(this.saveCode);
            if (match.Success)
            {
                return @"\" + match.Groups[2].Value + @"\"; ;
            }
            else
            {
                //return the groupname as path
                return @"\" + this.groupName + @"\";
            }
        }
        private void addGroupNameToSaveCode()
        {
            var regex = new Regex(@"(\[group=)(.+)\]", RegexOptions.IgnoreCase);
            var match = regex.Match(this.saveCode);
            if (!match.Success)
            {
                //add the group indicator to the saveCode
                this._saveCode = $"{this.language.commentLine}[group={this.groupName}]{Environment.NewLine}{this.saveCode}";
            }
        }

        /// <summary>
        /// Given an include string of the form "!INC group.name" parse this and return [group, name]
        /// </summary>
        /// <param name="include">the include string of the form "!INC group.name"</param>
        /// <returns>a string array [group, name]</returns>
        /// <exception cref="InvalidOperationException">thrown if the include is malformed</exception>
        public string[] parseGroupAndNameFromINC(string include)
        {
            return parseGroupAndNameFromINC(include, this.groupName, this.name);
        }

        /// <summary>
        /// Given an include string of the form "!INC group.name" parse this and return [group, name]
        /// </summary>
        /// <param name="include">the include string of the form "!INC group.name"</param>
        /// <param name="scriptGroupName">the groupName of the script the include is from</param>
        /// <param name="scriptName">the name of the script the include is from</param>
        /// <returns>a string array [group, name]</returns>
        /// <exception cref="InvalidOperationException">thrown if the include is malformed</exception>
        public static string[] parseGroupAndNameFromINC(string include, string scriptGroupName, string scriptName)
        {
            // Validate !INC is of format <group>.<script>
            // throw away leading !INC leaving just the <group>.<script>
            string groupAndNameAsString = include.Substring("!INC ".Length);
            string[] groupAndName = groupAndNameAsString.Split('.');
            if (groupAndName.Length != 2)
            {
                throw new InvalidOperationException($"{scriptGroupName}.{scriptName} contains malformed !INC, must be <group>.<script> but was instead: '{include}'");
            }
            return groupAndName;
        }

        public static void exportScripts(Model model, string filename)
        {
            model.wrappedModel.GetProjectInterface().ExportReferenceData(filename, "Automation Scripts");
        }

        public static bool importScripts(string refdataFile, string connectionString)
        {
            var success = false;
            var targetModel = new global::EA.Repository();
            if (targetModel.OpenFile(connectionString))
            {
                success = targetModel.GetProjectInterface().ImportReferenceData(refdataFile, "Automation Scripts");
                targetModel.CloseFile();
                targetModel.Exit();
            }
            return success;
        }

        /// <summary>
        /// executes the function with the given name
        /// </summary>
        /// <param name="functionName">name of the function to execute</param>
        /// <param name="parameters">the parameters needed by this function</param>
        /// <returns>whathever (if anything) the function returns</returns>
        internal object executeFunction(string functionName, object[] parameters)
        {
            return this.scriptController.Run(functionName, parameters);
        }
        /// <summary>
        /// executes the function with the given name
        /// </summary>
        /// <param name="functionName">name of the function to execute</param>
        /// <returns>whathever (if anything) the function returns</returns>
        internal object executeFunction(string functionName)
        {
            return this.scriptController.Run(functionName, new object[0]);
        }
        /// <summary>
        /// add a function with based on the given operation
        /// </summary>
        /// <param name="operation">the operation to base this function on</param>
        /// <returns>the new function</returns>
        public ScriptFunction addFunction(MethodInfo operation)
        {
            //translate the methodeinfo into code
            string functionCode = this.language.translate(operation);
            //add the code to the script
            this.addCode(functionCode);
            //reload the script code
            this.reloadCode();
            //return the new function
            return functions.Find(x => x.name == operation.Name);
        }

        /// <summary>
        /// adds the given code to the end of the script
        /// </summary>
        /// <param name="functionCode">the code to be added</param>
        public void addCode(string functionCode)
        {
            this.code += functionCode;
            string SQLUpdate = "update t_script set script = '" + this.model.escapeSQLString(this.code) + "' where ScriptID = " + this.scriptID;
            this.model.executeSQL(SQLUpdate);
        }
    }
}
