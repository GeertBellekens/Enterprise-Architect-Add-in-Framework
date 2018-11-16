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
        public AttributeMappingNode(TSF_EA.AttributeWrapper sourceAttribute, MappingSettings settings, MP.ModelStructure structure) : this(sourceAttribute, null, settings, structure) { }
        public AttributeMappingNode(TSF_EA.AttributeWrapper sourceAttribute, ClassifierMappingNode parent, MappingSettings settings, MP.ModelStructure structure) : this(sourceAttribute, parent, settings, structure, null) { }
        public AttributeMappingNode(TSF_EA.AttributeWrapper sourceAttribute, ClassifierMappingNode parent, MappingSettings settings, MP.ModelStructure structure, UML.Classes.Kernel.NamedElement virtualOwner) : base((UML.Classes.Kernel.NamedElement)sourceAttribute, parent, settings, structure, virtualOwner) { }

        protected override List<TaggedValue> sourceTaggedValues => this.sourceAttribute?.taggedValues.ToList();


        internal TSF_EA.AttributeWrapper sourceAttribute
        {
            get => this.source as TSF_EA.AttributeWrapper;
            set => this.source = value as UML.Classes.Kernel.NamedElement;
        }


        public override void setChildNodes()
        {
            //only for message structures
            if (this.structure == MP.ModelStructure.Message)
            {
                //the type of the attribute should be set as type
                var attributeType = this.sourceAttribute.type as TSF_EA.ElementWrapper;
                if (attributeType != null && !this.allChildNodes.Any(x => x.source?.uniqueID == attributeType.uniqueID))
                {
                    var childNode = new ClassifierMappingNode(attributeType, this, this.settings, this.structure);
                }
            }
        }

        protected override UMLItem createMappingItem(MappingNode targetNode)
        {
            return this.createTaggedValueMappingItem(targetNode);
        }
    }
}
