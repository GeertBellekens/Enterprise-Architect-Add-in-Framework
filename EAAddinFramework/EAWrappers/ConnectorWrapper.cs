using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class ConnectorWrapper : Element, UML.Classes.Kernel.Relationship {
    internal global::EA.Connector wrappedConnector { get; set; }

    public ConnectorWrapper(Model model, global::EA.Connector connector)
      : base(model)
    {
      this.wrappedConnector = connector;
    }
    
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { return new HashSet<UML.Classes.Kernel.Element>(); }
      set { throw new NotImplementedException(); }
    }
    
    public override HashSet<UML.Classes.Kernel.Comment> ownedComments {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override UML.Classes.Kernel.Element owner {
      get { throw new NotImplementedException(); }
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

  }
}
