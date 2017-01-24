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
		public override string uniqueID
		{
			get {return this.wrappedPackage.PackageGUID;}
		}
		public Package(Model model,global::EA.Package package):base(model,package.Element)
		{
			this.wrappedPackage = package;
		}
		public override String notes 
		{
	        get { return this.WrappedPackage.Notes;  }
	        set { this.WrappedPackage.Notes = value; }
    	}
		public global::EA.Package WrappedPackage 
		{
			get { return this.wrappedPackage; }
		}
		/// <summary>
		/// returns the package at the root of this model branch
		/// </summary>
		/// <returns>package at the root of this model branch</returns>
		public UML.Classes.Kernel.Package getRootPackage()
		{
			var ownerPackage = this.owner as Package;
			if (ownerPackage != null)
			{
				return ownerPackage.getRootPackage();
			}
			else
			{
				return this;
			}
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
		
		public HashSet<UML.Classes.Kernel.Package> nestedPackages 
		{
			get 
			{
				return new HashSet<UML.Classes.Kernel.Package>(this.ownedElements.OfType<UML.Classes.Kernel.Package>());
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
	    		foreach ( var eaDiagram in this.wrappedPackage.Diagrams)
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
		public override string guid 
		{
			get 
			{
				return this.wrappedPackage.PackageGUID;
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
		public override void save()
		{
			this.wrappedPackage.Update();
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
			this.model.wrappedModel.SuppressEADialogs = true;
			var projectInterface = this.model.wrappedModel.GetProjectInterface();
			string xmlGUID = projectInterface.GUIDtoXML(this.guid);
			projectInterface.ExportPackageXMI(xmlGUID,global::EA.EnumXMIType.xmiEADefault,2,3,1,0,filePath);
			this.model.wrappedModel.SuppressEADialogs = false;
		}
		public void refresh()
		{
			this.model.wrappedModel.RefreshModelView(this.packageID);
		}
		public override List<UML.Extended.UMLItem> findOwnedItems(string itemDescriptor)
		{
			List<UML.Extended.UMLItem> foundItems = new List<UML.Extended.UMLItem>();
			//first try to find it in a faster way
			//get the idstrings of this package and all its owned packages
			string packageTreeIDString = this.getPackageIDString(getNestedPackageTree(true));
			//get the individual parts
			var descriptorParts = itemDescriptor.Split('.').ToList();
			//if there's only one part then look for an element with that name, then for a diagram
			if (descriptorParts.Count == 1) 
			{
				//look for element
				foundItems.AddRange(getOwnedElements(descriptorParts[0], packageTreeIDString));
				//look for a diagram
				foundItems.AddRange(getOwnedDiagrams(descriptorParts[0],packageTreeIDString));
			}
			else if (descriptorParts.Count == 2)
			{
				//we take the first two items and try to find a match
				string ownerName = descriptorParts[0];
				string attributeName = descriptorParts[1];
				//first look for an attribute
				foundItems.AddRange(getOwnedAttributes(ownerName,attributeName,packageTreeIDString));
				//TODO then look for a nested class or linked class
				//TODO then look for an operation
				//TODO then look for an association
			}
			else if (descriptorParts.Count > 2)
			{
				//top down approach
				//look for the first part and start searching from there
				foundItems.AddRange(findOwnedItems(descriptorParts));
			}
			//if still nothing found then get the base implemetation
			if (foundItems.Count == 0) foundItems.AddRange(base.findOwnedItems(itemDescriptor));
			return foundItems;
		}
		
		public override List<UML.Extended.UMLItem> findOwnedItems(List<String> descriptionParts)
		{
			List<UML.Extended.UMLItem> ownedItems =new List<UML.Extended.UMLItem>();
			if (descriptionParts.Count > 0)
			{
				string firstpart = descriptionParts[0];
					//start by finding an element with the given name
				var directOwnedElements = getOwnedElements(firstpart,this.packageID.ToString());
				if (descriptionParts.Count > 1)
				{
					//loop the owned elements and get their owned items 
					foreach (var element in directOwnedElements) 
					{
						//remove the first part
						descriptionParts.RemoveAt(0);         
						//go one level down
						ownedItems.AddRange(element.findOwnedItems(descriptionParts));
					}
				}
				else
				{
					//only one item so add the direct owned elements
					ownedItems.AddRange(directOwnedElements);
					//Add also the diagrams owned by this package
					ownedItems.AddRange(getOwnedDiagrams(firstpart,this.packageID.ToString()));
				}
			}
			return ownedItems;
		}
		
		public List<Attribute> getOwnedAttributes(string ownerName,string attributeName, string packageIDList)
		{
			//owner.Attribute
			string sqlGetAttributes = @"select a.ea_guid from (t_attribute a
										inner join t_object o on o.Object_ID = a.Object_ID)
										where a.Name = '" + attributeName + @"'
										and o.Name = '" + ownerName + @"'
										and o.Package_ID in (" + packageIDList + @")";

			return this.model.getAttributesByQuery(sqlGetAttributes);
		}

		public List<ElementWrapper> getOwnedElements(string elementName, string packageIDList)
		{
			string sqlGetOwnedElement = "select o.Object_ID from t_object o " +
										" where " +
										" o.Name = '" + elementName + "' " +
				" and o.Package_ID in (" + packageIDList + ") ";
			return this.model.getElementWrappersByQuery(sqlGetOwnedElement);
		}
		public List<Diagram> getOwnedDiagrams(string diagramName, string packageIDList)
		{
			string sqlGetOwnedDiagram = "select d.Diagram_ID from t_diagram d " +
										" where " +
										" d.Name = '" + diagramName + "' " +
										" and d.Package_ID in (" + packageIDList + ") ";
			return this.model.getDiagramsByQuery(sqlGetOwnedDiagram);
		}
		public HashSet<UML.Classes.Kernel.Package> getNestedPackageTree(bool includeThis)
		{
			var nestedPackageTree = this.nestedPackages;
			//add this package if needed
			if (includeThis) nestedPackageTree.Add(this);
			foreach (var subPackage in this.nestedPackages) 
			{
				foreach (var subSubPackage in subPackage.getNestedPackageTree(false)) 
				{
					nestedPackageTree.Add(subSubPackage);
				}
			}
			return nestedPackageTree;
		}

		public HashSet<UML.Classes.Kernel.Element> getAllOwnedElements()
		{
			var allOwnedElements = this.ownedElements;
			foreach (var subPackage in this.getNestedPackageTree(false)) 
			{
				foreach (var element in subPackage.ownedElements) 
				{
					allOwnedElements.Add(element);
				}
			}
			return allOwnedElements;
		}

		public string getPackageIDString(ICollection<UML.Classes.Kernel.Package> packages)
		{
			return string.Join(",",packages);
		}
		
		
	}
}
