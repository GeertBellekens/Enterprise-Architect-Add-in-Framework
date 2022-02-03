using EA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UML = TSF.UmlToolingFramework.UML;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using EAAddinFramework.WorkTracking.TFS;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace EAAddinFramework.Utilities
{
    public class AddinConfig
    {
        private Configuration configuration { get; set; }
        private TSF_EA.Package package { get; set; }
        public AddinConfigType type { get; private set; }
        public bool isDirty { get; set; } = false;
        public string name
        {
            get
            {
                switch (this.type)
                {
                    case AddinConfigType.Default:
                        return "Default Config";
                    case AddinConfigType.User:
                        return "User Config";
                    case AddinConfigType.Package:
                        return this.package?.name ?? "<unnamed package>";
                    default:
                        return "Unknown AddinConfig type in AddinConfig.name";
                }
            }
        }
        private string addinName { get; set; }
        internal string tagName => addinName + "_config";
        public string path
        {
            get => this.package != null ?
                   this.package.fqn :
                   this.configuration.FilePath;
        }
        public bool isSame(AddinConfig otherConfig)
        {
            foreach (KeyValueConfigurationElement configEntry in this.configuration.AppSettings.Settings)
            {
                if (!otherConfig.configuration.AppSettings.Settings.AllKeys.Contains(configEntry.Key))
                {
                    return false;
                }
                if (otherConfig.configuration.AppSettings.Settings[configEntry.Key].Value != configEntry.Value)
                {
                    return false;
                }
            }
            //all the same, return true
            return true;
        }
        internal AddinConfig(TSF_EA.Package package, string configurationsDirectoryPath, string defaultConfigFilePath, string addinName)
        {
            this.type = AddinConfigType.Package;
            this.package = package;
            this.addinName = addinName;
            var configFileName = configurationsDirectoryPath + package.guid + ".config";
            //check if folder exists, if not create it
            if (!System.IO.Directory.Exists(configurationsDirectoryPath))
            {
                System.IO.Directory.CreateDirectory(configurationsDirectoryPath);
            }
            //check if package has tagged value for config
            var configTag = this.package.getTaggedValue(tagName);
            if (configTag != null)
            {
                //store contents of tagged value in file (or create new file based on default config

                System.IO.StreamWriter configFile = new System.IO.StreamWriter(configFileName);
                configFile.Write(configTag.comment);
                configFile.Close();
            }
            else
            {
                //if the file already exists then delete it
                if (System.IO.File.Exists(configFileName))
                {
                    System.IO.File.Delete(configFileName);
                }
            }
            loadconfig(configFileName, defaultConfigFilePath);
        }
        internal AddinConfig(string configFileName, string defaultConfigFilePath, string addinName, AddinConfigType type)
        {
            this.type = type;
            this.addinName = addinName;
            loadconfig(configFileName, defaultConfigFilePath);
        }
        private void loadconfig(string configFileName, string defaultConfigFilePath)
        {
            this.configuration = getConfiguration(configFileName);
            this.mergeDefaultSettings(defaultConfigFilePath);
        }
        private Configuration getConfiguration(string configFileName)
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFileName;
            return ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }
        protected void mergeDefaultSettings(string defaultConfigFilePath)
        {
            var defaultConfig = getConfiguration(defaultConfigFilePath);
            bool keysAdded = false;
            //defaultConfig.AppSettings.Settings["menuOwnerEnabled"].Value
            foreach (KeyValueConfigurationElement configEntry in defaultConfig.AppSettings.Settings)
            {
                if (!this.configuration.AppSettings.Settings.AllKeys.Contains(configEntry.Key))
                {
                    this.configuration.AppSettings.Settings.Add(configEntry.Key, configEntry.Value);
                    keysAdded = true;
                }
            }
            if (keysAdded)
            {
                // save the configuration if needed
                try
                {
                    this.Save();
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    //swallow exception if it says Element locked
                    if (!e.Message.Contains("Element locked"))
                    {
                        throw (e);
                    }
                }
            }
        }

        internal void openContext()
        {
            if (this.type == AddinConfigType.Package)
            {
                this.package?.select();
            }
            else
            {
                //open the folder in the explorer
                Process.Start(System.IO.Path.GetDirectoryName(this.configuration.FilePath));
            }
        }

        internal void delete()
        {
            if (this.type == AddinConfigType.Default || this.type == AddinConfigType.User)
            {
                throw new InvalidOperationException("Configurations of type Default or User cannot be deleted");
            }
            //delete the tagged value
            if (this.package != null)
            {
                if (this.package.isReadOnly) this.package.makeWritable(true);
                var configTag = this.package.getTaggedValue(this.tagName);
                configTag?.delete();
            }
        }

        internal string getValue(string key)
        {
            return this.configuration.AppSettings.Settings[key].Value;
        }
        internal void setValue(string key, string value)
        {
            if (this.configuration.AppSettings.Settings[key].Value != value)
            {
                this.configuration.AppSettings.Settings[key].Value = value;
                this.isDirty = true;
            }
        }
        internal void refresh()
        {
            //reload the config from file
            this.configuration = getConfiguration(this.configuration.FilePath);
        }
        public void Save()
        {
            if (this.isDirty)
            {
                //save config file
                this.configuration.Save();
                //get xml content
                var xmlContent = System.IO.File.ReadAllText(this.configuration.FilePath);
                //save the tagged value with max 3 retries
                try
                {
                    saveTag(xmlContent, 0);
                    //unset isDirty
                    this.isDirty = false;
                }
                catch (COMException e)
                {
                    if (e.Message.Contains("Element locked"))
                    {
                        MessageBox.Show(this.package.EAModel.mainEAWindow
                                        , $"Package {package.name} is read-only.{Environment.NewLine}Please apply user lock and try again"
                                        , "Package read-only"
                                        , MessageBoxButtons.OK
                                        , MessageBoxIcon.Error);
                        package.select();
                    }
                }


            }
        }
        private void saveTag(string xmlContent, int retryCount)
        {
            try
            {
                //save config tagged value
                if (this.package != null)
                {
                    Logger.log($"Before makewritable retryCount: {retryCount}");
                    if (this.package.isReadOnly) this.package.makeWritable(true);
                    Logger.log($"After makewritable retryCount: {retryCount}");
                    this.package.addTaggedValue(this.tagName, "<memo>", xmlContent);
                    Logger.log($"After addTaggedValue retryCount: {retryCount}");
                }
            }
            catch (COMException e)
            {
                if (retryCount > 0)
                {
                    if (e.Message.Contains("Element locked"))
                    {
                        //wait a bit
                        Thread.Sleep(1000);
                        this.saveTag(xmlContent, retryCount - 1);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }
    }
    public enum AddinConfigType
    {
        Default,
        User,
        Package
    }
}
