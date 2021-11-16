using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.EASpecific
{
    public class ScriptGroup
    {
        private static Dictionary<string, ScriptGroup> scriptGroups = new Dictionary<string, ScriptGroup>();
        public static ScriptGroup getScriptGroup(string groupName, Model model)
        {
            var groupKey = model != null ?
                model.projectGUID + groupName
                : groupName;
            
            if (! scriptGroups.ContainsKey(groupKey))
            {
                scriptGroups.Add(groupKey, new ScriptGroup(groupName));
            }
            return scriptGroups[groupKey];
        }
        private ScriptGroup(string name)
        {
            this.name = name;
        }
        public string name { get; private set; }
        private Dictionary<string, Script> _scripts = new Dictionary<string, Script>();
        public IEnumerable<Script> scripts { get => this._scripts.Values; }
        public void addScript(Script script)
        {
            if (! this._scripts.ContainsKey(script.scriptkey))
            {
                this._scripts.Add(script.scriptkey, script);
            }
        }
        public void removeScript(Script script)
        {
            if (this._scripts.ContainsKey(script.scriptkey))
            {
                this._scripts.Remove(script.scriptkey);
            }
        }
    }
}
