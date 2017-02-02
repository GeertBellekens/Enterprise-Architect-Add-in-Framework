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
	public abstract class Mapping:MP.Mapping
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
		public Mapping(Element sourceElement, Element targetElement,string basepath)
			:this(new MappingEnd(sourceElement,basepath),new MappingEnd(targetElement)){}
		public Mapping(Element sourceElement, Element targetElement,string basepath, ElementWrapper targetRoot)
			:this(new MappingEnd(sourceElement,basepath),new MappingEnd(targetElement,targetRoot)){}
		public Mapping(Element sourceElement, Element targetElement,string basepath,string targetBasePath)
			:this(new MappingEnd(sourceElement,basepath),new MappingEnd(targetElement,targetBasePath)){}
		
		/// <summary>
		/// strips the given path from the element name at the end.
		/// So if the path ends with .ElementName then that part is stripped off
		/// </summary>
		/// <param name="endElement">the element at the end of the mapping</param>
		/// <param name="path">the path string</param>
		/// <returns>the stripped path</returns>
		protected string stripPathFromElementName(Element endElement, string path)
		{
			if (path.EndsWith( "." + endElement.name, StringComparison.InvariantCulture))
			{
				return path.Substring(0,path.Length - (endElement.name.Length +1));
			}
			//else return unchanged
			return path;
		}
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

		public abstract void save();
		public abstract MP.MappingLogic mappingLogic {get;set;}


		#endregion
	}
}
