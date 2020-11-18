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

namespace EAAddinFramework.Utilities
{
    public class AddinConfig
    {
        private Configuration configuration { get; set; }
        private TSF_EA.Package package { get; set; }
        public AddinConfigType type { get; private set; }
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
        private string tagName => addinName + "_config";
        public string path
        {
            get => this.package != null ?
                   this.package.fqn :
                   this.configuration.FilePath;
        }

        internal AddinConfig(TSF_EA.Package package, string configurationsDirectoryPath, string defaultConfigFilePath, string addinName)
        {
            this.type = AddinConfigType.Package;
            this.package = package;
            this.addinName = addinName;
            var configFileName = configurationsDirectoryPath + package.guid + ".config";
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
        internal AddinConfig(string configFileName, string defaultConfigFilePath, AddinConfigType type)
        {
            this.type = type;
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
            //defaultConfig.AppSettings.Settings["menuOwnerEnabled"].Value
            foreach (KeyValueConfigurationElement configEntry in defaultConfig.AppSettings.Settings)
            {
                if (!this.configuration.AppSettings.Settings.AllKeys.Contains(configEntry.Key))
                {
                    this.configuration.AppSettings.Settings.Add(configEntry.Key, configEntry.Value);
                }
            }
            // save the configuration
            this.Save();
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
            this.configuration.AppSettings.Settings[key].Value = value;
        }
        internal void refresh()
        {
            //reload the config from file
            this.configuration = getConfiguration(this.configuration.FilePath);
        }
        public void Save()
        {
            this.configuration.Save();

            if (this.package != null)
            {
                if (this.package.isReadOnly) this.package.makeWritable(true);
                //get xml content
                var xmlContent = System.IO.File.ReadAllText(this.configuration.FilePath);
                this.package.addTaggedValue(this.tagName, "<memo>", xmlContent);
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
