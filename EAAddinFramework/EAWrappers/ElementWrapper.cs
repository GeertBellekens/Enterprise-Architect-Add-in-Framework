using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	
  public class ElementWrapper : Element, UML.Classes.Kernel.NamedElement 
  {
    internal global::EA.Element wrappedElement {get; set; }
    private UML.Classes.Kernel.Element _owner;

    public ElementWrapper(Model model, global::EA.Element wrappedElement) 
      : base(model)
    {
      this.wrappedElement = wrappedElement;
    }

    public override string name {
      get { return this.wrappedElement.Name;  }
      set { this.wrappedElement.Name = value; }
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
      get {
        return new HashSet<UML.Classes.Kernel.Property>
        (Factory.getInstance().createElements
          ( this.wrappedElement.Attributes)
          .Cast<UML.Classes.Kernel.Property>());
      }
      set { throw new NotImplementedException(); }
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
    
    public override HashSet<UML.Classes.Kernel.Comment> ownedComments {
      get { throw new NotImplementedException(); }
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
      set { throw new NotImplementedException(); }
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
      List<UML.Classes.Kernel.Relationship> allRelationships = 
      	this.model.factory.createElements(this.wrappedElement.Connectors).Cast<UML.Classes.Kernel.Relationship>().ToList();
      List<T> returnedRelationships = new List<T>();
      // we still need to filter out those relationships that are there because of linked features
      foreach (UML.Classes.Kernel.Relationship relationship in allRelationships) 
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
    
    public override List<UML.Classes.Kernel.Relationship> relationships {
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
    
    /// creates a new element of the given type as an owned element of this 
    /// element
    public T addOwnedElement<T>(String name) 
      where T : class, UML.Classes.Kernel.Element 
    {
    	return this.addOwnedElement<T>(name, typeof(T).Name);
    }
    /// creates a new element of the given type as an owned element of this 
    /// element
    public T addOwnedElement<T>(String name, string EAType) 
      where T : class, UML.Classes.Kernel.Element 
    {	
      System.Type type = typeof(T);
      T newElement;

      if(((Factory)this.model.factory).isEAAtttribute(type)) {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
          ( this.wrappedElement.Attributes, name, EAType  );
      } else if(((Factory)this.model.factory).isEAOperation(type)) {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
          ( this.wrappedElement.Methods, name, EAType  );
      } else if (((Factory)this.model.factory).isEAConnector(type)) {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
          ( this.wrappedElement.Connectors, name, EAType  );
      } else {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
          ( this.wrappedElement.Elements, name, EAType );
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
	public override UML.UMLItem getItemFromRelativePath(List<string> relativePath)
	{
		UML.UMLItem item = null;
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

  	

  }
}
