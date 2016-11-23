using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of Mapping.
	/// </summary>
	public class Mapping:MP.Mapping
	{
		internal MappingEnd _source;
		internal MappingEnd _target;
		internal MappingLogic _mappingLogic;
		public Mapping(MappingEnd sourceEnd, MappingEnd targetEnd)
		{
			_source = sourceEnd;
			_target = targetEnd;
		}
		public Mapping(MappingEnd sourceEnd, MappingEnd targetEnd, MappingLogic logic):this(sourceEnd,targetEnd)
		{
			_mappingLogic = logic;
		}
		public Mapping(Element sourceElement, Element targetElement)
			:this(new MappingEnd(sourceElement),new MappingEnd(targetElement)){}

		#region Mapping implementation

		public MP.MappingEnd source {
			get {
				return _source;
			}
			set {
				_source = (MappingEnd)value;
			}
		}

		public MP.MappingEnd target {
			get {
				return _target;
			}
			set {
				_target = (MappingEnd)value;
			}
		}

		public MP.MappingLogic mappingLogic {
			get {
				return _mappingLogic;
			}
			set {
				_mappingLogic = (MappingLogic)value;
			}
		}

		#endregion
	}
}
