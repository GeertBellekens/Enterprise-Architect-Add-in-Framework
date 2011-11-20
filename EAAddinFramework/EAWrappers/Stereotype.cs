using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Stereotype : Element, UML.Profiles.Stereotype {
    private Element _owner;
    private String _name;

    public Stereotype(Model model, Element owner, String stereotype)
      : base(model) 
    {
      this.name = stereotype;
      this.owner = owner;
    }
    public String name {
      get { return this._name;  }
      set { this._name = value; }
    }

    public UML.Classes.Kernel.VisibilityKind visibility {
      get { return this.owningNamespace.visibility; }
      set {}
    }

    public String qualifiedName {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    /// if the owning element is a namespace then this is returned.
    /// If not then the owning namespace of the owner is returned
    public UML.Classes.Kernel.Namespace owningNamespace {
      get { return this.getOwner<UML.Classes.Kernel.Namespace>(); }
      set { this.owner = value; }
    }
    
    public override UML.Classes.Kernel.Element owner {
      get { return this._owner; }
      set { this._owner = value as Element; }
    }
    
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override HashSet<UML.Classes.Kernel.Comment> ownedComments {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
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
      //nothing to save here as it is save together with the owning element
    }

    public override String notes {
      get { return string.Empty; }
      set { /* do nothing, stereotypes do not have comments in EA */ }
    }
  	
	public override TSF.UmlToolingFramework.UML.UMLItem getItemFromRelativePath(List<string> relativePath)
	{
		return null;
	}
  }
}
