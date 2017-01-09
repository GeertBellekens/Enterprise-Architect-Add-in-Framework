
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
		public void updatePosition(int i)
		{
			if (newDatabaseItem != null) newDatabaseItem.position = i;
			if (existingDatabaseItem != null) existingDatabaseItem.position = i;
		}

		DatabaseItemComparison _ownerComparison;
		public DatabaseItemComparison ownerComparison {
			get {
				return _ownerComparison;
			}
			set {
				_ownerComparison = value;
			}
		}


		List<DatabaseItemComparison> _ownedComparisons = new List<DatabaseItemComparison>();
		public List<DatabaseItemComparison> ownedComparisons {
			get {
				return _ownedComparisons;
			}
			set {
				_ownedComparisons = value;
			}
		}

		public DatabaseItemComparison addOwnedComparison(DB.DatabaseItem existingItem, DB.DatabaseItem newItem)
		{
			var newComparison = new EADatabaseItemComparison(existingItem,newItem);
			this.ownedComparisons.Add(newComparison);
			newComparison.ownerComparison = this;
			return newComparison;
		}

		public void rename(string newName)
		{
			if (this.newDatabaseItem != null)
			{
				//don't rename new item if there are physical duplicates
				if (!hasPhysicalDuplicate())
				{
					this.newDatabaseItem.name = newName;
				}
				else
				{
					//in that case only rename the physical item
					if (this.existingDatabaseItem != null) this.existingDatabaseItem.name = newName;
					this.newDatabaseItem.isRenamed = true;
				}
			}
		}
		bool hasPhysicalDuplicate()
		{
			return this._ownerComparison.ownedComparisons.Any(x => x.existingDatabaseItem != null && !x.existingDatabaseItem.Equals(this._existingDatabaseItem) 
			                                           && x.newDatabaseItem != null &&  x.newDatabaseItem == this._newDatabaseItem);
		}
		public bool hasLogicalDuplicate()
		{
			return this._ownerComparison.ownedComparisons.Any(x => x.existingDatabaseItem != null && x.existingDatabaseItem == this._existingDatabaseItem 
			                                           && x.newDatabaseItem != null && ! x.newDatabaseItem.Equals(this._newDatabaseItem));
		}

		public void save(DB.Database existingDatabase)
		{
			switch (this.comparisonStatus) 
			{
				case DatabaseComparisonStatusEnum.equal:
					//make sure the translation sticks
					foreach (var logical in newDatabaseItem.logicalElements) 
					{
						if (logical != null) logical.save();
					}
					//make sure the position is saved
					this.existingDatabaseItem.save();
					break;
				case DatabaseComparisonStatusEnum.changed:
					this.existingDatabaseItem.update(this.newDatabaseItem);
					break;
				case DatabaseComparisonStatusEnum.dboverride:
					if (this.newDatabaseItem != null && this.newDatabaseItem.isOverridden)
					{
						if (this.existingDatabaseItem !=null) this.existingDatabaseItem.update(this.newDatabaseItem);
						else
						{
							//save the override tag?
							//TODO: check if this is needed
						}
					}
					break;
				case DatabaseComparisonStatusEnum.deletedItem:
					this.existingDatabaseItem.delete();
					break;					
				case DatabaseComparisonStatusEnum.newItem:
					if (ownerComparison != null
					   && ownerComparison.existingDatabaseItem != null)
					{
						this.newDatabaseItem.createAsNewItem(ownerComparison.existingDatabaseItem);
					}
					else
					{
						this.newDatabaseItem.createAsNewItem(existingDatabase);
					}
					break;				
			}

		}


		private void compare()
		{
			//rename new column if existing column was renamed
			var existingColumn = this.existingDatabaseItem as Column;
			var newColumn = this.newDatabaseItem as Column;
			if (existingColumn != null 
			    && existingColumn.isRenamed
			    && newColumn != null 
			    && newColumn.name != existingColumn.name)
			{
				newDatabaseItem.name = existingDatabaseItem.name;
			}
			//if the status is already overridden then don't bother
			if (this.comparisonStatus != DatabaseComparisonStatusEnum.dboverride)
			{
				if (newDatabaseItem == null)
				{
					if (existingDatabaseItem != null)
					{
						if (existingDatabaseItem.isOverridden)
						{
							comparisonStatus = DatabaseComparisonStatusEnum.dboverride;
						}
						else
						{
							comparisonStatus = DatabaseComparisonStatusEnum.deletedItem;
						}
					}
					else
					{
						//both are null, so equal?
						comparisonStatus = DatabaseComparisonStatusEnum.equal;
					}
				}
				else
				{
					//only new item exists
					if (existingDatabaseItem == null)
					{
						if (newDatabaseItem.isOverridden) 
						{
							comparisonStatus = DatabaseComparisonStatusEnum.dboverride;
						}
						else
						{
							comparisonStatus = DatabaseComparisonStatusEnum.newItem;
						}
					}
					else
					{
						//both items exist
						if (newDatabaseItem.isOverridden && existingDatabaseItem.isOverridden)
						{
							comparisonStatus = DatabaseComparisonStatusEnum.dboverride;
						}
						else if (existingDatabaseItem.name + existingDatabaseItem.properties == newDatabaseItem.name + newDatabaseItem.properties)
						{
							comparisonStatus = DatabaseComparisonStatusEnum.equal;
						}
						else if (existingDatabaseItem.isOverridden)
						{
							comparisonStatus = DatabaseComparisonStatusEnum.dboverride;
						}
						else if (newDatabaseItem.isOverridden) //is this realistic? this part may be removed
						{
							comparisonStatus = DatabaseComparisonStatusEnum.dboverride;
							this.existingDatabaseItem.update(this.newDatabaseItem, false);
						}
						else
						{
							comparisonStatus = DatabaseComparisonStatusEnum.changed;
						}
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
		public override bool Equals(object obj)
		{
			EADatabaseItemComparison other = obj as EADatabaseItemComparison;
			if (other == null) return false;
			bool existingEqual = false;
			bool newEqual = false;
			existingEqual = (this.existingDatabaseItem != null && this.existingDatabaseItem.Equals(other.existingDatabaseItem))
				|| this.existingDatabaseItem == null && other.existingDatabaseItem == null;
			newEqual = (this.newDatabaseItem != null && this.newDatabaseItem.Equals(other.newDatabaseItem))
				|| this.newDatabaseItem == null && other.newDatabaseItem == null;
			return existingEqual && newEqual;
		}

	}
}
