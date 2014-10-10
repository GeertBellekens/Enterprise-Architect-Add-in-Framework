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

using MSScriptControl;
using EAWrappers = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of Script.
	/// </summary>
	public class Script
	{
		private string _code;
		private ScriptControl scriptController;
		public List<ScriptFunction> functions {get;set;}
		public Script(string code)
		{
			this._code = code;
			this.scriptController = new ScriptControl();
			this.scriptController.Language = "VBScript";
			this.scriptController.AddCode(code);
		}
		public static List<Script> getAllScripts(EAWrappers.Model model)
		{
			 XmlDocument xmlScripts = model.SQLQuery("select * from t_script");
			 throw new NotImplementedException();
		}
	}
}
