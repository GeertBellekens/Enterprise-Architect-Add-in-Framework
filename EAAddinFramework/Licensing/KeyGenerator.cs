/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 5/12/2014
 * Time: 4:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EAAddinFramework.Licensing
{
	/// <summary>
	/// Description of KeyGenerator.
	/// </summary>
	public class KeyGenerator
	{
		private string privateKey;
		private DSACryptoServiceProvider provider;
		public KeyGenerator(string privateKey)
		{
			this.privateKey = privateKey;
			this.initialize();
		}
		/// <summary>
		/// searches for a file called privateKey.txt in the assembly's folder
		/// </summary>
		public KeyGenerator()
		{
			this.privateKey = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\privateKey.txt");
			this.initialize();
		}
		private void initialize()
		{
			this.provider = new DSACryptoServiceProvider();
			this.provider.FromXmlString(this.privateKey);
		}
			
		/// <summary>
		/// encripts the given contents with the private key
		/// </summary>
		/// <param name="contents">the contents of the license key to be generator</param>
		/// <returns>the encrypted key</returns>
		public string generateKey(string contents)
		{
			string generatedKey = string.Empty;
			//add guid to make it unique
			contents += ("|" + Guid.NewGuid().ToString());
			//convert to base64 to get rid of any weird characters
			string base64Content = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(contents));
			string signature = Convert.ToBase64String (provider.SignData(Encoding.Unicode.GetBytes(base64Content)));
			generatedKey = base64Content + "|" + signature;
			return generatedKey;
		}
		
		/// <summary>
		/// creates the required number of licenses
		/// </summary>
		/// <param name="application"></param>
		/// <param name="validUntil"></param>
		/// <param name="floating"></param>
		/// <param name="client"></param>
		/// <param name="nbrOfKeys"></param>
		/// <returns></returns>
		public List<License> generateLicenses(string application, DateTime validUntil, bool floating, string client,int nbrOfKeys)
		{
			List<License> generatedKeys = new List<License>();
			for (int i = 0; i < nbrOfKeys; i++) 
			{
				
				generatedKeys.Add(this.generateLicense(application, validUntil, floating,client));
			}
			return generatedKeys;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="application"></param>
		/// <param name="validUntil"></param>
		/// <param name="floating"></param>
		/// <param name="client"></param>
		/// <returns></returns>
		public License generateLicense(string application, DateTime validUntil, bool floating, string client)
		{
			//create the LicenseKey object
			License license = new License(application, validUntil,floating,client);
			//add the basic contents
			string keyContents = validUntil.ToString() + "|" + client+ "|" + Guid.NewGuid().ToString();
			//if the license is a floating license then we need to add the name of the application in plain text, else it is base64 encoded
			if (floating)
			{
				//convert to base64 to get rid of any weird characters
				keyContents = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(keyContents));
				//add application
				keyContents = application + "-" + keyContents;
				
			}
			else //non floating
			{
				//add application
				keyContents = application + "|" + keyContents;
				//convert to base64 to get rid of any weird characters
				keyContents = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(keyContents));
			}
	
			string signature = Convert.ToBase64String(provider.SignData(Encoding.Unicode.GetBytes(keyContents)));
			//the key is the a combination of the contents and the signature
			license.key = keyContents + "|" + signature;
			return license;
		}
		
		public static void getKeyPair(int keySize, out string publicKey, out string privateKey)
    	{
	        using (DSACryptoServiceProvider provider = new DSACryptoServiceProvider(keySize))
	        {
	        	privateKey = provider.ToXmlString(true);
		        publicKey = provider.ToXmlString(false);
	        }
    	}
		public static List<int> validKeySizes()
		{
			List<int> keySizes = new List<int>();
			foreach (KeySizes keySize   in new DSACryptoServiceProvider().LegalKeySizes) 
			{
				for (int i = keySize.MinSize; i<= keySize.MaxSize; i+= keySize.SkipSize)
				{
					keySizes.Add(i);
				}
			}
			return keySizes;
		}
	}
}
