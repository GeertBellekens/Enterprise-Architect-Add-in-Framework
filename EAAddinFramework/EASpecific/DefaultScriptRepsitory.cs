using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAAddinFramework.EASpecific
{
    public abstract class DefaultScriptRepsitory : ScriptRepository
    {
        private readonly Dictionary<string, Script> cachedScripts = new Dictionary<string, Script>();

        public ScriptRepository scriptRepository { get; }

        public DefaultScriptRepsitory(ScriptRepository scriptRepository) {
            this.scriptRepository = scriptRepository;
        }

        internal void clearCache()
        {
            cachedScripts.Clear();
        }

        public override void addScript(Script script)
        {
            Logger.log($"INFO: Adding script: {script.fullyQualifiedName}");
            cachedScripts.Add(script.fullyQualifiedName, script);
        }

        public override Script getScriptByGroupAndName(string group, string name)
        {
            string key = $"{group}.{name}";
            Script result = null;
            if (cachedScripts.ContainsKey(key))
            {
                result = cachedScripts[key];
            }
            return result;
        }

        public override List<Script> getEAMaticScripts()
        {
            List<Script> allEAMaticScripts = new List<Script>();

            foreach (Script script in cachedScripts.Values)
            {
                if (script.isEAMaticScript)
                {
                    allEAMaticScripts.Add(script);
                }
            }
            return allEAMaticScripts;
        }

        public override List<Script> getAllScripts()
        {
            return cachedScripts.Values.ToList();
        }

    }
}
