
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
			set {
				throw new NotImplementedException();
			}
		}
		public List<DB.ForeignKey> foreignKeys 
		{
			get 
			{
				return this.constraints.OfType<ForeignKey>().Cast<DB.ForeignKey>().ToList();
			}
			set {
				throw new NotImplementedException();
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
