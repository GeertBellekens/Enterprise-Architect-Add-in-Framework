
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
		string _renamedName = string.Empty;
		public string renamedName 
		{
			get 
			{
				return _renamedName;
			}
		}
		bool? _isRenamed = null;

		public bool isRenamed 
		{
			get
			{
				if (! _isRenamed.HasValue)
				{
					determineRenamed();
				}
				if (_isRenamed.HasValue) return _isRenamed.Value;
				return false;
			}
		}
		private void determineRenamed()
		{
			//check if the existing item is renamed
			if (this.newDatabaseItem != null && this.newDatabaseItem.isRenamed)
			{
				_isRenamed = true;
				this._renamedName = this.newDatabaseItem.renamedName;
			}
			if (this.existingDatabaseItem != null && this.existingDatabaseItem.isRenamed)
			{
				_isRenamed = true;
				this._renamedName = this.existingDatabaseItem.renamedName;
				if (this.newDatabaseItem != null && !hasPhysicalDuplicate())
				{
					this.newDatabaseItem.name = _renamedName;
					this.newDatabaseItem.isRenamed = true;
					this.newDatabaseItem.renamedName = _renamedName;
				}
			}

		}
		
		public EADatabaseItemComparison(DB.DatabaseItem newDatabaseItem, DB.DatabaseItem existingDatabaseItem)
		{
			this._newDatabaseItem = newDatabaseItem;
			this._existingDatabaseItem = existingDatabaseItem;
			//determine if this item was renamed
			determineRenamed();
			//compare the new against the existing item
			this.compare();
		}
		public void updatePosition(int i)
		{
			if (existingDatabaseItem != null
			   && existingDatabaseItem.position != i)
			{
				existingDatabaseItem.position = i;
				existingDatabaseItem.isEqualDirty = true;
			}
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
					this.newDatabaseItem.isRenamed = true;
					this.newDatabaseItem.renamedName = newName;
				}
				//set renamed
				this._isRenamed = true;
				this._renamedName =newName;
				if (this.existingDatabaseItem != null )
				{
					this.existingDatabaseItem.isRenamed = true;
					this.existingDatabaseItem.renamedName = newName;
				}

			}
		}
		bool hasPhysicalDuplicate()
		{
			return this.ownerComparison != null 
			        && this.ownerComparison.ownedComparisons != null
			        && this._ownerComparison.ownedComparisons.Any(x => x.existingDatabaseItem != null && !x.existingDatabaseItem.Equals(this._existingDatabaseItem)
			                                           && x.newDatabaseItem != null &&  x.newDatabaseItem == this._newDatabaseItem);
		}


		public void save(DB.Database existingDatabase)
		{
			//take care of the renames
			if (this.isRenamed && this.existingDatabaseItem != null)
			{
				this.existingDatabaseItem.name = this.renamedName;
			}
			switch (this.comparisonStatus) 
			{
				case DatabaseComparisonStatusEnum.equal:
					//make sure the translation sticks
					if (newDatabaseItem.isEqualDirty)
					{
						foreach (var logical in newDatabaseItem.logicalElements) 
						{
							if (logical != null) logical.save();
						}
					}
					if (existingDatabaseItem.isEqualDirty)
					{
						//make sure the position is saved
						this.existingDatabaseItem.save();
					}
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
						if (isRenamed)
						{
							this.newDatabaseItem.createAsNewItem(ownerComparison.existingDatabaseItem, this.renamedName);
						}
						else
						{
							this.newDatabaseItem.createAsNewItem(ownerComparison.existingDatabaseItem);
						}
					}
					else
					{
						if (this.isRenamed)
						{
							this.newDatabaseItem.createAsNewItem(existingDatabase, this.renamedName);
						}
						else
						{
							this.newDatabaseItem.createAsNewItem(existingDatabase);
						}
					}
					break;				
			}

		}


		private void compare()
		{
			//rename new column if existing column was renamed
			var existingColumn = this.existingDatabaseItem as Column;
			var newColumn = this.newDatabaseItem as Column;
//			if (existingColumn != null 
//			    && existingColumn.isRenamed
//			    && newColumn != null 
//			    && newColumn.name != existingColumn.name)
//			{
//				newDatabaseItem.name = existingDatabaseItem.name;
//			}
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
						if (newDatabaseItem.isNotRealized || newDatabaseItem.isOverridden)
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
						//in case of a constraint we follow the database when it comes to the order of the involved columns
						fixInvolvedColumns();

						//both items exist
						if (newDatabaseItem.isOverridden && existingDatabaseItem.isOverridden)
						{
							comparisonStatus = DatabaseComparisonStatusEnum.dboverride;
						}
						else if (this.isRenamed
						        && this.existingDatabaseItem.name + existingDatabaseItem.properties == this.renamedName + newDatabaseItem.properties) 
						{
							comparisonStatus = DatabaseComparisonStatusEnum.equal;
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
		//fix the order of the columns of the constraints. In case both new and existing constraint exist, the order of the existing item is leading
		void fixInvolvedColumns()
		{
			var existingConstraint = this.existingDatabaseItem as Constraint;
			var newConstraint = this.newDatabaseItem as Constraint;
			//first replace the equivalent columns
			if (existingConstraint != null
			    && newConstraint != null)
			{
				this.replaceEquivalentColumns(existingConstraint, newConstraint);
				//only if both constraints exist and they both have the same columns
				if (existingConstraint.involvedColumns.Count == newConstraint.involvedColumns.Count
				    && existingConstraint.involvedColumns.All(x => newConstraint.involvedColumns.Any(y => y.name == x.name)))
				{
					var newInvolvedColumns = new List<DB.Column>();
					foreach (var existingInvolvedColumn in existingConstraint.involvedColumns) 
					{
						//get the corresponding new column
						var newcolumn = newConstraint.involvedColumns.First(x => x.name == existingInvolvedColumn.name);
						newInvolvedColumns.Add(newcolumn);
					} 
					//set the involved columns in the correct order
					newConstraint.involvedColumns = newInvolvedColumns;
				}
			}
		}
		/// <summary>
		/// In case of equivalent columns (both columns derived from the same logical elements) the existing constraint is leading. 
		/// So we replace the involved column by its equivalent following the columns in the existing constraint.
		/// </summary>
		/// <param name="existingConstraint">the constraint from the existing database</param>
		/// <param name="newConstraint">the newly derived constraint</param>
		void replaceEquivalentColumns(Constraint existingConstraint, Constraint newConstraint)
		{
			List<Column> usedExistingColumns = new List<Column>();
			List<DB.Column> newInvolvedColumns = newConstraint.involvedColumns;
			bool involvedColumnsUpdated = false;
			foreach (Column newColumn in newConstraint.involvedColumns)
			{
				//find corresponding column in existing constraint
				//first find columns with same name
				var correspondingColumn = existingConstraint.involvedColumns.FirstOrDefault(x => x.name == newColumn.name
				                                                                    && !usedExistingColumns.Contains(x));
				//then find the column with the same logical item
				if (correspondingColumn == null)
				{
					foreach (Column existingInvolvedColumn in existingConstraint.involvedColumns.Where(x => !usedExistingColumns.Contains(x)))
					{
						bool hasEquivalentLogicalElements = existingInvolvedColumn.logicalElements.All(x => newColumn.logicalElements.Any(y => y.Equals(x)));
						if (hasEquivalentLogicalElements)
						{
							correspondingColumn = existingInvolvedColumn;
							//replace the involvedColumn by the equivalent column in the new table
							var correspondingNewColumn = newColumn._ownerTable.columns.FirstOrDefault(x => x.name == correspondingColumn.name);
							if (correspondingNewColumn != null)
							{
								//replace the column
								newInvolvedColumns[newConstraint.involvedColumns.IndexOf(newColumn)] = correspondingNewColumn;
								involvedColumnsUpdated = true;
								break;
							}
						}
					}
				}
				if (correspondingColumn != null)
				{
					//add it to the list of columns that have been used
					usedExistingColumns.Add((Column)correspondingColumn);
				}
			}
			//if we changed anything then set the involved columns on the new constraint
			if (involvedColumnsUpdated)
			{
				newConstraint.involvedColumns = newInvolvedColumns;
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
