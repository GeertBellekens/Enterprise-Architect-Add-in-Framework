
using System;
using System.Collections.Generic;
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
			if (this._wrappedOperation != null)
			{
				this._wrappedOperation.addTaggedValue("sourceAssociation",string.Empty);
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
