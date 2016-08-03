
using System;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of ForeignKey.
	/// </summary>
	public class ForeignKey:Constraint, DB.ForeignKey
	{
		public ForeignKey(Table owner,Operation operation):base(owner,operation)
		{
		}

		#region ForeignKey implementation

		public DB.Table foreignTable {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		#endregion


	}
}
