using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSScriptControl;

namespace EAAddinFramework.EASpecific
{
    internal abstract class ScriptCtrl
    {
        public abstract string Language {get; set; }
        public abstract void AddObject(string name, object objectToAdd);
        public abstract void AddCode(string code);
        public abstract Error Error { get; }
        public abstract List<ScriptFunction> GetScriptFunctions(Script script);
        public abstract object Run(string procedureName, object[] parameters);

    }
}
