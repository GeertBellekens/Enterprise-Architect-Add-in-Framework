
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
		/// <summary>
		/// copy the workingset tot the given user
		/// </summary>
		/// <param name="user">the user to copy the working set to</param>
		/// <param name="overwrite">if true then the first workingset found with the same name
		/// for the given user will be overwritten</param>
		public void copyToUser(User user, bool overwrite)
		{
			if (overwrite)
			{
				//check if a workingset with the same name already exists
				WorkingSet workingSetToOverwrite = this.model.workingSets.Find(w => 
				                                                    w.user != null
				                                                    && w.user.login == user.login 
				                                                    && w.name == this.name);
				if (workingSetToOverwrite != null)
				{
					workingSetToOverwrite.delete();
				}
			}
			string insertQuery = @"insert into t_document (DocID,DocName, Notes, Style,ElementID, ElementType,StrContent,BinContent,DocType,Author,DocDate )
								select '"+Guid.NewGuid().ToString("B")+@"',d.DocName, d.Notes, d.Style,
								d.ElementID, d.ElementType,d.StrContent,d.BinContent,d.DocType,'" + user.fullName + @"',d.DocDate from t_document d
								where d.DocID like '"+this.ID+"'";
			this.model.executeSQL(insertQuery);
		
		}
		/// <summary>
		/// deletes the workingset from the database
		/// </summary>
		void delete()
		{
			string deleteQuery = "delete from t_document where docid like '"+ this.ID + "'";
			this.model.executeSQL(deleteQuery);
		}
	}
}
