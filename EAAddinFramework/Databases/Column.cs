
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
		public Column(Table owner, TSF_EA.Attribute attribute)
		{
			this._owner = owner;
			this._wrappedattribute = attribute;
		}

		#region Column implementation

		public DB.Table owner {
			get {return this._owner;}
			set {this._owner = (Table)value;}
		}

		public string name {
			get {return this._wrappedattribute.name;}
			set {this._wrappedattribute.name = value;}
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

		public bool isNotNullable {
			get { return this._wrappedattribute.allowDuplicates;}
			set {this._wrappedattribute.allowDuplicates = value;}
		}
		private DataType getDataType()
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
