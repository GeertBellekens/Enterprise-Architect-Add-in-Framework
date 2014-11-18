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
using System.Xml;
using System.Linq;

using MSScriptControl;
using EAWrappers = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of Script.
	/// </summary>
	public class Script
	{
		static string scriptLanguageIndicator = "Language=\"";
		static string scriptNameIndicator = "Script Name=\"";
		private string _code;
		public string errorMessage {get;set;}
		private ScriptControl scriptController;
		public List<ScriptFunction> functions {get;set;}
		public string name{get;set;}
		public string displayName 
		{
			get
			{
				return this.name + " - " + this.scriptController.Language;
			}
		}
		public Script(string scriptName, string code, string language, EAWrappers.Model model)
		{
			this.name = scriptName;
			this.functions = new List<ScriptFunction>();
			this._code = code;
			this.scriptController = new ScriptControl();
			this.scriptController.Language = language;
			this.scriptController.AddObject("Repository", model.getWrappedModel());
			//Add the actual code. This must be done in a try/catch because a syntax error in the script will result in an exception from AddCode
			try
			{
				//add the actual code
				this.scriptController.AddCode(code);
				//set the functions
				foreach (MSScriptControl.Procedure procedure in this.scriptController.Procedures) 
				{
					functions.Add(new ScriptFunction(this, procedure));
				}
			}
			catch (Exception e)
			{
				//the addCode didn't work, probably because of a syntax error, or unsupported syntaxt in the code
				this.errorMessage = e.Message;
			}
			
		}
		/// <summary>
		/// gets all scripts defined in the model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static List<Script> getAllScripts(EAWrappers.Model model)
		{
			List<Script> allScripts = new List<Script>();
			if (model != null)
			{
			 XmlDocument xmlScripts = model.SQLQuery("select * from t_script");
			 XmlNodeList scriptNodes = xmlScripts.SelectNodes(model.formatXPath("//Row"));
              foreach (XmlNode scriptNode in scriptNodes)
              {
              	//TODO get the <notes> node. If it countaints "Group Type=" then it is a group. Else we need to find "Language=" 
              	XmlNode notesNode = scriptNode.SelectSingleNode(model.formatXPath("Notes"));
              	if (notesNode.InnerText.Contains(scriptLanguageIndicator))
          	    {
          	    	//we have an actual script.
          	    	//the name of the script
          	    	string scriptName = getValueByName(notesNode.InnerText, scriptNameIndicator);
					//now figure out the language
					string language = getValueByName(notesNode.InnerText, scriptLanguageIndicator);
					//then get teh code
					XmlNode codeNode = scriptNode.SelectSingleNode(model.formatXPath("Script"));	
					if (codeNode != null && language != string.Empty)
					{
						//and create the script if both code and language are found
						allScripts.Add(new Script(scriptName,codeNode.InnerText, language,model));
					}
          	    }
              }
			}
			return allScripts;
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
			return this.scriptController.Run(functionName,parameters);
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
		
		
	}
}
