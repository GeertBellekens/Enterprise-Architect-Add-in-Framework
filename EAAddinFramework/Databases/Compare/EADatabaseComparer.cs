
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
	/// Description of EADatabaseComparer.
	/// </summary>
	public class EADatabaseComparer:DatabaseComparer
	{
		internal Database _newDatabase;
		internal Database _existingDatabase;
		public EADatabaseComparer(Database newDatabase, Database existingDatabase)
		{
			this._newDatabase = newDatabase;
			this._existingDatabase = existingDatabase;
		}
		#region DatabaseComparer implementation
		public void save()
		{
			//both existing and new database should exist to even begin
			if (_existingDatabase == null || _newDatabase == null) throw new Exception("Both existign and new database should exist in order to save the database!");
			//first save all tables
			foreach (var tableCompareItem in this.comparedItems.Where(x => x.itemType.Equals("Table",StringComparison.InvariantCultureIgnoreCase)))
			{
				tableCompareItem.save(_existingDatabase);
			}
			//then save all columns
			foreach (var columnCompareItem in this.comparedItems.Where(x => x.itemType.Equals("Column",StringComparison.InvariantCultureIgnoreCase)))
			{
				columnCompareItem.save(_existingDatabase);
			}
			//then save all primary keys
			foreach (var PKCompareItem in this.comparedItems.Where(x => x.itemType.Equals("Primary Key",StringComparison.InvariantCultureIgnoreCase)))
			{
				PKCompareItem.save(_existingDatabase);
			}
			//then save all foreign keys
			foreach (var FKCompareItem in this.comparedItems.Where(x => x.itemType.Equals("Foreign Key",StringComparison.InvariantCultureIgnoreCase)))
			{
				FKCompareItem.save(_existingDatabase);
			}
		}

		public void compare()
		{
			this.comparedItems = new List<DatabaseItemComparison>();
			//compare each new table
			foreach (var newTable in _newDatabase.tables) 
			{
				Table existingTable = null;
				if (_existingDatabase != null) existingTable = (Table)_existingDatabase.getCorrespondingTable(newTable);
				var comparedItem = new EADatabaseItemComparison(newTable,existingTable);
				addToComparison(comparedItem);
				this.addComparisonDetails(comparedItem);
			}
			if (_existingDatabase != null)
			{
				//get existing tables that don't exist in the new database
				foreach (var existingTable in _existingDatabase.tables) 
				{
					//if the existingTable does not exist in the comparedItems then add it.
					if (! comparedItems.Any(x => x.existingDatabaseItem == existingTable))
					{
						var comparedItem = new EADatabaseItemComparison(null,existingTable);
						addToComparison(comparedItem);
						this.addComparisonDetails(comparedItem);
					}
				}
			}	
		}
		private void addToComparison(DatabaseItemComparison comparedItem)
		{
			//if new item alrady compared then make it null
			if (comparedItems.Any(x => x.newDatabaseItem == comparedItem.newDatabaseItem))
			{
				comparedItem.newDatabaseItem = null;
			}
			//if existing item already compared then make it null
			if ( comparedItems.Any(x => x.existingDatabaseItem == comparedItem.existingDatabaseItem))
			{
				comparedItem.existingDatabaseItem = null;
			}
			//only add the commparison if not both of them are null
			if (comparedItem.existingDatabaseItem != null 
			    || comparedItem.newDatabaseItem != null)
			{
				comparedItems.Add(comparedItem);
			}
		}
		private void addToComparison(List<DatabaseItemComparison> addedComparedItems)
		{
			foreach (var comparedItem in addedComparedItems) 
			{
				addToComparison( comparedItem);
			}
		}
		public void addComparisonDetails(EADatabaseItemComparison comparedItem)
		{
			List<DatabaseItemComparison> addedComparedItems = new List<DatabaseItemComparison>();
			var existingTable = comparedItem.existingDatabaseItem as Table;
			var newTable = comparedItem.newDatabaseItem as Table;
			if (existingTable != null)
			{
				//add all existing columns
				foreach (Column existingColumn in existingTable.columns) 
				{
					Column newColumn = null;
					if (newTable != null) newColumn = newTable.getCorrespondingColumn(existingColumn);
					addedComparedItems.Add(new EADatabaseItemComparison(newColumn,existingColumn));
				}
				//now the new columns that don't have a corresponding existing column
				if (newTable != null)
				{
					foreach (Column newColumn in newTable.columns) 
					{
						if (! addedComparedItems.Any(x => x.newDatabaseItem == newColumn))
						{
							addedComparedItems.Add(new EADatabaseItemComparison(newColumn, null));
						}
					}
				}
				//add all existing constraints
				foreach (var existingConstraint in existingTable.constraints) 
				{
					Constraint newConstraint = null;
					if (newTable != null)
					{
						if (existingConstraint is ForeignKey)
						{
							newConstraint = newTable.getCorrespondingForeignKey((ForeignKey)existingConstraint);
						}
						else
						{
							newConstraint = (Constraint)newTable.primaryKey ;
						}
						addedComparedItems.Add(new EADatabaseItemComparison(newConstraint,existingConstraint));
					}
				}
				//then add the new constraints don't have a corresponding existing constraint
				if (newTable != null)
				{
					foreach (var newConstraint in newTable.constraints) 
					{
						if (! addedComparedItems.Any(x => x.newDatabaseItem == newConstraint))
						{
							addedComparedItems.Add(new EADatabaseItemComparison(newConstraint, null));
						}
					}
				}
			}
			else
			{
				//no existing table, everything is new
				foreach (var newColumn in newTable.columns) 
				{
					addedComparedItems.Add(new EADatabaseItemComparison(newColumn,null));
				}
				foreach (var newConstraint in newTable.constraints) 
				{
					addedComparedItems.Add(new EADatabaseItemComparison(newConstraint,null));
				}
			}
			
			//add the new compared items
			addToComparison(addedComparedItems);
		}
		public DB.Database newDatabase {
			get { return _newDatabase;}
			set {this._newDatabase = (Database)value;}
		}

		public DB.Database existingDatabase {
			get { return _existingDatabase;}
			set {this._existingDatabase = (Database)value;}
		}

		public List<DatabaseItemComparison> comparedItems { get;set;}

		#endregion
	}
}
