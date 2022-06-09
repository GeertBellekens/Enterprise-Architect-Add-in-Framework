using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAAddinFramework.EASpecific
{
    internal class ScriptCtrl64: ScriptCtrl
    {
        private dynamic scriptControl;
        public ScriptCtrl64()
        {
            Type scriptControllerType = Type.GetTypeFromProgID("ScriptControl");
            this.scriptControl = Activator.CreateInstance(scriptControllerType);
        }

        public override string Language
        {
            get => this.scriptControl.Language;
            set => this.scriptControl.Language = value;
        }

        public override MSScriptControl.Error Error => this.scriptControl.Error;

        public override void AddCode(string code)
        {
            this.scriptControl.AddCode(code);
        }

        public override void AddObject(string name, object objectToAdd)
        {
            this.scriptControl.AddObject(name, objectToAdd);
        }

        public override List<ScriptFunction> GetScriptFunctions(Script script)
        {
            var functions = new List<ScriptFunction>();
            foreach (string procedureName in this.scriptControl.CodeObject)
            {
                functions.Add(new ScriptFunction(script, procedureName));
            }
            return functions;
        }

        public override object Run(string procedureName, object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return this.scriptControl.Run(procedureName);
            }
            return this.scriptControl.Run(procedureName, parameters);
        }
    }
}
