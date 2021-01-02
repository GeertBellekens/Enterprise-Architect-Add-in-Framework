/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 26/11/2014
 * Time: 5:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.IO;

namespace EAAddinFramework.EASpecific
{
    /// <summary>
    /// Description of JavaScriptLanguageBase.
    /// </summary>
    public abstract class JavaScriptLanguageBase : ScriptLanguage
    {
        protected override string functionStart => "function ";
        protected override string parameterListStart => "(";
        protected override string parameterSeparator => ", ";
        protected override string parameterListEnd => ")"; 
        protected override string bodyStart => "{"; 
        protected override string bodyEnd => "}";
        protected override string functionEnd => string.Empty;
        public override string commentLine => @"//";
        public override string extension => ".js";



    }
}
