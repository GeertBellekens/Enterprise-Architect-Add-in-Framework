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
    public bool isID 
	{
		get 
    	{
    		return (bool)this.getProperty(getPropertyNameName(),this.wrappedAttribute.IsID);
    	}
		set 
		{
			this.setProperty(getPropertyNameName(),value,this.wrappedAttribute.IsID);
		}
	}
    public bool allowDuplicates
    {
    	get 
    	{
    		return (bool)this.getProperty(getPropertyNameName(),this.wrappedAttribute.AllowDuplicates);
    	}
		set 
		{
			this.setProperty(getPropertyNameName(),value,this.wrappedAttribute.AllowDuplicates);
		}
    }
    public UML.Classes.Kernel.ValueSpecification defaultValue 
    {
		get 
		{
			return this.model.factory.createValueSpecificationFromString((string)this.getProperty(getPropertyNameName(),this.wrappedAttribute.Default));
		}
		set 
		{
			this.setProperty(getPropertyNameName(),value.ToString(),this.wrappedAttribute.Default);
		}
    }
    /// the isStatic property defines context of the attribute.
    /// If true then the context is the class
    /// If false then the context is the instance.
    public bool isStatic 
    {
    	get 
    	{
    		return (bool)this.getProperty(getPropertyNameName(),this.wrappedAttribute.IsStatic);
    	}
		set 
		{
			this.setProperty(getPropertyNameName(),value,this.wrappedAttribute.IsStatic);
		}
    }
    public UML.Classes.Kernel.VisibilityKind visibility 
    {
      get 
      {
        return VisibilityKind.getUMLVisibilityKind
        	( (string)this.getProperty(getPropertyNameName(),this.wrappedAttribute.Visibility),
            UML.Classes.Kernel.VisibilityKind._private );
      }
      set 
      {
      	this.setProperty(getPropertyNameName(),VisibilityKind.getEAVisibility(value),this.wrappedAttribute.Visibility);
      }
    }

    public Multiplicity EAMultiplicity 
    {
    	get
    	{

    		return (Multiplicity)this.getProperty(getPropertyNameName(),getInitialMultiplicity());
    	}
    	set
    	{
    		this.setProperty(getPropertyNameName(),value,getInitialMultiplicity());
    	}
    }

	internal override void saveElement()
	{
		if (this.getProperty("isID") != null) this.wrappedAttribute.IsID = (bool)this.getProperty("isID");
		if (this.getProperty("allowDuplicates") != null) this.wrappedAttribute.AllowDuplicates = (bool)this.getProperty("allowDuplicates");
		if (this.getProperty("defaultValue") != null) this.wrappedAttribute.Default = (string)this.getProperty("defaultValue");
		if (this.getProperty("isStatic") != null) this.wrappedAttribute.IsStatic = (bool)this.getProperty("isStatic");
		if (this.getProperty("visibility") != null) this.wrappedAttribute.Visibility = (string)this.getProperty("visibility");
		//multiplicity is a bit of an exception. We only save the info if it actually has changed
		var multiplicityProperty = this.getPropertyInfo("EAMultiplicity");
		if (multiplicityProperty != null && multiplicityProperty.isDirty)
		{
			this.WrappedAttribute.LowerBound = ((Multiplicity)multiplicityProperty.propertyValue).lower.ToString();
    		this.WrappedAttribute.UpperBound = ((Multiplicity)multiplicityProperty.propertyValue).upper.ToString();
		}
		base.saveElement();
	}
	 public UML.Classes.Kernel.UnlimitedNatural upper 
    {
      get {return this.EAMultiplicity.upper;}
      set {this.EAMultiplicity.upper = value;}
    }

    public uint lower 
    {
      get { return this.EAMultiplicity.lower ;}
      set {this.EAMultiplicity.lower = value;}
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
	private Multiplicity getInitialMultiplicity()
    {
	   	//default for attributes is 1..1
		string lowerString = "1";
		string upperString = "1";
		//debug
		if (this.WrappedAttribute.LowerBound.Length > 0)
		{
			lowerString = this.wrappedAttribute.LowerBound;
		}
		if (this.WrappedAttribute.UpperBound.Length > 0)
		{
			upperString = this.wrappedAttribute.UpperBound;
		}
		return new Multiplicity(lowerString, upperString);
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

    public UML.Classes.Kernel.AggregationKind aggregation {
      get { return UML.Classes.Kernel.AggregationKind.none; }
      set { /* do nothing */ }
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
