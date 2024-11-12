using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using EAAddinFramework.Utilities;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.EASpecific
{
    public class ModelScriptRepository : DefaultScriptRepsitory
    {
        private Dictionary<string, string> lastSavedScripts = new Dictionary<string, string>();

        private static string baseSqlGetScripts = @"select s.ScriptID, s.Notes, s.Script,ps.Script as SCRIPTGROUP, ps.Notes as GROUPNOTES 
                                                        from t_script s
                                                        inner join t_script ps on s.ScriptAuthor = ps.ScriptName
                                                        where s.ScriptAuthor <> 'ScriptDebugging'";

        private Model model;

        public bool onlyLoadEAMaticScripts { get; set; }

        public ModelScriptRepository(ScriptRepository scriptRepository, Model model, bool onlyLoadEAMaticScripts) : base(scriptRepository)
        {
            this.model = model;
            this.onlyLoadEAMaticScripts = onlyLoadEAMaticScripts;
        }

        public override void saveScripts(string scriptPath)
        {
            foreach (Script script in getAllScripts())
            {
                var scriptFullPath = script.getFullPath(scriptPath);
                if (lastSavedScripts.ContainsKey(scriptFullPath))
                {
                    var lastSavedScript = lastSavedScripts[scriptFullPath];
                    var currentCode = script.code;
                    if (lastSavedScript != currentCode)
                    {
                        script.save(scriptPath);
                        lastSavedScripts[scriptFullPath] = currentCode;
                    }
                }
                else
                {
                    script.save(scriptPath);
                    lastSavedScripts.Add(scriptFullPath, script.code);
                }
            }
        }

        /// <summary>
        /// Load all the scripts from the Model.
        /// 
        /// If onlyEAMatic is true then only the EA-Matic scripts will be loaded from the Model.
        /// </summary>
        /// <param name="onlyEAMatic">true to only load EA-Matic scripts, false to load all scripts</param>
        /// <returns>List of scripts from the model</returns>
        public void loadScriptsFromModel()
        {
            if (model == null)
            {
                return;
            }

            string sqlGetScripts = baseSqlGetScripts;
            if (onlyLoadEAMaticScripts)
            {
                //limit scripts to EA-Matic scripts
                sqlGetScripts += Environment.NewLine + " and s.Script like '%EA-Matic%'";
            }
            List<Script> scriptsFromModel = loadScriptBySQL(sqlGetScripts);
            Logger.log($"Model Scripts Loaded: {(scriptsFromModel.Any() ? string.Join(", ", scriptsFromModel) : "<Empty>")}");
            loadIncludeDependenciesFor(scriptsFromModel);
        }

        /// <summary>
        /// Load the scripts from the model found by the provided sql.
        /// Also adds them to the reposiroty via addScript().
        /// </summary>
        /// <param name="sqlGetScripts">SQL used to query the model</param>
        /// <returns>list of Scripts loaded</returns>
        private List<Script> loadScriptBySQL(string sqlGetScripts)
        {
            XmlDocument xmlScripts = model.SQLQuery(sqlGetScripts);
            XmlNodeList scriptNodes;

            scriptNodes = xmlScripts.SelectNodes("//Row");

            List<Script> scriptsFromModel = new List<Script>();
            foreach (XmlNode scriptNode in scriptNodes)
            {
                //get the <notes> node. If it contains "Group Type=" then it is a group. Else we need to find "Language=" 
                XmlNode notesNode = scriptNode.SelectSingleNode(model.formatXPath("Notes"));
                if (notesNode.InnerText.Contains(Script.scriptLanguageIndicator))
                {
                    Script script = createScriptFromNotes(scriptNode, notesNode);
                    addScript(script);
                    scriptsFromModel.Add(script);
                }
            }

            return scriptsFromModel;
        }

        /// <summary>
        /// For the given list of scripts, find the !INC include dependencies and load these scripts from the repostiory if they do not already exist.
        /// 
        /// As includes may refernce scripts in other repositories not every include will be resolvable by this repository.
        /// </summary>
        /// <param name="scripts">list of script that needs their dependencies loaded</param>
        private void loadIncludeDependenciesFor(List<Script> scripts)
        {
            HashSet<string> allIncludeStatements = new HashSet<string>();
            // Collate unique set of include statements from all provided scripts
            foreach (Script script in scripts)
            {
                List<string> includeStatmentsFromScript = script.getINCStatements();
                allIncludeStatements.UnionWith(includeStatmentsFromScript);
            }

            // build the SQL
            string sqlGetScripts = baseSqlGetScripts;
            sqlGetScripts += Environment.NewLine + "and (";
            List<string> whereGroupAndNameMatches = new List<string>();
            foreach (string includeStatement in allIncludeStatements)
            {
                // getINCStatements() has already parsed the INC statements properly now just needs the split parts
                string[] groupAndScript = Script.parseGroupAndNameFromINC(includeStatement, "not used", "not used");
                if (groupAndScript.Length == 2)
                {
                    string group = groupAndScript[0];
                    string name = groupAndScript[1];
                    Script scriptInRepository = getScriptByGroupAndName(group, name);
                    if (scriptInRepository == null)
                    {
                        // Group = ps.Script
                        // Name = s.Notes like '%name="<SCRIPT>"%'
                        whereGroupAndNameMatches.Add($"(ps.Script = '{group}' and s.Notes like '%name=\"{name}\"%')");
                    }
                }
            }
            sqlGetScripts += Environment.NewLine + "      " + String.Join(Environment.NewLine + "   or ", whereGroupAndNameMatches);
            sqlGetScripts += Environment.NewLine + ")";

            // Load all the scripts not already in the repository
            // If all the scripts are already in the repository the whereGroupAndNameMatches will be empty
            if (whereGroupAndNameMatches.Count > 0)
            {
                Logger.logDebug($"sqlGetScripts={sqlGetScripts}");
                List<Script> newScripts = loadScriptBySQL(sqlGetScripts);
                loadIncludeDependenciesFor(newScripts);
            }
        }

        private Script createScriptFromNotes(XmlNode scriptNode, XmlNode notesNode)
        {
            Script script = null;

            //we have an actual script.
            //the name of the script
            string scriptName = getValueByName(notesNode.InnerText, Script.scriptNameIndicator);
            //now figure out the language
            string language = getValueByName(notesNode.InnerText, Script.scriptLanguageIndicator);
            //get the ID
            XmlNode idNode = scriptNode.SelectSingleNode(model.formatXPath("ScriptID"));
            string scriptID = idNode.InnerText;
            //get the group
            XmlNode groupNode = scriptNode.SelectSingleNode(model.formatXPath("SCRIPTGROUP"));
            string groupName = groupNode.InnerText;
            //then get teh code
            XmlNode codeNode = scriptNode.SelectSingleNode(model.formatXPath("Script"));
            if (codeNode != null && language != string.Empty)
            {
                //if the script is still empty EA returns NULL
                string scriptCode = codeNode.InnerText;
                if (scriptCode.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
                {
                    scriptCode = string.Empty;
                }
                //and create the script if both code and language are found
                script = new Script(scriptRepository, scriptID, scriptName, groupName, scriptCode, language, model);
            }
            return script;
        }

        /// <summary>
        /// gets the value from the content of the notes.
        /// The value can be found after "name="
        /// </summary>
        /// <param name="notesContent">the contents of the notes node</param>
        /// <param name="name">the name of the tag</param>
        /// <returns>the value string</returns>
        private string getValueByName(string notesContent, string name)
        {
            string returnValue = string.Empty;
            if (notesContent.Contains(name))
            {
                int startName = notesContent.IndexOf(name) + name.Length;
                int endName = notesContent.IndexOf("\"", startName);
                if (endName > startName)
                {
                    returnValue = notesContent.Substring(startName, endName - startName);
                }

            }
            return returnValue;

        }

        public override void loadScripts()
        {
            loadScriptsFromModel();
        }

        public override void resetScripts()
        {
            clearCache();
            loadScripts();
            // Only EA-Matic scripts need to be reloaded as they are the only ones whos code is run.
            foreach(Script script in getEAMaticScripts())
            {
                script.reloadCode();
            }
        }
    }
}
