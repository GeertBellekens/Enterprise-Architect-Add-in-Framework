
using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of Table.
	/// </summary>
	public class Table:DatabaseItem,DB.Table
	{
		internal Class _wrappedClass;
		internal Database _owner;
		internal List<Column> _columns;
		internal List<Constraint> _constraints;
		internal List<Class> _logicalClasses;
		private string _name;
		public Table(Database owner,Class wrappedClass)
		{
			this._wrappedClass = wrappedClass;
			this._owner = owner;
		}
		public Table(Database owner, string name)
		{
			this._name = name;
			this._owner = owner;
			this.databaseOwner.addTable(this);
		}
		public override TSF.UmlToolingFramework.UML.Classes.Kernel.Element logicalElement {
			get {
				return this.logicalClasses.FirstOrDefault();
			}
		}
		#region implemented abstract members of DatabaseItem
		internal override void createTraceTaggedValue()
		{
			//don't think that's used here?
			throw new NotImplementedException();
		}
		protected override void updateDetails(DB.DatabaseItem newDatabaseItem)
		{
			//nothing extra to do there
		}
		internal override Element wrappedElement {
			get {
				return _wrappedClass;
			}
			set {
				this._wrappedClass = (Class)value;
			}
		}
		internal override TaggedValue traceTaggedValue {
			get {
				return null;
			}
			set {
				// do nothing, not used here
			}
		}
		#endregion
		private int _position;
		public override int position 
		{
			get 
			{
				if (_wrappedClass != null)
				{
					_position = _wrappedClass.position;
				}
				return _position;
			}
			set 
			{
				_position = value;
				if (_wrappedClass != null)
				{
					_wrappedClass.position =_position;
				}
			}
		}

		public DB.Database databaseOwner {
			get {
				return _owner;
			}
			set {
				
				_owner = (Database)value;
			}
		}

		#region implemented abstract members of DatabaseItem
		public override bool isValid 
		{
			get 
			{
				//table is valid if it has a name
				// there is no other table with the same name
				// all columns and constraints are valid
				return (! string.IsNullOrEmpty(this.name)
				        && this.databaseOwner.tables.Count(x => x.name == this.name) == 1
				        && this.columns.All(x => x.isValid)
				        && this.constraints.All(x => x.isValid));
			}
		}
		#endregion


		public override void save()
		{
			if (_wrappedClass == null) 
			{
				var databasePackage = this._owner._wrappedPackage;
				
				if (databasePackage != null)
				{
					Package tablePackage = databasePackage.ownedElements.OfType<Package>()
						.FirstOrDefault(x => x.name.Equals("Tables",StringComparison.InvariantCultureIgnoreCase));
					Package ownerPackage = databasePackage;
					if (tablePackage != null) ownerPackage = tablePackage;
					_wrappedClass = this._factory._modelFactory.createNewElement<Class>(ownerPackage, this.name);
					//TODO: provide wrapper function for gentype?
					_wrappedClass.wrappedElement.Gentype = this.factory.databaseName;
					_wrappedClass.setStereotype("table");
					_wrappedClass.save();
					//add trace relation to logical class(ses)
					setTracesToLogicalClasses();
				}
				else
				{
					throw new Exception(string.Format("cannot save {0} because wrapped pacakge for database {1} does not exist",this.name, this.databaseOwner.name));
				}
				
			}
			
		}
		private void setTracesToLogicalClasses()
		{
			foreach (var logicalClass in this._logicalClasses) 
			{
				if (!this._wrappedClass.relationships.OfType<Abstraction>().Any(x => x.stereotypes
				           .Any(y => y.name.Equals("trace",StringComparison.InvariantCultureIgnoreCase)
				                                      && logicalClass.Equals(((ConnectorWrapper)y).target))))
	           {
					var newTrace = this._factory._modelFactory.createNewElement<Abstraction>(_wrappedClass, string.Empty);
					newTrace.target = logicalClass;
					newTrace.save();
	           }
				//save logical class to save the tablename in the alias
				logicalClass.save();
			}
			
			
		}
		public override void delete()
		{
			if (_wrappedClass != null) _wrappedClass.delete();
		}	


		public override DB.DatabaseItem createAsNewItem(DB.Database existingDatabase, bool save = true)
		{
			var newTable = new Table((Database)existingDatabase,this.name);
			newTable._logicalClasses = new List<Class>(_logicalClasses);
			if (save) newTable.save(); 
			return newTable;
		}
	
		public List<Class> logicalClasses
		{
			get
			{
				if (_logicalClasses == null)
				{
					_logicalClasses = new List<Class>();
					//check if wrapped class exists
					if (_wrappedClass != null)
					{
						_logicalClasses.AddRange( _wrappedClass.relationships.OfType<Abstraction>()
						                         .Where(x => x.target is Class 
						                                && x.stereotypes.Any( z => z.name.Equals("trace",StringComparison.InvariantCultureIgnoreCase)))
												.Select(y => y.target).Cast<Class>());
					}
				}
				return _logicalClasses;
			}
			set
			{
				_logicalClasses = value;
			}
		}
		/// <summary>
		/// gets te columns that corresponds to the given column.
		/// If the properties are exactly the same then this column is returned. 
		/// If not the columns that is based on the same logical attribute is returned.
		/// </summary>
		/// <param name="newColumn">the column to compare to</param>
		/// <returns>the corresponding column</returns>
		public Column getCorrespondingColumn(Column newColumn, List<Column> alreadyMappedColumns)
		{
			var correspondingColumn = this._columns.FirstOrDefault( x => x.name + x.properties == newColumn.name + newColumn.properties
			                                                       && !alreadyMappedColumns.Contains(x));
			if (correspondingColumn == null) correspondingColumn = 
											this._columns.FirstOrDefault( x => x.logicalAttribute != null
				                						&& x.logicalAttribute.Equals(newColumn.logicalAttribute)
				                						&& !alreadyMappedColumns.Contains(x));
			if (correspondingColumn == null && alreadyMappedColumns != null && alreadyMappedColumns.Count > 0) 
				correspondingColumn = getCorrespondingColumn(newColumn, new List<Column>());//try again without the list of already mapped columns
			return correspondingColumn;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newForeignKey"></param>
		/// <returns></returns>
		public ForeignKey getCorrespondingForeignKey(ForeignKey newForeignKey)
		{
			var correspondingForeignKey = this.constraints.OfType<ForeignKey>()
							.FirstOrDefault( x => x.name + x.properties == newForeignKey.name + newForeignKey.properties);
			if (correspondingForeignKey == null) correspondingForeignKey = this.constraints.OfType<ForeignKey>()
				.FirstOrDefault( x => x.logicalAssociation != null && x.logicalAssociation.Equals(newForeignKey.logicalAssociation));
			return correspondingForeignKey;
		}
		#region Table implementation

		public override string name 
		{
			get 
			{
				if (this._wrappedClass != null) this._name = this._wrappedClass.name;
				return this._name;
			}
			set 
			{
				this._name = value;
				if (this._wrappedClass != null) this._wrappedClass.name = this._name;
			}
		}

		public void addColumn(DB.Column column)
		{
			//initialise columns
			if (this.columns != null) this._columns.Add((Column) column);
		}

		public void addConstraint(DB.Constraint constraint)
		{
			if (this.constraints != null) this._constraints.Add((Constraint) constraint);
		}

		public override string itemType {
			get {return "Table";}
		}


		public override string properties {
			get { return string.Empty;}
		}

		public override DB.DatabaseItem owner 
		{
			get {return this._owner;}
		}
		public List<DB.Column> columns 
		{
			get 
			{
				if (_columns == null)
				{
					this.getColumnsFromAttributes();
				}
				return _columns.Cast<DB.Column>().ToList();
			}
			set 
			{
				this._columns = value.Cast<Column>().ToList();
			}
		}
		public List<DB.Constraint> constraints 
		{
			get 
			{
				if (_constraints == null)
				{
					_constraints = new List<Constraint>();
					if (this._wrappedClass != null)
					{
						foreach (var operation in this._wrappedClass.ownedOperations)
						{
							if (operation.stereotypes.Any(x=> x.name.Equals("PK",StringComparison.InvariantCultureIgnoreCase)))
							{
								_constraints.Add(new PrimaryKey(this,(Operation) operation));
							}
							else if (operation.stereotypes.Any(x=> x.name.Equals("FK",StringComparison.InvariantCultureIgnoreCase)))
							{
								_constraints.Add(new ForeignKey(this, (Operation) operation));
							}
							
						}
					}
				}
				return _constraints.Cast<DB.Constraint>().ToList();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public DB.PrimaryKey primaryKey 
		{
			get 
			{
				return this.constraints.FirstOrDefault(x => x is PrimaryKey) as PrimaryKey;
			}
			set 
			{
				if (this.primaryKey != null)
				{
					this.constraints.Remove(this.primaryKey);
				}
				this.constraints.Add(value);
			}
		}
		public List<DB.ForeignKey> foreignKeys 
		{
			get 
			{
				return this.constraints.OfType<ForeignKey>().Cast<DB.ForeignKey>().ToList();
			}
			set 
			{
				this.constraints.RemoveAll(x => x is ForeignKey);
				this.constraints.AddRange(value);
			}
		}
		#endregion
		private void getColumnsFromAttributes()
		{
			_columns = new List<Column>();
			if( this._wrappedClass != null)
			{
				foreach (TSF_EA.Attribute attribute in this._wrappedClass.attributes) 
				{
					if (attribute.stereotypes.Any( x => x.name.Equals("column", StringComparison.CurrentCultureIgnoreCase)))
				    {
						_columns.Add( new Column(this,attribute));
				    }
				}
			}
		}


		//Table can't be overriden
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
