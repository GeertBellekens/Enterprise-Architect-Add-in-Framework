using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of PackageMappingSet.
	/// </summary>
	public class PackageMappingSet:MappingSet
	{
		internal Package wrappedPackage {get;set;}
		internal bool source {get;private set;}
		public PackageMappingSet(Package wrappedPackage, bool source)
		{
			this.wrappedPackage = wrappedPackage;
			this.source = source;
			base.name = wrappedPackage.name;
			base.mappings = this.getMappings().Cast<MP.Mapping>().ToList();;
		}
		
		List<Mapping> getMappings()
		{
			//get the connector mappings
			//check if the package has a trace to another package
			ElementWrapper targetRootElement = null;
			var packageTrace = this.wrappedPackage.relationships.OfType<Abstraction>().FirstOrDefault(x => x.target is Element && x.stereotypes.Any(y => y.name == "trace"));
			if (packageTrace != null) targetRootElement = packageTrace.target as ElementWrapper;
			List<Mapping> returnedMappings = new List<Mapping>();
			foreach (var classElement in wrappedPackage.ownedElements.OfType<ElementWrapper>()) 
			{
				returnedMappings.AddRange(MappingFactory.createOwnedMappings(classElement,classElement.name,targetRootElement,true));
			}
			return returnedMappings;
		}
	}
}
