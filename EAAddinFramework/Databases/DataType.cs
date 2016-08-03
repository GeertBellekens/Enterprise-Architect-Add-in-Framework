
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

		public string name {
			get {return this.type.name;}
		}
		public int length {get;set;}

		public int precision {get;set;}

		#endregion
	}
}
