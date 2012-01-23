
using System;
using EAWrappers = TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of User.
	/// </summary>
	public class User
	{
		public EAWrappers.Model model {get;set;}
		public string login {get;set;}
		public string firstName {get;set;}
		public string lastName {get;set;}
		public string fullName
		{
			get
			{
				if (this.model.isSecurityEnabled)
				{
					return this.firstName + " " + this.lastName;
				}
				else
				{
					return this.login;
				}
			}
		}
		
		/// <summary>
		/// creates a new user based on the given details
		/// </summary>
		/// <param name="model">the model containing the user</param>
		/// <param name="login">the string used to log in into the tool</param>
		/// <param name="firstName">the first name of the user</param>
		/// <param name="lastName">the last name of the user</param>
		public User(EAWrappers.Model model,string login,string firstName,string lastName)
		{
			this.model = model;
			this.login = login;
			this.firstName = firstName;
			this.lastName = lastName;
		}
	}
}
