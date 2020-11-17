using EA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Utilities
{
    public class AddinConfig
    {
        private Configuration configuration { get; set; }
        private UML.Classes.Kernel.Package package { get; set; }
        public string name { get; }
        public string path
        {
            get => this.package != null ?
                   this.package.fqn :
                   this.configuration.FilePath;
        }

        internal AddinConfig(UML.Classes.Kernel.Package package)
        {
            this.package = package;
        }
        internal AddinConfig(string configFileName, string defaultConfigFilePath, string name)
        {
            this.name = name;
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
            //TODO save tagged value
        }

    }
}
