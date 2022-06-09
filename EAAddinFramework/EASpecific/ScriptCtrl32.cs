using MSScriptControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EAAddinFramework.EASpecific
{
    internal class ScriptCtrl32 : ScriptCtrl
    {
        private MSScriptControl.ScriptControl scriptControl;
        public ScriptCtrl32() 
        {
            this.scriptControl = new ScriptControl();
        }
        
        public override string Language 
        {
            get => this.scriptControl.Language; 
            set => this.scriptControl.Language = value;
        }

        public override MSScriptControl.Error Error => ((IScriptControl) this.scriptControl).Error;

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
            foreach (MSScriptControl.Procedure procedure in this.scriptControl.Procedures)
            {
                functions.Add(new ScriptFunction(script, procedure));
            }
            return functions;
        }

        public override object Run(string procedureName, object[] parameters)
        {
            return this.scriptControl.Run(procedureName, parameters);
        }
    }
}
