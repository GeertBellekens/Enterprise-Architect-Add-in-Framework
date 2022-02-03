/*
 * Created by SharpDevelop.
 * User: LaptopGeert
 * Date: 6/07/2016
 * Time: 5:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using EAAddinFramework.EASpecific;
using System.Collections;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using EA;
using TSF.UmlToolingFramework.UML.Extended;
using UML = TSF.UmlToolingFramework.UML;
using EAAddinFramework.WorkTracking.TFS;

namespace EAAddinFramework.Utilities
{
    /// <summary>
    /// Description of AddinSettings.
    /// </summary>
    public abstract class AddinSettings
    {

        protected abstract string addinName { get; }
        protected abstract string configSubPath { get; }
        protected abstract string defaultConfigAssemblyFilePath { get; }
        public TSF_EA.Model model { get; set; }
        public AddinConfig addConfig()
        {
            //can't add any new configs if the model doesn't exist
            if (this.model == null) return null;
            //let the user select a package
            var configPackage = model.getUserSelectedPackage() as TSF_EA.Package;
            //return if no package was selected
            if (configPackage == null) return null;
            //create the new config
            return new AddinConfig(configPackage, this.configurationsDirectoryPath, this.defaultConfigFilePath, this.addinName);
        }
        private string configurationsDirectoryPath { get; set; }
        /// <summary>
        /// sets the currentConfig to the config for the contextElement
        /// </summary>
        /// <param name="contextElement"></param>
        /// <returns>True if the config was changed, false if not.</returns>
        public bool setContextConfig(UML.Classes.Kernel.Element contextElement)
        {
            var tempConfig = this.currentConfig;
            if (contextElement == null)
            {
                this.currentConfig = this.userConfig;
            }
            else
            {
                var contextPackage = this.getOwningPackage(contextElement);
                this.currentConfig = getContextConfig((TSF_EA.Package)contextPackage);
            }
            //return the true if the config was changed
            return !this.currentConfig.isSame(tempConfig);
        }
        private UML.Classes.Kernel.Package getOwningPackage(UML.Classes.Kernel.Element element)
        {
            if (element is UML.Classes.Kernel.Package)
            {
                return element as UML.Classes.Kernel.Package;
            }
            var owner = element.owner;
            if (owner == null)
            {
                return null;
            }
            return this.getOwningPackage(owner);
        }
        private AddinConfig getContextConfig(TSF_EA.Package contextPackage)
        {
            //check if tag exists at this package
            var configTag = contextPackage.getTaggedValue(this.currentConfig.tagName);
            if (configTag != null)
            {
                //found the tag, return new config
                return new AddinConfig(contextPackage, this.configurationsDirectoryPath, this.defaultConfigFilePath, this.addinName);
            }
            //check parent
            var parentPackage = contextPackage.owningPackage as TSF_EA.Package;
            if (parentPackage is TSF_EA.RootPackage)
            {
                //root packages can't have tagged values, no need to check further
                return null;
            }
            //go up a level
            return getContextConfig(parentPackage);
        }

        internal List<AddinConfig> getAllConfigs()
        {
            var allConfigs = new List<AddinConfig>();
            allConfigs.Add(userConfig);
            allConfigs.Add(defaultConfig);
            allConfigs.AddRange(this.getAllPackageConfigs());
            return allConfigs;
        }
        private List<AddinConfig> getAllPackageConfigs()
        {
            var packageConfigs = new List<AddinConfig>();
            if (this.model == null) return packageConfigs;
            //get all configuration tags
            var sqlGetPackageObjects = @"select o.Object_ID from t_objectproperties tv 
                                        inner join t_object o on o.Object_ID = tv.Object_ID "
                                        + $" where tv.Property = '{this.addinName}_config'";
            var configPackages = this.model.getElementWrappersByQuery(sqlGetPackageObjects);
            foreach (var configPackage in configPackages.OfType<TSF_EA.Package>())
            {
                var packageConfig = new AddinConfig(configPackage, this.configurationsDirectoryPath, this.defaultConfigFilePath, this.addinName);
                packageConfigs.Add(packageConfig);
            }
            return packageConfigs;
        }

        private string defaultConfigFilePath => this.defaultConfigAssemblyFilePath + ".config";
        private AddinConfig _defaultConfig;
        protected AddinConfig defaultConfig
        {
            get
            {
                if (this._defaultConfig == null)
                {
                    this._defaultConfig = new AddinConfig(this.defaultConfigFilePath, this.defaultConfigFilePath, this.addinName, AddinConfigType.Default);
                }
                return this._defaultConfig;
            }
        }

        internal void deleteCurrentConfig()
        {
            this.currentConfig.delete();
            this.currentConfig = null;
        }

        private AddinConfig _currentConfig;
        internal AddinConfig currentConfig
        {
            get => this._currentConfig ?? this.userConfig;
            set => this._currentConfig = value;
        }
        protected AddinConfig userConfig { get; set; }
        protected AddinSettings()
        {
            //Configuration roamingConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);

            //the roamingConfig now get a path such as C:\Users\<user>\AppData\Roaming\Sparx_Systems_Pty_Ltd\DefaultDomain_Path_2epjiwj3etsq5yyljkyqqi2yc4elkrkf\9,_2,_0,_921\user.config
            // which I don't like. So we move up three directories and then add a directory for the Adding so that we get
            // C:\Users\<user>\AppData\Roaming\<configSubPath>\user.config
            //string configFileName = System.IO.Path.GetFileName(roamingConfig.FilePath);

            //string configDirectory = System.IO.Directory.GetParent(roamingConfig.FilePath).Parent.Parent.Parent.FullName;

            this.configurationsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + this.configSubPath;
            string newConfigFilePath = configurationsDirectoryPath + "user.config";

            // Get the mapped configuration file.
            this.userConfig = new AddinConfig(newConfigFilePath, this.defaultConfigFilePath, this.addinName, AddinConfigType.User);
        }


        protected string getValue(string key)
        {
            return this.currentConfig.getValue(key);
        }
        protected void setValue(string key, string value)
        {
            this.currentConfig.setValue(key, value);
        }
        protected bool getBooleanValue(string key)
        {
            bool result;
            return bool.TryParse(this.getValue(key), out result) ? result : true;
        }
        protected void setBooleanValue(string key, bool boolValue)
        {
            this.setValue(key, boolValue.ToString());
        }
        protected Dictionary<string, string> getDictionaryValue(string key)
        {
            var returnedDictionary = new Dictionary<string, string>();
            foreach (var keyValues in this.getListValue(key))
            {
                var keyValue = keyValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValue.Count() == 2)
                {
                    returnedDictionary.Add(keyValue[0], keyValue[1]);
                }
            }
            return returnedDictionary;
        }
        protected void setDictionaryValue(string key, Dictionary<string, string> dictionaryValue)
        {
            var keyValues = new List<string>();
            foreach (var keyValuePair in dictionaryValue)
            {
                keyValues.Add(string.Join(";", keyValuePair.Key, keyValuePair.Value));
            }
            this.setListValue(key, keyValues);
        }
        protected List<string> getListValue(string key)
        {
            return this.getValue(key).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
        }
        protected void setListValue(string key, List<string> listValue)
        {
            this.setValue(key, string.Join(",", listValue));
        }
        protected int getIntValue(string key)
        {
            int returnInteger;
            return int.TryParse(this.getValue(key), out returnInteger) ? returnInteger : 0;
        }
        protected void setIntValue(string key, int intValue)
        {
            this.setValue(key, intValue.ToString());
        }
        /// <summary>
        /// saves the settings to the config file
        /// </summary>
        public void save()
        {
            this.currentConfig.Save();
        }
        public void refresh()
        {
            //clear the cached elements
            clearCache();
            this.currentConfig.refresh();
        }
        /// <summary>
        /// clears the cached settings
        /// </summary>
        protected virtual void clearCache() { }
    }
}
