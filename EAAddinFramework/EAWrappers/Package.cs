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
		/// creates a new element of the given type as an owned element of this 
	    /// element
	    public override T addOwnedElement<T>(String name, string EAType) 
	    {	
	      System.Type type = typeof(T);
	      T newElement;
	
	      if(((Factory)this.model.factory).isEAAtttribute(type)) 
	      {
	      	throw new Exception("Cannot add an Attribute to a Package");
	      } 
	      else if(((Factory)this.model.factory).isEAOperation(type))
	      {
	        throw new Exception("Cannot add an Attribute to a Package");
	      } 
	      else if (((Factory)this.model.factory).isEAConnector(type))
	      {
	        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
	          ( this.wrappedPackage.Connectors, name, EAType  );
	      } else {
	        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
	          ( this.wrappedPackage.Elements, name, EAType );
	      }
	      return newElement;
	    }
	    /// <summary>
	    /// creates a new diagram under this package
	    /// </summary>
	    /// <param name="name">the name of the new diagram</param>
	    /// <returns>the new diagram</returns>
		public override T addOwnedDiagram<T>(string name)
		{
			return ((Factory)this.model.factory).addNewDiagramToEACollection<T>(this.wrappedPackage.Diagrams,name);
		}
		/// <summary>
		/// deletes an element owned by this Package
		/// </summary>
		/// <param name="ownedElement">the owned element to delete</param>
		public override void deleteOwnedElement(Element ownedElement)
		{
			if (ownedElement is ElementWrapper)
			{
				for (short i = 0; i< this.WrappedPackage.Elements.Count; i++)
				{
					var eaElement = this.WrappedPackage.Elements.GetAt(i) as global::EA.Element;
					if (eaElement.ElementGUID == ownedElement.guid)
					{   
						this.WrappedPackage.Elements.Delete(i);
						this.WrappedPackage.Elements.Refresh();
						break;
					}
				}
			}
			else
			{
				//currently only deleting elements is supported
				throw new NotImplementedException();
			}
		}
		/// <summary>
		/// export this package to xmi in the default format
		/// </summary>
		/// <param name="filePath">the filepath to save the xmi file to</param>
		public void exportToXMI(string filePath)
		{
			var projectInterface = this.model.getWrappedModel().GetProjectInterface();
			string xmlGUID = projectInterface.GUIDtoXML(this.guid);
			projectInterface.ExportPackageXMI(xmlGUID,global::EA.EnumXMIType.xmiEADefault,2,3,0,0,filePath);
		}
	}
}
