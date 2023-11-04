using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAAddinFramework.EASpecific
{

    public abstract class ScriptRepository
    {
        /// <summary>
        /// Search the repository for a script with matching group and name, if a match is found return the script.
        /// Otherwise return null
        /// </summary>
        /// <param name="group">the name of the script group</param>
        /// <param name="name">the name of the script</param>
        /// <returns>the matching script, otherwise null</returns>
        public abstract Script getScriptByGroupAndName(string group, string name);

        /// <summary>
        /// Search the repository for any scripts whose code contains a comment line like: 
        /// 'EA-Matic
        /// and return a list of matching scripts.
        /// </summary>
        /// <returns>a list of scripts that contain "'EA-Matic"</returns>
        public abstract List<Script> getEAMaticScripts();

        /// <summary>
        /// Return all scripts in this repository.
        /// </summary>
        /// <returns>a list of all scripts contained in the repository</returns>
        public abstract List<Script> getAllScripts();

        /// <summary>
        /// Add script to repository
        /// </summary>
        /// <param name="script">the script to add</param>
        public abstract void addScript(Script script);

        /// <summary>
        /// Load the scripts from the repository
        /// </summary>
        public abstract void loadScripts();

        /// <summary>
        /// Re-read the scripts from the repository.
        /// 
        /// Not all repositories support this operation and it will be silently ignored.
        /// 
        /// Most useful for model based repositories where the in-memory copy of the script is stale and needs to be reset to match
        /// the one held in the Model.
        /// </summary>
        public abstract void resetScripts();

        /// <summary>
        /// If the repository supports saving scripts, then write the scripts into the specified scriptPath
        /// </summary>
        /// <param name="scriptPath">the path on disk to save the scripts to</param>
        public abstract void saveScripts(string scriptPath);


    }
}
