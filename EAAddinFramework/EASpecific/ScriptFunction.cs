/*
 * Created by SharpDevelop.
 * User: Geert
 * Date: 7/10/2014
 * Time: 19:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of ScriptFunction.
	/// </summary>
	public class ScriptFunction
	{
		private Script owner {get;set;}
		public string name {get;set;}
		
		public ScriptFunction(Script owner, string name)
		{
			this.owner = owner;
			this.name = name;
		}
		/// <summary>
		/// execute this function
		/// </summary>
		/// <param name="parameters">the parameters needed by this function</param>
		/// <returns>whatever gets returned by the the actual script function</returns>
		public object execute(object[] parameters)
		{
			return this.owner.executeFunction(this.name, parameters);
		}
	}
}
