using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;
namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of ElementMappingSet.
	/// </summary>
	public class ElementMappingSet:MappingSet
	{
		internal ElementWrapper wrappedElement {get;set;}
		internal bool source {get;private set;}
		public ElementMappingSet(ElementWrapper rootElement,bool source)
		{
			this.wrappedElement = rootElement;
			this.source = source;
			base.name = wrappedElement.name;
			base.mappings = this.getMappings().Cast<MP.Mapping>().ToList();
		}
		List<Mapping> getMappings()
		{
			return MappingFactory.createRootMappings(this.wrappedElement, this.wrappedElement.name);
		}
	}
}
