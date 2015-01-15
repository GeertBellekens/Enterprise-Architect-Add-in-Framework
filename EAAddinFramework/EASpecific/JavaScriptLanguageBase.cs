/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 26/11/2014
 * Time: 5:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of JavaScriptLanguageBase.
	/// </summary>
	public abstract class JavaScriptLanguageBase : ScriptLanguage
	{
		protected override string functionStart {get {return "function ";}}
		protected override string parameterListStart {get {return "(";}}
		protected override string parameterSeparator {get {return ", ";}}
		protected override string parameterListEnd {get {return ")";}}
		protected override string bodyStart {get {return "{";}}
		protected override string bodyEnd {get {return "}";}}
		protected override string functionEnd {get {return string.Empty;}}
		protected override string commentLine {get {return "//";}}
		/// <summary>
		/// removes the statements that execute a function/procedure from the code
		/// </summary>
		/// <param name="code">the code with executing statements</param>
		/// <returns>the code without executing statements</returns>
		public override string removeExecutingStatements(string code)
		{
			//TODO: implement for javascript;
			return code;
		}
	}
}
