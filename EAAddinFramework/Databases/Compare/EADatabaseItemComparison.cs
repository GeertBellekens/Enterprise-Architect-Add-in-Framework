
using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA=TSF.UmlToolingFramework.Wrappers.EA;
using DB=DatabaseFramework;
using DB_EA = EAAddinFramework.Databases;
using EAAddinFramework.Utilities;
using DatabaseFramework.Compare;

namespace EAAddinFramework.Databases.Compare
{
	/// <summary>
	/// Description of EADatabaseItemComparison.
	/// </summary>
	public class EADatabaseItemComparison:DatabaseItemComparison
	{
		DB.DatabaseItem _existingDatabaseItem;
		DB.DatabaseItem _newDatabaseItem;
		
		public EADatabaseItemComparison(DB.DatabaseItem newDatabaseItem, DB.DatabaseItem existingDatabaseItem)
		{
			this.newDatabaseItem = newDatabaseItem;
			this.existingDatabaseItem = existingDatabaseItem;
		}
		private void compare()
		{
			if (newDatabaseItem == null)
			{
				if (existingDatabaseItem != null)
				{
					comparisonStatus = DatabaseComparisonStatusEnum.deletedItem;
				}
				else
				{
					//both are null, so equal?
					comparisonStatus = DatabaseComparisonStatusEnum.equal;
				}
			}
			else
			{
				if (existingDatabaseItem == null)
				{
					comparisonStatus = DatabaseComparisonStatusEnum.newItem;
				}else
				{
					//both items exist
					if (existingDatabaseItem.name + existingDatabaseItem.properties == newDatabaseItem.name + newDatabaseItem.properties)
					{
						comparisonStatus = DatabaseComparisonStatusEnum.equal;
					}
					else if (existingDatabaseItem.isOverridden)
					{
						comparisonStatus = DatabaseComparisonStatusEnum.dboverride;
					}
					else
					{
						comparisonStatus = DatabaseComparisonStatusEnum.changed;
					}
				}
			}
		}

		#region DatabaseItemComparison implementation

		
		public DB.DatabaseItem newDatabaseItem {
			get {
				return _newDatabaseItem;
			}
			set {
				_newDatabaseItem = value;
				this.compare();
			}
		}

		
		public DB.DatabaseItem existingDatabaseItem {
			get {
				return _existingDatabaseItem;
			}
			set {
				_existingDatabaseItem = value;
				this.compare();
			}
		}

		public string itemType 
		{
			get 
			{
				if (this.newDatabaseItem != null) return this.newDatabaseItem.itemType;
				if (this.existingDatabaseItem != null) return this.existingDatabaseItem.itemType;
				return string.Empty;
			}
		}
		public string comparisonStatusName {
			get 
			{
				switch (this.comparisonStatus) 
				{
					case DatabaseComparisonStatusEnum.changed: 
						return "Changed";
					case DatabaseComparisonStatusEnum.deletedItem: 
						return "Deleted";
					case DatabaseComparisonStatusEnum.newItem: 
						return "New";
					case DatabaseComparisonStatusEnum.equal: 
						return "Equal";			
					case DatabaseComparisonStatusEnum.dboverride: 
						return "Overridden";								
					default:
						return "unknown";
				}
			}
		}
		public DatabaseComparisonStatusEnum comparisonStatus {get;set;}


		#endregion
	}
}
