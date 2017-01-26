
using System;
using System.Collections.Generic;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingSet.
	/// </summary>
	public class MappingSet:MP.MappingSet
	{
		private string _name;
		internal List<Mapping> _mappings;
		public MappingSet()
		{
			_mappings = new List<Mapping>();
		}

		#region MappingSet implementation

		public string name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}

		public List<MP.Mapping> mappings {
			get {
				return _mappings.Cast<MP.Mapping>().ToList();
			}
			set {
				_mappings = value.Cast<Mapping>().ToList();
			}
		}
		
		public void addMapping(MP.Mapping mapping)
		{
			this._mappings.Add((Mapping)mapping);
		}
		#endregion
	}
}
