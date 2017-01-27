
using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML = TSF.UmlToolingFramework.UML;

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
		public string _tableSpace;
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
		public  override List<UML.Classes.Kernel.Element> logicalElements {
			get 
			{
				return this.logicalClasses.Cast<UML.Classes.Kernel.Element>().ToList();
			}
			set
			{
				this._logicalClasses = value.Cast<Class>().ToList();
			}
		}
		internal bool compareOnly
		{
			get
			{	
				return ((Database)databaseOwner).compareonly;
			}
		}
		public string tableSpace {
			get 
			{
				if (_tableSpace == null)
				{
					//get the tagged value if it exists
					if (this.wrappedElement != null)
					{
						var tableSpaceTag = this.wrappedElement.taggedValues
							.FirstOrDefault( x => x.name.Equals("TableSpace",StringComparison.InvariantCultureIgnoreCase));
						_tableSpace = tableSpaceTag != null ? tableSpaceTag.tagValue.ToString() : null;
					}
				}
				return _tableSpace;
			}
			set
			{
				this._tableSpace = value;
				//create tagged value if needed if not overridden then we don't need the tagged value;
				if (this.wrappedElement != null)
				{
					wrappedElement.addTaggedValue("Tablespace",value);
				}
			}
		}

		public bool isAbstract
		{
			get
			{
				return this.logicalClasses.Any(x => x.isAbstract || x.generalizations.Any(y => x.Equals(y.target)));
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
					//set override and rename and tableSpace
					this.isOverridden = this.isOverridden;
					this.isRenamed = this.isRenamed;
					this.tableSpace = this.tableSpace;
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
					newTrace.addStereotype(this._factory._modelFactory.createStereotype(newTrace, "trace"));
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
			//remove from the database
			this.databaseOwner.removeTable(this);
		}	


		public override DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner, bool save = true)
		{
			Database existingDatabase = owner as Database;
			var newTable = new Table((Database)existingDatabase,this.name);
			newTable.logicalElements = this.logicalElements;
			newTable.derivedFromItem = this;
			newTable.tableSpace = this.tableSpace;
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
			//find exact match
			var correspondingColumn = this.columns.FirstOrDefault( x => x.name + x.properties == newColumn.name + newColumn.properties
			                                                       && !alreadyMappedColumns.Contains(x));
			//find based on the same logical elements
			if (correspondingColumn == null)
			{
				foreach (var mycolumn in this.columns) 
				{
					if (alreadyMappedColumns.All(x => x != mycolumn))
					{
						foreach (var logical in mycolumn.logicalElements) 
						{
							if (newColumn.logicalElements.Any(logical.Equals))
						    {
								//check if a better match has already been found
								if (correspondingColumn != null)
								{
									//if the name of the original corresponding column doesn't match and we find a better match based on the name
									if ( correspondingColumn.name != newColumn.name || correspondingColumn.isRenamed && correspondingColumn.renamedName == newColumn.name)
									{
										//there is a better match in name
										if (mycolumn.name == newColumn.name || mycolumn.isRenamed && mycolumn.renamedName == newColumn.name)
										{
											correspondingColumn = mycolumn;
										}
										else
										{
											//we have a better match in properties
											if (mycolumn.properties == newColumn.properties)
											{
												correspondingColumn = mycolumn;
											}
										}
									}
									else
									{
										//name corresponds
										if (correspondingColumn.properties != newColumn.properties)
										{
											//but there is a better match in properties
											if ((mycolumn.name == newColumn.name || mycolumn.isRenamed && mycolumn.renamedName == newColumn.name)
											    && mycolumn.properties == newColumn.properties)
											{
												correspondingColumn = mycolumn;
											}
										}
									}
									
								}
								else
								{
									correspondingColumn = mycolumn;
								}
								
						    }
						}
					}
				}
			}
			//if not found search again without the already mapped columns
			if (correspondingColumn == null && alreadyMappedColumns != null && alreadyMappedColumns.Count > 0) 
				correspondingColumn = getCorrespondingColumn(newColumn, new List<Column>());//try again without the list of already mapped columns
			return correspondingColumn as Column;
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
				if (!string.IsNullOrEmpty(_name) && _name != value) this.isRenamed = true;
				this._name = value;
				if (this._wrappedClass != null) this._wrappedClass.name = this._name;
				//set the alias
				if (this.logicalElement != null ) ((Class)this.logicalElement).alias = value;
			}
		}

		public void addColumn(DB.Column column)
		{
			//initialise columns
			if (this.columns != null) this._columns.Add((Column) column);
		}

		public void removeColumn(DB.Column column)
		{
			if (this.columns != null)this._columns.Remove((Column)column);
			foreach (var constraint in this.constraints.Where(x => x.involvedColumns.Contains(column)))
			{
				constraint.removeColumn(column);
			}
		}
		/// <summary>
		/// remove the given column from the table and replace it in constraints by the replacement columns
		/// </summary>
		/// <param name="column">the column to remove</param>
		/// <param name="replacement">the column to be used as replacement in constraints</param>
		public void removeColumn(DB.Column column, DB.Column replacement)
		{
			if (this.columns != null)this._columns.Remove((Column)column);
			foreach (var constraint in this.constraints.Where(x => x.involvedColumns.Contains(column)))
			{
				constraint.removeColumn(column,replacement);
			}
		}

		public void addConstraint(DB.Constraint constraint)
		{
			if (this.constraints != null) this._constraints.Add((Constraint) constraint);
		}

		public void removeConstraint(DB.Constraint constraint)
		{
			if (this.constraints != null) this._constraints.Remove((Constraint) constraint);
		}

    public DB.Constraint getConstraint(string name) {
      return this.constraints.FirstOrDefault(x => x.name == name);
    }
    
    public DB.Column getColumn(string name) {
      return this.columns.FirstOrDefault(x => x.name == name);
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
							//indexes and check constraints are not needed in compareOnly mode because they won't exist in the new database, and so we don't compare them.
							else if( ! compareOnly && operation.stereotypes.Any( x => x.name.Equals("index",StringComparison.InvariantCultureIgnoreCase)))
				            {
				                _constraints.Add(new Index(this, (Operation) operation));
				            }
							else if(! compareOnly && operation.stereotypes.Any( x => x.name.Equals("check",StringComparison.InvariantCultureIgnoreCase)))
				            {
				                _constraints.Add(new CheckConstraint(this, (Operation) operation));
				            }
						}
					}
				}
				return _constraints.Cast<DB.Constraint>().ToList();
			}
			set 
			{
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



	}
}
