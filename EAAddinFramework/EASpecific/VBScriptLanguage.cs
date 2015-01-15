/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 22/11/2014
 * Time: 7:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using System.IO;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of VBScriptLanuguage.
	/// </summary>
	public class VBScriptLanguage:ScriptLanguage
	{
		public VBScriptLanguage()
		{
		}
		protected override string functionStart {get {return "function ";}}
		protected override string parameterListStart {get {return "(";}}
		protected override string parameterSeparator {get {return ", ";}}
		protected override string parameterListEnd {get {return ")";}}
		protected override string bodyStart {get {return string.Empty;}}
		protected override string bodyEnd {get {return string.Empty;}}
		protected override string functionEnd {get {return "end function";}}
		protected override string commentLine {get {return "'";}}
		
		private string subStart {get {return "sub ";}}
		private string subEnd {get{return "end sub";}}
		
		
		public override string name 
		{
			get 
			{
				return "VBScript";
			}
		}
		/// <summary>
		/// removes the statements that execute a function/procedure from the code
		/// </summary>
		/// <param name="code">the code with executing statements</param>
		/// <returns>the code without executing statements</returns>
		public override string removeExecutingStatements(string code)
		{
			StringReader reader = new StringReader(code);
			string cleanedCode = code;
			string line;
			bool functionStarted = false;
			bool subStarted = false;
			while (null != (line = reader.ReadLine()))
			{ 

				if (line != string.Empty)
				{
				    if (line.StartsWith(this.functionStart))
				    {
				    	functionStarted = true;
				    }else if (line.StartsWith(this.subStart))
				    {
				    	subStarted = true;
				    }else if (functionStarted && line.StartsWith(this.functionEnd))
				    {
				    	functionStarted = false;
				    }
				    else if (subStarted && line.StartsWith(this.subEnd))
				    {
				    	subStarted = false;
				    }else if (!functionStarted && !subStarted)
				    {
				    	//code outside of a function or sub, figure out if this code calls another sub or function
				    	foreach (string linepart in line.Split(new char[] {' ' , '(' },StringSplitOptions.RemoveEmptyEntries))
				    	{
				    		if (cleanedCode.Contains(this.functionStart + linepart)
				    		    ||cleanedCode.Contains(this.subStart + linepart))
							{
				    			//found a line that calls an existing function or sub, replace it by an empty string.
				    			cleanedCode = cleanedCode.Replace(Environment.NewLine +line + Environment.NewLine, 
				    			                                  Environment.NewLine + string.Empty + Environment.NewLine);
				    		}
				    	}
				    }
				}
			}
			return cleanedCode;
		}
	}
}
