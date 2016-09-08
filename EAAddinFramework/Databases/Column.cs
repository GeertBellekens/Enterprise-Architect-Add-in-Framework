
using System;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using TSF_EA=TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;
namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of Column.
	/// </summary>
	public class Column:DatabaseItem, DB.Column
	{
		internal Table _owner;
		internal TSF_EA.Attribute _wrappedattribute;
		internal DataType _type;
		private TSF_EA.TaggedValue _traceTaggedValue;
		private TSF_EA.Attribute _logicalAttribute;
		private string _name;
		private bool _isNotNullable;
		public Column(Table owner, TSF_EA.Attribute attribute)
		{
			this._owner = owner;
			this._wrappedattribute = attribute;
		}
		public Column(Table owner, string name)
		{
			this._owner = owner;
			this.name = name;
			this.ownerTable.addColumn(this);
		}
		#region implemented abstract members of DatabaseItem
		public override void save()
		{
			//create the _wrapped attribute if needed
			if (_wrappedattribute == null)
			{
				if (this._owner._wrappedClass == null)
				{
					this.ownerTable.save();
				}
				//now the wrappedClass should exist. if note then we have a problem
				this._wrappedattribute = this.factory.modelFactory.createNewElement<TSF_EA.Attribute>(this._owner._wrappedClass,this.name);		
			}
			if (_wrappedattribute != null)
			{
				//set steretotype
				this._wrappedattribute.setStereotype("column");
				//set datatype;
				_wrappedattribute.type = this.factory.modelFactory.createPrimitiveType(this.type.name);
				_wrappedattribute.length = this.type.length;
				_wrappedattribute.precision = this.type.precision;
				//is not nullable
				this.isNotNullable = _isNotNullable;
				//save
				_wrappedattribute.save();
				//logical attribute tag value
				if (traceTaggedValue == null) createTraceTaggedValue();
				

			}
			//save the columnn name in the alias
			if (logicalAttribute != null) logicalAttribute.save(); 
			

				
		}
		public override TSF.UmlToolingFramework.UML.Classes.Kernel.Element logicalElement 
		{
			get 
			{
				return logicalAttribute;
			}
		}
		#region implemented abstract members of DatabaseItem
		public override bool isValid 
		{
			get 
			{
				// a column is valid if it has a name, a type, and if there's no other column in the table with the same name
				return (! string.IsNullOrEmpty(this.name)
				        && this.type != null
				        && this.type.isValid
				        && ownerTable.columns.Count(x => x.name == this.name) == 1);
				       
			}
		}
		#endregion
		#region implemented abstract members of DatabaseItem
		public override void createAsNewItem(DB.Database existingDatabase)
		{
			//look for corresponding table in existingDatabase
			Table newTable = (Table)existingDatabase.tables.FirstOrDefault(x => x.name == this.ownerTable.name);
			if (newTable != null)
			{
				var newColumn = new Column(newTable,this.name);
				newColumn.isNotNullable = _isNotNullable;
				newColumn.type = _type;
				newColumn.logicalAttribute = _logicalAttribute;
				newColumn.save();
			}
		}
		#endregion
		#region implemented abstract members of DatabaseItem
		protected override void updateDetails(DB.DatabaseItem newDatabaseItem)
		{
			var newColumn = (Column)newDatabaseItem;
			this._isNotNullable = newColumn.isNotNullable;
			this._logicalAttribute = newColumn.logicalAttribute;
			this._type = newColumn._type;
			this.isOverridden = newColumn.isOverridden;
		}
		#endregion		
		
		public override void delete()
		{
			if (_wrappedattribute != null) _wrappedattribute.delete();
		}


		internal override void createTraceTaggedValue()
		{
			if (this._wrappedattribute != null)
			{
				string tagValue = string.Empty;
				if (_logicalAttribute != null) tagValue = _logicalAttribute.guid;
				 this._wrappedattribute.addTaggedValue("sourceAttribute",tagValue);
			}
			
		}
		internal override Element wrappedElement 
		{
			get 
			{
				return _wrappedattribute;
			}
			set 
			{
				this._wrappedattribute = (TSF_EA.Attribute)value;
			}
		}
		internal override TaggedValue traceTaggedValue 
		{
			get 
			{
				if (_traceTaggedValue == null)
				{
					if (_wrappedattribute != null)
					{
						_traceTaggedValue = _wrappedattribute.taggedValues.OfType<TaggedValue>().FirstOrDefault(x => x.name.Equals("sourceAttribute",StringComparison.InvariantCultureIgnoreCase));
					}
				}
				//no wrapped attribute so retur null
				return _traceTaggedValue;
			}
			set 
			{
				_traceTaggedValue = value;
				if (_wrappedattribute != null
				   && value != null)
				{
					_traceTaggedValue = _wrappedattribute.addTaggedValue(value.name, value.eaStringValue);
					_traceTaggedValue.comment = value.comment;
					_traceTaggedValue.save();
				}
			}
		}
		#endregion		
		public TSF_EA.Attribute logicalAttribute
		{
			get
			{
				if (_logicalAttribute == null
				   && _wrappedattribute != null)
				{
					_logicalAttribute = _wrappedattribute.taggedValues
						.Where(x => x.name.Equals("sourceAttribute",StringComparison.InvariantCultureIgnoreCase)
						             && x.tagValue is TSF_EA.Attribute)
						.Select(y => y.tagValue as TSF_EA.Attribute).FirstOrDefault();
				}
				return _logicalAttribute;
			}
			set
			{
				_logicalAttribute = value;
				this.createTraceTaggedValue();
			}
		}
		
		#region Column implementation

		public DB.Table ownerTable {
			get {return this._owner;}
			set {this._owner = (Table)value;}
		}

		#region implemented abstract members of DatabaseItem
		public override DB.DatabaseItem owner {
			get {
				return ownerTable;
			}
		}
		#endregion
		public override string itemType {
			get {return "Column";}
		}
		public override string properties {
			get 
			{
				
				string _properties = string.Empty;
				if (this.type != null ) _properties += this.type.properties;
				if (this.isNotNullable)
				{
					_properties += " Not Null";
				}
				return _properties;
			}
		}
		public override string name {
			get 
			{
				if(_wrappedattribute != null) _name = _wrappedattribute.name;
				return _name;
			}
			set 
			{
				_name = value;
				if (_wrappedattribute != null) this._wrappedattribute.name = _name;
			}
		}

		public DB.DataType type 
		{
			get 
			{
				if (_type == null)
				{
					_type = this.getDataType();
				}
				return _type;
			}
			set 
			{
				_type = (DataType)value;
				this.setDataType();
			}
		}

		public bool isNotNullable 
		{
			get 
			{
				if(_wrappedattribute != null) _isNotNullable = _wrappedattribute.allowDuplicates;
				return _isNotNullable;
			}
			set 
			{
				_isNotNullable = value;
				if (_wrappedattribute != null) this._wrappedattribute.allowDuplicates = _isNotNullable;
			}
		}


		private DataType getDataType()
		{
			if (this._wrappedattribute != null)
			{
				var basetype = this.ownerTable.databaseOwner.databaseFactory.baseDataTypes.FirstOrDefault(x => x.name == this._wrappedattribute.type.name);
				if (basetype != null)
				{
					if (basetype.hasPrecision)
					{
						return  new DataType((BaseDataType)basetype, this._wrappedattribute.precision, this._wrappedattribute.scale);
					}
					else
					{
						return  new DataType((BaseDataType)basetype, this._wrappedattribute.length, 0);
					}
				}
			}
			return null;
		}
		private void setDataType()
		{
			if (this._wrappedattribute != null)
			{
				this._wrappedattribute.type = this._wrappedattribute.model.factory.createPrimitiveType(_type.type.name);
				if (this.type.type.hasPrecision)
				{
					this._wrappedattribute.precision = _type.length;
					this._wrappedattribute.scale = _type.precision;
				}
				else
				{
					this._wrappedattribute.length = _type.length;
				}
				
			}
		}

		#endregion

	}
}
