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
        public ElementMappingSet(MP.MappingModel source, MP.MappingModel target) : base(source, target) { }
		List<Mapping> getMappings()
		{
			return MappingFactory.createRootMappings(this.wrappedElement, this.wrappedElement.name);
		}
	}
}
