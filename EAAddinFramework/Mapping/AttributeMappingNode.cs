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
        public AttributeMappingNode(TSF_EA.Attribute sourceAttribute) : this(sourceAttribute, null) { }
        public AttributeMappingNode(TSF_EA.Attribute sourceAttribute, ClassifierMappingNode parent) : base(sourceAttribute, parent) { }
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
        public override IEnumerable<MP.Mapping> getMappings(MP.MappingNode targetRootNode)
        {
            //TODO
            throw new NotImplementedException();
        }

        protected override void setChildNodes()
        {
            //the type of the attribute should be set as type
            var attributeType = this.sourceAttribute.type as TSF_EA.ElementWrapper;
            if (attributeType != null)
            {
                var childNode = new ClassifierMappingNode(attributeType, this);
            }
        }
    }
}
