using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class ElementWrapper : Element, UML.Classes.Kernel.NamedElement {
    internal global::EA.Element wrappedElement {get; set; }

    public ElementWrapper(Model model, global::EA.Element wrappedElement) 
      : base(model)
    {
      this.wrappedElement = wrappedElement;
    }

    public String name {
      get { return this.wrappedElement.Name;  }
      set { this.wrappedElement.Name = value; }
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
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
      get {
        return ((Factory)this.model.factory).createStereotypes
          ( this, this.wrappedElement.StereotypeEx );
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

    /// returns the Relationships with the given type T
    public override List<T> getRelationships<T>() {
      List<UML.Classes.Kernel.Relationship> allRelationships = 
        this.model.factory.createElements(this.wrappedElement.Connectors)
        .Cast<UML.Classes.Kernel.Relationship>().ToList();
      List<T> returnedRelationships = new List<T>();
      foreach (UML.Classes.Kernel.Relationship relationship 
               in allRelationships) 
      {
        if (relationship is T) {
          returnedRelationships.Add((T)relationship);
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
          if( dependency.client == this ) {
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
          if( dependency.supplier == this ) {
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
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    /// two elementwrappers represent the same object 
    /// if their ea.guid are the same
    public override bool Equals(object obj){
      ElementWrapper otherElement = obj as ElementWrapper;
      return otherElement != null 
             && this.wrappedElement.ElementGUID == 
                otherElement.wrappedElement.ElementGUID;
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
      System.Type type = typeof(T);
      T newElement;

      if(((Factory)this.model.factory).isEAAtttribute(type)) {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
          ( this.wrappedElement.Attributes, name );
      } else if(((Factory)this.model.factory).isEAOperation(type)) {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
          ( this.wrappedElement.Methods, name );
      } else if (((Factory)this.model.factory).isEAConnector(type)) {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
          ( this.wrappedElement.Connectors, name );
      } else {
        newElement = ((Factory)this.model.factory).addElementToEACollection<T>
          ( this.wrappedElement.Elements, name );
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
    //returns a list of diagrams that somehow use this element.
    public override HashSet<T> getUsingDiagrams<T>() 
    {
        string sqlGetDiagrams = @"select disctinct d.diagram_ID from DiagramObjects d
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
  }
}
