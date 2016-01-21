using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
  public class ConnectorWrapper : Element, UML.Classes.Kernel.Relationship 
  {
    internal global::EA.Connector wrappedConnector { get; set; }    
    private UML.Classes.Kernel.Element _owner;
    private UML.Classes.Kernel.Element _source;
    private UML.Classes.Kernel.Element _target;
    private AssociationEnd _sourceEnd;
    private AssociationEnd _targetEnd;
    public int id
    {
    	get{return this.wrappedConnector.ConnectorID;}
    }

    public ConnectorWrapper(Model model, global::EA.Connector connector)
      : base(model)
    {
      this.wrappedConnector = connector;
    }
    public global::EA.Connector WrappedConnector {
    	get { return this.wrappedConnector; }
    }
	public override void open()
	{
		this.owner.open();
	}
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { return new HashSet<UML.Classes.Kernel.Element>(); }
      set { throw new NotImplementedException(); }
    }

    /// <summary>
    /// not fully correct, but we will return the element at the source of the relation
    /// TODO: fix this so it uses the actual ownership as prescribed by UML
    /// </summary>
    public override UML.Classes.Kernel.Element owner {
      get 
      {
// removed caching to try and solve multithreading issue.      	
//      	if (this._owner == null)
//      	{
      		this._owner = this.model.getElementWrapperByID(this.wrappedConnector.ClientID);
//      	}
      	return this._owner;
      }
      set { throw new NotImplementedException(); }
    }
    /// the stereotypes defined on this Relationship
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
      get {
        return ((Factory)this.model.factory).createStereotypes
          ( this, this.wrappedConnector.StereotypeEx );
      }
      set 
      { 
      	this.WrappedConnector.StereotypeEx = Stereotype.getStereotypeEx(value); 
      }
    }
    /// returns the related elements.
    /// In EA the Connectoris a binary relationship. So only two Elements will 
    /// ever be returned.
    /// If there is a linked feature then the linked feature will be returned instead of the EA.Element.
    public List<UML.Classes.Kernel.Element> relatedElements {
      get 
      {
        var returnedElements =  new List<UML.Classes.Kernel.Element>();
        returnedElements.Add(this.source);
        returnedElements.Add(this.target);
        return returnedElements;
      }
      set { throw new NotImplementedException(); }
    }
    private UML.UMLItem getLinkedFeature(bool start)
    {
    	string styleEx = this.wrappedConnector.StyleEx;
    	string key;
    	UML.UMLItem linkedFeature = null;
    	//determine start or end keyword
		key = start ? "LFSP=" : "LFEP=";
   		int guidStart = styleEx.IndexOf(key) + key.Length ;
   		if (guidStart >= key.Length)
    	{
    		string featureGUID = styleEx.Substring(guidStart,this.WrappedConnector.ConnectorGUID.Length);
    		linkedFeature = this.model.getItemFromGUID(featureGUID);
    	}
    	return linkedFeature;
    }
    public UML.Classes.Kernel.Element target {
      get 
      {
      	if (this._target == null)
      	{
	     	 UML.Classes.Kernel.Element linkedSupplierFeature = this.getLinkedFeature(false) as UML.Classes.Kernel.Element;
	        if (linkedSupplierFeature != null)
	        {
	        	this._target =linkedSupplierFeature;
	        }
	        else
	        {
	        	this._target = this.model.getElementWrapperByID(this.wrappedConnector.SupplierID);
	        }
      	}
      	return this._target;
      }
      set 
      {
		if (value is ElementWrapper)
		{
			this._target = value;
			this.WrappedConnector.SupplierID = ((ElementWrapper)value).id;
		}
		else
		{
			//currently only implemented for ElementWrappers
			throw new NotImplementedException();
		}      	
      }
    }
    
    public UML.Classes.Kernel.Element source {
      get 
      {
      	if (this._source == null)
      	{
	        //check first if there is a linked feature
	        UML.Classes.Kernel.Element linkedClientFeature = this.getLinkedFeature(true) as UML.Classes.Kernel.Element;
	        if (linkedClientFeature != null)
	        {
	        	this._source = linkedClientFeature;
	        }
	        else
	        {
	        	this._source = this.model.getElementWrapperByID(this.wrappedConnector.ClientID);
	        }
      	}
      	return this._source;
      }
      set
     {
		if (value is ElementWrapper)
		{
			this._source = value;
			this.WrappedConnector.ClientID = ((ElementWrapper)value).id;
		}
		else
		{
			//currently only implemented for ElementWrappers
			throw new NotImplementedException();
		}      	
      }
    }
    
    
    public bool isDerived {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Kernel.Property> navigableOwnedEnds {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Kernel.Property> ownedEnds {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Kernel.Type> endTypes {
      get {
        var returnTypes =new List<UML.Classes.Kernel.Type>();
        foreach( UML.Classes.Kernel.Property end in memberEnds ) {
          returnTypes.Add(end.type);
        }
        return returnTypes;
      }
      set { throw new NotImplementedException(); }
    }

    /// Each end represents participation of instances of the classifier 
    /// connected to the end in links of the association. This is
    /// an ordered association. Subsets Namespace::member.
    public List<UML.Classes.Kernel.Property> memberEnds 
    {
      get {
        var returnedMembers = new List<UML.Classes.Kernel.Property>();
        returnedMembers.Add(this.sourceEnd as UML.Classes.Kernel.Property) ;
        returnedMembers.Add(this.targetEnd as UML.Classes.Kernel.Property);
        return returnedMembers;
      }
      set { throw new NotImplementedException(); }
    }
    public AssociationEnd sourceEnd
    {
    	get
    	{
    		if (this._sourceEnd == null)
    		{
    			this._sourceEnd = ((Factory)this.model.factory).createAssociationEnd(this, this.wrappedConnector.ClientEnd) ;
    		}
    		return this._sourceEnd;
    	}
    }
    public AssociationEnd targetEnd
    {
    	get
    	{
    		if (this._targetEnd == null)
    		{
    			this._targetEnd = ((Factory)this.model.factory).createAssociationEnd(this, this.wrappedConnector.SupplierEnd); 
    		}
    		return this._targetEnd;
    	}
    }
    public bool isAbstract {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Generalization> generalizations {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    public HashSet<UML.Classes.Kernel.Property> attributes {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Feature> features {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public bool isLeaf {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.RedefinableElement> redefinedElements {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Classifier> redefinitionContexts {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public string name {
      get { return this.wrappedConnector.Name;  }
      set { this.wrappedConnector.Name = value; }
    }
    
    public UML.Classes.Kernel.VisibilityKind visibility {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public String qualifiedName {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.Namespace owningNamespace {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.NamedElement client {
      get { return this.source as UML.Classes.Kernel.NamedElement; }
      set { this.source = value as Element; }
    }
    
    public UML.Classes.Kernel.NamedElement supplier {
      get { return this.target as UML.Classes.Kernel.NamedElement; }
      set { this.target = value as Element; }
    }
    
    public UML.Classes.Kernel.OpaqueExpression mapping {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Dependencies.Substitution> substitutions {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Dependencies.Dependency> clientDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Dependencies.Dependency> supplierDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    /// <summary>
    /// Due to a bug in EA we first need to save the end with aggregation kind none, and then the other end.
    /// If not then the aggregationkind of the other set is reset to none.
    /// </summary>
	public override void save()
	{
		this.WrappedConnector.Update();
		if (this.sourceEnd.aggregation == UML.Classes.Kernel.AggregationKind.none)
		{
			this.sourceEnd.save();
			this.targetEnd.save();
		}else
		{
			this.targetEnd.save();
			this.sourceEnd.save();
		}
	}
    
    internal override void saveElement()
    {
    	this.wrappedConnector.Update();
    }

    public override string notes {
      get { return this.wrappedConnector.Notes;  }
      set { this.wrappedConnector.Notes = value; }
    }

  	
	public override TSF.UmlToolingFramework.UML.UMLItem getItemFromRelativePath(List<string> relativePath)
	{
		UML.UMLItem item = null;
		if (ElementWrapper.filterName(relativePath,this.name))
		{
	    	if (relativePath.Count ==1)
	    	{
	    		item = this;
	    	}
		}
		return this; 
	}
	
	public override HashSet<UML.Profiles.TaggedValue> taggedValues
	{
		get 
		{
			return new HashSet<UML.Profiles.TaggedValue>(this.model.factory.createTaggedValues(this.wrappedConnector.TaggedValues));
		}
		set { throw new NotImplementedException();}
	}
	public override HashSet<TSF.UmlToolingFramework.UML.Profiles.TaggedValue> getReferencingTaggedValues()
	{
		return this.model.getTaggedValuesWithValue(this.wrappedConnector.ConnectorGUID);
	}
	#region Equals and GetHashCode implementation
	public override bool Equals(object obj)
	{
		ConnectorWrapper other = obj as ConnectorWrapper;
		if (other != null)
		{
			if (other.wrappedConnector.ConnectorGUID == this.wrappedConnector.ConnectorGUID)
			{
				return true;	
			}
		}
		return false;
	}
	
	public override int GetHashCode()
	{
		return new Guid(this.wrappedConnector.ConnectorGUID).GetHashCode();
	}
	#endregion
	/// <summary>
	/// adding both the start and the end element to the diagram will automatically add the connector to the diagram
	/// </summary>
	public override void addToCurrentDiagram()
	{
		foreach (UML.Classes.Kernel.Element element in this.relatedElements) 
		{
			element.addToCurrentDiagram();
		}
	}

  	/// <summary>
  	/// adds a related element to this connector.
  	/// This operation checks if the source or target is empty and then adds the related element to the empty spot.
  	/// If none of the two are empty then the target is being replaced
  	/// </summary>
  	/// <param name="relatedElement"></param>
	public void addRelatedElement(UML.Classes.Kernel.Element relatedElement)
	{
		if (this.WrappedConnector.ClientID <= 0 )
		{
			this.source = relatedElement;
		}
		else
		{
			this.target = relatedElement;
		}

	}
  	
	internal override global::EA.Collection eaTaggedValuesCollection {
		get {
			return this.WrappedConnector.TaggedValues;
		}
	}
  	
	public override string guid {
		get 
		{
			return this.WrappedConnector.ConnectorGUID;
		}
	}
  }
}
