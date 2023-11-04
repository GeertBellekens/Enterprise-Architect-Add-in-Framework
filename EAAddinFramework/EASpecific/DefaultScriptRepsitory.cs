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
        private static string VBSCRIPT = new VBScriptLanguage().name;

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
            if (script.language.name != VBSCRIPT)
            {
                Logger.logWarning($"Ignoring non-{VBSCRIPT} script {script.fullyQualifiedName}");
                return;
            }

            string scriptKey = script.fullyQualifiedName;
            // VBScript is case-insensitive
            string scriptKeyLowerCase = scriptKey.ToLower();
            if (cachedScripts.ContainsKey(scriptKeyLowerCase))
            {
                throw new Exception($"script already exists in cache: {scriptKey}");
            }
            else
            {
                Logger.logDebug($"Adding script: {scriptKey}");
                cachedScripts.Add(scriptKeyLowerCase, script);
            }
        }

        public override Script getScriptByGroupAndName(string group, string name)
        {
            // VBScript is case-insensitive
            string key = $"{group}.{name}".ToLower();
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
