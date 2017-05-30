using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public abstract class Parameter : Element, UML.Classes.Kernel.Parameter {
    protected Parameter(Model model) : base(model) {}
    
    public abstract UML.Classes.Kernel.ParameterDirectionKind direction 
      { get; set; }
    public abstract UML.Classes.Kernel.ValueSpecification defaultValue 
      { get; set; }
    public abstract UML.Classes.Kernel.ValueSpecification upperValue 
      { get; set; }
    public abstract UML.Classes.Kernel.ValueSpecification lowerValue 
      { get; set; }
    public UML.Classes.Kernel.Operation operation 
    {
    	get { return this.owner as UML.Classes.Kernel.Operation; }
     	set { throw new NotImplementedException(); }
    }
		public override UML.Classes.Kernel.Package owningPackage {
			get {
				return base.owningPackage;
			}
			set {
				((Operation)this.owner).owningPackage = value;
			}
		}
	public abstract UML.Classes.Kernel.Multiplicity multiplicity { get; set; }
	public abstract UML.Classes.Kernel.UnlimitedNatural upper { get; set; }
    public abstract UML.Classes.Kernel.Type type { get; set; }
    public abstract UML.Classes.Kernel.VisibilityKind visibility { get; set; }
    public abstract UML.Classes.Kernel.Namespace owningNamespace {get; set; }            
    public abstract String _default { get; set; }
    public abstract bool isOrdered { get; set; }
    public abstract bool isUnique { get; set; }
    public abstract uint lower { get; set; }
    public abstract uint position { get; set; }
    
    public abstract String qualifiedName { get; set; }

    public List<UML.Classes.Dependencies.Dependency> clientDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public List<UML.Classes.Dependencies.Dependency> supplierDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    
    public override UML.Classes.Kernel.Element owner {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    /// <summary>
	/// opens the (standard) properties dialog in EA
	/// For parameters the properties of the owner (operation) will be opened
	/// </summary>
	public override void openProperties()
	{
		this.owner.openProperties();
	}
  }
}
