/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 7/12/2014
 * Time: 6:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

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
		
	}
}
