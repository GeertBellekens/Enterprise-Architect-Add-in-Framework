﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TSF.UmlToolingFramework.UML.CommonBehaviors.Communications;
using UML_SM = TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class Factory : UML.Extended.UMLFactory
    {

        private Dictionary<UML_SM.StateMachine, HashSet<Trigger>> stateMachineTriggersMap;
        public Model EAModel => (Model)this.model;
        protected Factory(Model model)
            : base(model)
        {
            this.stateMachineTriggersMap = new Dictionary<UML_SM.StateMachine, HashSet<Trigger>>();
        }

        /// returns the singleton instance for the given model.
        public static Factory getInstance(Model model)
        {
            var factory = getFactory(model) as Factory;
            if (factory == null)
            {
                factory = new Factory(model);
            }
            return factory;
        }


        /// <summary>
        /// creates a diagram based on the given EA.Diagram object
        /// </summary>
        /// <param name="diagramToWrap">the EA.Diagram object to wrap</param>
        /// <returns>a diagram wrapping the given EA.Diagram object</returns>
        public override UML.Diagrams.Diagram createDiagram(object diagramToWrap)
        {
            Diagram newDiagram = null;
            global::EA.Diagram eaDiagramToWrap = diagramToWrap as global::EA.Diagram;
            if (eaDiagramToWrap != null)
            {
                switch (eaDiagramToWrap.Type)
                {
                    case "Sequence":
                        newDiagram = new SequenceDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Collaboration":
                        newDiagram = new CommunicationDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Activity":
                        newDiagram = new ActivityDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Logical":
                        newDiagram = new ClassDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Component":
                        newDiagram = new ComponentDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "CompositeStructure":
                        newDiagram = new CompositeStructureDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Deployment":
                        newDiagram = new DeploymentDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "InteractionOverview":
                        newDiagram = new InteractionOverviewDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Object":
                        newDiagram = new ObjectDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Package":
                        newDiagram = new PackageDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Statechart":
                        newDiagram = new StateMachineDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Timing":
                        newDiagram = new TimingDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    case "Use Case":
                        newDiagram = new UseCaseDiagram(this.EAModel, eaDiagramToWrap);
                        break;
                    // TODO add creation of profile diagram					
                    default:
                        newDiagram = new Diagram(this.EAModel, eaDiagramToWrap);
                        break;
                }
            }
            return newDiagram;
        }


        public UML.Diagrams.DiagramElement createDiagramElement
          (global::EA.DiagramObject objectToWrap)
        {
            return new DiagramObjectWrapper(this.EAModel, objectToWrap);
        }
        public UML.Diagrams.DiagramElement createDiagramElement(global::EA.DiagramLink objectToWrap)
        {
            return new DiagramLinkWrapper(this.EAModel, objectToWrap);
        }

        public HashSet<UML.Diagrams.DiagramElement> createDiagramElements
          (global::EA.Collection objectsToWrap)
        {
            HashSet<UML.Diagrams.DiagramElement> returnedDiagramElements =
              new HashSet<UML.Diagrams.DiagramElement>();
            foreach (global::EA.DiagramObject diagramObject in objectsToWrap)
            {
                returnedDiagramElements.Add(this.createDiagramElement(diagramObject));
            }
            return returnedDiagramElements;
        }

        public HashSet<UML.Diagrams.DiagramElement> createDiagramElements
          (List<UML.Classes.Kernel.Element> elements, Diagram diagram)
        {
            HashSet<UML.Diagrams.DiagramElement> returnedDiagramElements =
              new HashSet<UML.Diagrams.DiagramElement>();
            foreach (UML.Classes.Kernel.Element element in elements)
            {
                UML.Diagrams.DiagramElement diagramElement = null;
                if (element is ConnectorWrapper)
                {
                    diagramElement = new DiagramLinkWrapper
                      (this.EAModel, element as ConnectorWrapper, diagram);
                    // don't return isHidden relations
                    if (((DiagramLinkWrapper)diagramElement).isHidden)
                    {
                        diagramElement = null;
                    }
                }
                else if (element is ElementWrapper)
                {
                    diagramElement = new DiagramObjectWrapper
                      (this.EAModel, element as ElementWrapper, diagram);
                }
                if (diagramElement != null)
                {
                    returnedDiagramElements.Add(diagramElement);
                }
            }
            return returnedDiagramElements;
        }

        #region implemented abstract members of UMLFactory


        public override UML.Classes.Kernel.Element createElement(object objectToWrap, UML.Classes.Kernel.Element owner)
        {
            var newElement = this.createElement(objectToWrap);
            newElement.owner = owner;
            return newElement;
        }


        #endregion

        /// creates a new UML element based on the given object to wrap
        public override UML.Classes.Kernel.Element createElement
          (Object objectToWrap)
        {
            if (objectToWrap is global::EA.Element)
            {
                var elementWrapper = new EADBElement(this.EAModel, (global::EA.Element)objectToWrap);
                return this.createEAElementWrapper(elementWrapper);
            }
            else if (objectToWrap is EADBElement)
            {
                return this.createEAElementWrapper((EADBElement)objectToWrap);
            }
            else if (objectToWrap is global::EA.Attribute)
            {
                var attributeWrapper = new EADBAttribute(this.EAModel, (global::EA.Attribute)objectToWrap);
                return this.createEAAttributeWrapper(attributeWrapper);
            }
            else if (objectToWrap is EADBAttribute)
            {
                return this.createEAAttributeWrapper((EADBAttribute)objectToWrap);
            }
            else if (objectToWrap is EADBConnector)
            {
                return this.createEAConnectorWrapper((EADBConnector)objectToWrap);
            }
            else if (objectToWrap is EADBConnectorConstraint)
            {
                return this.createConnectorConstraint((EADBConnectorConstraint)objectToWrap);
            }
            else if (objectToWrap is global::EA.ConnectorConstraint)
            {
                return this.createConnectorConstraint(objectToWrap as global::EA.ConnectorConstraint);
            }
            else if (objectToWrap is global::EA.Connector)
            {
                return this.createEAConnectorWrapper(objectToWrap as global::EA.Connector);
            }
            else if (objectToWrap is global::EA.Method)
            {
                return this.createOperation(objectToWrap as global::EA.Method);
            }
            else if (objectToWrap is global::EA.Parameter)
            {
                return this.createParameter(objectToWrap as global::EA.Parameter);
            }
            else if (objectToWrap is global::EA.Package)
            {
                return this.createPackage(objectToWrap as global::EA.Package);
            }
            else if (objectToWrap is global::EA.Constraint)
            {
                return this.createConstraint(new EADBElementConstraint(this.EAModel, (global::EA.Constraint)objectToWrap));
            }
            else if (objectToWrap is EADBElementConstraint)
            {
                return this.createConstraint((EADBElementConstraint)objectToWrap);
            }
            else if (objectToWrap is global::EA.AttributeConstraint)
            {
                return this.createAttributeConstraint(objectToWrap as global::EA.AttributeConstraint);
            }
            else if (objectToWrap is EADBAttributeConstraint)
            {
                return this.createAttributeConstraint(objectToWrap as EADBAttributeConstraint);
            }

            return null;
        }
        //create a new constraint based on the given EA.Constraint
        public UML.Classes.Kernel.Element createConstraint(EADBElementConstraint constraint)
        {
            return new Constraint(this.model as UTF_EA.Model, constraint);
        }
        public UML.Classes.Kernel.Element createAttributeConstraint(global::EA.AttributeConstraint constraint)
        {
            return new AttributeConstraint(this.model as UTF_EA.Model, new EADBAttributeConstraint(this.EAModel, constraint));
        }
        public UML.Classes.Kernel.Element createAttributeConstraint(EADBAttributeConstraint constraint)
        {
            return new AttributeConstraint(this.model as UTF_EA.Model, constraint);
        }
        public UML.Classes.Kernel.Element createConnectorConstraint(EADBConnectorConstraint constraint)
        {
            return new ConnectorConstraint(this.model as UTF_EA.Model, constraint);
        }
        public UML.Classes.Kernel.Element createConnectorConstraint(global::EA.ConnectorConstraint constraint)
        {
            return new ConnectorConstraint(this.model as UTF_EA.Model, new EADBConnectorConstraint(this.EAModel, constraint));
        }

        private Package createPackage(global::EA.Package package)
        {
            if (package.ParentID == 0
                || Package.getElementForPackage((Model)this.model, package) == null)
            {
                return new RootPackage(this.EAModel, package);
            }
            else
            {
                return new Package(this.EAModel, package);
            }
        }

        /// returns a new EAParameter based on the given EA.Parameter
        private ParameterWrapper createParameter(global::EA.Parameter parameter)
        {
            return new ParameterWrapper(this.EAModel, parameter);
        }

        /// returns a new EAOperatation wrapping the given EA.Method
        private Operation createOperation(global::EA.Method operation)
        {
            return new Operation(this.EAModel, operation);
        }
        private ConnectorWrapper createEAConnectorWrapper
        (global::EA.Connector connector)
        {
            return this.createEAConnectorWrapper(new EADBConnector(this.EAModel, connector));
        }

        /// creates a new EAConnectorWrapper wrapping the given EA.Connector
        private ConnectorWrapper createEAConnectorWrapper(EADBConnector connector)
        {
            switch (connector.Type)
            {
                case "Abstraction":
                    return new Abstraction(this.EAModel, connector);
                case "Generalization":
                    return new Generalization(this.EAModel, connector);
                case "Aggregation":
                case "Association":
                    return new Association(this.EAModel, connector);
                case "Dependency":
                    return new Dependency(this.EAModel, connector);
                case "Usage":
                    return new Usage(this.EAModel, connector);
                case "Sequence":
                case "Collaboration":
                    return new Message(this.EAModel, connector);
                case "Realization":
                case "Realisation":
                    return this.createEARealization(connector);
                case "StateFlow":
                    return new BehaviorStateMachines.Transition(this.EAModel, connector);
                case "InformationFlow":
                    return new InformationFlow(this.EAModel, connector);
                default:
                    return new ConnectorWrapper(this.EAModel, connector);
            }
        }

        private Realization createEARealization(EADBConnector connector)
        {
            // first create an EARealization, then check if this realization is 
            // between an interface and a behaviored classifier.
            // in that case create a new EAInterfaceRealization
            Realization realization = new Realization(this.EAModel,
                                                       connector);
            if (realization.supplier is UML.Classes.Interfaces.Interface &&
                realization.client is UML.Classes.Interfaces.BehavioredClassifier)
            {
                realization = new InterfaceRealization(this.EAModel, connector);
            }
            return realization;
        }

        /// creates a new EAAttribute based on the given EA.Attribute
        internal AttributeWrapper createEAAttributeWrapper(EADBAttribute attributeToWrap)
        {
            if (EnumerationLiteral.isLiteralValue(attributeToWrap, this.EAModel))
            {
                return new EnumerationLiteral(this.EAModel, attributeToWrap);
            }
            else
            {
                return new Attribute(this.EAModel, attributeToWrap);
            }
        }
        /// <summary>
        /// checks if this element is an associationclass.
        /// Associationclasses have the connectorID in the Miscdata[3] (PDATA4)
        /// We have to use this because elementToWrap.IsAssocationClass() throws an exception when used in a background trhead
        /// </summary>
        /// <param name="elementToWrap">the element to check</param>
        /// <returns>true if it is an associationclass</returns>
        private bool isAssociationClass(EADBElement elementToWrap)
        {
            int connectorID;
            bool associationClass = false;
            if (elementToWrap.Type == "Class")
            {
                //normally the class should reference the relation in PData4
                if (int.TryParse(elementToWrap.MiscData[3].ToString(), out connectorID))
                {
                    associationClass = connectorID > 0;
                }
                //but sometimes that is empty. Then test for the existance of an assoiation that has the elementID in it's PDATA1
                if (!associationClass)
                {
                    associationClass = this.EAModel
                                       .getRelationsByQuery($@"select c.Connector_ID from t_connector c
                                                where c.Connector_Type = 'Association'
                                                and PDATA1 = '{elementToWrap.ElementID}'")
                                       .Any();
                }
            }
            return associationClass;
        }
        /// creates a new EAElementWrapper based on the given EA.Element
        internal ElementWrapper createEAElementWrapper
          (EADBElement elementToWrap)
        {
            //first check if this element already exists in the cache
            if (this.EAModel.useCache)
            {
                var elementWrapper = this.EAModel.getElementFromCache(elementToWrap.ElementID);
                if (elementWrapper != null) return elementWrapper;
            }
            var newElementWrapper = getCorrectElementWrapperType(elementToWrap);

            //add the element to the cache if needed
            if (this.EAModel.useCache) this.EAModel.addElementToCache(newElementWrapper);
            return newElementWrapper;
        }
        /// <summary>
        /// get the correct elementWrapper based on the EA.Element
        /// </summary>
        /// <param name="elementToWrap">the element to be wrapped</param>
        /// <returns>the correct ElementWrapper</returns>
        /// <exception cref="Exception"></exception>
        private ElementWrapper getCorrectElementWrapperType(EADBElement elementToWrap)
        {

            switch (elementToWrap.Type)
            {
                case "Class":
                    //first check if this isn't an enumeration.
                    // Enumerations are stored as type Class but with the stereotype enumeration
                    if (elementToWrap.StereotypeEx.Contains("enumeration"))
                    {
                        return new Enumeration(this.EAModel, elementToWrap);
                    }
                    else
                    {
                        //check if associationclass
                        //elementToWrap.IsAssocationClass() returns an exception when used in a background thread so we use our own method to figure out if its an associationClass.
                        if (this.isAssociationClass(elementToWrap))
                        {
                            return new AssociationClass(this.EAModel, elementToWrap);
                        }
                        else
                        {
                            //just a regular class
                            return new Class(this.EAModel, elementToWrap);
                        }
                    }
                case "Object":
                    return new InstanceSpecification(this.EAModel, elementToWrap);
                case "Enumeration":
                    // since version 10 there are also "real" enumerations Both are still supported
                    return new Enumeration(this.EAModel, elementToWrap);
                case "Interface":
                    return new Interface(this.EAModel, elementToWrap);
                case "Association":
                    return new NaryAssociation(this.EAModel, elementToWrap);
                case "Note":
                    return new NoteComment(this.EAModel, elementToWrap);

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
                            return new CallOperationAction(this.EAModel, elementToWrap);

                        }
                    }

                    // simple Action
                    return new Action(this.EAModel, elementToWrap);

                case "Interaction":
                    return new Interaction(this.EAModel, elementToWrap);

                case "Activity":
                    return new Activity(this.EAModel, elementToWrap);

                case "StateMachine":
                    return new BehaviorStateMachines.StateMachine(this.EAModel, elementToWrap);

                case "State":
                    return new BehaviorStateMachines.State(this.EAModel, elementToWrap, null);

                case "StateNode":
                    string metaType = elementToWrap.MetaType;
                    if (metaType == "Pseudostate" ||
                       metaType == "Synchronisation")
                    {
                        return new BehaviorStateMachines.PseudoState(this.EAModel, elementToWrap, null);

                    }
                    else if (metaType == "FinalState")
                    {
                        return new BehaviorStateMachines.FinalState(this.EAModel, elementToWrap, null);

                    }
                    return new ElementWrapper(this.EAModel, elementToWrap);

                case "Package":
                    int packageID;
                    if (int.TryParse(elementToWrap.MiscData[0], out packageID))
                    {
                        return ((Model)this.model).getElementWrapperByPackageID(packageID);

                    }
                    else
                    {
                        throw new Exception("WrappedElement " + elementToWrap.Name + " is not a package");
                    }
                case "DataType":
                    return new DataType(this.EAModel, elementToWrap);

                case "PrimitiveType":
                    return new PrimitiveType(this.EAModel, elementToWrap);

                case "InformationItem":
                    return new InformationItem(this.EAModel, elementToWrap);

                case "ProxyConnector":
                    return new ProxyConnector(this.EAModel, elementToWrap);

                case "Part":
                    return new Property(this.EAModel, elementToWrap);

                case "InteractionFragment":
                    return new InteractionFragment(this.EAModel, elementToWrap);

                case "Component":
                    return new Component(this.EAModel, elementToWrap);

                default:
                    return new ElementWrapper(this.EAModel, elementToWrap);

            }
        }

        /// creates a new primitive type based on the given typename
        public override UML.Classes.Kernel.PrimitiveType createPrimitiveType
          (Object typeName)
        {
            return new PrimitiveType(this.EAModel, typeName as string);
        }

        /// creates a new EAParameterReturnType based on the given operation
        internal ParameterReturnType createEAParameterReturnType
          (Operation operation)
        {
            ParameterReturnType returntype = new ParameterReturnType
              (this.EAModel, operation);
            // if the name of the returntype is empty that means that there is no 
            // returntype defined in EA.
            return returntype.type.name == string.Empty ? null : returntype;
        }

        /// returns a new stereotype based on the given name and attached to the 
        /// given element
        public override UML.Profiles.Stereotype createStereotype
          (UML.Classes.Kernel.Element owner, String name)
        {
            return new Stereotype(this.EAModel, owner as Element, name);
        }
        /// returns a new stereotype based on the given name and attached to the 
        /// given diagram
        public UML.Profiles.Stereotype createStereotype
          (UML.Diagrams.Diagram owner, String name)
        {
            return new Stereotype(this.EAModel, owner as Element, name);
        }
        /// creates a set of stereotypes based on the comma seperated names string
        /// and attaches it to the given element
        public HashSet<UML.Profiles.Stereotype> createStereotypes
          (UML.Classes.Kernel.Element owner, String names)
        {
            HashSet<UML.Profiles.Stereotype> newStereotypes =
              new HashSet<UML.Profiles.Stereotype>();
            String[] stereotypeNames = names.Split(',');
            foreach (String name in stereotypeNames)
            {
                if (name != String.Empty)
                {
                    UML.Profiles.Stereotype stereotype =
                      this.createStereotype(owner, name);
                    if (stereotype != null)
                    {
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
            foreach (String name in stereotypeNames)
            {
                if (name != String.Empty)
                {
                    UML.Profiles.Stereotype stereotype =
                      this.createStereotype(owner, name);
                    if (stereotype != null)
                    {
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
            ElementWrapper owningElement = this.getOwningElement(elementWrapper);
            global::EA.Diagram masterDiagram = null;
            if (owningElement != null)
            {
                // Get the master state diagram from the owning element if available
                masterDiagram = this.getMasterStateDiagram(owningElement, elementWrapper.wrappedElement);
            }
            if (masterDiagram == null)
            {
                // Get the master state diagram from the current element if available
                masterDiagram = this.getMasterStateDiagram(elementWrapper, elementWrapper.wrappedElement);
            }

            if (elementWrapper.wrappedElement.Partitions.Count == 0)
            {
                // Check if the wrapped element contains any sub states
                if (elementWrapper.wrappedElement.IsComposite || elementWrapper.wrappedElement.Type == "StateMachine")
                {
                    // Create an implicit default region
                    UML.StateMachines.BehaviorStateMachines.Region defaultRegion =
                        new BehaviorStateMachines.Region(this.EAModel, elementWrapper, masterDiagram, null);
                    newRegions.Add(defaultRegion);
                }
            }
            else
            {
                // Create a region for all partitions of the wrapped element
                short regionPos = 0;
                foreach (global::EA.Partition partition in elementWrapper.wrappedElement.Partitions)
                {
                    UML.StateMachines.BehaviorStateMachines.Region newRegion =
                        new BehaviorStateMachines.Region(this.EAModel, elementWrapper, masterDiagram, partition, regionPos);
                    newRegions.Add(newRegion);
                    ++regionPos;
                }
            }
            return newRegions;
        }

        internal UML_SM.Region getContainingRegion(ElementWrapper elementWrapper)
        {
            ElementWrapper parentElement = this.getOwningElement(elementWrapper);
            if (parentElement is BehaviorStateMachines.StateMachine)
            {
                BehaviorStateMachines.StateMachine owningStateMachine =
                    parentElement as BehaviorStateMachines.StateMachine;
                foreach (BehaviorStateMachines.Region region in owningStateMachine.regions)
                {
                    if (region.isContainedElement(elementWrapper))
                    {
                        return region;
                    }
                }
            }
            else if (parentElement is BehaviorStateMachines.State)
            {
                BehaviorStateMachines.State owningState =
                    parentElement as BehaviorStateMachines.State;
                foreach (BehaviorStateMachines.Region region in owningState.regions)
                {
                    if (region.isContainedElement(elementWrapper))
                    {
                        return region;
                    }
                }
            }

            return null;
        }

        internal HashSet<UML_SM.Vertex> createVertices
            (BehaviorStateMachines.Region region
            )
        {
            HashSet<UML_SM.Vertex> newVertices =
                new HashSet<UML_SM.Vertex>();
            var parentElement = region.wrappedElement;
            foreach (global::EA.Element childElement in parentElement.Elements)
            {
                UML_SM.Vertex newVertex = null;
                switch (childElement.Type)
                {
                    case "State":
                    case "StateMachine":
                    case "StateNode":
                        newVertex = this.createElement(childElement) as UML_SM.Vertex;
                        break;
                }
                if (newVertex != null)
                {
                    if (region.isContainedElement(newVertex as ElementWrapper))
                    {
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
            foreach (global::EA.Connector connector in vertex.wrappedElement.Connectors)
            {
                if (connector.Type == "StateFlow" &&
                   connector.ClientID == vertex.wrappedElement.ElementID)
                {
                    UML_SM.Transition transition = this.createEAConnectorWrapper(connector) as UML_SM.Transition;
                    if (transition != null)
                    {
                        outgoingTransitions.Add(transition);
                    }
                }
            }
            return outgoingTransitions;
        }

        internal HashSet<UML_SM.Transition> createIncomingTransitions(BehaviorStateMachines.Vertex vertex)
        {
            HashSet<UML_SM.Transition> incomingTransitions = new HashSet<UML_SM.Transition>();
            foreach (global::EA.Connector connector in vertex.wrappedElement.Connectors)
            {
                if (connector.Type == "StateFlow" &&
                   connector.SupplierID == vertex.wrappedElement.ElementID)
                {
                    UML_SM.Transition transition = this.createEAConnectorWrapper(connector) as UML_SM.Transition;
                    if (transition != null)
                    {
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

            UML_SM.StateMachine stateMachine = this.getContainingStateMachine(transition.source);
            if (stateMachine != null)
            {
                string[] events = transition.wrappedConnector.TransitionEvent.Split(", ".ToCharArray());
                if (events.Length > 0)
                {
                    if (!this.stateMachineTriggersMap.ContainsKey(stateMachine))
                    {
                        this.stateMachineTriggersMap[stateMachine] = this.createContainedTriggers(stateMachine as UTF_EA.BehaviorStateMachines.StateMachine);
                    }
                    foreach (string eventName in events)
                    {
                        foreach (UML.CommonBehaviors.Communications.Trigger trigger in this.stateMachineTriggersMap[stateMachine])
                        {
                            if (trigger.event_.name == eventName)
                            {
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
            if (stateMachine != null)
            {
                if (!this.stateMachineTriggersMap.ContainsKey(stateMachine))
                {
                    this.stateMachineTriggersMap[stateMachine] = this.createContainedTriggers(stateMachine as UTF_EA.BehaviorStateMachines.StateMachine);
                }
                return this.stateMachineTriggersMap[stateMachine];
            }

            return null;
        }

        internal HashSet<UML.CommonBehaviors.Communications.Trigger> createContainedTriggers(UTF_EA.BehaviorStateMachines.StateMachine stateMachine)
        {
            HashSet<UML.CommonBehaviors.Communications.Trigger> triggers =
                new HashSet<UML.CommonBehaviors.Communications.Trigger>();
            if (stateMachine != null)
            {
                foreach (global::EA.Element element in stateMachine.wrappedElement.Elements)
                {
                    if (element.MetaType == "Trigger")
                    {
                        triggers.Add(this.createElement(element) as UTF_EA.BehaviorStateMachines.Trigger); //TODO check if ceateElement creates the correct type of trigger.
                    }
                }
            }
            return triggers;
        }

        public UML_SM.StateMachine getContainingStateMachine(UML.Extended.UMLItem umlItem)
        {
            if (umlItem != null)
            {
                if (umlItem.owner is UML_SM.StateMachine)
                {
                    return umlItem.owner as UML_SM.StateMachine;
                }
                else
                {
                    return this.getContainingStateMachine(umlItem.owner);
                }
            }

            return null;
        }

        internal ElementWrapper getOwningElement(ElementWrapper elementWrapper)
        {
            if (elementWrapper.wrappedElement.ParentID != 0)
            {
                return elementWrapper.EAModel.getElementWrapperByID(elementWrapper.wrappedElement.ParentID);
            }

            return null;
        }

        internal global::EA.Diagram getMasterStateDiagram(ElementWrapper elementWrapper, EADBElement stateChartElement)
        {
            foreach (global::EA.Diagram diagram in elementWrapper.wrappedElement.Diagrams)
            {
                // Just return the first state chart diagram found that contains the stateChartElement
                if (diagram.Type == "Statechart")
                {
                    foreach (global::EA.DiagramObject diagramObject in diagram.DiagramObjects)
                    {
                        if (stateChartElement.ElementID == diagramObject.ElementID)
                        {
                            return diagram;
                        }
                    }
                }
            }

            return null;
        }

        internal AssociationEnd createAssociationEnd(ConnectorWrapper connector, EADBConnectorEnd associationEnd)
        {
            return new AssociationEnd(this.EAModel, connector, associationEnd);
        }
        public override UML.Profiles.TaggedValue createNewTaggedValue(UML.Classes.Kernel.Element owner, string name)
        {
            var myOwner = owner as Element;
            if (myOwner != null)
            {
                var eaTaggedValues = myOwner.eaTaggedValuesCollection;
                if (eaTaggedValues != null)
                {
                    return this.createTaggedValue(myOwner, eaTaggedValues.AddNew(name, ""));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// create a new element as owned element of the given owner
        public override T createNewElement<T>(UML.Classes.Kernel.Element owner, String name)
        {
            T returnedValue = null;
            if (owner is Package)
            {
                returnedValue = ((Package)owner).addOwnedElement<T>(name);
            }
            else if (owner is ElementWrapper)
            {
                returnedValue = ((ElementWrapper)owner).addOwnedElement<T>(name);
            }
            else if (owner is AttributeWrapper)
            {
                if (typeof(T).Name == "AttributeConstraint"
                    || typeof(T).Name == "Constraint")
                {
                    returnedValue = ((AttributeWrapper)owner).addAttributeConstraint(name) as T;
                }
            }
            else if (owner is ConnectorWrapper)
            {
                if (typeof(T).Name == "ConnectorConstraint"
                    || typeof(T).Name == "Constraint")
                {
                    returnedValue = ((ConnectorWrapper)owner).addConnectorConstraint(name) as T;
                }
            }
            else if (owner is Operation)
            {
                //We can only add Parameters as owned elements to an operation
                returnedValue = ((Operation)owner).addOwnedParameter(name) as T;
            }
            else
            {
                returnedValue = default(T);
            }
            //new elements are always dirty
            var returnedElement = returnedValue as Element;
            if (returnedElement != null)
            {
                returnedElement.isNew = true;
            }

            return returnedValue;
        }

        internal T addElementToEACollection<T>(global::EA.Collection collection,
                                                string name, string EAType)
          where T : class, UML.Classes.Kernel.Element
        {
            Element newElement;
            //creating an enumeration is a bit special because EA thinks its just an attribute
            if (typeof(T).Name == "EnumerationLiteral")
            {
                newElement = new EnumerationLiteral((Model)this.model 
                                       , new EADBAttribute(this.EAModel   
                                                    ,(global::EA.Attribute)collection.AddNew(name, this.translateTypeName(EAType))));
            }
            //Creating Associationclasses is a bit special too. It only becomes an associationclass according to EA once it is linked to an association
            else if (typeof(T).Name == "AssociationClass")
            {
                newElement = new AssociationClass(this.EAModel
                                        , new EADBElement(this.EAModel
                                                    , (global::EA.Element)collection.AddNew(name, this.translateTypeName(EAType))));
            }
            else
            {
                var eaElement = collection.AddNew(name, this.translateTypeName(EAType));
                //we need to save a package element because otherwise the EA.element does not exist in the model yet.
                //that means we get a nullpointerexception when saving the new package.
                var eaPackage = eaElement as global::EA.Package;
                if (eaPackage != null)
                {
                    eaPackage.Update();
                }
                newElement = this.model.factory.createElement(eaElement) as Element;
            }
            //indicate it's new
            newElement.isNew = true;
            return newElement as T;
        }

        internal T addElementToEACollection<T>(global::EA.Collection collection,
                                                String name)
          where T : class, UML.Classes.Kernel.Element
        {
            return this.addElementToEACollection<T>(collection, name, this.translateTypeName(typeof(T).Name));
        }

        /// translates the UML type name to the EA equivalent
        internal String translateTypeName(String typeName)
        {
            switch (typeName)
            {
                case "Property":
                case "EnumerationLiteral":
                    return "Attribute";
                case "AssociationClass":
                    return "Class";
                case "NoteComment":
                    return "Note";
                case "InstanceSpecification":
                    return "Object";
                case "NaryAssociation":
                    return "Association";
                default:
                    return typeName;
            }
        }

        public bool isEAPackage(Type type)
        {
            return type.Name == "Package";
        }
        internal bool isEAAtttribute(System.Type type)
        {
            return (type.Name == "Property"
                    || type.Name == "EnumerationLiteral"
                    || type.Name == "Attribute");
        }

        internal bool isEAOperation(System.Type type)
        {
            return type.Name == "Operation";
        }

        internal bool isEAParameter(System.Type type)
        {
            return type.Name == "Parameter";
        }
        internal bool isEAEmbeddedElement(System.Type type)
        {
            //issue in how to make the different between a property element an
            return type.Name == "Port";
        }
        internal bool isEAConnector(System.Type type)
        {
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
                  || type.Name == "Abstraction"
                  || type.Name == "ConnectorWrapper";

        }

        public override UML.Diagrams.DiagramElement createNewDiagramElement
          (UML.Diagrams.Diagram owner, UML.Classes.Kernel.Element element)
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
        internal T addNewDiagramToEACollection<T>(global::EA.Collection collection, string name) where T : class, UML.Diagrams.Diagram
        {
            string EADiagramType = this.translateUMLDiagramtypes(typeof(T).Name);
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

        public override UML.Profiles.TaggedValue createTaggedValue(UML.Classes.Kernel.Element owner, object objectToWrap)
        {
            UML.Profiles.TaggedValue newTaggedValue = null;
            if (objectToWrap is global::EA.TaggedValue)
            {
                newTaggedValue = this.createElementTag((Element)owner, (global::EA.TaggedValue)objectToWrap);
            }
            else if (objectToWrap is EADBElementTag)
            {
                newTaggedValue = this.createElementTag((Element)owner, (EADBElementTag)objectToWrap);
            }
            else if (objectToWrap is global::EA.AttributeTag)
            {
                newTaggedValue = this.createAttributeTag((Element)owner, (global::EA.AttributeTag)objectToWrap);
            }
            else if (objectToWrap is EADBAttributeTag)
            {
                newTaggedValue = this.createAttributeTag((Element)owner, (EADBAttributeTag)objectToWrap);
            }
            else if (objectToWrap is global::EA.MethodTag)
            {
                newTaggedValue = this.createOperationTag((Element)owner, (global::EA.MethodTag)objectToWrap);
            }
            else if (objectToWrap is EADBOperationTag)
            {
                newTaggedValue = this.createOperationTag((Element)owner, (EADBOperationTag)objectToWrap);
            }
            else if (objectToWrap is global::EA.ConnectorTag)
            {
                newTaggedValue = this.createRelationTag((Element)owner, (global::EA.ConnectorTag)objectToWrap);
            }
            else if (objectToWrap is EADBConnectorTag)
            {
                newTaggedValue = this.createRelationTag((Element)owner, (EADBConnectorTag)objectToWrap);
            }
            else if (objectToWrap is global::EA.ParamTag) //TODO add EADB variant
            {
                newTaggedValue = this.createParameterTag((Element)owner, (global::EA.ParamTag)objectToWrap);
            }
            else if (objectToWrap is global::EA.RoleTag) 
            {
                newTaggedValue = this.createRoleTag((Element)owner, (global::EA.RoleTag)objectToWrap);
            }
            else if (objectToWrap is EADBRoleTag) 
            {
                newTaggedValue = this.createRoleTag((Element)owner, (EADBRoleTag)objectToWrap);
            }
            return newTaggedValue;
        }

        public ElementTag createElementTag(Element owner, global::EA.TaggedValue objectToWrap)
        {
            return new ElementTag((Model)this.model, owner, new EADBElementTag(this.EAModel, objectToWrap));
        }
        public ElementTag createElementTag(Element owner, EADBElementTag objectToWrap)
        {
            return new ElementTag((Model)this.model, owner, objectToWrap);
        }
        public AttributeTag createAttributeTag(Element owner, global::EA.AttributeTag objectToWrap)
        {
            return new AttributeTag((Model)this.model, owner, new EADBAttributeTag(this.EAModel, objectToWrap));
        }
        public AttributeTag createAttributeTag(Element owner, EADBAttributeTag objectToWrap)
        {
            return new AttributeTag((Model)this.model, owner, objectToWrap);
        }
        public OperationTag createOperationTag(Element owner, global::EA.MethodTag objectToWrap)
        {
            return new OperationTag((Model)this.model, owner, new EADBOperationTag(this.EAModel, objectToWrap));
        }
        public OperationTag createOperationTag(Element owner, EADBOperationTag objectToWrap)
        {
            return new OperationTag((Model)this.model, owner, objectToWrap);
        }
        public RelationTag createRelationTag(Element owner, global::EA.ConnectorTag objectToWrap)
        {
            return new RelationTag((Model)this.model, owner, new EADBConnectorTag(this.EAModel, objectToWrap));
        }
        public RelationTag createRelationTag(Element owner, EADBConnectorTag objectToWrap)
        {
            return new RelationTag((Model)this.model, owner, objectToWrap);
        }
        public ParameterTag createParameterTag(Element owner, global::EA.ParamTag objectToWrap) //TODO change to EADB variant
        {
            return new ParameterTag((Model)this.model, owner, objectToWrap);
        }
        public RoleTag createRoleTag(Element owner, global::EA.RoleTag objectToWrap)
        {
            return this.createRoleTag(owner, new EADBRoleTag(this.EAModel, objectToWrap));
        }
        public RoleTag createRoleTag(Element owner, EADBRoleTag objectToWrap)
        {
            return new RoleTag((Model)this.model, owner, objectToWrap);
        }
        public DescriptionComment createDescriptionComment(Element owner)
        {
            return new DescriptionComment((Model)this.model, owner);
        }
        public EAAddinFramework.EASpecific.User createUser(string login, string firstname, string lastname)
        {
            return new EAAddinFramework.EASpecific.User((Model)this.model, login, firstname, lastname);
        }
        public EAAddinFramework.EASpecific.WorkingSet createWorkingSet(string name, string ID, EAAddinFramework.EASpecific.User user)
        {
            return new EAAddinFramework.EASpecific.WorkingSet((Model)this.model, ID, user, name);
        }
        public override UML.Classes.Kernel.ValueSpecification createValueSpecification(object objectToWrap)
        {
            //check the type fo the object
            if (objectToWrap == null)
            {
                return new LiteralNull();
            }

            if (objectToWrap is int)
            {
                return new LiteralInteger((int)objectToWrap);
            }

            if (objectToWrap is bool)
            {
                return new LiteralBoolean((bool)objectToWrap);
            }

            if (objectToWrap is string)
            {
                return new LiteralString((string)objectToWrap);
            }

            if (objectToWrap is UnlimitedNatural)
            {
                return new LiteralUnlimitedNatural((UnlimitedNatural)objectToWrap);
            }
            //if its something else then we don't now
            return null;
        }
        public override UML.Classes.Kernel.ValueSpecification createValueSpecificationFromString(string stringRepresentation)
        {
            if (stringRepresentation != null &&
                stringRepresentation.Equals("null", StringComparison.InvariantCultureIgnoreCase))
            {
                return this.createValueSpecification(null);
            }

            int integerRepresentation;
            if (int.TryParse(stringRepresentation, out integerRepresentation))
            {
                return this.createValueSpecification(integerRepresentation);
            }

            bool boolRepresentation;
            if (bool.TryParse(stringRepresentation, out boolRepresentation))
            {
                return this.createValueSpecification(boolRepresentation);
            }

            UnlimitedNatural unlimitedNaturalRepresentation;
            if (UnlimitedNatural.TryParse(stringRepresentation, out unlimitedNaturalRepresentation))
            {
                return this.createValueSpecification(unlimitedNaturalRepresentation);
            }
            //default string
            return this.createValueSpecification(stringRepresentation);
        }
    }
}
