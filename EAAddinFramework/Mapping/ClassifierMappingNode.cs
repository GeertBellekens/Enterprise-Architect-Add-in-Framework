using System;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.UML.Extended;
using TSF.UmlToolingFramework.UML.Profiles;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of ElementMappingSet.
	/// </summary>
	public class ClassifierMappingNode:MappingNode
	{

        public ClassifierMappingNode(TSF_EA.ElementWrapper sourceElement, MappingSettings settings, MP.ModelStructure structure) : this(sourceElement, null, settings, structure) { }
        public ClassifierMappingNode(TSF_EA.ElementWrapper sourceElement, MappingNode parent, MappingSettings settings, MP.ModelStructure structure) : this(sourceElement, parent, settings, structure, null) { }
        public ClassifierMappingNode(TSF_EA.ElementWrapper sourceElement, MappingNode parent, MappingSettings settings, MP.ModelStructure structure, UML.Classes.Kernel.NamedElement virtualOwner) : base(sourceElement, parent, settings, structure, virtualOwner) { }
        protected override List<TaggedValue> sourceTaggedValues => this.sourceElement?.taggedValues.ToList();

        internal TSF_EA.ElementWrapper sourceElement
        {
            get
            {
                return this.source as TSF_EA.ElementWrapper;
            }
            set
            {
                this.source = value;
            }
        }

        public override IEnumerable<MP.Mapping> getOwnedMappings(MP.MappingNode targetRootNode)
        {
            var foundMappings = new List<MP.Mapping>();
            //get the mappings using trace relations
            //TODO: is this fast enough?
            foreach (var trace in this.sourceElement.getRelationships<TSF_EA.Abstraction>().Where(x => x.target is TSF_EA.Element && x.stereotypes.Any(y => y.name == "trace")))
            {
                //get the mappings based on traces
                var mapping = MappingFactory.getMapping(this, trace, (MappingNode)targetRootNode);
                if (mapping != null) foundMappings.Add(mapping);
            }
            //also add the base mappings
            foundMappings.AddRange(base.getOwnedMappings(targetRootNode));
            return foundMappings;
        }
        public override void setChildNodes()
        {
            addElementPropertiesToChildNodes(null);
        }
        private void addElementPropertiesToChildNodes(TSF_EA.ElementWrapper virtualElement)
        {
            //figure out the if we have virtual element
            var element = virtualElement != null ? virtualElement : this.sourceElement;

            if (element == null) return; //failsafe 

            //create child nodes for each attribute
            foreach (TSF_EA.Attribute ownedAttribute in element.ownedAttributes)
            {
                if (!this.allChildNodes.Any(x => x.source?.uniqueID == ownedAttribute.uniqueID))
                {
                    var childNode = new AttributeMappingNode(ownedAttribute, this, this.settings, this.structure, virtualElement);
                }
            }
            //create child nodes for all enum values
            var sourceEnum = element as TSF_EA.Enumeration;
            if (sourceEnum != null)
            {
                foreach (var enumLiteral in sourceEnum.ownedLiterals)
                {
                    if (!this.allChildNodes.Any(x => x.source?.uniqueID == enumLiteral.uniqueID))
                    {
                        var childNode = new AttributeMappingNode((TSF_EA.AttributeWrapper)enumLiteral, this, this.settings, this.structure, virtualElement);
                    }
                }
            }
            //create child nodes for each owned classifier
            foreach (TSF_EA.ElementWrapper ownedClassifier in element.ownedElements.OfType<UML.Classes.Kernel.Namespace>())
            {
                if (!this.allChildNodes.Any(x => x.source?.uniqueID == ownedClassifier.uniqueID))
                {
                    var childNode = new ClassifierMappingNode(ownedClassifier, this, this.settings, this.structure, virtualElement);
                }
            }
            //create child nodes for each owned association
            foreach (var ownedAssociation in element.getRelationships<TSF_EA.Association>())
            {
                if ((ownedAssociation.targetEnd.isNavigable && ownedAssociation.sourceElement.uniqueID == element.uniqueID
                    || ownedAssociation.sourceEnd.isNavigable && ownedAssociation.targetElement.uniqueID == element.uniqueID)
                    && !this.allChildNodes.Any(x => x.source?.uniqueID == ownedAssociation.uniqueID))
                {
                    var childNode = new AssociationMappingNode(ownedAssociation, this, this.settings, this.structure, virtualElement);
                }
            }
            //do the same for all superclasses
            foreach (var superClass in element.superClasses)
            {
                addElementPropertiesToChildNodes((TSF_EA.ElementWrapper)superClass);
            }
        }

        protected override UMLItem createMappingItem(MappingNode targetNode)
        {
            var classifierTargetNode = targetNode as ClassifierMappingNode;
            if (classifierTargetNode != null)
            {
                var trace = this.sourceElement.EAModel.factory.createNewElement<UML.Classes.Dependencies.Abstraction>(this.sourceElement, "");
                trace.addStereotype(this.sourceElement.EAModel.factory.createStereotype(trace, "trace"));
                trace.target = classifierTargetNode.sourceElement;
                trace.save();
                return trace;
            }
            else
            {
                return this.createTaggedValueMappingItem(targetNode);
            }
        }
    }
}
