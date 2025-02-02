using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EAWrappers = TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinFramework.EASpecific
{
    public class LocalScriptRepository : DefaultScriptRepsitory
    {
        private bool _loaded = false;

        public LocalScriptRepository(ScriptRepository scriptRepository) : base(scriptRepository)
        {
        }

        public override void saveScripts(string scriptPath)
        {
            // Local Script Repository does not support saving scripts
        }

        /// <summary>
        /// The local scripts are located in the "ea program files"\scripts (so usually C:\Program Files (x86)\Sparx Systems\EA\Scripts or C:\Program Files\Sparx Systems\EA\Scripts)
        /// </summary>
        public override void loadScripts()
        {
            try
            {
                string scriptsDirectory = Path.GetDirectoryName(EAWrappers.Model.applicationFullPath) + "\\Scripts";
                //Logger.logDebug($"scriptsDirectory={scriptsDirectory}");
                if (Directory.Exists(scriptsDirectory))
                {
                    string[] scriptFiles = Directory.GetFiles(scriptsDirectory, "*.*", SearchOption.AllDirectories);
                    foreach (string scriptfile in scriptFiles)
                    {
                        string scriptLanguage = Script.getLanguageFromPath(scriptfile);

                        if (scriptLanguage == "VBScript")
                        {
                            //Logger.logDebug($"scriptfile={scriptfile}");

                            string scriptGroup = "Local Scripts";
                            string scriptCode = File.ReadAllText(scriptfile);
                            string scriptName = Path.GetFileNameWithoutExtension(scriptfile);
                            Script script = new Script(scriptRepository, scriptGroup + "." + scriptName, scriptName, scriptGroup, scriptCode, scriptLanguage, null, true);
                            this.addScript(script);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Logger.logError(string.Format("Error occured: {0} stacktrace: {1}", e.Message, e.StackTrace));
            }
        }

        public override void resetScripts()
        {
            // Local Script Repository does not need to be reset as the contents on disk never change at runtime
            if (!_loaded)
            {
                loadScripts();
                _loaded = true;
            }
        }
    }
}
