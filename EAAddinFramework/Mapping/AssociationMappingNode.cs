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
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, ElementMappingNode parent, MappingSettings settings, MP.ModelStructure structure) : this(sourceAssociation, parent, settings, structure, null) { }
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, ElementMappingNode parent, MappingSettings settings, MP.ModelStructure structure, UML.Classes.Kernel.NamedElement virtualOwner) : base(sourceAssociation, parent, settings, structure, virtualOwner) { }
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
                        return this.sourceAssociation.targetEnd.name + "." + this.sourceAssociation.target.name;
                    }
                    //check if source association has a name
                    else if (!string.IsNullOrEmpty(this.sourceAssociation.name))
                    {
                        return this.sourceAssociation.name + "." + this.sourceAssociation.target.name;
                    }
                }
                //default case
                return base.displayName;
            }
        }
        public override MP.MappingNode findNode(List<string> mappingPathNames)
        {
            var foundNode = base.findNode(mappingPathNames);
            //if the node is not found then we look further. It might be using the UN/CEFACT naming standard.
            //in those cases the name of a node is targetRole + target ElementName with all "_" removed
            if (this.structure == MP.ModelStructure.Message
                && foundNode == null 
                && mappingPathNames.Any())
            {
                var nameToFind = mappingPathNames.First();
                var unCefactName = (this.sourceAssociation.targetEnd.name + this.sourceAssociation.target.name).Replace("_", string.Empty);
                if (nameToFind.Equals(unCefactName,System.StringComparison.InvariantCultureIgnoreCase))
                {
                    if (mappingPathNames.Count == 1)
                    {
                        foundNode = this.allChildNodes.FirstOrDefault();
                    }
                    else
                    {
                        //remove the first name
                        var reducedPathNames = mappingPathNames.Where((v, i) => i != 0).ToList();
                        //loop child nodes
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
            }
            return foundNode;
        }



        public override void setChildNodes()
        {
            //we only traverse associations in case of message structrure
            if (this.structure == MP.ModelStructure.Message)
            {
                //create mapping node for target element
                var targetElement = this.sourceAssociation.targetElement as TSF_EA.ElementWrapper;
                if (!existAsParent(targetElement)
                    && targetElement != null && !this.allChildNodes.Any(x => x.source?.uniqueID == targetElement.uniqueID))
                {
                    var childNode = new ElementMappingNode(targetElement, this, this.settings, this.structure);
                }
            }
        }


    }
}
