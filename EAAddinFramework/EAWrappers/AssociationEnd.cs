using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class AssociationEnd : Element , UML.Classes.Kernel.Property {
    internal global::EA.ConnectorEnd wrappedAssociationEnd { get; set; }

    private ConnectorWrapper _association { get; set; }

    public AssociationEnd(Model model, ConnectorWrapper linkedAssocation,
                          global::EA.ConnectorEnd associationEnd ):base(model)
    {
      this.wrappedAssociationEnd = associationEnd;
      this._association = linkedAssocation;
    }

    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { return new HashSet<UML.Classes.Kernel.Element>(); }
      set { /* do nothing */  }
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
        return ((Factory) this.model.factory).createStereotypes(
                 this, this.wrappedAssociationEnd.StereotypeEx);
      }
      set { throw new NotImplementedException(); }
    }

    public bool isDerived {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public bool isDerivedUnion {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public bool isComposite {
      get {
        return this.aggregation ==
          UML.Classes.Kernel.AggregationKind.composite;
      }
      set {
        if( value ) {
          this.aggregation = UML.Classes.Kernel.AggregationKind.composite;
        } else if( this.isComposite ) {
            this.aggregation = UML.Classes.Kernel.AggregationKind.shared;
        }
      }
    }

    public String _default {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.AggregationKind aggregation {
      get {
        return AggregationKind.getUMLAggregationKind(
          this.wrappedAssociationEnd.Aggregation);
      }
      set {
        this.wrappedAssociationEnd.Aggregation =
          AggregationKind.getEAAggregationKind(value);
      }
    }

    public UML.Classes.Kernel.ValueSpecification defaultValue {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public HashSet<UML.Classes.Kernel.Property> redefinedProperties {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public HashSet<UML.Classes.Kernel.Property> subsettedProperties {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.Property opposite {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.Classifier classifier {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.Class _class {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.Association owningAssociation {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.Association association {
      get { return this._association as Association;  }
      set { this._association = value as Association; }
    }

    public UML.Classes.Kernel.DataType datatype {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public bool isReadOnly {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    /// isStatic is not supported by AssociationEnd in EA
    public bool isStatic {
      get { return false;    }
      set { /* do nothing */ }
    }

    public HashSet<UML.Classes.Kernel.Classifier> featuringClassifiers {
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

    public String name {
      get { return this.wrappedAssociationEnd.Role;  }
      set { this.wrappedAssociationEnd.Role = value; }
    }

    public UML.Classes.Kernel.VisibilityKind visibility {
      get {
        return VisibilityKind.getUMLVisibilityKind
          ( this.wrappedAssociationEnd.Visibility,
            UML.Classes.Kernel.VisibilityKind._public );
      }
      set {
        this.wrappedAssociationEnd.Visibility =
          VisibilityKind.getEAVisibility(value);
      }
    }

    public String qualifiedName {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.Namespace owningNamespace {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    /// The type of an associationend is stored in the EA.Association class
    /// itself in the fields ClientID and SupplierID
    public UML.Classes.Kernel.Type type {
      get {
        if( this.wrappedAssociationEnd.End == "Supplier" ) {
          return this._association.target as UML.Classes.Kernel.Type;
        } else {
          return this._association.source as UML.Classes.Kernel.Type;
        }
      }
      set { throw new NotImplementedException(); }
    }

    public bool isOrdered {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public bool isUnique {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.UnlimitedNatural upper {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public int lower {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.ValueSpecification upperValue {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.ValueSpecification lowerValue {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public bool isNavigable(){
      return this.wrappedAssociationEnd.Navigable == "Navigable";
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
      this.wrappedAssociationEnd.Update();
    }

    public override string notes {
      get { return this.wrappedAssociationEnd.RoleNote;  }
      set { this.wrappedAssociationEnd.RoleNote = value; }
    }
    
    public bool _isNavigable {
      get { return this.wrappedAssociationEnd.Navigable == "Navigable"; }
      set {
        if (value) {
          this.wrappedAssociationEnd.Navigable = "Navigable";
        }else {
          this.wrappedAssociationEnd.Navigable = "Non-Navigable";
        }
      }
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
  }
}
