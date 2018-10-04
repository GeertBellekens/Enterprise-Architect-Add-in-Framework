﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MP = MappingFramework;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Mapping
{
    public class AssociationMappingNode : MappingNode
    {
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, MappingSettings settings, MP.ModelStructure structure) : this(sourceAssociation, null, settings, structure) { }
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, ClassifierMappingNode parent, MappingSettings settings, MP.ModelStructure structure) : base(sourceAssociation, parent, settings, structure) { }

        internal TSF_EA.Association sourceAssociation
        {
            get
            {
                return this.source as TSF_EA.Association;
            }
            set
            {
                this.source = value;
            }
        }

        public override IEnumerable<MP.Mapping> getOwnedMappings(MP.MappingNode targetRootNode)
        {
            //the mappings are stored in a tagged value
            var foundMappings = new List<MP.Mapping>();
            //Mappings are stored in tagged values
            foreach (var mappingTag in this.sourceAssociation.taggedValues.Where(x => x.name == this.settings.linkedAttributeTagName))
            {
                var mapping = MappingFactory.getMapping(this, (TSF_EA.TaggedValue)mappingTag, (MappingNode)targetRootNode);
                if (mapping != null) foundMappings.Add(mapping);
            }
            //loop subNodes
            foreach (MappingNode childNode in this.allChildNodes)
            {
                foundMappings.AddRange(childNode.getOwnedMappings(targetRootNode));
            }
            return foundMappings;
        }

        public override void setChildNodes()
        {
            //we only traverse associations in case of message structrure
            if (this.structure == MP.ModelStructure.Message)
            {
                //create mapping node for target element
                var targetElement = this.sourceAssociation.targetElement as TSF_EA.ElementWrapper;
                if (targetElement != null && !this.allChildNodes.Any(x => x.source?.uniqueID == targetElement.uniqueID))
                {
                    var childNode = new ClassifierMappingNode(targetElement, this, this.settings, this.structure);
                }
            }
        }
    }
}
