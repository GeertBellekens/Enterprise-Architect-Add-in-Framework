
using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML=TSF.UmlToolingFramework.UML;

using System.Linq;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of Constraint.
	/// </summary>
	public abstract class Constraint:DatabaseItem,DB.Constraint
	{
		internal Operation _wrappedOperation;
		internal Table _owner;
		internal string _name;
		protected List<Column> _involvedColumns;
		public Constraint(Table owner,Operation operation)
		{
			_owner = owner;
			_wrappedOperation = operation;
		}
		public Constraint(Table owner, List<Column> involvedColumns)
		{
			_owner = owner;
			_involvedColumns = involvedColumns;
			this.ownerTable.addConstraint(this);
		}

		#region implemented abstract members of DatabaseItem
		protected override void updateDetails(DB.DatabaseItem newDatabaseItem)
		{
			var newConstraint = (Constraint)newDatabaseItem;
			this.involvedColumns = newConstraint.involvedColumns;
		}
		
		internal abstract override TaggedValue traceTaggedValue {get;set;}
		#endregion
		public override void save()
		{
			if (this._wrappedOperation == null)
			{
				this._wrappedOperation = this._factory._modelFactory.createNewElement<Operation>(this._owner._wrappedClass,this._name);
				this._wrappedOperation.setStereotype(getStereotype());
			}
			if (this._wrappedOperation != null)
			{
				this._wrappedOperation.save();
				//get the corresponding columns from this table
				this.involvedColumns = this.ownerTable.columns.Where (x => _involvedColumns.Any(y => y.name == x.name)).ToList();
				if (this._wrappedOperation != null)
				{
					//first remove all existing parameters
					
					this._wrappedOperation.ownedParameters = new HashSet<UML.Classes.Kernel.Parameter>();
					foreach (var column in _involvedColumns) 
					{
						{
							ParameterWrapper parameter = this._wrappedOperation.model.factory.createNewElement<Parameter>(this._wrappedOperation, column.name) as ParameterWrapper;
							parameter.type = ((Column)column)._wrappedattribute.type;
							//TODO: this would be nicer if we could keep the parameters in memory and not save them directly
							parameter.save();
						}
					}
				}
			}
		}

		#region implemented abstract members of DatabaseItem


		public override bool isValid 
		{
			get
			{
				//valid if there's no other constraint with the same name
				// and if there is at least one involved column
				return (this.involvedColumns.Count > 0 
				        && ownerTable.constraints.Count( x => x.name == this.name) == 1);
			}
		}


		#endregion

		protected abstract string getStereotype();


		public override void delete()
		{
			if (this._wrappedOperation != null) this._wrappedOperation.delete();
		}

		public override bool isOverridden {
			get 
			{
				return base.isOverridden || this.involvedColumns.Any(x => x.isOverridden);
			}
			set {
				base.isOverridden = value;
			}
		}

		#region implemented abstract members of DatabaseItem
		internal abstract override void createTraceTaggedValue();
		internal override Element wrappedElement {
			get {
				return this._wrappedOperation;
			}
			set {
				this._wrappedOperation = (Operation)value;
			}
		}
		#endregion
		#region Constraint implementation

		public override string name 
		{
			get 
			{
				if (_wrappedOperation != null)
				{
					return _wrappedOperation.name;
				}
				else
				{
					return _name;
				}
			}
			set 
			{
				if (_wrappedOperation != null)
				{
					_wrappedOperation.name = value;
				}
				this._name = value;
			}
		}

		public DB.Table ownerTable {
			get {
				return _owner;
			}
			set {
				throw new NotImplementedException();
			}
		}
		public override DB.DatabaseItem owner 
		{
			get 
			{
				return _owner;
			}
		}

		public override string itemType 
		{
			get {return "Constraint";}
		}


		public override string properties 
		{
			get 
			{
				string _properties = string.Empty;
				if (this.involvedColumns.Count > 0)
				{
					_properties += " (" 
						+ string.Join(", ",this.involvedColumns.Select( x => x.name).ToArray().OrderBy(x => x.ToString()))
						+ ")";
				}
				return _properties;
			}
		}

		public List<DB.Column> involvedColumns 
		{
			get 
			{
				if (_involvedColumns == null)
				{
					this.getInvolvedColumns();
				}
				return _involvedColumns.Cast<DB.Column>().ToList();
			}
			set 
			{
				_involvedColumns = value.Cast<Column>().ToList();

			}
		}
		
		private void getInvolvedColumns()
		{
			_involvedColumns = new List<Column>();
			if (this._wrappedOperation != null)
			{
				foreach (var parameter in this._wrappedOperation.ownedParameters) 
				{
					Column involvedColumn = _owner.columns.FirstOrDefault(x => x.name == parameter.name) as Column;
					if (involvedColumn != null)
					{
						_involvedColumns.Add(involvedColumn);
					}
				}
			}
		}
		#endregion

	}
}
