
using System;
using EAWrappers = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.EASpecific
{
	/// <summary>
	/// Description of WorkingSet.
	/// </summary>
	public class WorkingSet
	{
		public User user {get;set;}
		public string name {get;set;}
		public EAWrappers.Model model {get;set;}
		public string ID {get;set;}
		
		public WorkingSet(EAWrappers.Model model,string ID,User user, string name)
		{
			this.model = model;
			this.ID = ID;
			this.user = user;
			this.name = name;
		}
		public void copyToUser(User user)
		{
			string insertQuery = @"insert into t_document (DocID,DocName, Notes, Style,ElementID, ElementType,StrContent,BinContent,DocType,Author,DocDate )
								select '"+Guid.NewGuid().ToString("B")+@"',d.DocName, d.Notes, d.Style,
								d.ElementID, d.ElementType,d.StrContent,d.BinContent,d.DocType,'" + user.firstName +" "+ user.lastName + @"',d.DocDate from t_document d
								where d.DocID like '"+this.ID+"'";
			this.model.executeSQL(insertQuery);
		}
	}
}
