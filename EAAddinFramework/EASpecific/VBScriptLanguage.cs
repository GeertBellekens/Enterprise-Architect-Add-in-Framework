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
		
		public override string name 
		{
			get 
			{
				return "VBScript";
			}
		}
	}
}
