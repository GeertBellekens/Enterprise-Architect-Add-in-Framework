using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Utilities
{
	/// <summary>
	/// Description of KeyValuePairsHelper.
	/// </summary>
	public static class KeyValuePairsHelper
	{
		/// <summary>
		/// parses the the given source string for any keyvalue pairs and returns the value corresponding to the given key.
		/// The format of the soure string is assumed as follows: key1=value1;key2=value2,...
		/// </summary>
		/// <param name="key">the key to searche for</param>
		/// <param name = "source">the source string to search in</param>
		/// <returns>the value correspondign to the given key</returns>
		public static string getValueForKey(string key,string source)
		{
			//first split the string into keyvalue pairs
			foreach (var keyValuePair in source.Split(';')) 
			{
				//then split the key and value
				var keyValues = keyValuePair.Split('=');
				if (keyValues.Count() >= 2 && keyValues[0] == key)
				{
					return keyValues[1];
				}
			}
			//if nothing found then return null;
			return null;
		}
		public static string setValueForKey(string key,string value,string source)
		{
			var keyValuePairs = new List<string>();
			//first split the string into keyvalue pairs
			if (!string.IsNullOrEmpty(source))
			{
				keyValuePairs = source.Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries).ToList();
				foreach (var keyValuePair in keyValuePairs) 
				{
					//then split the key and value
					var keyValues = keyValuePair.Split('=');
					if (keyValues.Count() >= 2 && keyValues[0] == key)
					{
						keyValues[1] = value;
						return source.Replace(keyValuePair, string.Join("=", keyValues));
					}
				}
			}
			//if nothing found then weadd it
			keyValuePairs.Add(key + "=" + value);
			return string.Join(";",keyValuePairs);
		}
	}
}
