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

namespace EAAddinFramework.Utilities
{
	/// <summary>
	/// Description of AddinSettings.
	/// </summary>
	public abstract class AddinSettings
	{
		protected abstract string configSubPath {get;}
		protected abstract string defaultConfigFilePath {get;}
		protected Configuration defaultConfig {get;set;}
		protected Configuration currentConfig {get;set;}
		protected AddinSettings()
		{
		  Configuration roamingConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
		  
		  //the roamingConfig now get a path such as C:\Users\<user>\AppData\Roaming\Sparx_Systems_Pty_Ltd\DefaultDomain_Path_2epjiwj3etsq5yyljkyqqi2yc4elkrkf\9,_2,_0,_921\user.config
		  // which I don't like. So we move up three directories and then add a directory for the Adding so that we get
		  // C:\Users\<user>\AppData\Roaming\<configSubPath>\user.config
		  string configFileName =  System.IO.Path.GetFileName(roamingConfig.FilePath);
		  string configDirectory = System.IO.Directory.GetParent(roamingConfig.FilePath).Parent.Parent.Parent.FullName;
		  
		  string newConfigFilePath = configDirectory + configSubPath + configFileName;
		  // Map the roaming configuration file. This
		  // enables the application to access 
		  // the configuration file using the
		  // System.Configuration.Configuration class
		  ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
		  configFileMap.ExeConfigFilename = newConfigFilePath;		
		  // Get the mapped configuration file.
		   currentConfig = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
		  //merge the default settings
		  this.mergeDefaultSettings();
		}
		
		/// <summary>
		/// gets the default settings config.
		/// </summary>
		protected void getDefaultSettings()
		{
			defaultConfig = ConfigurationManager.OpenExeConfiguration(defaultConfigFilePath);
		}
		/// <summary>
		/// merge the default settings with the current config.
		/// </summary>
		protected void mergeDefaultSettings()
		{
			if (this.defaultConfig == null)
			{
				this.getDefaultSettings();
			}
			//defaultConfig.AppSettings.Settings["menuOwnerEnabled"].Value
			foreach ( KeyValueConfigurationElement configEntry in defaultConfig.AppSettings.Settings) 
			{
				if (!currentConfig.AppSettings.Settings.AllKeys.Contains(configEntry.Key))
				{
					currentConfig.AppSettings.Settings.Add(configEntry.Key,configEntry.Value);
				}
			}
			// save the configuration
			currentConfig.Save();
		}
		protected string getValue(string key)
		{
			return this.currentConfig.AppSettings.Settings[key].Value;
		}
		protected void setValue(string key, string value)
		{
			this.currentConfig.AppSettings.Settings[key].Value = value;
		}
		protected bool getBooleanValue(string key)
		{
			bool result;
			return bool.TryParse(this.getValue(key), out result) ? result : true;
		}
		protected void setBooleanValue(string key, bool boolValue)
		{
			this.setValue(key,boolValue.ToString());
		}
		protected Dictionary<string,string> getDictionaryValue(string key)
		{
			var returnedDictionary = new Dictionary<string,string>();
			foreach (var keyValues in this.getListValue(key))
			{
				var keyValue = keyValues.Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries);
				if (keyValue.Count() == 2)
				{
					returnedDictionary.Add(keyValue[0],keyValue[1]);
				}
			}
			return returnedDictionary;
		}
		protected void setDictionaryValue(string key, Dictionary<string,string> dictionaryValue)
		{
			var keyValues = new List<string>();
			foreach (var keyValuePair in dictionaryValue) 
			{
				keyValues.Add(string.Join(";",keyValuePair.Key,keyValuePair.Value));
			}
			this.setListValue(key,keyValues);
		}
		protected List<string> getListValue(string key)
		{
			return this.getValue(key).Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries).ToList<string>();
		}
		protected void setListValue(string key, List<string> listValue)
		{
			this.setValue(key,string.Join(",",listValue));
		}
		protected int getIntValue(string key)
		{
			int returnInteger;
			return  int.TryParse(this.getValue(key),out returnInteger) ? returnInteger:0;
		}
		protected void setIntValue(string key, int intValue)
		{
			this.setValue(key,intValue.ToString());
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
		  ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
		  configFileMap.ExeConfigFilename = currentConfig.FilePath;		
		  // Get the mapped configuration file.
		   currentConfig = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
		}
	}
}
