using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAAddinFramework.EASpecific
{
    public class ScriptRepositoryProxy : ScriptRepository
    {
        private List<ScriptRepository> proxies = new List<ScriptRepository>();

        public ScriptRepositoryProxy() { }

        public void addRepository(ScriptRepository repositoryToProxy)
        {
            proxies.Add(repositoryToProxy);
        }

        public override void addScript(Script script)
        {
            // the proxy does not contain scripts, only the repositoryToProxies do.
            throw new NotImplementedException();
        }

        public override List<Script> getAllScripts()
        {
            List<Script> scripts = new List<Script>();
            foreach (ScriptRepository proxyRepository in proxies)
            {
                scripts.AddRange(proxyRepository.getAllScripts());
            }
            return scripts;
        }

        public override List<Script> getEAMaticScripts()
        {
            List<Script> scripts = new List<Script>();
            foreach (ScriptRepository proxyRepository in proxies)
            {
                scripts.AddRange(proxyRepository.getEAMaticScripts());
            }
            return scripts;
        }

        public override Script getScriptByGroupAndName(string group, string name)
        {
            foreach (ScriptRepository proxyRepository in proxies)
            {
                Script script = proxyRepository.getScriptByGroupAndName(group, name);
                if (script != null)
                {
                    return script;
                }
            }
            return null;
        }

        public override void loadScripts()
        {
            // Not used by the proxy directly, used internally by the real repositories to load the scripts
            throw new NotImplementedException();
        }

        public override void resetScripts()
        {
            foreach (ScriptRepository proxyRepository in proxies)
            {
                DateTime timeStart = DateTime.Now;
                Logger.log($"TRACE: resetScripts {proxyRepository.GetType()} start");
                proxyRepository.resetScripts();
                DateTime timeEnd = DateTime.Now; ;
                TimeSpan difference = timeEnd - timeStart;
                Logger.log($"TRACE: resetScripts {proxyRepository.GetType()} end: {difference.TotalSeconds}");
            }
        }

        public override void saveScripts(string scriptPath)
        {
            foreach (ScriptRepository proxyRepository in proxies)
            {
                proxyRepository.saveScripts(scriptPath);
            }
        }
    }
}
