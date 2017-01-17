using System;
using System.Collections.Generic;
using System.Linq;

using EAAddinFramework.Utilities;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Attribute : AttributeWrapper, UML.Classes.Kernel.Property {


    public Attribute(Model model, global::EA.Attribute wrappedAttribute) 
      : base(model, wrappedAttribute)
    {}
    public bool allowDuplicates
    {
    	get {return this.wrappedAttribute.AllowDuplicates;}
    	set {this.wrappedAttribute.AllowDuplicates = value;}
    }
	public override List<UML.Classes.Kernel.Relationship> relationships {
		get 
		{
			string selectRelationsSQL = @"select c.Connector_ID from t_connector c
,t_attribute a where a.ea_guid = '"+this.wrappedAttribute.AttributeGUID +@"' 
and c.StyleEx like '%LF_P="+this.wrappedAttribute.AttributeGUID+"%'"
+@" and ((c.Start_Object_ID = a.Object_ID and c.End_Object_ID <> a.Object_ID)
    or (c.Start_Object_ID <> a.Object_ID and c.End_Object_ID = a.Object_ID))";
			return this.model.getRelationsByQuery(selectRelationsSQL).Cast<UML.Classes.Kernel.Relationship>().ToList();
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
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public string _default {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
	public void setStereotype(string stereotype)
    {
    	this.wrappedAttribute.StereotypeEx = stereotype;
    }
	public bool isID 
	{
		get 
		{
			return this.wrappedAttribute.IsID;
		}
		set 
		{
			this.wrappedAttribute.IsID = value;
		}
	}
    public UML.Classes.Kernel.AggregationKind aggregation {
      get { return UML.Classes.Kernel.AggregationKind.none; }
      set { /* do nothing */ }
    }

    public UML.Classes.Kernel.ValueSpecification defaultValue 
    {
      get 
      {
      	return this.model.factory.createValueSpecificationFromString(this.wrappedAttribute.Default);
      }
      set 
      {
      	this.wrappedAttribute.Default = value.ToString();
      }
    }
	public void setDefaultValue(string defaultStringValue)
	{
		this.defaultValue = this.model.factory.createValueSpecificationFromString(defaultStringValue);
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
    
    public bool isUMLReadOnly {
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
    
    
    
    public bool isOrdered {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public bool isUnique {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
	public UML.Classes.Kernel.UnlimitedNatural upper 
    {
      get {return this.EAMultiplicity.upper;}
      set 
      { 
      	this.WrappedAttribute.UpperBound = value.ToString();
      }
    }

    public uint lower 
    {
      get { return this.EAMultiplicity.lower ;}
      set 
      { 
      	this.WrappedAttribute.LowerBound = value.ToString();
      }
    }
    public UML.Classes.Kernel.Multiplicity multiplicity 
	{
		get 
		{
			return this.EAMultiplicity;
		}
		set 
		{
			this.EAMultiplicity = (Multiplicity)value;
		}
	}
    public Multiplicity EAMultiplicity 
    {
    	get
    	{
    		//default for attributes is 1..1
    		string lowerString = "1";
    		string upperString = "1";
    		//debug
    		if (this.WrappedAttribute.LowerBound.Length > 0)
    		{
    			lowerString = this.WrappedAttribute.LowerBound;
    		}
    		if (this.WrappedAttribute.UpperBound.Length > 0)
    		{
    			upperString = this.WrappedAttribute.UpperBound;
    		}

    		return new Multiplicity(lowerString, upperString);
    	}
    	set
    	{
    		this.WrappedAttribute.LowerBound = value.lower.ToString();
    		this.WrappedAttribute.UpperBound = value.upper.ToString();
    	}
    }
    
    public UML.Classes.Kernel.ValueSpecification upperValue {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.ValueSpecification lowerValue {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
   
    
    public bool getIsNavigable(){ throw new NotImplementedException(); }
    
    public List<UML.Classes.Dependencies.Dependency> clientDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Dependencies.Dependency> supplierDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
	public int length
	{
		get
		{
			int returnedLenght  = 0;
			int.TryParse(this.wrappedAttribute.Length,out returnedLenght);
			return returnedLenght;
		}
		set
		{
			this.wrappedAttribute.Length = value.ToString();
		}
	}
	public int precision
	{
		get
		{
			int returnedPrecision  = 0;
			int.TryParse(this.wrappedAttribute.Precision,out returnedPrecision);
			return returnedPrecision;
		}
		set
		{
			this.wrappedAttribute.Precision = value.ToString();
		}
	}
	public int scale
	{
		get
		{
			int returnedScale  = 0;
			int.TryParse(this.wrappedAttribute.Scale,out returnedScale);
			return returnedScale;
		}
		set
		{
			this.wrappedAttribute.Scale = value.ToString();
		}
	}
   
    
    public bool isNavigable {
      get { return true; }
      set { /* do nothing */ }
    }

  	

  }
}
