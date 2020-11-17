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

namespace EAAddinFramework.Utilities
{
    /// <summary>
    /// Description of AddinSettings.
    /// </summary>
    public abstract class AddinSettings
    {



        protected abstract string configSubPath { get; }
        protected abstract string defaultConfigAssemblyFilePath { get; }
        private string defaultConfigFilePath => this.defaultConfigAssemblyFilePath + ".config";
        private AddinConfig _defaultConfig;
        protected AddinConfig defaultConfig
        {
            get
            {
                if (this._defaultConfig == null)
                {
                    this._defaultConfig = new AddinConfig(this.defaultConfigFilePath, this.defaultConfigFilePath);
                }
                return this._defaultConfig;
            }
        }
        private AddinConfig _currentConfig;
        protected AddinConfig currentConfig
        {
            get => this._currentConfig ?? this.userConfig;
            set => this._currentConfig = value;
        }
        protected AddinConfig userConfig { get; set; }
        protected AddinSettings()
        {
            Configuration roamingConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);

            //the roamingConfig now get a path such as C:\Users\<user>\AppData\Roaming\Sparx_Systems_Pty_Ltd\DefaultDomain_Path_2epjiwj3etsq5yyljkyqqi2yc4elkrkf\9,_2,_0,_921\user.config
            // which I don't like. So we move up three directories and then add a directory for the Adding so that we get
            // C:\Users\<user>\AppData\Roaming\<configSubPath>\user.config
            string configFileName = System.IO.Path.GetFileName(roamingConfig.FilePath);
            string configDirectory = System.IO.Directory.GetParent(roamingConfig.FilePath).Parent.Parent.Parent.FullName;
            string newConfigFilePath = configDirectory + configSubPath + configFileName;

            // Get the mapped configuration file.
            this.userConfig = new AddinConfig(newConfigFilePath, this.defaultConfigFilePath);
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
