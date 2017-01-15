/*
 * Created by SharpDevelop.
 * User: Geert
 * Date: 7/10/2014
 * Time: 19:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using Microsoft.Win32;
using MSScriptControl;
using EAWrappers = TSF.UmlToolingFramework.Wrappers.EA;
using EAAddinFramework.Utilities;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of Script.
	/// </summary>
	public class Script
	{
		static string scriptLanguageIndicator = "Language=\"";
		static string scriptNameIndicator = "Script Name=\"";
		private static int scriptHash;
		private static List<Script> allEAMaticScripts = new List<Script>();
		private static Dictionary<string,string> _includableScripts ;
		private static Dictionary<string,string> staticIncludableScripts ;
		private static List<Script> staticEAMaticScripts = new List<Script>();
		private static Dictionary<string,string> modelIncludableScripts = new Dictionary<string, string>(); 
		private static bool reloadModelIncludableScripts;
		private EAWrappers.Model model;
		private string scriptID;
		private string _code;
		public string errorMessage {get;set;}
		private ScriptControl scriptController;
		public List<ScriptFunction> functions {get;set;}
		private ScriptLanguage language;
		public string name{get;set;}
		public string groupName {get;set;}
		public string displayName 
		{
			get
			{
				return this.name + " - " + this.scriptController.Language;
			}
		}
		public bool isStatic{get;private set;}
		static Script()
		{
			loadStaticIncludableScripts();
		}
		/// <summary>
		/// A dictionary with all the includable script.
		/// The key is the complete !INC statement, the value is the code
		/// </summary>
		private static Dictionary<string,string> includableScripts 
		{
			get
			{
				if (staticIncludableScripts == null)
				{
					loadStaticIncludableScripts();
				}
				//if _includableScript is null then it has been made empty because the model scripts changed
				if (reloadModelIncludableScripts)
				{
					//start with the static includeable scripts
					_includableScripts = new Dictionary<string, string>(staticIncludableScripts);
					//then add the model scripts
					foreach (KeyValuePair<string, string> script in modelIncludableScripts) 
					{
						_includableScripts.Add(script.Key, script.Value);
					}
					//turn off flag to reload scripts
					reloadModelIncludableScripts = false;					
				}
				return _includableScripts;					
			}
			set
			{
				_includableScripts = value;
			}

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
		public Script(string scriptID,string scriptName,string groupName, string code, string language, EAWrappers.Model model):this(scriptID, scriptName, groupName, code, language, false)
		{
			this.model = model;
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
		public Script(string scriptID,string scriptName,string groupName, string code, string language, bool isStatic)
		{
			this.scriptID = scriptID;
			this.name = scriptName;
			this.groupName = groupName;
			this.functions = new List<ScriptFunction>();
			this._code = code;
			this.setLanguage(language);
			this.isStatic = isStatic;
		}
		/// <summary>
		/// reload the code into the controller to refresh the functions
		/// </summary>
		private void reloadCode()
		{
			//remove all functions
			this.functions.Clear();
			//create new scriptcontroller
			this.scriptController = new ScriptControl();
			this.scriptController.Language = this.language.name;
			this.scriptController.AddObject("Repository", model.wrappedModel);
			//Add the actual code. This must be done in a try/catch because a syntax error in the script will result in an exception from AddCode
			try
			{
				//first add the included code
				string includedCode = this.IncludeScripts(this._code);

				//then add the included code to the scriptcontroller
				this.scriptController.AddCode(includedCode);
				//set the functions
				foreach (MSScriptControl.Procedure procedure in this.scriptController.Procedures) 
				{
					functions.Add(new ScriptFunction(this, procedure));
				}
			}
			catch (Exception e)
			{
				//the addCode didn't work, probably because of a syntax error, or unsupported syntaxt in the code
				MSScriptControl.IScriptControl iscriptControl = this.scriptController as MSScriptControl.IScriptControl;
				this.errorMessage = e.Message + " ERROR : " + iscriptControl.Error.Description + " | Line of error: " + iscriptControl.Error.Line + " | Code error: " + iscriptControl.Error.Text;
				EAAddinFramework.Utilities.Logger.logError("Error in loading code for script " + this.name +": "+ this.errorMessage);
			}
		}
		/// <summary>
		/// loads all static includable scripts. These scripts are stored outside the model and can not be changed by the user
		/// </summary>
		private static void loadStaticIncludableScripts()
		{
			staticIncludableScripts = new Dictionary<string, string>();
			//local scripts
			loadLocalScripts();
			//MDG scripts in the program folder
			loadLocalMDGScripts();
			// MDG scripts in other locations
			loadOtherMDGScripts();
			//store the staticIncludeable scripts in a separate dictionary
			_includableScripts = new Dictionary<string, string>(staticIncludableScripts);

		}

		/// <summary>
		/// The local scripts are located in the "ea program files"\scripts (so usually C:\Program Files (x86)\Sparx Systems\EA\Scripts or C:\Program Files\Sparx Systems\EA\Scripts)
		/// The contents of the local scripts is loaded into the includableScripts
		/// </summary>
		private static void loadLocalScripts()
		{
			try
			{
				string scriptsDirectory = Path.GetDirectoryName(EAWrappers.Model.applicationFullPath) + "\\Scripts";
				if (Directory.Exists(scriptsDirectory))
				{
					string[] scriptFiles = Directory.GetFiles(scriptsDirectory,"*.*",SearchOption.AllDirectories);
					foreach(string scriptfile in scriptFiles)
					{
						string scriptcode = File.ReadAllText(scriptfile);
						string scriptName = Path.GetFileNameWithoutExtension(scriptfile);
						string scriptLanguage = getLanguageFromPath(scriptfile);
						staticIncludableScripts.Add("!INC Local Scripts." + scriptName, scriptcode);
						//also check if the script needs to be loaded as static EA-Matic script
						loadStaticEAMaticScript(scriptName, "Local Scripts", scriptcode,scriptLanguage);
					}
				}
			}
			catch (Exception e)
			{
				Logger.logError(string.Format("Error occured: {0} stacktrace: {1}",e.Message, e.StackTrace));
			}
			
		}
		private static string getLanguageFromPath (string path)
		{
			string extension = Path.GetExtension(path);
			string foundLanguage = "VBScript";
			if (extension.Equals("vbs",StringComparison.InvariantCultureIgnoreCase))
		    {
		    	foundLanguage =  "VBScript";
			} 
			else if (extension.Equals("js",StringComparison.InvariantCultureIgnoreCase))
			{
				if (path.Contains("Jscript"))
				{
					foundLanguage =  "Jscript";
				}
				else
				{
					foundLanguage =  "JavaScript";
				}
			}
			return foundLanguage;
		}
			
		/// <summary>
		/// loads the mdg scripts from the locations added from MDG Technologies|Advanced. 
		/// these locations are stored as a comma separated string in the registry
		/// a location can either be a directory, or an url
		/// </summary>
		private static void loadOtherMDGScripts()
		{
			//read the registry key to find the locations
			string pathList = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Sparx Systems\EA400\EA\OPTIONS", "MDGTechnology PathList", null) as string;    
			if (pathList != null)
			{
				string[] mdgPaths = pathList.Split(',');
				foreach (string mdgPath in mdgPaths) 
				{
					//figure out it we have a folderpath or an url
					if (mdgPath.StartsWith("http",StringComparison.InvariantCultureIgnoreCase))
				    {
						//url
						loadMDGScriptsFromURL(mdgPath);
				    }
					else
					{
						//directory
						loadMDGScriptsFromFolder(mdgPath);
					}
				}
			}
	
		}
		/// <summary>
		/// load the mdg scripts from the mdg file located at the given url
		/// </summary>
		/// <param name="url">the url pointing to the mdg file</param>
		private static void loadMDGScriptsFromURL(string url)
		{
			try
			{
				loadMDGScripts(new WebClient().DownloadString(url));
			}
			catch (Exception e)
			{
				EAAddinFramework.Utilities.Logger.logError("Error in loadMDGScriptsFromURL: " + e.Message);
			}
		}
		/// <summary>
		/// get the mdg files in the local MDGtechnologies folder
		/// </summary>
		private static void loadLocalMDGScripts()
		{
			string mdgDirectory = Path.GetDirectoryName(EAWrappers.Model.applicationFullPath) + "\\MDGTechnologies";
			loadMDGScriptsFromFolder(mdgDirectory);
		}
		/// <summary>
		/// load the scripts from the mdg files in the given directory
		/// </summary>
		/// <param name="folderPath">the path to the directory</param>
		private static void loadMDGScriptsFromFolder(string folderPath)
		{
			try
			{
				string[] mdgFiles = Directory.GetFiles(folderPath,"*.xml",SearchOption.TopDirectoryOnly);
				foreach(string mdgfile in mdgFiles)
				{
					loadMDGScripts(File.ReadAllText(mdgfile));
				}
			}
			catch (Exception e)
			{
				EAAddinFramework.Utilities.Logger.logError("Error in loadMDGScriptsFromFolder: " + e.Message);
			}
		}
		/// <summary>
		/// loads the scripts described in the MDG file into the includable scripts
		/// </summary>
		/// <param name="mdgXmlContent">the string content of the mdg file</param>
		private static void loadMDGScripts(string mdgXmlContent)
		{
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(mdgXmlContent);
				//first get the name of the MDG
				XmlElement documentationElement = xmlDoc.SelectSingleNode("//Documentation") as XmlElement;
				if (documentationElement != null)
				{
					string mdgName = documentationElement.GetAttribute("id");
					//then get the scripts
					XmlNodeList scriptNodes = xmlDoc.SelectNodes("//Script");
					foreach (XmlNode scriptNode in scriptNodes) 
					{
						XmlElement scriptElement = (XmlElement)scriptNode;
						//get the name of the script
						string scriptName = scriptElement.GetAttribute("name");
						//get the language of the script
						string scriptLanguage = scriptElement.GetAttribute("language");
						//get the script itself
						XmlNode contentNode = scriptElement.SelectSingleNode("Content");
						if (contentNode != null)
						{
							//the script itstelf is base64 endcoded in the content tag
							string scriptcontent = System.Text.Encoding.Unicode.GetString( System.Convert.FromBase64String(contentNode.InnerText));
							staticIncludableScripts.Add("!INC "+ mdgName + "." + scriptName,scriptcontent);
							//also check if the script needs to be loaded as static EA-Matic script
							loadStaticEAMaticScript(scriptName, mdgName, scriptcontent,scriptLanguage);
						}
					}
				}
			}
			catch (Exception e)
			{
				EAAddinFramework.Utilities.Logger.logError("Error in loadMDGScripts: " + e.Message);
			}
		}
		private static void loadStaticEAMaticScript(string scriptName, string groupName, string scriptCode, string language)
		{
			if (scriptCode.Contains("EA-Matic"))
			{
				Script script = new Script(groupName + "." + scriptName,scriptName,groupName,scriptCode, language,true);
				staticEAMaticScripts.Add(script);
			}
		}
		/// <summary>
		/// replaces the !INC  statements with the actual code of the local script.
		/// The local scripts are located in the "ea program files"\scripts (so usually C:\Program Files (x86)\Sparx Systems\EA\Scripts or C:\Program Files\Sparx Systems\EA\Scripts)
		/// 
		/// </summary>
		/// <param name="code">the code containing the include parameters</param>
		/// <returns>the code including the included code</returns>
		private string IncludeScripts(string code,List<string> includedScripts = null)
		{
			string includedCode = code;
			if (includedScripts == null)
			{
				includedScripts = new List<string>();
				
			}
			//find all lines starting with !INC
			foreach (string includeString in this.getIncludes(code)) 
			{
				if (!includedScripts.Contains(includeString)) //prevent including code twice
				{
					includedScripts.Add(includeString);
					//then replace with the contents of the included script
					includedCode = includedCode.Replace(includeString + Environment.NewLine,this.IncludeScripts(this.getIncludedcode(includeString),includedScripts));
				}
				else
				{
					//remove the included string because the script was already included
					includedCode = includedCode.Replace(includeString + Environment.NewLine,string.Empty);
				}
			}
			
			return includedCode;
			
		}
		/// <summary>
		/// gets the code to be included based on the include string. !INC statements
		/// </summary>
		/// <param name="includeString">the include statement</param>
		/// <returns>the code to be included</returns>
		private string getIncludedcode(string includeString)
		{
			string includedCode = string.Empty;
			if (includableScripts.ContainsKey(includeString))
			{
				includedCode = includableScripts[includeString];
			}
			return includedCode;
		}
		/// <summary>
		/// finds each instance of "!INC" and returns the whole line
		/// </summary>
		/// <param name="code">the code to search</param>
		/// <returns>the contents of each line starting with "!INC"</returns>
		private List<string> getIncludes(string code)
		{
			List<string> includes = new List<string>();
			using (StringReader reader = new StringReader(code))
			{
				string line;
			    while ((line = reader.ReadLine()) != null)
			    {
			    	if (line.StartsWith("!INC"))
		    	    {
			    		includes.Add(line);
		    	    }
			    }
			}
			return includes;
		}
		
		private void setLanguage(string language)
		{
			switch (language) {
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
		/// <summary>
		/// gets all scripts defined in the model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static List<Script> getEAMaticScripts(EAWrappers.Model model)
		{			
			if (model != null)
			{
			 XmlDocument xmlScripts = model.SQLQuery(@"select s.ScriptID, s.Notes, s.Script,ps.Script as SCRIPTGROUP, ps.Notes as GROUPNOTES from t_script s
													   inner join t_script ps on s.ScriptAuthor = ps.ScriptName
														where s.Script like '%EA-Matic%'");
			 //check the hash before continuing
			 int newHash = xmlScripts.InnerXml.GetHashCode();
			 //only create the scripts of the hash is different
			 //otherwise we returned the cached scripts
			 if (newHash != scriptHash)
			 {
			  //set the new hashcode
			  scriptHash = newHash;
			  //reset scripts
		 	  allEAMaticScripts = new List<Script>();
		 	  
		 	  //set flag to reload scripts in includableScripts
		 	  reloadModelIncludableScripts = true;
		 	  modelIncludableScripts = new Dictionary<string, string>();
		 	  
		 	  XmlNodeList scriptNodes = xmlScripts.SelectNodes("//Row");
              foreach (XmlNode scriptNode in scriptNodes)
              {
              	//get the <notes> node. If it countaints "Group Type=" then it is a group. Else we need to find "Language=" 
              	XmlNode notesNode = scriptNode.SelectSingleNode(model.formatXPath("Notes"));
              	if (notesNode.InnerText.Contains(scriptLanguageIndicator))
          	    {
          	    	//we have an actual script.
          	    	//the name of the script
          	    	string scriptName = getValueByName(notesNode.InnerText, scriptNameIndicator);
					//now figure out the language
					string language = getValueByName(notesNode.InnerText, scriptLanguageIndicator);
					//get the ID
					XmlNode IDNode = scriptNode.SelectSingleNode(model.formatXPath("ScriptID"));
					string ScriptID = IDNode.InnerText;
					//get the group
					XmlNode groupNode = scriptNode.SelectSingleNode(model.formatXPath("SCRIPTGROUP"));
					string groupName = groupNode.InnerText;
					//then get teh code
					XmlNode codeNode = scriptNode.SelectSingleNode(model.formatXPath("Script"));	
					if (codeNode != null && language != string.Empty)
					{
						//if the script is still empty EA returns NULL
						string scriptCode = codeNode.InnerText;
						if (scriptCode.Equals("NULL",StringComparison.InvariantCultureIgnoreCase))
						{
							scriptCode = string.Empty;
						}
						//and create the script if both code and language are found
						Script script = new Script(ScriptID,scriptName,groupName,scriptCode, language,model); 
						allEAMaticScripts.Add(script);
						//also add the script to the include dictionary
						modelIncludableScripts.Add("!INC "+ script.groupName + "." + script.name,script._code);
					}
          	    }
              }
              //Add the static EA-Matic scripts to allEAMaticScripts
              foreach (Script staticScript in staticEAMaticScripts)
              {
              	//add the model to the static script first
              	staticScript.model = model;
              	//then add the static scrip to all scripts
              	allEAMaticScripts.Add(staticScript);
			  }              
			  //load the code of the scripts (because a script can include another script we can only load the code after all scripts have been created)
			  foreach (Script script in allEAMaticScripts) 
			  {
			  	script.reloadCode();
			  }			  
			 }
			}
			return allEAMaticScripts;
		}
		/// <summary>
		/// gets the value from the content of the notes.
		/// The value can be found after "name="
		/// </summary>
		/// <param name="notesContent">the contents of the notes node</param>
		/// <param name="name">the name of the tag</param>
		/// <returns>the value string</returns>
		private static string getValueByName(string notesContent,string name)
		{
			string returnValue = string.Empty;
			if (notesContent.Contains(name))
		    {
		    	int startName = notesContent.IndexOf(name) + name.Length;
		    	int endName = notesContent.IndexOf("\"",startName);
		    	if (endName > startName)
		    	{
		    		returnValue = notesContent.Substring(startName, endName - startName);
		    	}
		    	
		    }
			return returnValue;
			
		}
		/// <summary>
		/// executes the function with the given name
		/// </summary>
		/// <param name="functionName">name of the function to execute</param>
		/// <param name="parameters">the parameters needed by this function</param>
		/// <returns>whathever (if anything) the function returns</returns>
		internal object executeFunction(string functionName, object[] parameters)
		{
			return this.scriptController.Run(functionName, ref parameters);
		}
		/// <summary>
		/// executes the function with the given name
		/// </summary>
		/// <param name="functionName">name of the function to execute</param>
		/// <returns>whathever (if anything) the function returns</returns>
		internal object executeFunction(string functionName)
		{
      object[] x = new object[0];
			return this.scriptController.Run(functionName, ref x);
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
			this._code += functionCode;
			string SQLUpdate = "update t_script set script = '"+ this.model.escapeSQLString(this._code) +"' where ScriptID = " + this.scriptID ;
			this.model.executeSQL(SQLUpdate);
		}
	}
}
