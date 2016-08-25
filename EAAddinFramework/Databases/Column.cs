
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
	public class Column:DB.Column
	{
		internal Table _owner;
		internal TSF_EA.Attribute _wrappedattribute;
		internal DataType _type;
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
			this.owner.addColumn(this);
		}
		
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
			}
		}
		
		#region Column implementation

		public DB.Table owner {
			get {return this._owner;}
			set {this._owner = (Table)value;}
		}

		public string itemType {
			get {return "Column";}
		}
		public string properties {
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
		public string name {
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
				var basetype = this.owner.owner.factory.baseDataTypes.FirstOrDefault(x => x.name == this._wrappedattribute.type.name);
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
