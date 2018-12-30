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

        public ElementMappingNode(TSF_EA.ElementWrapper sourceElement, MappingSettings settings, MP.ModelStructure structure) : this(sourceElement, null, settings, structure) { }
        public ElementMappingNode(TSF_EA.ElementWrapper sourceElement, MappingNode parent, MappingSettings settings, MP.ModelStructure structure) : this(sourceElement, parent, settings, structure, null) { }
        public ElementMappingNode(TSF_EA.ElementWrapper sourceElement, MappingNode parent, MappingSettings settings, MP.ModelStructure structure, UML.Classes.Kernel.NamedElement virtualOwner) : base(sourceElement, parent, settings, structure, virtualOwner) { }
        protected override List<TaggedValue> sourceTaggedValues
        {
            get
            {
                //first check if the are any relevant tagged Values defined here to improve performance
                var sqlExistTaggedValues = "select tv.PropertyID from t_objectproperties tv " +
                                           $" where tv.Object_ID = {this.sourceElement.id} " +
                                           $" and tv.Property in ('{this.settings.linkedAssociationTagName}', '{this.settings.linkedAttributeTagName}', '{this.settings.linkedElementTagName}')";
                var queryResult = this.sourceElement.EAModel.SQLQuery(sqlExistTaggedValues);
                if (queryResult.SelectSingleNode(this.sourceElement.EAModel.formatXPath("//PropertyID")) != null)
                {
                    return this.sourceElement?.taggedValues.ToList();
                }
                else
                {
                    return new List<TaggedValue>();
                }
            }
        }

        internal TSF_EA.ElementWrapper sourceElement
        {
            get => this.source as TSF_EA.ElementWrapper;
            set => this.source = value;
        }

        public override IEnumerable<MP.Mapping> getOwnedMappings(MP.MappingNode targetRootNode)
        {
            var foundMappings = new List<MP.Mapping>();
            //get the mappings using trace relations
            //TODO: is this fast enough?
            foreach (var trace in this.sourceElement.getRelationships<TSF_EA.Abstraction>(true, false).Where(x => x.target is TSF_EA.Element && x.stereotypes.Any(y => y.name == "trace")))
            {
                //get the mappings based on traces
                var mapping = MappingFactory.getMapping(this, trace, (MappingNode)targetRootNode);
                if (mapping != null)
                {
                    foundMappings.Add(mapping);
                }
            }
            //also add the base mappings
            foundMappings.AddRange(base.getOwnedMappings(targetRootNode));
            return foundMappings;
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
                if (!existAsParent(ownedAttribute)
                    && !this.allChildNodes.Any(x => x.source?.uniqueID == ownedAttribute.uniqueID))
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
                    if (!existAsParent(enumLiteral)
                        && !this.allChildNodes.Any(x => x.source?.uniqueID == enumLiteral.uniqueID))
                    {
                        var childNode = new AttributeMappingNode((TSF_EA.AttributeWrapper)enumLiteral, this, this.settings, this.structure, virtualElement);
                    }
                }
            }
            //create child nodes for each owned element
            foreach (var ownedClassifier in element.ownedElements.OfType<TSF_EA.ElementWrapper>()
                                    .Where(x => x.EAElementType == "Class"
                                            || x.EAElementType == "Enumeration"
                                            || x.EAElementType == "DataType"
                                            || x.EAElementType == "Package"))
            {
                if (!existAsParent(ownedClassifier)
                    && !this.allChildNodes.Any(x => x.source?.uniqueID == ownedClassifier.uniqueID))
                {
                    var childNode = new ElementMappingNode(ownedClassifier, this, this.settings, this.structure, virtualElement);
                }
            }
            //create child nodes for each owned association
            foreach (var ownedAssociation in element.getRelationships<TSF_EA.Association>(true, false))
            {
                if (! existAsParent(ownedAssociation)
                    &&! this.allChildNodes.Any(x => x.source?.uniqueID == ownedAssociation.uniqueID))
                {
                    var childNode = new AssociationMappingNode(ownedAssociation, this, this.settings, this.structure, virtualElement);
                }
            }

            //do the same for all superclasses
            foreach (var superClass in element.superClasses)
            {
                this.addElementPropertiesToChildNodes((TSF_EA.ElementWrapper)superClass);
            }
        }
        public override MP.MappingNode findNode(List<string> mappingPathNames)
        {
            var foundNode = base.findNode(mappingPathNames);
            if (foundNode != null)
            {
                return foundNode;
            }
            //if not found then it might be a Class.attribute reference (or Class.Association)
            //So if there are only two names in the mapping path, AND the source node is a package,
            //we check if we can find a class with the given name in the package or sub-packages
            if (mappingPathNames.Any() 
                && mappingPathNames.Count <= 2 
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
                        foundNode = nodePath.Last().findNode(mappingPathNames);
                        if (foundNode != null)
                        {
                            return foundNode;
                        }
                    }
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
