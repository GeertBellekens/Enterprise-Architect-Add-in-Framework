
using System;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of BaseDataType.
	/// </summary>
	public class BaseDataType:DB.BaseDataType
	{
		public BaseDataType(string name, bool hasLength, bool hasPrecision)
		{
			this.name = name;
			this.hasLength = hasLength;
			this.hasPrecision = hasPrecision;
		}

		#region BaseDataType implementation

		public string name {get;set;}

		public bool hasLength {get;set;}

		public bool hasPrecision {get;set;}


		#endregion
	}
}
