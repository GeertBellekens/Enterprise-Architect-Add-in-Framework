
using System;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of DataType.
	/// </summary>
	public class DataType:DB.DataType
	{
		public DataType(BaseDataType baseType, int length, int precision)
		{
			this.type = baseType;
			this.length = length;
			this.precision = precision;
		}

		#region DataType implementation

		public DB.BaseDataType type {get;set;}

		public string itemType {
			get {return "DataType";}
		}
		public string properties {
			get 
			{
				string _properties = this.type.properties;
				if (this.type.hasLength)
				{
					_properties += " (" + this.length;
					if (this.type.hasPrecision)
					{
						_properties += "," + this.precision;
					}
					_properties += ")";
				}
				return _properties;
			}
		}
		public string name {
			get {return this.type.name;}
			set {throw new NotImplementedException();}
		}
		public int length {get;set;}

		public int precision {get;set;}
		


		#endregion

	}
}
