
using System;
using System.Collections.Generic;
using EAAddinFramework.Utilities;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of ForeignKey.
	/// </summary>
	public class ForeignKey:Constraint, DB.ForeignKey
	{
		private Association _wrappedAssociation;
		private Association _logicalAssociation;
		
		internal Table _foreignTable;
		
		public ForeignKey(Table owner,Operation operation):base(owner,operation)
		{
		}
		public ForeignKey(Table owner, List<Column> involvedColumns):base(owner, involvedColumns)
		{
			
		}

		#region implemented abstract members of Constraint
		protected override string getStereotype()
		{
			return "FK";
		}
		#endregion
		public override TSF.UmlToolingFramework.UML.Classes.Kernel.Element logicalElement {
			get {
				return this.logicalAssociation;
			}
		}
		public override void save()
		{
			base.save();
			if (this.traceTaggedValue == null) createTraceTaggedValue();
			//check if association to correct table
			if (this._wrappedAssociation == null)
			{
				//create wrapped association
				this._wrappedAssociation = this._factory._modelFactory.createNewElement<Association>(this._owner._wrappedClass, "");
			}
			if (this._wrappedAssociation != null)
			{
				if (this._foreignTable._wrappedClass != null)
				{
					this._wrappedAssociation.target = this._foreignTable._wrappedClass;
					this.wrappedAssociation.sourceEnd.name = this.name;
					if (this._foreignTable.primaryKey != null) this.wrappedAssociation.targetEnd.name = this._foreignTable.primaryKey.name;
					this.wrappedAssociation.sourceEnd.multiplicity = new Multiplicity("0..*");
					//TODO: implement multiplicities correctly by implementing isNotNullable on FK
					this.wrappedAssociation.targetEnd.multiplicity = new Multiplicity("1..1");
					this.wrappedAssociation.save();
				}
				else
				{
					Logger.logError(string.Format("foreign table {0} has not been saved yet",_foreignTable.name));
				}
			}
		}

		#region implemented abstract members of DatabaseItem


		public override DB.DatabaseItem createAsNewItem(DB.Database existingDatabase, bool save = true)
		{
			//look for corresponding table in existingDatabase
			Table newTable = (Table)existingDatabase.tables.FirstOrDefault(x => x.name == this.ownerTable.name);
			var newForeignTable = existingDatabase.tables.FirstOrDefault(x => x.name == this._foreignTable.name);
			if (newTable != null && newForeignTable != null)
			{
				var newForeignKey = new ForeignKey(newTable,this._involvedColumns);
				newForeignKey.name = name;
				newForeignKey._foreignTable = (Table)newForeignTable;
				newForeignKey._logicalAssociation = _logicalAssociation;
				newForeignKey.isOverridden = isOverridden;
				if (save) newForeignKey.save();
				return newForeignKey;
			}
			return null;
		}


		#endregion

		protected override void updateDetails(DB.DatabaseItem newDatabaseItem)
		{
			base.updateDetails(newDatabaseItem);
			ForeignKey newForeignKey = (ForeignKey)newDatabaseItem;
			//update the association
			this._foreignTable = newForeignKey._foreignTable;
			//set logical association
			this.logicalAssociation = newForeignKey.logicalAssociation;

		}

		public override void delete()
		{
			base.delete();
			if (this._wrappedAssociation != null) this._wrappedAssociation.delete();
		}

		public Association logicalAssociation
		{
			get
			{
				if (_logicalAssociation == null 
				    && this._wrappedOperation != null)
				{
					_logicalAssociation = this._wrappedOperation.taggedValues
						.Where(x => x.name.Equals("sourceAssociation",StringComparison.InvariantCultureIgnoreCase)
						             && x.tagValue is Association)
						.Select(y => y.tagValue as Association).FirstOrDefault();
				}
				return _logicalAssociation;
			}
			set
			{
				_logicalAssociation = value;
			}
		}
		internal Association wrappedAssociation
		{
			get
			{
				if (_wrappedAssociation == null)
				{
					this.getWrappedAssociation();
				}
				return _wrappedAssociation;
			}
			
		}
		private void getWrappedAssociation()
		{
			if (_wrappedOperation != null)
			{
				string getAssociationQuery = @"select c.Connector_ID from t_connector c 
											where c.Start_Object_ID = " + this._owner._wrappedClass.id.ToString() +
					" and c.SourceRole = '"+ _wrappedOperation.name +"'";
				_wrappedAssociation = this._wrappedOperation.model.getRelationsByQuery(getAssociationQuery).FirstOrDefault() as Association;
			}
		}
		public override string properties {
			get 
			{
				string _properties = base.properties;
				if (foreignTable != null)
				{
					_properties += " => " + foreignTable.name;
				}
				return _properties;
			}
		}
		public override string itemType {
			get {return "Foreign Key";}
		}

		#region implemented abstract members of Constraint


		internal override void createTraceTaggedValue()
		{
			if (this._wrappedOperation != null && _logicalAssociation != null)
			{
				this._wrappedOperation.addTaggedValue("sourceAssociation",_logicalAssociation.guid);
			}
		}


		internal override TaggedValue traceTaggedValue 
		{
			get 
			{
				if (_wrappedOperation != null) return _wrappedOperation.taggedValues.OfType<TaggedValue>()
					.FirstOrDefault(x => x.name.Equals("sourceAssociation",StringComparison.InvariantCultureIgnoreCase));
				//if no wrappped operation then no tagged value
				return null;
			}
			set 
			{
				if (_wrappedOperation != null
				   && value != null)
				{
					var tag = _wrappedOperation.addTaggedValue(value.name, value.eaStringValue);
					tag.comment = value.comment;
				}
			}
		}


		#endregion

		#region ForeignKey implementation
		public DB.Table foreignTable {
			get 
			{
				if (_foreignTable == null)
				{
					this.getForeignTable();
				}
				return _foreignTable;
			}
			set 
			{
				_foreignTable = (Table)value;
			}
		}
		#endregion
		private void getForeignTable()
		{
			if (wrappedAssociation != null)
			{
				var targetClass = _wrappedAssociation.target as Class;
				if (targetClass != null)
				{
					_foreignTable = new Table(this._owner._owner, targetClass);
				}
			}
		}

	}
}
