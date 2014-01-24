using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Description of Package.
	/// </summary>
	public class Package:ElementWrapper, UML.Classes.Kernel.Package
	{
		private string _fqn = string.Empty;
		internal global::EA.Package wrappedPackage {get;set;}
		public int packageID
		{
			get {return this.wrappedPackage.PackageID;}
		}
		
		public Package(Model model,global::EA.Package package):base(model,package.Element)
		{
			this.wrappedPackage = package;
		}

		public global::EA.Package WrappedPackage {
			get { return this.wrappedPackage; }
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
		public override HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> ownedDiagrams 
		{
			get 
			{
	    		HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> diagrams = new HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram>();
	    		foreach ( global::EA.Diagram eaDiagram in this.wrappedPackage.Diagrams)
	    		{
	    			diagrams.Add(((Factory)this.model.factory).createDiagram(eaDiagram));
	    		}
	    		return diagrams;
			}
			set {throw new NotImplementedException();}
		}
		public override string fqn 
		{
			get 
			{  
				if (this._fqn == string.Empty)
				{
					this._fqn = this.getFQN(string.Empty, this.packageID);
				}
				return this._fqn;
			}
		}
		/// <summary>
		/// gets the FQN without the need of instantiating all the owners up to the root element.
		/// From a package we are sure that only packages are the owners so its safe to check only t_package.
		/// </summary>
		/// <param name="fqnString">the fqn string so far</param>
		/// <param name="parentID">the id of the package</param>
		/// <returns>the fqn of the package</returns>
		private string getFQN(string fqnString, int packageID)
		{
			string newFQN = fqnString;
			XmlDocument result = this.model.SQLQuery("select p.Parent_ID, p.Name from t_package p where p.Package_ID = " + packageID.ToString());
			XmlNode parentIDNode = result.SelectSingleNode(this.model.formatXPath("//Parent_ID"));
			XmlNode nameNode = result.SelectSingleNode(this.model.formatXPath("//Name"));
			if (nameNode != null)
			{
				//add the "." if necesarry
				if (newFQN != string.Empty)
				{
					newFQN = "." + newFQN;	
				}
				newFQN = nameNode.InnerText + newFQN;
				
				int parentID;
				if (parentIDNode != null && int.TryParse(parentIDNode.InnerText, out parentID))
				{
					newFQN = getFQN(newFQN, parentID);
				}
			}
			return newFQN;
		}
	}
}
