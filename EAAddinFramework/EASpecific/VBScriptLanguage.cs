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
    public class VBScriptLanguage : ScriptLanguage
    {
        public VBScriptLanguage()
        {
        }
        protected override string functionStart => "function ";
        protected override string parameterListStart => "(";
        protected override string parameterSeparator => ", ";
        protected override string parameterListEnd => ")";
        protected override string bodyStart => string.Empty;
        protected override string bodyEnd => string.Empty;
        protected override string functionEnd => "end function";
        public override string commentLine => "'";
        private string subStart => "sub ";
        private string subEnd => "end sub";
        public override string name => "VBScript";
        public override string extension => ".vbs";
    }
}
