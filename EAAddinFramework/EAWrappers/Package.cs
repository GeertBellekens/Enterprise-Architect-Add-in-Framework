using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Description of Package.
	/// </summary>
	public class Package:ElementWrapper, UML.Classes.Kernel.Package
	{
		internal global::EA.Package wrappedPackage {get;set;}
		
		public Package(Model model,global::EA.Package package):base(model,package.Element)
		{
			this.wrappedPackage = package;
		}

		
		public TSF.UmlToolingFramework.UML.Classes.Kernel.PackageableElement ownedMembers {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Type> ownedTypes {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Package> nestedPackages {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public TSF.UmlToolingFramework.UML.Classes.Kernel.Package nestingPackage {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.PackageMerge> packageMerges {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
				
		/// <summary>
		/// 
		/// </summary>
		public override HashSet<UML.Classes.Kernel.Element> ownedElements {
			get 
			{ 
				List<UML.Classes.Kernel.Element> elements = this.model.factory.createElements( this.wrappedPackage.Elements).Cast<UML.Classes.Kernel.Element>().ToList();
				elements.AddRange(this.model.factory.createElements( this.wrappedPackage.Packages).Cast<UML.Classes.Kernel.Element>());
				return new HashSet<UML.Classes.Kernel.Element>(elements);
			}
			set 
			{ 
				throw new NotImplementedException();
			}
		}
		public override HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> ownedDiagrams {
		get {
    		HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> diagrams = new HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram>();
    		foreach ( global::EA.Diagram eaDiagram in this.wrappedPackage.Diagrams)
    		{
    			diagrams.Add(((Factory)this.model.factory).createDiagram(eaDiagram));
    		}
    		return diagrams;
		}
		set {
			throw new NotImplementedException();
		}
	}
	}
}
