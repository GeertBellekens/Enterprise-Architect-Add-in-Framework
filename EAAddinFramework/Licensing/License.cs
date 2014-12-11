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

namespace EAAddinFramework.Licensing
{
	/// <summary>
	/// Description of License.
	/// </summary>
	public class License
	{
		private string _key;
		public string application {get;set;}
		public DateTime validUntil {get;set;}
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
		
		public License(string application, DateTime validUntil, bool floating, string client)
		{
			this.application = application;
			this.validUntil = validUntil;
			this.floating = floating;
			this.client = client;
		}
		public License(string key, string publicKey)
		{
			DSACryptoServiceProvider provider = new DSACryptoServiceProvider();
			provider.FromXmlString(publicKey);
			string[] keyParts = key.Split('|');
			if (keyParts.Length == 2)
			{
				string data = keyParts[0];
				string signature = keyParts[1];
				//decode the signature
				signature = Encoding.Unicode.GetString(Convert.FromBase64String(signature));
				//validate the signature
				this.isValid = provider.VerifyData(Encoding.Unicode.GetBytes(data),Encoding.Unicode.GetBytes(signature));
				//decode the contens of the key if the key was valid
				if (this.isValid)
				{
					if (data.IndexOf("-") > 0) //in case of a floating license the first part is "ApplicationName-" in plain text
					{
						string[] dataparts = data.Split('-');
						data = dataparts[0] + "|" + Encoding.Unicode.GetString(Convert.FromBase64String(dataparts[1]));
						
					}else
					{
						data = Encoding.Unicode.GetString(Convert.FromBase64String(data));
					}
					//split up the data
					string[] datasplitted = data.Split('|');
					if (datasplitted.Length == 4)
					{
						this.application = datasplitted[0];
						DateTime validDate;
						if (DateTime.TryParse(datasplitted[1], out validDate))
					    {
					    	this.validUntil = validDate;
					    }
						this.client = datasplitted[2];
						//datasplitted[3] is the guid, which we don't need right now
					}
				}
				
				
				
				
				
			}
			
		}
		
	}
}
