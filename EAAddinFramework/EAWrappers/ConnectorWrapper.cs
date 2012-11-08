using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
  public class ConnectorWrapper : Element, UML.Classes.Kernel.Relationship 
  {
    internal global::EA.Connector wrappedConnector { get; set; }    
    private UML.Classes.Kernel.Element _owner;
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

    public override HashSet<UML.Classes.Kernel.Comment> ownedComments {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    /// <summary>
    /// not fully correct, but we will return the element at the source of the relation
    /// TODO: fix this so it uses the actual ownership as prescribed by UML
    /// </summary>
    public override UML.Classes.Kernel.Element owner {
      get 
      {
// removed cachign to try and solve multithreading issue.      	
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
      set { throw new NotImplementedException(); }
    }
    /// returns the related elements.
    /// In EA the Connectoris a binary relationship. So only two Elements will 
    /// ever be returned.
    public List<UML.Classes.Kernel.Element> relatedElements {
      get {
        List<UML.Classes.Kernel.Element> returnedElements = 
          new List<UML.Classes.Kernel.Element>();
        returnedElements.Add(this.model.getElementWrapperByID
          (this.wrappedConnector.ClientID));
        returnedElements.Add(this.model.getElementWrapperByID
          (this.wrappedConnector.SupplierID));
        return returnedElements;
      }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.Element target {
      get {
        return this.model.getElementWrapperByID
          ( this.wrappedConnector.SupplierID );
      }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.Element source {
      get {
        return this.model.getElementWrapperByID
          ( this.wrappedConnector.ClientID );
      }
      set { throw new NotImplementedException(); }
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
        List<UML.Classes.Kernel.Type> returnTypes =
          new List<UML.Classes.Kernel.Type>();
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
    public List<UML.Classes.Kernel.Property> memberEnds {
      get {
        List<UML.Classes.Kernel.Property> returnedMembers = 
          new List<UML.Classes.Kernel.Property>();
        returnedMembers.Add
          ( ((Factory)this.model.factory).createAssociationEnd
            (this, this.wrappedConnector.ClientEnd) 
              as UML.Classes.Kernel.Property);
        returnedMembers.Add
          (((Factory)this.model.factory).createAssociationEnd
            (this, this.wrappedConnector.SupplierEnd) 
              as UML.Classes.Kernel.Property);
        return returnedMembers;
      }
      set { throw new NotImplementedException(); }
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

    internal override void saveElement(){
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


  }
}
