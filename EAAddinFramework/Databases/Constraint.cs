
using System;
using System.Collections.Generic;
using EAAddinFramework.Databases.Strategy;
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

		public Constraint(Table owner,Operation operation, DatabaseItemStrategy strategy):base(strategy)
		{
			_owner = owner;
			_wrappedOperation = operation;
		}
		public Constraint(Table owner, List<Column> involvedColumns, DatabaseItemStrategy strategy):base(strategy)
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
		/// <summary>
		/// remove the given column from the involved columns
		/// </summary>
		/// <param name="column">the column to remove</param>
		public void removeColumn(DB.Column column)
		{
			if (involvedColumns.Contains(column))
			{
				_involvedColumns.Remove((Column)column);
			}
		}
		/// <summary>
		/// remove the given columns from the involved columns and replace it by the replacement
		/// </summary>
		/// <param name="column"></param>
		/// <param name="replacement"></param>
		public void removeColumn(DB.Column column, DB.Column replacement)
		{
			int columnIndex = -1;
			int replacementIndex = -1;
			for (int i = 0; i < this.involvedColumns.Count; i++) 
			{
				var currentColumn = involvedColumns[i];
				if (currentColumn == column) columnIndex = i;
				if (currentColumn == replacement) replacementIndex = i;
				if (columnIndex > -1 && replacementIndex > -1) break; //found both don't bother continuing
			}
			if (columnIndex > -1 && columnIndex != replacementIndex)
			{
				//check if the replacement is not already there
				if (replacementIndex > -1)
				{
					_involvedColumns.RemoveAt(columnIndex);
				}
				else
				{
					//replace column by its replacement
					this._involvedColumns[columnIndex] = (Column)replacement;
				}
				
			}
		}

		internal abstract override TaggedValue traceTaggedValue {get;set;}
		#endregion
		protected override void saveMe()
		{
			if (this._wrappedOperation == null)
			{
				this._wrappedOperation = this._factory._modelFactory.createNewElement<Operation>(this._owner.wrappedClass,this._name);
				this._wrappedOperation.setStereotype(getStereotype());
			}
			if (this._wrappedOperation != null)
			{
				this._wrappedOperation.save();
				this.isOverridden = this.isOverridden;
				this.isRenamed = this.isRenamed;
				//get the corresponding columns from this table
				this.involvedColumns = getCorrespondingColumns();
				if (this._wrappedOperation != null)
				{
					//first remove all existing parameters
					
					this._wrappedOperation.ownedParameters = new HashSet<UML.Classes.Kernel.Parameter>();
					uint parameterPosition = 0;
					foreach (var column in _involvedColumns) 
					{
						{
							ParameterWrapper parameter = this._wrappedOperation.EAModel.factory.createNewElement<Parameter>(this._wrappedOperation, column.name) as ParameterWrapper;
							parameter.type =  this._wrappedOperation.EAModel.factory.createPrimitiveType(column.type.name);
							parameter.position = parameterPosition;
							//TODO: this would be nicer if we could keep the parameters in memory and not save them directly
							parameter.save();
							parameterPosition++;
						}
					}
				}
			}
			//set all attributes to static
			foreach (Column involvedColumn in this.involvedColumns) 
			{
				if (involvedColumn.wrappedattribute != null
				   && ! involvedColumn.wrappedattribute.isStatic)
				{
					involvedColumn.wrappedattribute.isStatic = true;
					involvedColumn.wrappedattribute.save();
				}
			}
		}
		private List<DB.Column> getCorrespondingColumns()
		{
			var correspondingColumns = new List<DB.Column>();
			foreach (var involvedColumn in _involvedColumns) 
			{
				DB.Column correspondingColumn = involvedColumn;
				if (involvedColumn.ownerTable != this.ownerTable) 
				{
					correspondingColumn = this.ownerTable.columns.FirstOrDefault(x => x.name == involvedColumn.name);
				}
				if (correspondingColumn != null)
				{
					correspondingColumns.Add(correspondingColumn);
				}
			}
			return correspondingColumns;
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


		protected override void deleteMe()
		{
			if (this._wrappedOperation != null) this._wrappedOperation.delete();
			this.ownerTable.removeConstraint(this);
		}

		public override bool isOverridden 
		{
			get 
			{
				if (!_isOverridden.HasValue)
				{
					//get the initial override
					if (this.involvedColumns.Any(x => ((Column)x).getInitialOverride()))
					{
						_isOverridden = true;	
					}

				}
				return base.isOverridden;
			}
			set 
			{
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
						+ string.Join(", ",this.involvedColumns.Select( x => x.name).ToArray())
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

    public Column getInvolvedColumn(string name) {
      return (Column)this.involvedColumns.FirstOrDefault(x => x.name == name);
    }

		private void getInvolvedColumns()
		{
			_involvedColumns = new List<Column>();
			if (this._wrappedOperation != null)
			{
				foreach (var parameter in this._wrappedOperation.ownedParameters.OrderBy(x => x.position))
				{
					Column involvedColumn = _owner.columns.FirstOrDefault(x => x.name == parameter.name) as Column;
					if (involvedColumn != null)
					{
						_involvedColumns.Add(involvedColumn);
					}
				}
			}
		}

    // mark a column as included by adding it's GUID to "includes" TaggedValue
    public void markAsIncluded(Column column) {
      if( this.wrappedElement != null ) {
        String guid = column.wrappedattribute.guid;
        TaggedValue tag = this.wrappedElement.getTaggedValue("includes");
        if(tag == null) {
          this.wrappedElement.addTaggedValue("includes", guid);
        } else {
          this.wrappedElement.addTaggedValue(
            "includes",
            tag.tagValue.ToString() + "," + guid
          );
        }
      }
    }

		#endregion

		
	}
}
