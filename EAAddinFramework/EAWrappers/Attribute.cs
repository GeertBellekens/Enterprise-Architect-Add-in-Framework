using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Attribute : Element, UML.Classes.Kernel.Property {
    internal global::EA.Attribute wrappedAttribute { get; set; }

    public Attribute(Model model, global::EA.Attribute wrappedAttribute) 
      : base(model)
    {
      this.wrappedAttribute = wrappedAttribute;
    }

    public global::EA.Attribute WrappedAttribute
    {
    	get { return wrappedAttribute; }
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
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public string _default {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.AggregationKind aggregation {
      get { return UML.Classes.Kernel.AggregationKind.none; }
      set { /* do nothing */ }
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
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.DataType datatype {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public bool isReadOnly {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    /// the isStatic property defines context of the attribute.
    /// If true then the context is the class
    /// If false then the context is the instance.
    public bool isStatic {
      get { return this.wrappedAttribute.IsStatic;  }
      set { this.wrappedAttribute.IsStatic = value; }
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

    public string name {
      get { return this.wrappedAttribute.Name;  }
      set { this.wrappedAttribute.Name = value; }
    }
    public UML.Classes.Kernel.VisibilityKind visibility {
      get {
        return VisibilityKind.getUMLVisibilityKind
          ( this.wrappedAttribute.Visibility, 
            UML.Classes.Kernel.VisibilityKind._private );
      }
      set {
        this.wrappedAttribute.Visibility = 
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
    
    public UML.Classes.Kernel.Type type {
      get {
        UML.Classes.Kernel.Type type = this.model.getElementWrapperByID
          ( this.wrappedAttribute.ClassifierID ) as UML.Classes.Kernel.Type;
        // check if the type is defined as an element in the model.
        if( type == null ) {
          // no element, create primitive type based on the name of the type
          type = this.model.factory.createPrimitiveType
            (this.wrappedAttribute.Type);
        }
        return type;
      }
      set {
        if( value is ElementWrapper ) {
          this.wrappedAttribute.ClassifierID = ((ElementWrapper)value).id;
        } else {
          this.wrappedAttribute.Type = value.name;
        }
      }
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
    
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { return new HashSet<UML.Classes.Kernel.Element>(); }
      set { throw new NotImplementedException(); }
    }

    public override HashSet<UML.Classes.Kernel.Comment> ownedComments {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override UML.Classes.Kernel.Element owner {
      get { return this.model.getElementWrapperByID(this.wrappedAttribute.ParentID);}
      set { throw new NotImplementedException(); }
    }
    
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
      get {
        return ((Factory)this.model.factory).createStereotypes
          ( this, this.wrappedAttribute.StereotypeEx );
      }
      set { throw new NotImplementedException(); }
    }
    
    public bool isNavigable(){ throw new NotImplementedException(); }
    
    public List<UML.Classes.Dependencies.Dependency> clientDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Dependencies.Dependency> supplierDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    internal override void saveElement(){
      this.wrappedAttribute.Update();
    }

    public override String notes {
      get { return this.wrappedAttribute.Notes;  }
      set { this.wrappedAttribute.Notes = value; }
    }
    
    public bool _isNavigable {
      get { return true; }
      set { /* do nothing */ }
    }

  	
	public override TSF.UmlToolingFramework.UML.UMLItem getItemFromRelativePath(List<string> relativePath)
	{
		UML.UMLItem item = null;
		List<string> filteredPath = new List<string>(relativePath);
		if (ElementWrapper.filterName( filteredPath,this.name))
		{
	    	if (filteredPath.Count ==1)
	    	{
	    		item = this;
	    	}
		}
		return item; 
	}
	
	public override HashSet<UML.Profiles.TaggedValue> taggedValues
	{
		get 
		{
			return new HashSet<UML.Profiles.TaggedValue>(this.model.factory.createTaggedValues(this.wrappedAttribute.TaggedValues));
		}
		set { throw new NotImplementedException();}
	}
	public override HashSet<TSF.UmlToolingFramework.UML.Profiles.TaggedValue> getReferencingTaggedValues()
	{
		return this.model.getTaggedValuesWithValue(this.wrappedAttribute.AttributeGUID);
	}
	
	#region Equals and GetHashCode implementation
	public override bool Equals(object obj)
	{
		Attribute other = obj as Attribute;
		if (other != null)
		{
			if (other.wrappedAttribute.AttributeGUID == this.wrappedAttribute.AttributeGUID)
			{
				return true;	
			}
		}
		return false;
	}
	
	public override int GetHashCode()
	{
		return new Guid(this.wrappedAttribute.AttributeGUID).GetHashCode();
	}
	#endregion

  }
}
