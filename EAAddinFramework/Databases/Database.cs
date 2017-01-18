
using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of Database.
	/// </summary>
	public class Database:DatabaseItem, DB.Database
	{
		internal Package _wrappedPackage;
		internal List<Table> _tables;
		private string _name;
		private DatabaseFactory __factory;
		internal bool compareonly{get;set;}
		public Database(Package package,DatabaseFactory factory,bool compareOnly = false)
		{
			this._wrappedPackage = package;
			this.__factory = factory;
			this.compareonly = compareOnly;
		}
		public Database(string name,DatabaseFactory factory)
		{
			this._name = name;
			this.__factory = factory;
		}
		public override DB.DataBaseFactory factory {
			get 
			{
				return __factory;
			}
		}

		#region implemented abstract members of DatabaseItem


		public override bool isValid 
		{
			get 
			{
				//database is valid if all columns and constraints are valid
				return this.tables.All(x => x.isValid);
			}
		}

		#region implemented abstract members of DatabaseItem
		public override List<UML.Classes.Kernel.Element> logicalElements {
			get 
			{
				//TODO: return logical package if ever needed
				return new List<UML.Classes.Kernel.Element>();
			}
		}
		#endregion

		#endregion

		public void addTable(DB.Table table)
		{
			//initialize 
			int nbrOfTable = this.tables.Count;
			this._tables.Add(table as Table);
		}

		public void removeTable(DB.Table table)
		{
			this._tables.Remove((Table)table);
		}
		#region implemented abstract members of DatabaseItem
		internal override void createTraceTaggedValue()
		{
			//do nothing?
		}

		#region implemented abstract members of DatabaseItem


		protected override void updateDetails(DB.DatabaseItem newDatabaseItem)
		{
			//nothing extra to do here
		}

		#region implemented abstract members of DatabaseItem
		public override DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner, bool save = true)
		{
			//TODO: figure out how to handle creation of new databases
			return null;
		}
		#endregion

		#endregion

		internal override Element wrappedElement {
			get {
				return this._wrappedPackage;
			}
			set {
				this._wrappedPackage = (Package)value;
			}
		}
		internal override TaggedValue traceTaggedValue {
			get {
				//do we need any at this level?
				return null;
			}
			set 
			{
				//do nothing?
			}
		}
		public override DB.DatabaseItem owner 
		{
			get 
			{
				return null;
			}
		}
		#endregion
		public override void save()
		{
			if (this._wrappedPackage != null) this._wrappedPackage.save();
			//TODO: figure out some way to get a new database to be saved
		}
		public override void delete()
		{
			if (this._wrappedPackage != null) this._wrappedPackage.delete();
		}

		#region Database implementation

		public override string name 
		{
			get 
			{
				if (this._wrappedPackage != null)
				{
					this._name = this._wrappedPackage.name;
				}
				return this._name;
			}
			set 
			{
				this._name = value;
				if (this._wrappedPackage != null)
				{
					this._wrappedPackage.name = _name;
				}
			}
		}

		public override string itemType {
			get {return "Database";}
		}
		public override string properties {
			get {return this._factory.type;}
		}
		public DB.DataBaseFactory databaseFactory 
		{
			get 
			{
				return this._factory;
			}
			set
			{
				this.__factory = (DatabaseFactory)value;
			}
		}
		public string type 
		{
			get 
			{
				return this._factory.type;
			}
			set 
			{
				if (!value.Equals(this._factory.type, StringComparison.InvariantCultureIgnoreCase))
				{
					throw new NotImplementedException();
				}
			}
		}

		public List<DB.Table> tables 
		{
			get 
			{
				if (this._tables == null)
				{
					this._tables = this.getTablesFromPackage(this._wrappedPackage);
				}
				return this._tables.Cast<DB.Table>().ToList();
			}
			set 
			{
				throw new NotImplementedException();
			}
		}

		public DB.Table getCorrespondingTable(DB.Table externalTable)
		{
			var _externalTable = (Table) externalTable;
			//first check for exact match
			var correspondingTable = this.tables.FirstOrDefault(x => x.name == externalTable.name);
			if (correspondingTable == null)
			{
				//if no exact match found then get the one that is derived from the same logical classes
				foreach (Table table in this.tables) 
				{
					bool match = false;
					//each logical class of the external table should be equal to the logical class of this table
					foreach (var logicalClass in table.logicalClasses) 
					{
						if (_externalTable.logicalClasses.Any(x => x.Equals(logicalClass)))
					    {
							match = true;
					    }
						else
						{
							match = false;
						}
					}
					if (match)
					{
						//found it;
						correspondingTable = table;
						break;
					}
				}
			}
			return correspondingTable;
		}

    public DB.Table getTable(string name) {
      return this.tables.FirstOrDefault(x => x.name == name);
    }

		private List<Table> getTablesFromPackage(Package package)
		{
			
			var foundTables = new List<Table>();
			if (package != null)
			{
				var ownedElements = package.ownedElements;
				foreach( Class tableElement in ownedElements
									.Where(x => x is Class
				                    && x.stereotypes.Any( y => y.name.Equals("table", StringComparison.CurrentCultureIgnoreCase))))
				{
					foundTables.Add(new Table(this, tableElement));
				}
				//also check subPackages
				foreach (Package ownedPackage in ownedElements.OfType<Package>()) 
				{
					foundTables.AddRange(getTablesFromPackage(ownedPackage));
				}
			}
			return foundTables;
		}
		#endregion

		//Database can't be overriden
		public bool isOverridden 
		{
			get 
			{
				return false;
			}
			set {
				//do nothing
			}
		}

	}
}
