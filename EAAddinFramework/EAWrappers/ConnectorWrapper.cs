using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using EAAddinFramework.Utilities;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
  public class ConnectorWrapper : Element, UML.Classes.Kernel.Relationship 
  {
    internal global::EA.Connector wrappedConnector { get; set; }    
    private UML.Classes.Kernel.Element _owner;
    private UML.Classes.Kernel.Element _source;
    private UML.Classes.Kernel.Element _target;
    private AssociationEnd _sourceEnd;
    private AssociationEnd _targetEnd;
    internal Element sourceElement
    {
    	get
    	{
    		//check if elementwrapper
			var elementSource = this.source as ElementWrapper;
			if (elementSource != null) return elementSource;
			//check if attributeWrapper
			var attributeSource = this.source as AttributeWrapper;
			if (attributeSource != null) return attributeSource;
			//check if operationWrapper
			var operationSource = this.source as Operation;
			if (operationSource != null) return operationSource;
			//hceck if connectorWrapper
			var connectorSource = this.source as ConnectorWrapper;
			if (connectorSource != null) return connectorSource;
			//nothing found
			return null;
    	}
    }
    internal Element targetElement
    {
    	get
    	{
    		//check if elementwrapper
			var elementSource = this.target as ElementWrapper;
			if (elementSource != null) return elementSource;
			//check if attributeWrapper
			var attributeSource = this.target as AttributeWrapper;
			if (attributeSource != null) return attributeSource;
			//check if operationWrapper
			var operationSource = this.target as Operation;
			if (operationSource != null) return operationSource;
			//hceck if connectorWrapper
			var connectorSource = this.target as ConnectorWrapper;
			if (connectorSource != null) return connectorSource;
			//nothing found
			return null;
    	}
    }
    public int id
    {
    	get{return this.wrappedConnector.ConnectorID;}
    }
	
    public ConnectorWrapper(Model model, global::EA.Connector connector)
      : base(model)
    {
      this.wrappedConnector = connector;
    }
    public global::EA.Connector WrappedConnector {
    	get { return this.wrappedConnector; }
    }
	public override void open()
	{
		var diagrams = this.getDependentDiagrams();
		if (diagrams.Count > 0)
		{
			diagrams[0].open();
			diagrams[0].selectItem(this);
		}
		else
		{
			this.owner.open();
		}
	}
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { return new HashSet<UML.Classes.Kernel.Element>(); }
      set { throw new NotImplementedException(); }
    }
	/// <summary>
	/// 
	/// </summary>
	public override string uniqueID 
	{
		get 
		{
			return this.wrappedConnector.ConnectorGUID;
		}
	}
	public string alias
	{
		get{return this.wrappedConnector.Alias;}
		set{this.wrappedConnector.Alias = value;}
	}
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public override List<UML.Diagrams.Diagram> getDependentDiagrams()
	{
		var dependentDiagrams = new List<UML.Diagrams.Diagram>();
		string sqlQuery = "select dl.DiagramID as Diagram_ID from t_diagramlinks dl where dl.ConnectorID = " + this.id;
		dependentDiagrams.AddRange(this.model.getDiagramsByQuery(sqlQuery));
		return dependentDiagrams;
	}
		
    /// <summary>
    /// not fully correct, but we will return the element at the source of the relation
    /// TODO: fix this so it uses the actual ownership as prescribed by UML
    /// </summary>
    public override UML.Classes.Kernel.Element owner {
      get 
      {
// removed caching to try and solve multithreading issue.      	
//      	if (this._owner == null)
//      	{
      		this._owner = this.model.getElementWrapperByID(this.wrappedConnector.ClientID);
//      	}
      	return this._owner;
      }
      set { throw new NotImplementedException(); }
    }
    /// the stereotypes defined on this Relationship
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
      get {
        return ((Factory)this.model.factory).createStereotypes
          ( this, this.wrappedConnector.StereotypeEx );
      }
      set 
      { 
      	this.WrappedConnector.StereotypeEx = Stereotype.getStereotypeEx(value); 
      }
    }
    /// returns the related elements.
    /// In EA the Connectoris a binary relationship. So only two Elements will 
    /// ever be returned.
    /// If there is a linked feature then the linked feature will be returned instead of the EA.Element.
    public List<UML.Classes.Kernel.Element> relatedElements {
      get 
      {
        var returnedElements =  new List<UML.Classes.Kernel.Element>();
        returnedElements.Add(this.source);
        returnedElements.Add(this.target);
        return returnedElements;
      }
      set { throw new NotImplementedException(); }
    }
    private UML.Extended.UMLItem getLinkedFeature(bool isSource)
    {
    	UML.Extended.UMLItem linkedFeature = null;
    	//determine start or end keyword
		string key = isSource ? "LFSP" : "LFEP";
   		string featureGUID = KeyValuePairsHelper.getValueForKey(key,this.wrappedConnector.StyleEx);
   		if (!string.IsNullOrEmpty(featureGUID))
    	{
	   		if (featureGUID.EndsWith("}R") 
	   		    || featureGUID.EndsWith("}L"))
	   		{
	   			//remove the last "R" or "L"
	   			featureGUID = featureGUID.Substring(0,featureGUID.Length -1);
	   		}
   			//get the linked feature
    		linkedFeature = this.model.getItemFromGUID(featureGUID);
    	}
    	return linkedFeature;
    }
    private void setLinkedFeature(UML.Classes.Kernel.Element linkedFeature, bool isSource)
    {
		//check if attribute
		Element actualEnd = linkedFeature as Attribute;
		//or maybe operation
		if (actualEnd == null) actualEnd = linkedFeature as Operation;
		if (actualEnd != null)
		{
			
			//set the client id to the id of the owner
			if (isSource)
			{
				_source = linkedFeature;
				this.wrappedConnector.ClientID = ((ElementWrapper)actualEnd.owner).id;
			}
			else
			{
				_target = linkedFeature;
				this.wrappedConnector.SupplierID = ((ElementWrapper)actualEnd.owner).id;
			}
			//set the linked feature
			string key = isSource ? "LFSP" : "LFEP";
			string suffix = isSource ? "R":"L";
			string styleEx = KeyValuePairsHelper.setValueForKey(key,linkedFeature.uniqueID + suffix,this.wrappedConnector.StyleEx);
			this.wrappedConnector.StyleEx = styleEx;
		}
    	
    }
    public UML.Classes.Kernel.Element target {
      get 
      {
      	if (this._target == null)
      	{
	     	 UML.Classes.Kernel.Element linkedSupplierFeature = this.getLinkedFeature(false) as UML.Classes.Kernel.Element;
	        if (linkedSupplierFeature != null)
	        {
	        	this._target =linkedSupplierFeature;
	        }
	        else
	        {
	        	var targetElementWrapper = this.model.getElementWrapperByID(this.wrappedConnector.SupplierID); 
	        	this._target = targetElementWrapper;			
	       		//in case the source is linked to a connector there's a dummy element with type ProxyConnector
				if (targetElementWrapper.EAElementType == "ProxyConnector")
				{
					//get the source connector
					this._source = this.model.getRelationByID(targetElementWrapper.wrappedElement.ClassifierID);
				}
	        }
      	}
      	return this._target;
      }
      set 
      {
		if (value is ElementWrapper)
		{
			this._target = value;
			this.WrappedConnector.SupplierID = ((ElementWrapper)value).id;
		}
		else
		{
			this.setLinkedFeature(value,false);
		}      	
      }
    }
	public override List<UML.Classes.Kernel.Relationship> relationships {
		get 
		{
			string sqlGetRelationships = @"select c.Connector_ID
											from (t_connector c
											inner join t_object o on (o.Object_ID in (c.Start_Object_ID, c.End_Object_ID)
											and o.Object_Type = 'ProxyConnector'))
											where o.Classifier_guid = '"+this.uniqueID+"'";
			return this.model.getRelationsByQuery(sqlGetRelationships).Cast<UML.Classes.Kernel.Relationship>().ToList();
		}
		set 
		{
			base.relationships = value;;
		}
	}
  
    /// <summary>
    /// returns a list of elements that are linked to this relationship (but not the related elements)
    /// these are the notes or constrains linked to the relation, or the elments linked to this relation through another relation
    /// </summary>
    /// <returns>the list of elements that are somehow related tot this relation direction (without being the source or target)</returns>
	public List<UML.Classes.Kernel.Element> getLinkedElements()
	{
		List<UML.Classes.Kernel.Element> foundElements = new List<UML.Classes.Kernel.Element>();
		//add the elements linked to this relation via another connector
		foundElements.AddRange(this.relationships.OfType<ConnectorWrapper>().Where(x => x.target != this).Select(y => y.target));	
		foundElements.AddRange(this.relationships.OfType<ConnectorWrapper>().Where(x => x.source != this).Select(y => y.source));
		//then add the notes/constrains that are linked to this relation via a notelink
		string selecNoteLinkElements = "select o.Object_ID from t_object o where o.pdata4 like '%idref_=" + this.id + ";%'";
		foundElements.AddRange(this.model.getElementWrappersByQuery(selecNoteLinkElements));
		//return the found elements
		return foundElements;
	}
	public void addLinkedElement(ElementWrapper element)
	{
		if (element is NoteComment
		    || element is UML.Classes.Kernel.Constraint)
		{
			//set idref to the id of this relation to create the notelink
			string pdata4 = element.wrappedElement.MiscData[3].ToString();
			string idRefString = getNextIdRefString(pdata4);
			pdata4 = KeyValuePairsHelper.setValueForKey(idRefString,this.id.ToString(),pdata4);
			string sqlUpdateNoteLink = "update t_object set PDATA4 = '"+ pdata4 +"' where ea_guid =" + element.uniqueID;
			this.model.executeSQL(sqlUpdateNoteLink);
		}
		else
		{
			//create an element of type ProxyConnector with
			//t_object.ClassifierGUID = sourceConnectorGUID
			//t_object.Classifier = sourcConnector.ConnectorID
			var proxyConnector = this.model.factory.createNewElement<ProxyConnector>(this.owningPackage,string.Empty);
			proxyConnector.connector = this;
			proxyConnector.save();
			//then create a connector between the ProxyConnector Element and the target element
			var elementLink = this.model.factory.createNewElement<Dependency>(proxyConnector,string.Empty);
			elementLink.target = element;
			elementLink.save();
		}
	}
	private string getNextIdRefString(string pdata)
	{
		int i = 0;
		string idRefValue = null;
		do
		{
			i++;
			idRefValue = KeyValuePairsHelper.getValueForKey("idref" + i,pdata);
		}
		while (! string.IsNullOrEmpty(idRefValue));
		return "idref" + i;
	}
    public UML.Classes.Kernel.Element source {
      get 
      {
      	if (this._source == null)
      	{
	        //check first if there is a linked feature
	        UML.Classes.Kernel.Element linkedClientFeature = this.getLinkedFeature(true) as UML.Classes.Kernel.Element;
	        if (linkedClientFeature != null)
	        {
	        	this._source = linkedClientFeature;
	        }
	        else
	        {
				var sourceElementWrapper = this.model.getElementWrapperByID(this.wrappedConnector.ClientID);	        	
				this._source = sourceElementWrapper;
				//in case the source is linked to a connector there's a dummy element with type ProxyConnector
				if (sourceElementWrapper.EAElementType == "ProxyConnector")
				{
					//get the source connector
					this._source = this.model.getRelationByID(sourceElementWrapper.wrappedElement.ClassifierID);
				}
	        }
      	}
      	return this._source;
      }
      set
     {
		if (value is ElementWrapper)
		{
			this._source = value;
			this.WrappedConnector.ClientID = ((ElementWrapper)value).id;
		}
		else
		{
			this.setLinkedFeature(value,true);
		}      	
      }
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
        var returnTypes =new List<UML.Classes.Kernel.Type>();
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
    public List<UML.Classes.Kernel.Property> memberEnds 
    {
      get {
        var returnedMembers = new List<UML.Classes.Kernel.Property>();
        returnedMembers.Add(this.sourceEnd as UML.Classes.Kernel.Property) ;
        returnedMembers.Add(this.targetEnd as UML.Classes.Kernel.Property);
        return returnedMembers;
      }
      set { throw new NotImplementedException(); }
    }
    public AssociationEnd sourceEnd
    {
    	get
    	{
    		if (this._sourceEnd == null)
    		{
    			this._sourceEnd = ((Factory)this.model.factory).createAssociationEnd(this, this.wrappedConnector.ClientEnd,false) ;
    		}
    		return this._sourceEnd;
    	}
    }
    public AssociationEnd targetEnd
    {
    	get
    	{
    		if (this._targetEnd == null)
    		{
    			this._targetEnd = ((Factory)this.model.factory).createAssociationEnd(this, this.wrappedConnector.SupplierEnd,true); 
    		}
    		return this._targetEnd;
    	}
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
    
    public override string name {
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
	/// <summary>
    /// convenience method to return the information flows that realize this Relationship
    /// </summary>
    /// <returns>the information flows that realize this Relationship</returns>
	public virtual HashSet<UML.InfomationFlows.InformationFlow> getInformationFlows()
	{
		HashSet<UML.InfomationFlows.InformationFlow> informationFlows = new HashSet<UML.InfomationFlows.InformationFlow>();
		string sqlGetInformationFlowIDs = @"select x.description
			from (t_connector c
			inner join t_xref x on (x.client = c.ea_guid and x.Name = 'MOFProps'))
			where c.ea_guid = '" + this.guid + "'";
		var queryResult = this.model.SQLQuery(sqlGetInformationFlowIDs);
		var descriptionNode = queryResult.SelectSingleNode(this.model.formatXPath("//description"));
		if (descriptionNode != null)
		{
			foreach (string ifGUID in descriptionNode.InnerText.Split(','))
			{
				var informationFlow = this.model.getRelationByGUID(ifGUID) as UML.InfomationFlows.InformationFlow;
				if (informationFlow != null )
				{
					informationFlows.Add(informationFlow);
				}
			}
		}
		return informationFlows;
	}

    /// <summary>
    /// Due to a bug in EA we first need to save the end with aggregation kind none, and then the other end.
    /// If not then the aggregationkind of the other set is reset to none.
    /// </summary>
	public override void save()
	{
		this.WrappedConnector.Update();
		if (this.sourceEnd.aggregation == UML.Classes.Kernel.AggregationKind.none)
		{
			this.sourceEnd.save();
			this.targetEnd.save();
		}else
		{
			this.targetEnd.save();
			this.sourceEnd.save();
		}
	}
    
    internal override void saveElement()
    {
    	this.wrappedConnector.Update();
    }

    public override string notes {
      get { return this.wrappedConnector.Notes;  }
      set { this.wrappedConnector.Notes = value; }
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
	
	public override HashSet<UML.Profiles.TaggedValue> taggedValues
	{
		get 
		{
			//make sure we have an up-to date collection
			this.wrappedConnector.TaggedValues.Refresh();
			return new HashSet<UML.Profiles.TaggedValue>(this.model.factory.createTaggedValues(this.wrappedConnector.TaggedValues));
		}
		set { throw new NotImplementedException();}
	}

	#region Equals and GetHashCode implementation
	public override bool Equals(object obj)
	{
		ConnectorWrapper other = obj as ConnectorWrapper;
		if (other != null)
		{
			if (other.wrappedConnector.ConnectorGUID == this.wrappedConnector.ConnectorGUID)
			{
				return true;	
			}
		}
		return false;
	}
	
	public override int GetHashCode()
	{
		return new Guid(this.wrappedConnector.ConnectorGUID).GetHashCode();
	}
	#endregion
	/// <summary>
	/// adding both the start and the end element to the diagram will automatically add the connector to the diagram
	/// </summary>
	public override void addToCurrentDiagram()
	{
		foreach (UML.Classes.Kernel.Element element in this.relatedElements) 
		{
			element.addToCurrentDiagram();
		}
	}

  	/// <summary>
  	/// adds a related element to this connector.
  	/// This operation checks if the source or target is empty and then adds the related element to the empty spot.
  	/// If none of the two are empty then the target is being replaced
  	/// </summary>
  	/// <param name="relatedElement"></param>
	public void addRelatedElement(UML.Classes.Kernel.Element relatedElement)
	{
		if (this.WrappedConnector.ClientID <= 0 )
		{
			this.source = relatedElement;
		}
		else
		{
			this.target = relatedElement;
		}

	}
  	
	internal override global::EA.Collection eaTaggedValuesCollection {
		get {
			return this.WrappedConnector.TaggedValues;
		}
	}
  	
	public override string guid {
		get 
		{
			return this.WrappedConnector.ConnectorGUID;
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
		return this.source.makeWritable(overrideLocks);
	}

	public override string getLockedUser()
	{
		if (this.sourceElement != null) return sourceElement.getLockedUser();
		return string.Empty;
	}

	public override string getLockedUserID()
	{
		if (this.sourceElement != null) return sourceElement.getLockedUserID();
		return string.Empty;
	}

	#endregion
  }
}
