using System;
using System.Collections.Generic;

using System.Runtime.CompilerServices;
using System.Xml;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class AssociationEnd : Element , UML.Classes.Kernel.Property 
  {
    	/// <summary>
    	/// the default for multiplicities on associationEnds is 0..1
    	/// </summary>
		public const string defaultMultiplicity = "0..1";
	internal global::EA.ConnectorEnd wrappedAssociationEnd { get; set; }
    private ConnectorWrapper _association { get; set; }
	
    public AssociationEnd(Model model, ConnectorWrapper linkedAssocation,
                          global::EA.ConnectorEnd associationEnd, bool isTarget ):base(model)
    {
      this.wrappedAssociationEnd = associationEnd;
      this._association = linkedAssocation;
      this.isTarget = isTarget;
    }
    public bool isTarget {get;private set;}
    public bool isSource {
    	get
    	{
    		return !this.isTarget;
    	}
    }
    

    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { return new HashSet<UML.Classes.Kernel.Element>(); }
      set { /* do nothing */  }
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

	public bool isID 
	{
		get 
		{
			return (this.wrappedAssociationEnd.Constraint.Equals("id",StringComparison.InvariantCultureIgnoreCase));
		}
		set 
		{
			if (value)
			{
				this.wrappedAssociationEnd.Constraint = "id";
			}
			else
			{
				this.wrappedAssociationEnd.Constraint = string.Empty;
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

    public bool isUMLReadOnly {
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
      get 
      {
        if( this.wrappedAssociationEnd.End == "Supplier" ) 
        {
          return this._association.target as UML.Classes.Kernel.Type;
        } 
        else 
        {
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

    public UML.Classes.Kernel.UnlimitedNatural upper 
    {
      get {return this.EAMultiplicity.upper;}
      set 
      { 
      	Multiplicity newMultiplicity = this.EAMultiplicity;
      	newMultiplicity.upper = value;
      	this.EAMultiplicity = newMultiplicity;
      }
    }

    public uint lower 
    {
      get { return this.EAMultiplicity.lower ;}
      set 
      { 
      	Multiplicity newMultiplicity = this.EAMultiplicity;
      	newMultiplicity.lower = value;
      	this.EAMultiplicity = newMultiplicity;
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

    public bool getIsNavigable(){
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
    
    public bool isNavigable {
      get { 
    		//because of a bug in the API we don't alwas get the correct information. Therefore we need this workaround using a database call
    		string sqlGetNavigability = "select c.DestIsNavigable, c.DestStyle, c.SourceIsNavigable, c.SourceStyle from t_connector c where c.ea_guid = '"
    			+ this._association.uniqueID +"'";
    		var navigabilityInfo = this.model.SQLQuery(sqlGetNavigability);
    		XmlNode navigableNode;
    		XmlNode styleNode;
    		if (this.isTarget)
    		{
    			navigableNode = navigabilityInfo.SelectSingleNode(this.model.formatXPath("//DestIsNavigable"));
    			styleNode = navigabilityInfo.SelectSingleNode(this.model.formatXPath("//DestStyle"));
    		}else
    		{
    			navigableNode = navigabilityInfo.SelectSingleNode(this.model.formatXPath("//SourceIsNavigable"));
    			styleNode = navigabilityInfo.SelectSingleNode(this.model.formatXPath("//SourceStyle"));
    		}
    		if (navigableNode != null && navigableNode.InnerText == "1")
    		{
    			return true;
    		}
    		if (styleNode != null && styleNode.InnerText.Contains("Navigable=Navigable"))
		    {
    			return true;
		    }
    		//end of workaround
    		return this.wrappedAssociationEnd.Navigable == "Navigable"
    			|| this.wrappedAssociationEnd.IsNavigable; }
      set {
        if (value) {
    		this.wrappedAssociationEnd.Navigable = "Navigable";
          	this.wrappedAssociationEnd.IsNavigable = true;
        }else {
          this.wrappedAssociationEnd.Navigable = "Non-Navigable";
          this.wrappedAssociationEnd.IsNavigable = false;
        }
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
    		if (this.wrappedAssociationEnd.Cardinality.Length > 0)
    		{
    			return new Multiplicity(this.wrappedAssociationEnd.Cardinality);
    		}
    		else
    		{
    			return new Multiplicity(defaultMultiplicity);
    		}
    	}
    	set
    	{
    		this.wrappedAssociationEnd.Cardinality = value.EACardinality;
    	}
    }
	public override TSF.UmlToolingFramework.UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
	{
		UML.Extended.UMLItem item = null;
		if (ElementWrapper.filterName(relativePath,this.name))
		{
	    	if (relativePath.Count ==1)
	    	{
	    		item = this;
	    	}
		}
		return this; 
	}
  	
	internal override global::EA.Collection eaTaggedValuesCollection {
		get 
		{
			return this.wrappedAssociationEnd.TaggedValues;
		}
	}
  	
	public override string guid 
	{
		get 
		{	//association ends don't have their own guid, so the closes thing is the guid of the association
			return ((Association)this.owningAssociation).guid;
		}
	}

		#region implemented abstract members of Element

	public override void deleteOwnedElement(Element ownedElement)
	{
		throw new NotImplementedException();
	}

	#endregion

		#region implemented abstract members of Element
	
		public override bool makeWritable(bool overrideLocks)
		{
			return this.owner.makeWritable(overrideLocks);
		}
	
		public override string getLockedUser()
		{
			return this._association.getLockedUser();
		}
	
		public override string getLockedUserID()
		{
			return this._association.getLockedUserID();
		}
	
		#endregion
		public override List<UML.Extended.UMLItem> findOwnedItems(List<string> descriptionParts)
		{
			//return the owned items of the type of the associationEnd
			var ownedItems = new List<UML.Extended.UMLItem>();
			var elementType = this.type as Element;
			if (elementType != null)
			{
				ownedItems.AddRange(((Element)this.type).findOwnedItems(descriptionParts));
			}
			return ownedItems;
		}
  }
}
