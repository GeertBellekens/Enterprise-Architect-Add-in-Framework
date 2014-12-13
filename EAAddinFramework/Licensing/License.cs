/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 7/12/2014
 * Time: 6:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Security.Cryptography;
using System.Text;
using LogicNP.CryptoLicensing;

namespace EAAddinFramework.Licensing
{
	/// <summary>
	/// Description of License.
	/// </summary>
	public class License
	{
		private string _key;
		public bool floating {get;set;}
		public string client {get;set;}
		public bool isValid {get;private set;}
		public string key 
		{
			get
			{
				return this._key;
			}
			internal set
			{
				this._key = value;
			}
		}
		
		public License(string key, string publicKey)
		{
			
			this.key = key;
			//create the wrapped license
			CryptoLicense wrappedlicense = new CryptoLicense(key, publicKey);
			//validate the signature
			this.isValid = (wrappedlicense.Status == LicenseStatus.Valid);
			//get user data
			if (this.isValid)
			{
				this.client = wrappedlicense.GetUserDataFieldValue("Client", "|");
				string isFloating = wrappedlicense.GetUserDataFieldValue("Isfloating","|");
				bool isFloatingBool;
				if(bool.TryParse(isFloating,out isFloatingBool))
				{
					this.floating = isFloatingBool;
				}
				
			}
			
		}
		
	}
}
