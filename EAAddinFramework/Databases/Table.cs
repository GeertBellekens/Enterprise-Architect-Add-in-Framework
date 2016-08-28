
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
	public class Table:DB.Table
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
			this.owner.addTable(this);
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
		public Column getCorrespondingColumn(Column newColumn)
		{
			var correspondingColumn = this._columns.FirstOrDefault( x => x.name + x.properties == newColumn.name + newColumn.properties);
			if (correspondingColumn == null) correspondingColumn = 
											this._columns.FirstOrDefault( x => x.logicalAttribute != null
				                						&& x.logicalAttribute.Equals(newColumn.logicalAttribute));
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

		public string name 
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

		public string itemType {
			get {return "Table";}
		}


		public string properties {
			get { return string.Empty;}
		}

		public DB.Database owner 
		{
			get {return this._owner;}
			set {this._owner = (Database)value;}
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
