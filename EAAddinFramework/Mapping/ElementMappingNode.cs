using EAAddinFramework.Utilities;
using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.UML.Extended;
using TSF.UmlToolingFramework.UML.Profiles;
using MP = MappingFramework;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Mapping
{
    /// <summary>
    /// Description of ElementMappingSet.
    /// </summary>
    public class ElementMappingNode : MappingNode
    {

        public ElementMappingNode(TSF_EA.ElementWrapper sourceElement, MappingSettings settings, MP.ModelStructure structure, bool isTarget) 
            : this(sourceElement, null, settings, structure, isTarget) { }
        public ElementMappingNode(TSF_EA.ElementWrapper sourceElement, MappingNode parent, MappingSettings settings, MP.ModelStructure structure, bool isTarget) 
            : this(sourceElement, parent, settings, structure, null, isTarget) { }
        public ElementMappingNode(TSF_EA.ElementWrapper sourceElement, MappingNode parent, MappingSettings settings, MP.ModelStructure structure, UML.Classes.Kernel.NamedElement virtualOwner, bool isTarget) 
            : base(sourceElement, parent, settings, structure, virtualOwner, isTarget) { }

        protected override List<TaggedValue> sourceTaggedValues
        {
            get => this.sourceElement?.taggedValues.ToList();
        }

        internal TSF_EA.ElementWrapper sourceElement
        {
            get => this.source as TSF_EA.ElementWrapper;
            set => this.source = value;
        }

        public override IEnumerable<MP.Mapping> subClassMappings
        {
            get
            {
                var foundMappings = new List<MP.Mapping>();
                //get subclasses
                var subClasses = this.sourceElement.allSubClasses;
                //exit now if there are no subclasses
                if (! subClasses.Any())
                {
                    return foundMappings;
                }
                //get all other elementMappingNodes
                var otherElementMappingNodes = this.isTarget ?
                    this.mappingSet.mappings.Where(x => x.target is ElementMappingNode)
                    : this.mappingSet.mappings.Where(x => x.source is ElementMappingNode);
                foreach (var subClass in subClasses)
                {
                    //get the mappings to theis subclass
                    var correspondingMappings = this.isTarget ?
                                            otherElementMappingNodes.Where(x => x.target.source.uniqueID == subClass.uniqueID)
                                            : otherElementMappingNodes.Where(x => x.source.source.uniqueID == subClass.uniqueID);
                    //add those mappings to the list of mappings
                    foundMappings.AddRange(correspondingMappings);
                }
                return foundMappings;
            }
        }

        public override void setChildNodes()
        {
            // log progress
            EAOutputLogger.log($"Loading '{this.name}'");
            this.addElementPropertiesToChildNodes(null);
        }
        private void addElementPropertiesToChildNodes(TSF_EA.ElementWrapper virtualElement)
        {
            //figure out the if we have virtual element
            var element = virtualElement != null ? virtualElement : this.sourceElement;

            if (element == null)
            {
                return; //failsafe 
            }

            //create child nodes for each attribute
            foreach (TSF_EA.Attribute ownedAttribute in element.ownedAttributes)
            {
                if ((this.structure == MP.ModelStructure.DataModel 
                    || !existAsParent(ownedAttribute))
                    && !this.allChildNodes.Any(x => x.source?.uniqueID == ownedAttribute.uniqueID))
                {
                    var childNode = new AttributeMappingNode(ownedAttribute, this, this.settings, this.structure, virtualElement, this.isTarget);
                }
            }
            //create child nodes for all enum values
            var sourceEnum = element as TSF_EA.Enumeration;
            if (sourceEnum != null)
            {
                foreach (var enumLiteral in sourceEnum.ownedLiterals)
                {
                    if ((this.structure == MP.ModelStructure.DataModel || !existAsParent(enumLiteral))
                        && !this.allChildNodes.Any(x => x.source?.uniqueID == enumLiteral.uniqueID))
                    {
                        var childNode = new AttributeMappingNode((TSF_EA.AttributeWrapper)enumLiteral, this, this.settings, this.structure, virtualElement, this.isTarget);
                    }
                }
            }
            //create child nodes for each owned element
            foreach (var ownedClassifier in element.ownedElementWrappers
                                    .Where(x => x.EAElementType == "Class"
                                            || x.EAElementType == "Enumeration"
                                            || x.EAElementType == "DataType"
                                            || x.EAElementType == "Package"))
            {
                if ((this.structure == MP.ModelStructure.DataModel
                    ||!existAsParent(ownedClassifier))
                    && !this.allChildNodes.Any(x => x.source?.uniqueID == ownedClassifier.uniqueID))
                {
                    var childNode = new ElementMappingNode(ownedClassifier, this, this.settings, this.structure, virtualElement, this.isTarget);
                }
            }
            //create child nodes for each owned association
            foreach (var ownedAssociation in element.getRelationships<TSF_EA.Association>(true, false)
                                            .OrderBy(x => this.getSequence(x)).ThenBy(x => x.orderingName))
            {
                if ((this.structure == MP.ModelStructure.DataModel
                    || ! existAsParent(ownedAssociation))
                    &&! this.allChildNodes.Any(x => x.source?.uniqueID == ownedAssociation.uniqueID))
                {
                    var childNode = new AssociationMappingNode(ownedAssociation, this, this.settings, this.structure, virtualElement, this.isTarget);
                }
            }


            //do the same for all superclasses
            foreach (var superClass in element.superClasses)
            {
                this.addElementPropertiesToChildNodes((TSF_EA.ElementWrapper)superClass);
            }
        }
        private int getSequence(TSF_EA.Association association)
        {
            int sequence;
            if (int.TryParse(association.taggedValues.FirstOrDefault
                (x => x.name.Equals("sequencingKey", System.StringComparison.InvariantCultureIgnoreCase))
                ?.tagValue.ToString()
                , out sequence))
            {
                return sequence;
            }
            else
            {
                return int.MaxValue; //no tag found, return max number
            }
        }

        public override MP.MappingNode findNode(List<string> mappingPathNames)
        {
            var foundNode = base.findNode(mappingPathNames);
            if (foundNode != null)
            {
                return foundNode;
            }
            //try the alternative approach
            return findAlternative(mappingPathNames, false);
        }

        public MP.MappingNode findAlternative (List<string> mappingPathNames, bool reversed)
        {
            //if not found then it might be a Class.attribute reference (or Class.Association)
            //So if there are only two names in the mapping path, AND the source node is a package,
            //we check if we can find a class with the given name in the package or sub-packages
            if (mappingPathNames.Any()
                && mappingPathNames.Count <= 3
                && this.source is TSF_EA.Package)
            {
                var classes = ((TSF_EA.Package)this.sourceElement)
                        .findOwnedItems(mappingPathNames[0]).OfType<TSF_EA.ElementWrapper>()
                        .Where(x => x is TSF_EA.Class || x is TSF_EA.DataType || x is TSF_EA.Enumeration);
                foreach (var classElement in classes)
                {
                    var nodePath = this.findPath(classElement);
                    if (nodePath.Any())
                    {
                        if (mappingPathNames.Count == 1)
                        {
                            //only className was given, return immediately
                            return nodePath.Last();
                        }
                        //from this node find the attribute
                        var foundNode = nodePath.Last().findNode(mappingPathNames);
                        if (foundNode != null)
                        {
                            return foundNode;
                        }
                    }
                }
                //sometimes we also get class1.class2.attribute2, but there is only a association from class2 to class1
                // in that case we reverse the class2 and class1 and try again.
                if (! reversed && mappingPathNames.Count >= 2 )
                {
                    return findAlternative(new List<string> { mappingPathNames[1], mappingPathNames[0] }, true);
                }
            }
            //not found, return null
            return null;
        }

        private List<MappingNode> findPath(TSF_EA.Element itemToFind)
        {
            //if itemToFind is null we return an empty list
            if (itemToFind == null)
            {
                return new List<MappingNode>();
            }
            //check if this node's source is equal to the itemToFind
            if (this.source.Equals(itemToFind))
            {
                //found it, return this node in the list
                return new List<MappingNode> { this };
            }
            //not found, check owner
            var owner = itemToFind.owner;
            if (owner != null)
            {
                var ownerPath = this.findPath((TSF_EA.Element)owner);
                if (ownerPath.Any())
                {
                    var subNode = ownerPath.Last().allChildNodes.FirstOrDefault(x => x.source.Equals(itemToFind));
                    if (subNode != null)
                    {
                        ownerPath.Add((MappingNode)subNode);
                    }
                }
                return ownerPath;
            }
            else
            {
                //reach the root node without finding a match
                return new List<MappingNode>();
            }
        }
    }
}
