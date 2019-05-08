using System;
using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.UML.Extended;
using TSF.UmlToolingFramework.UML.Profiles;
using MP = MappingFramework;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Mapping
{
    public class AttributeMappingNode : MappingNode
    {
        public AttributeMappingNode(TSF_EA.AttributeWrapper sourceAttribute, MappingSettings settings, MP.ModelStructure structure, bool isTarget)
            : this(sourceAttribute, null, settings, structure, isTarget) { }
        public AttributeMappingNode(TSF_EA.AttributeWrapper sourceAttribute, ElementMappingNode parent, MappingSettings settings, MP.ModelStructure structure, bool isTarget) 
            : this(sourceAttribute, parent, settings, structure, null, isTarget) { }
        public AttributeMappingNode(TSF_EA.AttributeWrapper sourceAttribute, ElementMappingNode parent, MappingSettings settings, MP.ModelStructure structure, UML.Classes.Kernel.NamedElement virtualOwner, bool isTarget) 
            : base((UML.Classes.Kernel.NamedElement)sourceAttribute, parent, settings, structure, virtualOwner, isTarget) { }
        public override IEnumerable<MP.Mapping> subClassMappings
        {
            get
            {
                var foundMappings = new List<MP.Mapping>();
                var parent = this.parent.source as TSF_EA.ElementWrapper;
                //exit if no parent found
                if (parent == null) return foundMappings;
                //get the subclasses
                var allSubClasses = parent.allSubClasses;
                //exit if there are no subclasses
                if (!allSubClasses.Any()) return foundMappings;
                //get all other mappings to/from this attribute
                var allAttributeMappings = this.isTarget ?
                        this.mappingSet.mappings.Where(x => x.target.source.uniqueID == this.source.uniqueID && x.target != this)
                        : this.mappingSet.mappings.Where(x => x.source.source.uniqueID == this.source.uniqueID && x.source != this);
                //loop mappings and find the ones whose parent is a subclass of this attributes parent
                foreach (var mapping in allAttributeMappings)
                {
                    var otherAttributeNode = this.isTarget ?
                                            (AttributeMappingNode)mapping.target
                                            : (AttributeMappingNode)mapping.source;
                    //check of parentNode of the other attribute node points to a subclass of this attribute's parent node
                    if (allSubClasses.Any(x => x.uniqueID == otherAttributeNode.parent.source.uniqueID))
                    {
                        //found one, return it.
                        foundMappings.Add(mapping);
                    }
                }
                return foundMappings;
            }
        }
        

        protected override List<TaggedValue> sourceTaggedValues
        {
            get => this.sourceAttribute?.taggedValues.ToList();
        }


        internal TSF_EA.AttributeWrapper sourceAttribute
        {
            get => this.source as TSF_EA.AttributeWrapper;
            set => this.source = value as UML.Classes.Kernel.NamedElement;
        }

        public override MP.MappingNode findNode(List<string> mappingPathNames)
        {
            var foundNode = base.findNode(mappingPathNames);
            //if the node is not found then we look further. It might be using the UN/CEFACT naming standard.
            //in those cases the name of a node is targetRole + target ElementName with all "_" removed
            if (foundNode == null
                && mappingPathNames.Count > 1)
            {
                if (this.name.Equals(mappingPathNames.FirstOrDefault(), StringComparison.InvariantCultureIgnoreCase)
                || (this.structure == MP.ModelStructure.Message
                    && this.name.Replace("_", string.Empty).Equals(mappingPathNames.FirstOrDefault()
                    , StringComparison.InvariantCultureIgnoreCase)))
                {
                    //remove the first name
                    var reducedPathNames = mappingPathNames.Where((v, i) => i != 0).ToList();
                    //loop child nodes skipping one level for the type of the attribute
                    foreach (var childNode in this.allChildNodes.FirstOrDefault()?.allChildNodes)
                    {
                        foundNode = childNode.findNode(reducedPathNames);
                        if (foundNode != null)
                        {
                            break;
                        }
                    }

                }
            }
            return foundNode;
        }
        public override void setChildNodes()
        {
            //only for message structures
            if (this.structure == MP.ModelStructure.Message)
            {
                //the type of the attribute should be set as type
                var attributeType = this.sourceAttribute.type as TSF_EA.ElementWrapper;
                if (! this.existAsParent(attributeType)
                    && attributeType != null && !this.allChildNodes.Any(x => x.source?.uniqueID == attributeType.uniqueID))
                {
                    var childNode = new ElementMappingNode(attributeType, this, this.settings, this.structure, this.isTarget);
                }
            }
        }
    }
}
