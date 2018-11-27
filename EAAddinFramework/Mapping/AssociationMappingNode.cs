using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.UML.Extended;
using MP = MappingFramework;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Mapping
{
    public class AssociationMappingNode : MappingNode
    {
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, MappingSettings settings, MP.ModelStructure structure) : this(sourceAssociation, null, settings, structure) { }
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, ClassifierMappingNode parent, MappingSettings settings, MP.ModelStructure structure) : this(sourceAssociation, parent, settings, structure, null) { }
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, ClassifierMappingNode parent, MappingSettings settings, MP.ModelStructure structure, UML.Classes.Kernel.NamedElement virtualOwner) : base(sourceAssociation, parent, settings, structure, virtualOwner) { }
        protected override List<UML.Profiles.TaggedValue> sourceTaggedValues => this.sourceAssociation?.taggedValues.ToList();

        internal TSF_EA.Association sourceAssociation
        {
            get => this.source as TSF_EA.Association;
            set => this.source = value;
        }
        public override string name
        {
            get
            {
                //check if target role is filled in
                if (!string.IsNullOrEmpty(this.sourceAssociation.targetEnd.name))
                {
                    return this.sourceAssociation.targetEnd.name;
                }
                //check if source association has a name
                if (!string.IsNullOrEmpty(this.sourceAssociation.name))
                {
                    return this.sourceAssociation.name;
                }
                //else just return the name of the associated class
                return this.sourceAssociation.targetName;
            }
        }
        public override string displayName
        {
            get
            {
                if (this.structure == MP.ModelStructure.DataModel)
                {
                    //check if target role is filled in
                    if (!string.IsNullOrEmpty(this.sourceAssociation.targetEnd.name))
                    {
                        return this.sourceAssociation.targetEnd.name + "." + this.sourceAssociation.targetName;
                    }
                    //check if source association has a name
                    else if (!string.IsNullOrEmpty(this.sourceAssociation.name))
                    {
                        return this.sourceAssociation.name + "." + this.sourceAssociation.targetName;
                    }
                }
                //default case
                return base.displayName;
            }
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

        protected override UMLItem createMappingItem(MappingNode targetNode)
        {
            return this.createTaggedValueMappingItem(targetNode);
        }
    }
}
