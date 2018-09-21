using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;
namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of ElementMappingSet.
	/// </summary>
	public class ClassifierMappingNode:MappingNode
	{

        public ClassifierMappingNode(TSF_EA.ElementWrapper sourceElement) : this(sourceElement, null) { }
        public ClassifierMappingNode(TSF_EA.ElementWrapper sourceElement, MappingNode parent) : base(sourceElement, parent) { }
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

        public override IEnumerable<MP.Mapping> getMappings(MP.MappingNode targetRootNode)
        {
            //get the connector mappings
            //check if the package has a trace to another package
            TSF_EA.ElementWrapper targetRootElement = null;
            var packageTrace = this.sourceElement.relationships.OfType<TSF_EA.Abstraction>().FirstOrDefault(x => x.target is TSF_EA.Element && x.stereotypes.Any(y => y.name == "trace"));
            if (packageTrace != null) targetRootElement = packageTrace.target as TSF_EA.ElementWrapper;
            List<Mapping> returnedMappings = new List<Mapping>();
            foreach (var classElement in sourceElement.ownedElements.OfType<TSF_EA.ElementWrapper>())
            {
                returnedMappings.AddRange(MappingFactory.createOwnedMappings(classElement, sourceElement.name + "." + classElement.name, targetRootElement, true));
            }
            return returnedMappings.Cast<MP.Mapping>();
        }
        protected override void setChildNodes()
        {
            //create child nodes for each attribute
            foreach (TSF_EA.Attribute ownedAttribute in sourceElement.ownedAttributes)
            {
                var childNode = new AttributeMappingNode(ownedAttribute, this);
            }
            //create child nodes for each owned classifier
            foreach(TSF_EA.ElementWrapper ownedClassifier in sourceElement.ownedElements.OfType<UML.Classes.Kernel.Classifier>())
            {
                var childNode = new ClassifierMappingNode(ownedClassifier, this);
            }
            //create child nodes for each owned association
            foreach(TSF_EA.Association ownedAssociation in this.sourceElement.getRelationships<TSF_EA.Association>())
            {
                if (ownedAssociation.targetEnd.isNavigable && ownedAssociation.sourceElement.uniqueID == this.sourceElement.uniqueID
                    || ownedAssociation.sourceEnd.isNavigable && ownedAssociation.targetElement.uniqueID == this.sourceElement.uniqueID)
                {
                    var childNode = new AssociationMappingNode(ownedAssociation, this);
                }
            }
        }
    }
}
