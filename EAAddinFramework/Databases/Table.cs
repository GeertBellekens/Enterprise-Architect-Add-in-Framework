
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
		private Class _wrappedClass;
		private Database _owner;
		private List<Column> _columns;
		private List<Constraint> _constraints;
		public Table(Database owner,Class wrappedClass)
		{
			this._wrappedClass = wrappedClass;
			this._owner = owner;
		}

		#region Table implementation

		public string name 
		{
			get { return this._wrappedClass.name;}
			set {this._wrappedClass.name = value;}
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
				return _constraints.Cast<DB.Constraint>().ToList();
			}
			set {
				throw new NotImplementedException();
			}
		}

		#endregion
		private void getColumnsFromAttributes()
		{
			_columns = new List<Column>();
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
