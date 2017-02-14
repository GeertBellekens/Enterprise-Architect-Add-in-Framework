using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Xml;
using TSF.UmlToolingFramework.UML.CommonBehaviors.Communications;
using UML = TSF.UmlToolingFramework.UML;
using UML_SM = TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Factory : UML.Extended.UMLFactory {

	private Dictionary<UML_SM.StateMachine, HashSet<Trigger>> stateMachineTriggersMap;
	
	protected Factory(Model model) 
		: base(model) 
	{
		stateMachineTriggersMap = new Dictionary<UML_SM.StateMachine, HashSet<Trigger>>();
	}

    /// returns the singleton instance for the given model.
    public static Factory getInstance(Model model){
      Factory factory = UML.Extended.UMLFactory.getFactory(model) as Factory;
      if( factory == null ) {
        factory = new Factory(model);
      }
      return factory;
    }


	/// <summary>
	/// creates a diagram based on the given EA.Diagram object
	/// </summary>
	/// <param name="diagramToWrap">the EA.Diagram object to wrap</param>
	/// <returns>a diagram wrapping the given EA.Diagram object</returns>
    public override UML.Diagrams.Diagram createDiagram(object diagramToWrap){
        Diagram newDiagram = null;
        global::EA.Diagram eaDiagramToWrap = diagramToWrap as global::EA.Diagram;
        if (eaDiagramToWrap != null)
        {
	        switch (eaDiagramToWrap.Type)
	        {
	            case "Sequence":
	                newDiagram = new SequenceDiagram(this.model as Model, eaDiagramToWrap);
	                break;
	            case "Collaboration":
	            	newDiagram = new CommunicationDiagram(this.model as Model, eaDiagramToWrap);
	            	break;
	            case "Activity":
	            	newDiagram = new ActivityDiagram(this.model as Model, eaDiagramToWrap);
					break;
				case "Logical":
	            	newDiagram = new ClassDiagram(this.model as Model, eaDiagramToWrap);
					break;
				case "Component":
	            	newDiagram = new ComponentDiagram(this.model as Model, eaDiagramToWrap);
					break;	
				case "CompositeStructure":
	            	newDiagram = new CompositeStructureDiagram(this.model as Model, eaDiagramToWrap);
					break;	
				case "Deployment":
	            	newDiagram = new DeploymentDiagram(this.model as Model, eaDiagramToWrap);
					break;	
				case "InteractionOverview":
	            	newDiagram = new InteractionOverviewDiagram(this.model as Model, eaDiagramToWrap);
					break;	
				case "Object":
	            	newDiagram = new ObjectDiagram(this.model as Model, eaDiagramToWrap);
					break;	
				case "Package":
	            	newDiagram = new PackageDiagram(this.model as Model, eaDiagramToWrap);
					break;	
				case "Statechart":
	            	newDiagram = new StateMachineDiagram(this.model as Model, eaDiagramToWrap);
					break;	
				case "Timing":
	            	newDiagram = new TimingDiagram(this.model as Model, eaDiagramToWrap);
					break;	
				case "Use Case":
	            	newDiagram = new UseCaseDiagram(this.model as Model, eaDiagramToWrap);
					break;	
				// TODO add creation of profile diagram					
	            default:
	                newDiagram = new Diagram(this.model as Model, eaDiagramToWrap);
	            	break;
	        }
        }
        return newDiagram;
    }
    
    
    public UML.Diagrams.DiagramElement createDiagramElement
      (global::EA.DiagramObject objectToWrap)
    {
      return new DiagramObjectWrapper(this.model as Model, objectToWrap);
    }
    public UML.Diagrams.DiagramElement createDiagramElement(global::EA.DiagramLink objectToWrap)
    {
    	return new DiagramLinkWrapper(this.model as Model,objectToWrap);
    }
    
    public HashSet<UML.Diagrams.DiagramElement> createDiagramElements
      (global::EA.Collection objectsToWrap)
    {
      HashSet<UML.Diagrams.DiagramElement> returnedDiagramElements =
        new HashSet<UML.Diagrams.DiagramElement>();
      foreach(global::EA.DiagramObject diagramObject in objectsToWrap) {
        returnedDiagramElements.Add(this.createDiagramElement(diagramObject));
      }
      return returnedDiagramElements;
    }
    
    public HashSet<UML.Diagrams.DiagramElement> createDiagramElements
      (List<UML.Classes.Kernel.Element> elements, Diagram diagram) 
    {
      HashSet<UML.Diagrams.DiagramElement> returnedDiagramElements =
        new HashSet<UML.Diagrams.DiagramElement>();
      foreach(UML.Classes.Kernel.Element element in elements) {
        UML.Diagrams.DiagramElement diagramElement = null;
        if( element is ConnectorWrapper ) {
          diagramElement = new DiagramLinkWrapper
            ( this.model as Model, element as ConnectorWrapper, diagram);
          // don't return isHidden relations
          if(((DiagramLinkWrapper)diagramElement).isHidden) {
            diagramElement = null;
          }
        } else if( element is ElementWrapper )  {
          diagramElement = new DiagramObjectWrapper
            ( this.model as Model, element as ElementWrapper, diagram );
        }
        if( diagramElement != null ) {
          returnedDiagramElements.Add(diagramElement);
        }
      }
      return returnedDiagramElements;
    }
    /// creates a new UML element based on the given object to wrap
    public override UML.Classes.Kernel.Element createElement
      (Object objectToWrap)
    {
      if( objectToWrap is global::EA.Element ) 
      {
        return this.createEAElementWrapper (objectToWrap as global::EA.Element);
      } 
      else if( objectToWrap is global::EA.Attribute )  
      {
        return this.createEAAttributeWrapper(objectToWrap as global::EA.Attribute);
      }
      else if( objectToWrap is global::EA.Connector )  
      {
        return this.createEAConnectorWrapper (objectToWrap as global::EA.Connector);
      }
      else if( objectToWrap is global::EA.Method )
      {
        return this.createOperation(objectToWrap as global::EA.Method);
      } 
      else if( objectToWrap is global::EA.Parameter )
      {
        return this.createParameter(objectToWrap as global::EA.Parameter);
      }
      else if (objectToWrap is global::EA.Package)
      {
        return this.createPackage(objectToWrap as global::EA.Package);	
      }
      
      return null;
    }
    
    private Package createPackage(global::EA.Package package)
	{
    	if (package.Element == null)
    	{
    		return new RootPackage(this.model as Model,package);
    	}
    	else
    	{
    		return new Package(this.model as Model,package);
    	}
	}

    /// returns a new EAParameter based on the given EA.Parameter
    private ParameterWrapper createParameter(global::EA.Parameter parameter){
      return new ParameterWrapper(this.model as Model, parameter);
    }

    /// returns a new EAOperatation wrapping the given EA.Method
    private Operation createOperation(global::EA.Method operation){
      return new Operation(this.model as Model, operation);
    }

    /// creates a new EAConnectorWrapper wrapping the given EA.Connector
    private ConnectorWrapper createEAConnectorWrapper
      (global::EA.Connector connector)
    {
		switch (connector.Type) 
		{
			case "Abstraction":
				return new Abstraction(this.model as Model, connector);
			case "Generalization":
				return new Generalization(this.model as Model, connector);
			case "Association":
				return new Association(this.model as Model, connector);
			case "Dependency":
				return new Dependency(this.model as Model, connector);
			case "Sequence":
			case "Collaboration":
				return new Message(this.model as Model, connector);
			case "Realization":
			case "Realisation":
				return createEARealization(connector);
			case "StateFlow":
				return new BehaviorStateMachines.Transition(this.model as Model,connector);
			case "InformationFlow":
				return new InformationFlow(this.model as Model, connector);
			default:
				return new ConnectorWrapper(this.model as Model, connector);
		}
    }
    
    private Realization createEARealization(global::EA.Connector connector) {
      // first create an EARealization, then check if this realization is 
      // between an interface and a behaviored classifier.
      // in that case create a new EAInterfaceRealization
      Realization realization = new Realization( this.model as Model,
                                                 connector );
      if( realization.supplier is UML.Classes.Interfaces.Interface &&     
          realization.client is UML.Classes.Interfaces.BehavioredClassifier) 
      {
        realization = new InterfaceRealization(this.model as Model,connector);
      }
      return realization;
    }

    /// creates a new EAAttribute based on the given EA.Attribute
    internal AttributeWrapper createEAAttributeWrapper(global::EA.Attribute attributeToWrap) 
    {
    	if (EnumerationLiteral.isLiteralValue(this.model as Model, attributeToWrap))
    	{
    		return new EnumerationLiteral(this.model as Model, attributeToWrap);
    	}
    	else
    	{
      		return new Attribute(this.model as Model, attributeToWrap);
    	}
    }
    /// <summary>
    /// checks if this element is an associationclass.
    /// Associationclasses have the connectorID in the Miscdata[3] (PDATA4)
    /// We have to use this because elementToWrap.IsAssocationClass() throws an exception when used in a background trhead
    /// </summary>
    /// <param name="elementToWrap">the element to check</param>
    /// <returns>true if it is an associationclass</returns>
    private bool isAssociationClass (global::EA.Element elementToWrap)
    {
    	int connectorID;
    	if (int.TryParse(elementToWrap.MiscData[3].ToString(),out connectorID))
    	{
    		return (elementToWrap.Type == "Class" && connectorID > 0 );
    	}
    	return false;
    }
    /// creates a new EAElementWrapper based on the given EA.Element
    internal ElementWrapper createEAElementWrapper
      (global::EA.Element elementToWrap) 
    {
      switch (elementToWrap.Type) {
        case "Class":
    			//first check if this isn't an enumeration.
    			// Enumerations are stored as type Class but with the stereotype enumeration
    			if (elementToWrap.StereotypeEx.Contains("enumeration"))
			    {
			    	return new Enumeration(this.model as Model, elementToWrap);
			    }
    			else
    			{
    				//check if associationclass
    				//elementToWrap.IsAssocationClass() returns an exception when used in a background thread so we use our own method to figure out if its an associationClass.
    				if (isAssociationClass(elementToWrap))
    				{
    					return new AssociationClass(this.model as Model, elementToWrap);
    				}
    				else
    				{
    					//just a regular class
    					return new Class(this.model as Model, elementToWrap);
    				}
    			}
    	case "Enumeration":
			// since version 10 there are also "real" enumerations Both are still supported
			return new Enumeration(this.model as Model, elementToWrap);
        case "Interface":
          return new Interface(this.model as Model,elementToWrap);
        case "Note":
          return new NoteComment(this.model as Model, elementToWrap);
        case "Action":
        	// figure out wether this Action is a standard action or a
			// specialized action
			//elementToWrap.Properties;			
			XmlDocument descriptionXml = ((Model)this.model).SQLQuery(@"SELECT x.Description FROM t_object o
										inner join t_xref x on x.Client = o.ea_guid
										where o.Object_ID = " + elementToWrap.ElementID.ToString());
			XmlNodeList descriptionNodes = descriptionXml.SelectNodes(((EA.Model)this.model).formatXPath("//Description"));
			foreach (XmlNode descriptionNode in descriptionNodes) 
			{
				if (descriptionNode.InnerText.Contains("CallOperation"))
			    {
					return new CallOperationAction(this.model as Model, elementToWrap);
			    }
			} 
			
			// simple Action
			return new Action (this.model as Model, elementToWrap);
		case "Interaction":
			return new Interaction(this.model as Model, elementToWrap);
		case "Activity":
			return new Activity(this.model as Model, elementToWrap);
		case "StateMachine":
			return new BehaviorStateMachines.StateMachine(this.model as Model, elementToWrap);
		case "State":
			return new BehaviorStateMachines.State(this.model as Model, elementToWrap,null);
		case "StateNode":
			string metaType = elementToWrap.MetaType;
			if(metaType == "Pseudostate" ||
			   metaType == "Synchronisation") {
				return new BehaviorStateMachines.PseudoState(this.model as Model, elementToWrap,null);
			}
			else if(metaType == "FinalState") {
				return new BehaviorStateMachines.FinalState(this.model as Model, elementToWrap,null);
			}
	        return new ElementWrapper(this.model as Model,elementToWrap);
		case "Package":
			int packageID;
			if (int.TryParse(elementToWrap.MiscData[0],out packageID))
		    {
				return ((Model)this.model).getElementWrapperByPackageID(packageID);
		    }
			else 
			{
				throw new Exception("WrappedElement "+ elementToWrap.Name +" is not a package");
			}
		case "DataType":
		case "PrimitiveType": //TODO: fix primitive type so it can handle this
			return new DataType(this.model as Model, elementToWrap);
		case "InformationItem":
			return new InformationItem(this.model as Model, elementToWrap);
		case "ProxyConnector":
			return new ProxyConnector(this.model as Model, elementToWrap);			
        default:
          return new ElementWrapper(this.model as Model,elementToWrap);
      }
    }

    /// creates a new primitive type based on the given typename
    public override UML.Classes.Kernel.PrimitiveType createPrimitiveType
      (Object typeName)
    {
      return new PrimitiveType(this.model as Model,typeName as string);
    }

    /// creates a new EAParameterReturnType based on the given operation
    internal ParameterReturnType createEAParameterReturnType
      ( Operation operation ) 
    {
      ParameterReturnType returntype = new ParameterReturnType
        ( this.model as Model, operation );
      // if the name of the returntype is empty that means that there is no 
      // returntype defined in EA.
      return returntype.type.name == string.Empty ? null : returntype;
    }

    /// returns a new stereotype based on the given name and attached to the 
    /// given element
    public override UML.Profiles.Stereotype createStereotype
      (UML.Classes.Kernel.Element owner, String name)
    {
      return new Stereotype(this.model as Model, owner as Element, name);
    }
    /// returns a new stereotype based on the given name and attached to the 
    /// given diagram
    public UML.Profiles.Stereotype createStereotype
      (UML.Diagrams.Diagram owner, String name)
    {
      return new Stereotype(this.model as Model, owner as Element, name);
    }
    /// creates a set of stereotypes based on the comma seperated names string
    /// and attaches it to the given element
    public HashSet<UML.Profiles.Stereotype> createStereotypes
      (UML.Classes.Kernel.Element owner, String names)
    {
      HashSet<UML.Profiles.Stereotype> newStereotypes = 
        new HashSet<UML.Profiles.Stereotype>();
      String[] stereotypeNames = names.Split(',');
      foreach( String name in stereotypeNames ) {
        if( name != String.Empty ) {
          UML.Profiles.Stereotype stereotype = 
            this.createStereotype(owner, name);
          if( stereotype != null ) {
            newStereotypes.Add(stereotype);
          }
        }
      }
      return newStereotypes;
    }
    
    /// creates a set of stereotypes based on the comma separated names string
    /// and attaches it to the given Diagram
    public HashSet<UML.Profiles.Stereotype> createStereotypes
      (UML.Diagrams.Diagram owner, String names)
    {
      HashSet<UML.Profiles.Stereotype> newStereotypes = 
        new HashSet<UML.Profiles.Stereotype>();
      String[] stereotypeNames = names.Split(',');
      foreach( String name in stereotypeNames ) {
        if( name != String.Empty ) {
          UML.Profiles.Stereotype stereotype = 
            this.createStereotype(owner, name);
          if( stereotype != null ) {
            newStereotypes.Add(stereotype);
          }
        }
      }
      return newStereotypes;
    }
    
    
    internal HashSet<UML_SM.Region> 
    	createBehaviourStateMachineRegions(ElementWrapper elementWrapper)
    {
		HashSet<UML.StateMachines.BehaviorStateMachines.Region> newRegions = 
			new HashSet<UML.StateMachines.BehaviorStateMachines.Region>();
    	// Get the owning element
    	ElementWrapper owningElement = getOwningElement(elementWrapper);
    	global::EA.Diagram masterDiagram = null;
    	if(owningElement != null) {
    		// Get the master state diagram from the owning element if available
    		masterDiagram = getMasterStateDiagram(owningElement,elementWrapper.wrappedElement);
    	}
    	if(masterDiagram == null) {
    		// Get the master state diagram from the current element if available
    		masterDiagram = getMasterStateDiagram(elementWrapper,elementWrapper.wrappedElement);
    	}
    	
		if(elementWrapper.wrappedElement.Partitions.Count == 0) {
    		// Check if the wrapped element contains any sub states
    		if(elementWrapper.wrappedElement.IsComposite || elementWrapper.wrappedElement.Type == "StateMachine") {
				// Create an implicit default region
				UML.StateMachines.BehaviorStateMachines.Region defaultRegion = 
					new BehaviorStateMachines.Region(this.model as Model,elementWrapper,masterDiagram,null);
				newRegions.Add(defaultRegion);
    		}
		} else {
			// Create a region for all partitions of the wrapped element
			short regionPos = 0;
			foreach(global::EA.Partition partition in elementWrapper.wrappedElement.Partitions)
			{
				UML.StateMachines.BehaviorStateMachines.Region newRegion = 
					new BehaviorStateMachines.Region(this.model as Model,elementWrapper,masterDiagram,partition,regionPos);
				newRegions.Add(newRegion);
				++regionPos;
			}
		}
		return newRegions;
    }
    
    internal UML_SM.Region getContainingRegion(ElementWrapper elementWrapper)
    {
    	ElementWrapper parentElement = getOwningElement(elementWrapper);
    	if(parentElement is BehaviorStateMachines.StateMachine) {
    		BehaviorStateMachines.StateMachine owningStateMachine = 
    			parentElement as BehaviorStateMachines.StateMachine;
    		foreach(BehaviorStateMachines.Region region in owningStateMachine.regions) {
    			if(region.isContainedElement(elementWrapper)) {
    				return region;
    			}
    		}
    	}
    	else if(parentElement is BehaviorStateMachines.State) {
    		BehaviorStateMachines.State owningState = 
    			parentElement as BehaviorStateMachines.State;
    		foreach(BehaviorStateMachines.Region region in owningState.regions) {
    			if(region.isContainedElement(elementWrapper)) {
    				return region;   	
			   	}
			}
		}
    	
    	return null;
    }
    
    internal HashSet<UML_SM.Vertex> createVertices
    	( BehaviorStateMachines.Region region
    	)
    {
    	HashSet<UML_SM.Vertex> newVertices = 
    		new HashSet<UML_SM.Vertex>();
    	global::EA.Element parentElement = region.wrappedElement;
    	foreach(global::EA.Element childElement in parentElement.Elements) {
    		UML_SM.Vertex newVertex = null;
    		switch(childElement.Type) {
    			case "State":
    			case "StateMachine":
    			case "StateNode":
    				newVertex = createEAElementWrapper(childElement) as UML_SM.Vertex;
    				break;
    		}
    		if(newVertex != null) {
    			if(region.isContainedElement(newVertex as ElementWrapper)) {
    				newVertex.container = region;
    				newVertices.Add(newVertex);
    			}
    		}
    	}
    	return newVertices;
    }
    
    internal HashSet<UML_SM.Transition> createOutgoingTransitions(BehaviorStateMachines.Vertex vertex)
    {
    	HashSet<UML_SM.Transition> outgoingTransitions = new HashSet<UML_SM.Transition>();
    	foreach(global::EA.Connector connector in vertex.wrappedElement.Connectors) {
    		if(connector.Type == "StateFlow" && 
    		   connector.ClientID == vertex.wrappedElement.ElementID) {
    			UML_SM.Transition transition = createEAConnectorWrapper(connector) as UML_SM.Transition;
    			if(transition != null) {
    				outgoingTransitions.Add(transition);
    			}
    		}
    	}
    	return outgoingTransitions;
    }
    
    internal HashSet<UML_SM.Transition> createIncomingTransitions(BehaviorStateMachines.Vertex vertex)
    {
    	HashSet<UML_SM.Transition> incomingTransitions = new HashSet<UML_SM.Transition>();
    	foreach(global::EA.Connector connector in vertex.wrappedElement.Connectors) {
    		if(connector.Type == "StateFlow" && 
    		   connector.SupplierID == vertex.wrappedElement.ElementID) {
    			UML_SM.Transition transition = createEAConnectorWrapper(connector) as UML_SM.Transition;
    			if(transition != null) {
    				incomingTransitions.Add(transition);
    			}
    		}
    	}
    	return incomingTransitions;
    }
    
    public HashSet<UML.CommonBehaviors.Communications.Trigger> createTransitionTriggers(UTF_EA.BehaviorStateMachines.Transition transition)
    {
    	HashSet<UML.CommonBehaviors.Communications.Trigger> triggers = 
    		new HashSet<TSF.UmlToolingFramework.UML.CommonBehaviors.Communications.Trigger>();
    
    	UML_SM.StateMachine stateMachine = getContainingStateMachine(transition.owner);
    	if(stateMachine != null)
    	{
			string[] events = transition.wrappedConnector.TransitionEvent.Split( ", ".ToCharArray());
			if(events.Length > 0) {
				if(!stateMachineTriggersMap.ContainsKey(stateMachine)) {
					stateMachineTriggersMap[stateMachine] = createContainedTriggers(stateMachine as UTF_EA.BehaviorStateMachines.StateMachine);
				}
				foreach(string eventName in events) {
					foreach(UML.CommonBehaviors.Communications.Trigger trigger in stateMachineTriggersMap[stateMachine]) {
						if(trigger.event_.name == eventName) {
							triggers.Add(trigger);
							break;
						}
					}
				}
			}
    	}
				
    	return triggers;
    }
    
    public HashSet<UML.CommonBehaviors.Communications.Trigger> getContainedTriggers(UML_SM.StateMachine stateMachine)
    {
    	if(stateMachine != null) {
	    	if(!stateMachineTriggersMap.ContainsKey(stateMachine)) {
	    		stateMachineTriggersMap[stateMachine] = createContainedTriggers(stateMachine as UTF_EA.BehaviorStateMachines.StateMachine);
	    	}
	    	return stateMachineTriggersMap[stateMachine];
    	}
    	
    	return null;
    }
    
    internal HashSet<UML.CommonBehaviors.Communications.Trigger> createContainedTriggers(UTF_EA.BehaviorStateMachines.StateMachine stateMachine)
    {
    	HashSet<UML.CommonBehaviors.Communications.Trigger> triggers = 
    		new HashSet<UML.CommonBehaviors.Communications.Trigger>();
    	if(stateMachine != null) {
    		foreach(global::EA.Element element in stateMachine.wrappedElement.Elements) {
    			if(element.MetaType == "Trigger") {
    				triggers.Add(new UTF_EA.BehaviorStateMachines.Trigger(model as UTF_EA.Model,element));
    			}
    		}
    	}
    	return triggers;
    }
    
    public UML_SM.StateMachine getContainingStateMachine(UML.Extended.UMLItem umlItem)
    {
    	if(umlItem != null) {
	    	if(umlItem.owner is UML_SM.StateMachine) {
	    		return umlItem.owner as UML_SM.StateMachine;
	    	}
    		else {
	    		return getContainingStateMachine(umlItem.owner);
	    	}
    	}
    	
    	return null;
    }
    
    internal ElementWrapper getOwningElement(ElementWrapper elementWrapper)
    {
    	if(elementWrapper.wrappedElement.ParentID != 0)
    	{
    		return elementWrapper.model.getElementWrapperByID(elementWrapper.wrappedElement.ParentID);
    	}
    	
    	return null;
    }
    
    internal global::EA.Diagram getMasterStateDiagram(ElementWrapper elementWrapper,global::EA.Element stateChartElement)
    {
    	foreach(global::EA.Diagram diagram in elementWrapper.wrappedElement.Diagrams)
    	{
    		// Just return the first state chart diagram found that contains the stateChartElement
    		if(diagram.Type == "Statechart")
    		{
    			foreach(global::EA.DiagramObject diagramObject in diagram.DiagramObjects)
    			{
    				if(stateChartElement.ElementID == diagramObject.ElementID)
    				{
    					return diagram;
    				}
    			}
    		}
    	}
    	
    	return null;
    }
    
    internal AssociationEnd createAssociationEnd(ConnectorWrapper connector, global::EA.ConnectorEnd associationEnd, bool isTarget )
    {
      return new AssociationEnd( this.model as Model, connector,associationEnd,isTarget );
    }
	public override UML.Profiles.TaggedValue createNewTaggedValue(UML.Classes.Kernel.Element owner, string name)
	{
		if (owner is Element)
		{
			global::EA.Collection eaTaggedValues = ((Element)owner).eaTaggedValuesCollection;
			return this.createTaggedValue( eaTaggedValues.AddNew(name,""));
		}
		else
		{
			throw new NotImplementedException();
		}
	}

    /// create a new element as owned element of the given owner
    public override T createNewElement<T>
      ( UML.Classes.Kernel.Element owner, String name )
    {
      if( owner is ElementWrapper ) 
      {
        return ((ElementWrapper)owner).addOwnedElement<T>(name);
      }else if (owner is Operation)
      {
      	//We can only add Parameters as owned elements to an operation
      	return ((Operation)owner).addOwnedParameter(name) as T;
      }
	  else 
	  {
        return default(T);
      }
    }
	
    internal T addElementToEACollection<T>( global::EA.Collection collection,
                                            string name,string EAType) 
      where T : class, UML.Classes.Kernel.Element
    {
    	//creating an enumeration is a bit special because EA thinks its just an attribute
    	if (typeof(T).Name == "EnumerationLiteral")
    	{
    		return new EnumerationLiteral((Model)this.model, (global::EA.Attribute)collection.AddNew( name, this.translateTypeName(EAType))) as T;
    	}
    	//Creating Associationclasses is a bit special too. It only becomes an associationclass according to EA once it is linked to an association
    	else if (typeof(T).Name == "AssociationClass")
    	{
    		return new AssociationClass((Model)this.model, (global::EA.Element)collection.AddNew( name, this.translateTypeName(EAType))) as T;
    	}
    	else
    	{
      		return this.model.factory.createElement(collection.AddNew( name, this.translateTypeName(EAType))) as T;
    	}
    }
    
    internal T addElementToEACollection<T>( global::EA.Collection collection,
                                            String name) 
      where T : class, UML.Classes.Kernel.Element
    {
      return this.addElementToEACollection<T> (collection, name, this.translateTypeName(typeof(T).Name));
    }

    /// translates the UML type name to the EA equivalent
    internal String translateTypeName(String typeName){
      switch(typeName) {
        case "Property":
    	case "EnumerationLiteral":
          return "Attribute";
        case "AssociationClass":
          return "Class";
        default:
          return typeName;
      }
    }
    
    internal bool isEAAtttribute(System.Type type)
    {
    	return (type.Name == "Property" 
    	        || type.Name == "EnumerationLiteral" 
    	        || type.Name == "Attribute");
    }

    internal bool isEAOperation(System.Type type){
      return type.Name == "Operation";
    }
    
    internal bool isEAParameter(System.Type type){
      return type.Name == "Parameter";
    }
    
    internal bool isEAConnector(System.Type type){
      return type.Name == "Dependency"
	      || type.Name == "Realization"
	      || type.Name == "Generalization"
	      || type.Name == "Association"
	      || type.Name == "InterfaceRealization"
	      || type.Name == "Message"
	      || type.Name == "Substitution"
	      || type.Name == "Usage"
	      || type.Name == "ElementImport"
	      || type.Name == "PackageImport"
	      || type.Name == "PackageMerge"
      	  || type.Name == "Abstraction";
      
    }
    
    public override UML.Diagrams.DiagramElement createNewDiagramElement
      ( UML.Diagrams.Diagram owner, UML.Classes.Kernel.Element element)
    {
    	return owner.addToDiagram(element);
    }

    public override T createNewDiagram<T>(UML.Classes.Kernel.Element owner, string name)
    {
    	if (owner is ElementWrapper)
    	{
    		return ((ElementWrapper)owner).addOwnedDiagram<T>(name);
    	}
    	else
    	{
    		return default(T);
    	}
    }
    internal T addNewDiagramToEACollection<T>(global::EA.Collection collection, string name) where T:class, UML.Diagrams.Diagram
    {
    	string EADiagramType = translateUMLDiagramtypes(typeof(T).Name);
    	object newEADiagram = collection.AddNew(name, EADiagramType);
    	return this.createDiagram(newEADiagram) as T;
    }
    private string translateUMLDiagramtypes(string UMLDiagramtype)
    {
    	string EADiagramtype;
    	switch (UMLDiagramtype) 
    	{
    		case "ActivityDiagram":
    			EADiagramtype = "Activity";		
    			break;
    		case "ClassDiagram":
    			EADiagramtype = "Logical";		
    			break;
    		case "DeploymentDiagram":
    			EADiagramtype = "Deployment";		
    			break;
    		case "ComponentDiagram":
    			EADiagramtype = "Component";		
    			break;
    		case "SequenceDiagram":
    			EADiagramtype = "Sequence";		
    			break;
    		case "StateMachineDiagram":
    			EADiagramtype = "Statechart";		
    			break;	
    		case "UseCaseDiagram":
    			EADiagramtype = "Use Case";		
    			break;	   
    		case "CommunicationDiagram":
    			EADiagramtype = "Collaboration";		
    			break;	       			
    		case "CompositeStructureDiagram":
    			EADiagramtype = "CompositeStructure";		
    			break;	     
			case "InteractionOverviewDiagramram":
    			EADiagramtype = "InteractionOverview";		
    			break;	   			
			case "PackageDiagram":
    			EADiagramtype = "Package";		
    			break;				    			
			case "TimingDiagram":
    			EADiagramtype = "Timing";		
    			break;
			case "ObjectDiagram":
    			EADiagramtype = "Object";		
    			break;	    			
    		default:
    			EADiagramtype = "Custom";
    			break;
    	}
    	return EADiagramtype;
    }
  	
	public override UML.Profiles.TaggedValue createTaggedValue(object objectToWrap)
	{
		UML.Profiles.TaggedValue newTaggedValue = null;
		if (objectToWrap is global::EA.TaggedValue)
		{
			newTaggedValue = this.createElementTag((global::EA.TaggedValue) objectToWrap);
		}
		else if (objectToWrap is global::EA.AttributeTag)
		{
			newTaggedValue = this.createAttributeTag((global::EA.AttributeTag)objectToWrap);
		}
		else if (objectToWrap is global::EA.MethodTag)
		{
			newTaggedValue = this.createOperationTag((global::EA.MethodTag)objectToWrap);
		}
		else if (objectToWrap is global::EA.ConnectorTag)
		{
			newTaggedValue = this.createRelationTag((global::EA.ConnectorTag)objectToWrap);
		}
		else if (objectToWrap is global::EA.ParamTag)
		{
			newTaggedValue = this.createParameterTag((global::EA.ParamTag)objectToWrap);
		}
		return newTaggedValue;
	}
	
	public ElementTag createElementTag(global::EA.TaggedValue objectToWrap)
	{
		return new ElementTag((Model)this.model,objectToWrap);
	}
	public AttributeTag createAttributeTag(global::EA.AttributeTag objectToWrap)
	{
		return new AttributeTag((Model)this.model,objectToWrap);
	}
	public OperationTag createOperationTag(global::EA.MethodTag objectToWrap)
	{
		return new OperationTag((Model)this.model,objectToWrap);
	}
	public RelationTag createRelationTag(global::EA.ConnectorTag objectToWrap)
	{
		return new RelationTag((Model)this.model,objectToWrap);
	}
	public ParameterTag createParameterTag(global::EA.ParamTag objectToWrap)
	{
		return new ParameterTag((Model)this.model,objectToWrap);
	}
	public DescriptionComment createDescriptionComment(Element owner)
	{
		return new DescriptionComment((Model)this.model,owner);
	}
	public EAAddinFramework.EASpecific.User createUser(string login, string firstname, string lastname)
	{
		return new EAAddinFramework.EASpecific.User((Model)this.model,login,firstname,lastname);
	}
	public EAAddinFramework.EASpecific.WorkingSet createWorkingSet(string name,string ID, EAAddinFramework.EASpecific.User user)
	{
		return new EAAddinFramework.EASpecific.WorkingSet((Model)this.model,ID,user,name);
	}
	public override UML.Classes.Kernel.ValueSpecification createValueSpecification(object objectToWrap)
  	{
  		//check the type fo the object
  		if (objectToWrap == null) return new LiteralNull();
  		if (objectToWrap is int) return new LiteralInteger((int)objectToWrap);
  		if (objectToWrap is bool) return new LiteralBoolean((bool)objectToWrap);
  		if (objectToWrap is string) return new LiteralString((string)objectToWrap);
  		if (objectToWrap is UnlimitedNatural) return new LiteralUnlimitedNatural ((UnlimitedNatural) objectToWrap);
  		//if its something else then we don't now
  		return null;
  	}
	public override UML.Classes.Kernel.ValueSpecification createValueSpecificationFromString(string stringRepresentation)
	{
		if (stringRepresentation != null &&
		    stringRepresentation.Equals( "null",StringComparison.InvariantCultureIgnoreCase))
		    return createValueSpecification(null);
		int integerRepresentation;
		if (int.TryParse(stringRepresentation,out integerRepresentation))
			return createValueSpecification(integerRepresentation);
		bool boolRepresentation;
		if (bool.TryParse(stringRepresentation,out boolRepresentation))
			return createValueSpecification(boolRepresentation);
		UnlimitedNatural unlimitedNaturalRepresentation;
		if (UnlimitedNatural.TryParse(stringRepresentation,out unlimitedNaturalRepresentation))
			return createValueSpecification(unlimitedNaturalRepresentation);
		//default string
		return createValueSpecification(stringRepresentation);
	}
}
}
