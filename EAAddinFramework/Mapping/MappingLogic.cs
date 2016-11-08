
using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingLogic.
	/// </summary>
	public class MappingLogic:MP.MappingLogic
	{
		private string _description;
		public MappingLogic(string logicDescription)
		{
			_description = logicDescription;
		}

		#region MappingLogic implementation

		public string description {
			get {
				return _description;
			}
			set {
				_description = value;
			}
		}

		#endregion
	}
}
