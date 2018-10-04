using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using MP = MappingFramework;

namespace EAAddinFramework.Mapping
{
    public class AttributeMappingNode : MappingNode
    {
        public AttributeMappingNode(TSF_EA.Attribute sourceAttribute, MappingSettings settings, MP.ModelStructure structure) : this(sourceAttribute, null, settings, structure) { }
        public AttributeMappingNode(TSF_EA.Attribute sourceAttribute, ClassifierMappingNode parent, MappingSettings settings, MP.ModelStructure structure) : base(sourceAttribute, parent, settings, structure) { }
        internal TSF_EA.Attribute sourceAttribute
        {
            get
            {
                return this.source as TSF_EA.Attribute;
            }
            set
            {
                this.source = value;
            }
        }
        public override IEnumerable<MP.Mapping> getOwnedMappings(MP.MappingNode targetRootNode)
        {
            var foundMappings = new List<MP.Mapping>();
            //Mappings are stored in tagged values
            foreach (var mappingTag in this.sourceAttribute.taggedValues.Where(x => x.name == this.settings.linkedAttributeTagName))
            {
                var mapping = MappingFactory.getMapping(this, (TSF_EA.TaggedValue)mappingTag, (MappingNode)targetRootNode);
                if (mapping != null) foundMappings.Add(mapping);
            }
            //TODO: link to element feature connectors
            //loop subNodes
            foreach (MappingNode childNode in this.allChildNodes)
            {
                foundMappings.AddRange(childNode.getOwnedMappings(targetRootNode));
            }
            return foundMappings;
        }

        public override void setChildNodes()
        {
            //the type of the attribute should be set as type
            var attributeType = this.sourceAttribute.type as TSF_EA.ElementWrapper;
            if (attributeType != null && ! this.allChildNodes.Any(x => x.source?.uniqueID == attributeType.uniqueID))
            {
                var childNode = new ClassifierMappingNode(attributeType, this, this.settings, this.structure);
            }
        }
    }
}
