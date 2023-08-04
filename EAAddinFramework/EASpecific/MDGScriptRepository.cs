using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Xml;
using EAAddinFramework.Utilities;
using EAWrappers = TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinFramework.EASpecific
{
    public class MDGScriptRepository : DefaultScriptRepsitory
    {
        public MDGScriptRepository(ScriptRepository scriptRepository) : base(scriptRepository)
        {
        }

        private void createScript(string scriptName, string groupName, string scriptCode, string language)
        {
            Script script = new Script(scriptRepository, groupName + "." + scriptName, scriptName, groupName, scriptCode, language, null, true);
            addScript(script);
        }

        /// <summary>
        /// loads the scripts described in the MDG file into the includable scripts
        /// </summary>
        /// <param name="mdgXmlContent">the string content of the mdg file</param>
        private void loadMDGScripts(string mdgXmlContent)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(mdgXmlContent);
                //first get the name of the MDG
                XmlElement documentationElement = xmlDoc.SelectSingleNode("//Documentation") as XmlElement;
                if (documentationElement != null)
                {
                    string mdgName = documentationElement.GetAttribute("id");
                    //then get the scripts
                    XmlNodeList scriptNodes = xmlDoc.SelectNodes("//Script");
                    foreach (XmlNode scriptNode in scriptNodes)
                    {
                        XmlElement scriptElement = (XmlElement)scriptNode;
                        //get the name of the script
                        string scriptName = scriptElement.GetAttribute("name");
                        //get the language of the script
                        string scriptLanguage = scriptElement.GetAttribute("language");
                        //get the script itself
                        XmlNode contentNode = scriptElement.SelectSingleNode("Content");
                        if (contentNode != null)
                        {
                            //the script itstelf is base64 endcoded in the content tag
                            string scriptcontent = System.Text.Encoding.Unicode.GetString(System.Convert.FromBase64String(contentNode.InnerText));
                            createScript(scriptName, mdgName, scriptcontent, scriptLanguage);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.logError("Error in loadMDGScripts: " + e.Message);
            }
        }

        /// <summary>
        /// get the mdg files in the local MDGtechnologies folder
        /// </summary>
        private void loadLocalMDGScripts()
        {
            string mdgDirectory = Path.GetDirectoryName(EAWrappers.Model.applicationFullPath) + "\\MDGTechnologies";
            loadMDGScriptsFromFolder(mdgDirectory);
        }
        /// <summary>
        /// load the scripts from the mdg files in the given directory
        /// </summary>
        /// <param name="folderPath">the path to the directory</param>
        private void loadMDGScriptsFromFolder(string folderPath)
        {
            try
            {
                string[] mdgFiles = Directory.GetFiles(folderPath, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (string mdgfile in mdgFiles)
                {
                    if (File.Exists(mdgfile))
                        loadMDGScripts(File.ReadAllText(mdgfile));
                }
            }
            catch (Exception e)
            {
                Logger.logError("Error in loadMDGScriptsFromFolder: " + e.Message);
            }
        }

        /// <summary>
        /// load the mdg scripts from the mdg file located at the given url
        /// </summary>
        /// <param name="url">the url pointing to the mdg file</param>
        private void loadMDGScriptsFromURL(string url)
        {
            try
            {
                loadMDGScripts(new WebClient().DownloadString(url));
            }
            catch (Exception e)
            {
                Logger.logError("Error in loadMDGScriptsFromURL: " + e.Message);
            }
        }

        /// <summary>
        /// loads the mdg scripts from the locations added from MDG Technologies|Advanced. 
        /// these locations are stored as a comma separated string in the registry
        /// a location can either be a directory, or an url
        /// </summary>
        private void loadOtherMDGScripts()
        {
            //read the registry key to find the locations
            string pathList = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Sparx Systems\EA400\EA\OPTIONS", "MDGTechnology PathList", null) as string;
            if (pathList != null)
            {
                string[] mdgPaths = pathList.Split(',');
                foreach (string mdgPath in mdgPaths)
                {
                    //figure out it we have a folderpath or an url
                    if (mdgPath.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //url
                        loadMDGScriptsFromURL(mdgPath);
                    }
                    else
                    {
                        //directory
                        loadMDGScriptsFromFolder(mdgPath);
                    }
                }
            }

        }
        public override void loadScripts()
        {
            // These scripts are stored outside the model and can not be changed by the user
         
            //MDG scripts in the program folder
            loadLocalMDGScripts();
            // MDG scripts in other locations
            loadOtherMDGScripts();
        }

        public override void resetScripts()
        {
            // MDG Script Repository does not need to be reset as the contents in a MDG never change at runtime
        }

        public override void saveScripts(string scriptPath)
        {
            // MDG Script Repository does not support saving scripts
        }
    }
}
