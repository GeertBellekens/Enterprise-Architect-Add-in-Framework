using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  class PrimitiveType : Element, UML.Classes.Kernel.PrimitiveType {
    public PrimitiveType(Model model, String typeName) : base(model) {
      this.name = typeName;
    }

    public String name {get; set; }

    public HashSet<UML.Classes.Kernel.Operation> ownedOperations {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Property> ownedAttributes {
      get { throw new NotImplementedException(); }
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
    
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override HashSet<UML.Classes.Kernel.Comment> ownedComments {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override UML.Classes.Kernel.Element owner {
      get { return null;}
      set { 
      	// do nothing, in EA primitives don't have an owner
      }
    }
    
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
      get { return new HashSet<UML.Profiles.Stereotype>();  }
      set { /* do nothing, no stereotypes for primitives */ }
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
      // nothing to save in EA for primitive types
    }
    
    public override String notes {
      get { return String.Empty; }
      set { /* do nothing, primitive types do not have comments in EA */ }
    }

	public HashSet<T> getDependentTypedElements<T>() where T:UML.Classes.Kernel.TypedElement
    {
		throw new NotImplementedException();
    }
  	
	public HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> ownedDiagrams {
		get {
			throw new NotImplementedException();
		}
		set {
			throw new NotImplementedException();
		}
	}
  }
}
