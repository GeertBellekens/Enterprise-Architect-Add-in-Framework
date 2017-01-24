using System;
using System.Collections.Generic;
using System.Linq;

using System.Xml;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	
  public class ElementWrapper : Element, UML.Classes.Kernel.NamedElement 
  {
    internal global::EA.Element wrappedElement {get; set; }
    private UML.Classes.Kernel.Element _owner;
    private HashSet<UML.Classes.Kernel.Property> _attributes;
    private List<UML.Classes.Kernel.Relationship> _allRelationships;
    private HashSet<AttributeWrapper> _attributeWrappers;
    private HashSet<UML.Classes.Kernel.EnumerationLiteral> _ownedLiterals;

    public ElementWrapper(Model model, global::EA.Element wrappedElement) 
      : base(model)
    {
      this.wrappedElement = wrappedElement;
    }
	
    public override string name {
      get { return this.wrappedElement.Name;  }
      set { this.wrappedElement.Name = value; }
    }
	public string alias
	{
		get{return this.wrappedElement.Alias;}
		set{this.wrappedElement.Alias = value;}
	}
	public string status
	{
		get{return this.wrappedElement.Status;}
		set{this.wrappedElement.Status = value;}
	}
	public string genLinks
	{
		get{return this.wrappedElement.Genlinks;}
		set{this.wrappedElement.Genlinks = value;}
	}
	public List<string> primitiveParentNames
	{
		get
		{
			var parentNames = new List<string>();
			if (this.genLinks.Contains("Parent="))
			{
				foreach (string genLink in this.wrappedElement.Genlinks.Split(';'))
				{
					var parentTuple = genLink.Split('=');
					if (parentTuple.Count() == 2
					    && parentTuple[0] == "Parent"
					    && ! string.IsNullOrEmpty(parentTuple[1]))
					{
						parentNames.Add(parentTuple[1]);
					}
				}
			}
			return parentNames;
		}
	}
	public string phase
	{
		get{return this.wrappedElement.Phase;}
		set{this.wrappedElement.Phase = value;}
	}
	public virtual string EAElementType
	{
		get{return this.wrappedElement.Type;}
	}
    /// <summary>
    /// return the unique ID of this element
    /// </summary>
	public override string uniqueID 
	{
		get 
		{
			return this.wrappedElement.ElementGUID;
		}
	}
    public string subType
    {
    	get 
    	{
    		string wrappedType = this.wrappedElement.Type;
    		//PackagingComponents have Package as Type and 20 as subtype
    		if (wrappedType == "Package" && wrappedElement.Subtype == 20)
    		{
    			wrappedType = "PackagingComponent";
    		}
    		else if (wrappedType == "StateNode")
    		{
    			switch (wrappedElement.Subtype) 
    			{
    				case 100:
    					wrappedType = "ActivityInitial";
    					break;
    				case 101:
    					wrappedType = "ActivityFinal";
    					break;
    				case 102:
    					wrappedType = "FlowFinal";
    					break;
    				case 6:
    					wrappedType = "SynchronisationNode";
    					break;
    				case 3: 
    					wrappedType = "StateInitial";
    					break;
    				case 4: 
    					wrappedType = "StateFinal";
    					break;
    				case 5:
    					wrappedType = "StateHistory";
    					break;
    				case 11:
    					wrappedType = "StateChoice";
    					break;
    				case 10:
    					wrappedType = "StateJunction";
    					break;
    				case 13:
    					wrappedType = "StateEntryPoint";
    					break;
    				case 14:
    					wrappedType = "StateExitPoint";
    					break;
    				case 12:
    					wrappedType = "StateTerminate";
    					break;
    			}
    		}
    		else if (wrappedType == "Text")
    		{
    			switch (wrappedElement.Subtype) 
    			{
    				case 76:
    					wrappedType = "Legend";
    					break;
    				case 18:
    					wrappedType = "DiagramNotes";
    					break;
    				case 19:
    					wrappedType = "Hyperlink";
    					break;
    			}
    		}
    		return wrappedType;
    	}
    	set {this.wrappedElement.Type = value;}
    }
    
    public global::EA.Element WrappedElement {
    	get { return this.wrappedElement; }
    }

	public global::EA._CustomProperty getCustomProperty(string propertyName) {
		foreach(global::EA._CustomProperty property in wrappedElement.CustomProperties) {
			if(property.Name == propertyName) {
				return property;
			}
		}
		return null;
    }
    
    /// indicates whether this element is abstract.
    /// In the EA API this is stored as a string with the values "0" or "1"
    public bool isAbstract {
      get { return this.wrappedElement.Abstract == "1"; }
      set { this.wrappedElement.Abstract = value ? "1" : "0"; }
    }
	public override int position 
	{
		get 
		{
			return wrappedElement.TreePos;
		}
		set 
		{
			this.wrappedElement.TreePos = value;
		}
	}
    /// the visibility indicates the scope of the element
    public UML.Classes.Kernel.VisibilityKind visibility {
      get {
        return VisibilityKind.getUMLVisibilityKind
          ( this.wrappedElement.Visibility, 
            UML.Classes.Kernel.VisibilityKind._public );
      }
      set {
        this.wrappedElement.Visibility = 
          VisibilityKind.getEAVisibility(value);
      }
    }

    /// returns the attributes owned by this class
    /// This seems to be a subset of the classifiers attributes property, but 
    /// is is unclear what exactly the difference is.
    public HashSet<UML.Classes.Kernel.Property> ownedAttributes {
      get { return this.attributes; }
      set { throw new NotImplementedException(); }
    }

    public HashSet<UML.Classes.Kernel.Property> attributes {
      get 
      {
      	if (this._attributes == null)
      	{
      		this._attributes = new HashSet<UML.Classes.Kernel.Property>(this.attributeWrappers.OfType<UML.Classes.Kernel.Property>());
      	}
        return this._attributes;
      }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<AttributeWrapper> attributeWrappers
    {
    	get
    	{
    		if (this._attributeWrappers == null)
    		{
    			this._attributeWrappers = new HashSet<AttributeWrapper>(Factory.getInstance(this.model)
      		                   .createElements( this.wrappedElement.Attributes).Cast<AttributeWrapper>());
    		}
    		return this._attributeWrappers;
    	}
    }
    


    /// represents the internal EA's elementID
    public int id {
      get { return this.wrappedElement.ElementID; }
    }
    
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get {
        List<UML.Classes.Kernel.Element> elements =
          this.model.factory.createElements
            ( this.wrappedElement.Elements ).ToList();
        elements.AddRange
          (this.ownedAttributes.Cast<UML.Classes.Kernel.Element>());
        elements.AddRange
          (this.ownedOperations.Cast<UML.Classes.Kernel.Element>());
        return new HashSet<UML.Classes.Kernel.Element>(elements);
      }
      set { throw new NotImplementedException(); }
    }
    
    
    public override UML.Classes.Kernel.Element owner {
      get { 
    		// if the parentID is filled in then this element is owned by
    		// another element, otherwise it is owned by a package
    		    		
    		if (this.wrappedElement.ParentID > 0)
    		{
    			this._owner = this.model.getElementWrapperByID(this.wrappedElement.ParentID);
    		}else
    		{
    			this._owner = this.model.getElementWrapperByPackageID(this.wrappedElement.PackageID);
    		}
    		
			return this._owner;
			
    	}
      set { throw new NotImplementedException(); }
    }
    public void setStereotype(string stereotype)
    {
    	this.wrappedElement.StereotypeEx = stereotype;
    }
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
      get 
      {
    		Factory factory = (Factory)this.model.factory;
    		if (this.wrappedElement != null)
    		{
	    		string stereotypeString = this.wrappedElement.StereotypeEx;
	        	return factory.createStereotypes(this, stereotypeString );
    		}
    		else
    		{
    			return new HashSet<UML.Profiles.Stereotype>();
    		}
      }
      set 
      {
      	this.wrappedElement.StereotypeEx = Stereotype.getStereotypeEx(value);
      }
    }

    /// EA provides a shortcut tot the superclasses through its 
    /// element.BaseClasses
    /// Normally we would get those via the element.generalizations.general
    public HashSet<UML.Classes.Kernel.Class> superClasses {
      get {
        return new HashSet<UML.Classes.Kernel.Class>
          (this.model.factory.createElements(this.wrappedElement.BaseClasses)
          .Cast<UML.Classes.Kernel.Class>());
      }
      set { throw new NotImplementedException(); }
    }
	
       /// EA provides a shortcut tot the superclasses through its 
    /// element.BaseClasses
    /// Normally we would get those via the element.generalizations.general
    public HashSet<UML.Classes.Kernel.Class> subClasses {
      get {
    		var returnedSubclasses = new  HashSet<UML.Classes.Kernel.Class>();
    		foreach (var subclass in this.generalizations.Where(x => this.Equals(x.target)).Select(y => y.source as Class))
    		{
    			if (subclass != null) returnedSubclasses.Add(subclass);
    		}
    		return returnedSubclasses;
      }
    }
    /// the generalisations (inheritance relations) of this element
    public HashSet<UML.Classes.Kernel.Generalization> generalizations {
      get {
        return new HashSet<UML.Classes.Kernel.Generalization>
          (this.getRelationships<UML.Classes.Kernel.Generalization>());
      }
      set { throw new NotImplementedException(); }
    }
    /// the operations owned by this element
    public HashSet<UML.Classes.Kernel.Operation> ownedOperations {
      get {
        return new HashSet<UML.Classes.Kernel.Operation>
          (this.model.factory.createElements(this.wrappedElement.Methods)
          .Cast<UML.Classes.Kernel.Operation>());
      }
      set { throw new NotImplementedException(); }
    }

    /// <summary>
    /// returns the Relationships with the given type T
    /// </summary>
    /// <returns>the relations of type T</returns>
    public override List<T> getRelationships<T>() 
    {
    	if (this._allRelationships == null)
    	{
	    	//to make sure the connectors collection is still accurate we do a refresh first
	    	this.WrappedElement.Connectors.Refresh();
			this._allRelationships = this.model.factory.createElements(this.wrappedElement.Connectors).Cast<UML.Classes.Kernel.Relationship>().ToList();
    	}
		List<T> returnedRelationships = new List<T>();
		// we still need to filter out those relationships that are there because of linked features
		foreach (UML.Classes.Kernel.Relationship relationship in this._allRelationships) 
		{
			if (relationship is T) 
			{
				foreach (UML.Classes.Kernel.Element relatedElement in relationship.relatedElements) 
				{
					if (this.Equals(relatedElement))
					{
						returnedRelationships.Add((T)relationship);
					}
				}
			}
		}
		return returnedRelationships;
    }
    
    public override List<UML.Classes.Kernel.Relationship> relationships 
    {
      get { return this.getRelationships<UML.Classes.Kernel.Relationship>(); }
      set { throw new NotImplementedException(); }
    }

    /// from Dependencies.Named Element.
    /// contains the dependencies that have this element as client
    public List<UML.Classes.Dependencies.Dependency> clientDependencies {
      get {
        List<UML.Classes.Dependencies.Dependency> returnedDependencies = 
          new List<UML.Classes.Dependencies.Dependency>();
        foreach(UML.Classes.Dependencies.Dependency dependency 
                in this.getRelationships<Dependency>() )
        {
          if(dependency.client.Equals(this)) {
            returnedDependencies.Add(dependency);
          }
        }
        return returnedDependencies;
      }
      set { throw new NotImplementedException(); }
    }
    /// from Dependencies.Named Element.
    /// contains the dependencies that have this element as supplier
    public List<UML.Classes.Dependencies.Dependency> supplierDependencies {
      get {
        List<UML.Classes.Dependencies.Dependency> returnedDependencies = 
          new List<UML.Classes.Dependencies.Dependency>();
        foreach( UML.Classes.Dependencies.Dependency dependency 
                 in this.getRelationships<Dependency>() )
        {
        	if(dependency.supplier.Equals(this)) {
            returnedDependencies.Add(dependency);
          }
        }
        return returnedDependencies;
      }
      set { throw new NotImplementedException(); }
    }
    
    public String qualifiedName {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.Namespace owningNamespace {
      get {
    		return this.model.getElementWrapperByPackageID(wrappedElement.PackageID) as Package;
    	}
      set { throw new NotImplementedException(); }
    }
    
    /// two elementwrappers represent the same object 
    /// if their ea.guid are the same
    public override bool Equals(object obj){
      ElementWrapper otherElement = obj as ElementWrapper;
      return otherElement != null 
      	&& otherElement.wrappedElement != null
      	&& this.wrappedElement != null
        && this.wrappedElement.ElementGUID == otherElement.wrappedElement.ElementGUID;
    }

    /// return the hashcode based on the elements guid
    public override int GetHashCode(){
      return new Guid(this.wrappedElement.ElementGUID).GetHashCode();
    }
	/// <summary>
    /// creates a new diagram under this element
    /// </summary>
    /// <param name="name">the name of the new diagram</param>
    /// <returns>the new diagram</returns>
    public virtual T addOwnedDiagram<T>(String name) where T: class, UML.Diagrams.Diagram
    {
    	return ((Factory)this.model.factory).addNewDiagramToEACollection<T>(this.wrappedElement.Diagrams,name);
    }
    /// creates a new element of the given type as an owned element of this 
    /// element
    public T addOwnedElement<T>(String name) 
      where T : class, UML.Classes.Kernel.Element 
    {
    	return this.addOwnedElement<T>(name, typeof(T).Name);
    }
    /// creates a new element of the given type as an owned element of this 
    /// element
    public virtual T addOwnedElement<T>(String name, string EAType) 
      where T : class, UML.Classes.Kernel.Element 
    {	
      System.Type type = typeof(T);
      T newElement;

      if(((Factory)this.model.factory).isEAAtttribute(type)) 
      {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>( this.wrappedElement.Attributes, name, string.Empty  );
      } 
      else if(((Factory)this.model.factory).isEAOperation(type))
      {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>( this.wrappedElement.Methods, name, EAType  );
      }
      else if (((Factory)this.model.factory).isEAConnector(type))
      {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>( this.wrappedElement.Connectors, name, EAType  );
      }
	  else if (((Factory)this.model.factory).isEAParameter(type))
      {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>( this.wrappedElement.Connectors, name, EAType  );
      }     
      else
      {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>( this.wrappedElement.Elements, name, EAType );
      }
      return newElement;
    }

    internal override void saveElement(){
      this.wrappedElement.Update();
    }

    public override String notes {
      get { return this.wrappedElement.Notes;  }
      set { this.wrappedElement.Notes = value; }
    }
    public virtual HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> ownedDiagrams {
		get {
    		HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> diagrams = new HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram>();
    		foreach ( global::EA.Diagram eaDiagram in this.wrappedElement.Diagrams)
    		{
    			diagrams.Add(((Factory)this.model.factory).createDiagram(eaDiagram));
    		}
    		return diagrams;
		}
		set {
			throw new NotImplementedException();
		}
	}
    //returns a list of diagrams that somehow use this element.
    public override HashSet<T> getUsingDiagrams<T>() 
    {
        string sqlGetDiagrams = @"select distinct d.Diagram_ID from t_DiagramObjects d
                                  where d.Object_ID = " + this.wrappedElement.ElementID;
        List<UML.Diagrams.Diagram> allDiagrams = this.model.getDiagramsByQuery(sqlGetDiagrams).Cast<UML.Diagrams.Diagram>().ToList(); ; ;
        HashSet<T> returnedDiagrams = new HashSet<T>();
        foreach (UML.Diagrams.Diagram diagram in allDiagrams)
        {
            if (diagram is T)
            {
                T typedDiagram = (T)diagram;
                if (!returnedDiagrams.Contains(typedDiagram))
                {
                    returnedDiagrams.Add(typedDiagram);
                }
            }
        }
        return returnedDiagrams;
    }
    /// <summary>
    /// returns the attributes that use this Element as type
    /// </summary>
    /// <returns>the attributes that use this Element as type</returns>
    public HashSet<UML.Classes.Kernel.Property> getUsingAttributes()
    {
    	string sqlGetAttributes = @"select a.ea_guid from t_attribute a
    								where a.Classifier = '"+this.wrappedElement.ElementID.ToString()+"'";
    	return new HashSet<UML.Classes.Kernel.Property>(this.model.getAttributesByQuery(sqlGetAttributes));
    	
    }
    /// <summary>
    /// gets the TypedElements of the given type that use this element as type
    /// </summary>
    /// <returns>the TypedElements that use this element as type</returns>
    public HashSet<T> getDependentTypedElements<T>() where T:UML.Classes.Kernel.TypedElement
    {
    	HashSet<T> dependentTypedElements = new HashSet<T>();
    	// get the attributes
    	foreach (UML.Classes.Kernel.Property attribute in this.getUsingAttributes())
    	{
    		if (attribute is T)
    		{
    			dependentTypedElements.Add((T)attribute);
    		}
    	}
    	// get the parameters
    	foreach (UML.Classes.Kernel.Parameter parameter in this.getUsingParameters())
    	{
    		if (parameter is T)
    		{
    			dependentTypedElements.Add((T)parameter);
    		}
    	}
    	return dependentTypedElements;
    }
    /// <summary>
    /// convenience method to return the Information flows that convey this classifier
    /// </summary>
    /// <returns>all InformationFlows that convey this classifier</returns>
	public HashSet<UML.InfomationFlows.InformationFlow> getConveyingFlows()
	{
		var conveyingFlows = new HashSet<UML.InfomationFlows.InformationFlow>();
		string sqlGetInformationFlows =  @"select c.Connector_ID
									from ( t_xref x 
									inner join t_connector c on c.ea_guid like x.client)
									where x.Name = 'MOFProps' 
									and x.Behavior = 'conveyed'
									and x.Description like '%" + this.guid +"%'";
		foreach (var connector  in this.model.getRelationsByQuery(sqlGetInformationFlows)) 
		{
			InformationFlow informationFlow = connector as InformationFlow;
			if (informationFlow != null)
			{
				conveyingFlows.Add(informationFlow);
			}
		}
		return conveyingFlows;
	}



    /// <summary>
    /// returns the parameters having use this Element as type
    /// </summary>
    /// <returns>the parameters that use this element as type</returns>
	public HashSet<UML.Classes.Kernel.Parameter> getUsingParameters()
	{
		// get the "regular" parameters
		string sqlGetParameters = @"select p.ea_guid from t_operationparams p
    								where p.Classifier = '"+this.wrappedElement.ElementID.ToString()+"'";
    	HashSet<UML.Classes.Kernel.Parameter> parameters = new HashSet<UML.Classes.Kernel.Parameter>(this.model.getParametersByQuery(sqlGetParameters));
    	// get the return parameters
    	foreach (Operation operation in this.getOperationsWithMeAsReturntype()) 
    	{
    		parameters.Add(((Factory)this.model.factory).createEAParameterReturnType(operation));
    	} 
    	return parameters;
	}
	/// <summary>
	/// returns the operations that have this element as return type
	/// </summary>
	/// <returns>the operations that have this element as return type</returns>
	public HashSet<UML.Classes.Kernel.Operation> getOperationsWithMeAsReturntype()
	{
		// get the return-parameters
    	string sqlGetReturnParameters = @"select o.OperationID from t_operation o
    								where o.Classifier = '"+this.wrappedElement.ElementID.ToString()+"'";
    	return new HashSet<UML.Classes.Kernel.Operation>(this.model.getOperationsByQuery(sqlGetReturnParameters));
	}
	public override void open()
	{
		this.model.selectedElement = this;
	}
	public override void select()
	{
		this.model.selectedElement = this;
	}
	/// <summary>
  	/// gets the item from the given relative path.
  	/// </summary>
  	/// <param name="relativePath">the "." separated path</param>
  	/// <returns>the item with the given path</returns>
	public override UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
	{
		UML.Extended.UMLItem item = null;
		List<string> filteredPath = new List<string>(relativePath);
		if (filterName( filteredPath,this.name))
		{
			if (filteredPath.Count > 1)
			{
				//remove first item from filteredPath
				filteredPath.RemoveAt(0);
				//search deeper
				foreach ( UML.Classes.Kernel.Element element in this.ownedElements) 
				{
				
					item = element.getItemFromRelativePath(filteredPath);
					if (item != null)
					{
						return item;
					}
				}
				//still not found, now search diagram
				foreach ( UML.Diagrams.Diagram diagram in this.ownedDiagrams) 
				{
					item = diagram.getItemFromRelativePath(filteredPath);
					if (item != null)
					{
						return item;
					}
				}

			}
			else
			{
				item = this;
			}
		}
		
		return item;
	}
	internal static bool filterName(List<string> path, string name)
	{
		List<string> nameparts = name.Split('.').ToList<string>();
		List<string> newPath = new List<string>();
		bool found = false;
		if (path.Count > 0 
		    && path.Count >= nameparts.Count)
		{
			foreach (string namePart  in nameparts) 
			{
				// if "(" is present in the path, and not in the name
				// then only check the part before the "("
				if (path[0].Contains("(")
				    && !name.Contains("("))
				{
					path[0] = path[0].Substring(0,path[0].IndexOf("("));
				}
				if (namePart == path[0])
				{
					//pop the first one of the path
					path.RemoveAt(0);
				}
				else
				{
					//not correct, don't bother searching further
					return false;
				}						
			}
			//all strings matched, add name as first item;
			path.Insert(0,name);
			found = true;
		}
		return found;
	}
	public TSF.UmlToolingFramework.UML.CommonBehaviors.BasicBehaviors.BehavioralFeature specification 
	{
		get 
		{
			string sqlQuery = "select OperationID from t_operation where Behaviour like '" + this.wrappedElement.ElementGUID + "'";
			List<Operation> operations =  this.model.getOperationsByQuery(sqlQuery);
			if (operations.Count > 0)
			{
				return operations[0];
			}else
			{
				return null;
			}
		}
		set 
		{
			throw new NotImplementedException();
		}
	}
	public override HashSet<UML.Profiles.TaggedValue> taggedValues 
	{
		get 
		{
			//make sure we have an up-to-date collection
			this.wrappedElement.TaggedValues.Refresh();
			return new HashSet<UML.Profiles.TaggedValue>(this.model.factory.createTaggedValues(this.wrappedElement.TaggedValues));
		}
		set { throw new NotImplementedException();}
	}
	public override HashSet<TSF.UmlToolingFramework.UML.Profiles.TaggedValue> getReferencingTaggedValues()
	{
		return this.model.getTaggedValuesWithValue(this.wrappedElement.ElementGUID);
	}
  	
	public override TSF.UmlToolingFramework.UML.Diagrams.Diagram compositeDiagram 
	{
		get { return this.model.factory.createDiagram(this.wrappedElement.CompositeDiagram) ;}
		set { this.wrappedElement.SetCompositeDiagram(((Diagram)value).diagramGUID); }
	}

  	

  	
	internal override global::EA.Collection eaTaggedValuesCollection {
		get 
		{
			return this.wrappedElement.TaggedValues;
		}
	}
  	
	public override string guid {
		get 
		{
			return this.WrappedElement.ElementGUID;
		}
	}

	/// <summary>
	/// deletes an element owned by this element
	/// </summary>
	/// <param name="ownedElement">the owned element to delete</param>
	public override void deleteOwnedElement(Element ownedElement)
	{
		if (ownedElement is ElementWrapper)
		{
			for (short i = 0; i< this.WrappedElement.Elements.Count; i++)
			{
				var eaElement = this.WrappedElement.Elements.GetAt(i) as global::EA.Element;
				if (eaElement.ElementGUID == ownedElement.guid)
				{   
					this.WrappedElement.Elements.Delete(i);
					this.WrappedElement.Elements.Refresh();
					break;
				}
			}
		}
		else if (ownedElement is AttributeWrapper)
		{
			for (short i = 0; i< this.WrappedElement.Attributes.Count; i++)
			{
				var eaAttribute = this.WrappedElement.Attributes.GetAt(i) as global::EA.Attribute;
				if (eaAttribute.AttributeGUID == ownedElement.guid)
				{   
					this.WrappedElement.Attributes.Delete(i);
					this.WrappedElement.Attributes.Refresh();
					break;
				}
			}
		}
		else if (ownedElement is ConnectorWrapper)
		{
			for (short i = 0; i< this.WrappedElement.Connectors.Count; i++)
			{
				var eaConnector = this.WrappedElement.Connectors.GetAt(i) as global::EA.Connector;
				if (eaConnector.ConnectorGUID == ownedElement.guid)
				{   
					this.WrappedElement.Connectors.Delete(i);
					this.WrappedElement.Connectors.Refresh();
					break;
				}
			}
		}
		else if (ownedElement is Operation)
		{
			for (short i = 0; i< this.WrappedElement.Methods.Count; i++)
			{
				var eaMethod = this.WrappedElement.Methods.GetAt(i) as global::EA.Method;
				if (eaMethod.MethodGUID == ownedElement.guid)
				{   
					this.WrappedElement.Methods.Delete(i);
					this.WrappedElement.Methods.Refresh();
					break;
				}
			}
		}
		else
		{
			//type not supported (yet)
			throw new NotImplementedException();
		}
	}
		/// <summary>
        /// returns the name of the user currently locking this element
        /// </summary>
        /// <returns>the name of the user currently locking this element</returns>
        public override string getLockedUser()
        {
            string lockedUser = string.Empty;
            //if (this.wrappedElement.Locked)
            //{
                string SQLQuery = @"select u.FirstName, u.Surname from t_seclocks s
                                    inner join t_secuser u on s.userID = u.userID 
                                    where s.entityID = '" + this.guid + "'";
                var result = this.model.SQLQuery(SQLQuery);
                XmlNode firstNameNode = result.SelectSingleNode("//FirstName");
                XmlNode lastNameNode = result.SelectSingleNode("//Surname");
                if (firstNameNode != null && lastNameNode != null)
                {
                    lockedUser = firstNameNode.InnerText + " " + lastNameNode.InnerText;
                }
            //}
            return lockedUser;
        }
        public override string getLockedUserID()
        {
        	    string SQLQuery = @"select s.UserID from t_seclocks s
                                    where s.entityID = '" + this.guid+ "'";
                XmlDocument result = this.model.SQLQuery(SQLQuery);
                XmlNode userIDNode = result.SelectSingleNode("//UserID");
                return userIDNode.InnerText;
        }
        /// <summary>
        /// Makes the element writeable
        /// </summary>
        /// <param name="overrideLocks">remove any existing locks</param>
        /// <returns>true if successful</returns>
		public override bool makeWritable(bool overrideLocks)
		{
			//if security is not enabled then it is always writeable
			if (!this.model.isSecurityEnabled) return true;
			//if already writable return true
			if (!this.isReadOnly) return true;
			//TODO: override locks
            if (! this.isLocked)
            {
                //no lock found, go ahead and try to lock the element
                try
                {
                    return this.wrappedElement.ApplyUserLock();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else  
            {
                //lock found, don't even try it.
                return false;
            }
		}
		public HashSet<UML.Classes.Kernel.EnumerationLiteral> ownedLiterals 
		{
			get 
			{
		      	if (this._ownedLiterals == null)
		      	{
		      		this._ownedLiterals = new HashSet<UML.Classes.Kernel.EnumerationLiteral>(this.attributeWrappers.OfType<UML.Classes.Kernel.EnumerationLiteral>());
		      	}
		        return this._ownedLiterals;
			}
			set {
				throw new NotImplementedException();
			}
		}

		List<Attribute> getOwnedAttributes(string attributeName)
		{
			//owner.Attribute
			string sqlGetAttributes = "select a.ea_guid from t_attribute a " +
									" where a.Object_ID = " + this.id +
									" and a.name = '" + attributeName + "'";
			return this.model.getAttributesByQuery(sqlGetAttributes);
		}

		public List<ElementWrapper> getOwnedElements(string elementName)
		{
			string sqlGetOwnedElement = "select o.Object_ID from t_object o " +
										" where " +
										" o.Name = '" + elementName + "' " +
										" and o.ParentID = " + this.id;
			return this.model.getElementWrappersByQuery(sqlGetOwnedElement);
		}
		public List<Diagram> getOwnedDiagrams(string diagramName)
		{
			string sqlGetOwnedDiagram = "select d.Diagram_ID from t_diagram d " +
										" where " +
										" d.Name = '" + diagramName + "' " +
										" and d.ParentID = " + this.id;
			return this.model.getDiagramsByQuery(sqlGetOwnedDiagram);
		}
		public List<AssociationEnd> getRelatedAssociationEnds(string rolename)
		{
			var assocationEnds = new List<AssociationEnd>();
			//first get the assocations, then get the end that corresponds with the rolename
			string sqlGetConnectorWrappers = "select c.Connector_ID from t_connector c " +
										" where c.Start_Object_ID = " + this.id + 
										" and c.DestRole = '" + rolename + "' " +
										" union " +
										" select c.Connector_ID from t_connector c " +
										" where c.End_Object_ID = " + this.id + 
										" and c.SourceRole = '" + rolename + "' ";
			var connectorWrappers = this.model.getRelationsByQuery(sqlGetConnectorWrappers);
			//loop the associations to get the ends
			foreach (var connectorWrapper in connectorWrappers)
			{
				if (connectorWrapper.sourceEnd != null 
				    && connectorWrapper.sourceEnd.name == rolename
				    && ! this.Equals(connectorWrapper.source))
				{
					assocationEnds.Add(connectorWrapper.sourceEnd);
				}
				if (connectorWrapper.targetEnd != null 
				    && connectorWrapper.targetEnd.name == rolename
				    && ! this.Equals(connectorWrapper.target))
				{
					assocationEnds.Add(connectorWrapper.targetEnd);
				}
			}
			return assocationEnds;
		}
		#region implemented abstract members of Element

		public override List<UML.Extended.UMLItem> findOwnedItems(List<string> descriptionParts)
		{
			List<UML.Extended.UMLItem> ownedItems =new List<UML.Extended.UMLItem>();
			if (descriptionParts.Count > 0)
			{
				string firstpart = descriptionParts[0];
					//start by finding an element with the given name
				var directOwnedElements = getOwnedElements(firstpart);
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
					ownedItems.AddRange(getOwnedDiagrams(firstpart));
				}
			}
			return ownedItems;
		}

		#endregion
  }
}
