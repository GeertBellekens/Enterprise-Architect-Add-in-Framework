
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
	public class Database:DB.Database
	{
		internal Package _wrappedPackage;
		internal DatabaseFactory _factory;
		internal List<Table> _tables;
		public Database(Package package,DatabaseFactory factory)
		{
			this._wrappedPackage = package;
			this._factory = factory;
		}

		#region Database implementation

		public string name 
		{
			get 
			{
				return this._wrappedPackage.name;
			}
			set 
			{
				this._wrappedPackage.name = value;
			}
		}

		public DB.DataBaseFactory factory 
		{
			get 
			{
				return this._factory;
			}
			set 
			{
				this._factory = (DatabaseFactory) value;
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
				var newFactory = DatabaseFactory.getFactory(value);
				if (newFactory != null)
				{
					this.factory = newFactory;
				}
				else 
				{
					throw new ArgumentException(string.Format("Database type {0} is not known", value));
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
		private List<Table> getTablesFromPackage(Package package)
		{
			
			var foundTables = new List<Table>();
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
			return foundTables;
		}
		#endregion

	}
}
