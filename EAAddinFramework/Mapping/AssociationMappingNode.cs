using System.Collections.Generic;
using System.Linq;
using MP = MappingFramework;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Mapping
{
    public class AssociationMappingNode : MappingNode
    {
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, MappingSettings settings, MP.ModelStructure structure, bool isTarget)
            : this(sourceAssociation, null, settings, structure, isTarget) { }
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, ElementMappingNode parent, MappingSettings settings, MP.ModelStructure structure, bool isTarget)
            : this(sourceAssociation, parent, settings, structure, null, isTarget) { }
        public AssociationMappingNode(TSF_EA.Association sourceAssociation, ElementMappingNode parent, MappingSettings settings, MP.ModelStructure structure, UML.Classes.Kernel.NamedElement virtualOwner, bool isTarget)
            : base(sourceAssociation, parent, settings, structure, virtualOwner, isTarget) { }
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

        public override IEnumerable<MP.Mapping> subClassMappings
        {
            get
            {
                var foundMappings = new List<MP.Mapping>();
                var parent = this.parent.source as TSF_EA.ElementWrapper;
                //exit if no parent found
                if (parent == null)
                {
                    return foundMappings;
                }
                //get the subclasses
                var allSubClasses = parent.allSubClasses;
                //exit if there are no subclasses
                if (!allSubClasses.Any())
                {
                    return foundMappings;
                }
                //get all other mappings to/from this association
                var allAssociationMappings = this.isTarget ?
                        this.mappingSet.mappings.Where(x => x.target.source.uniqueID == this.source.uniqueID && x.target != this)
                        : this.mappingSet.mappings.Where(x => x.source.source.uniqueID == this.source.uniqueID && x.source != this);
                //loop mappings and find the ones whose parent is a subclass of this attributes parent
                foreach (var mapping in allAssociationMappings)
                {
                    var otherAssociationNode = this.isTarget ?
                                            (AssociationMappingNode)mapping.target
                                            : (AssociationMappingNode)mapping.source;
                    //check of parentNode of the other associaton node points to a subclass of this association's parent node
                    if (allSubClasses.Any(x => x.uniqueID == otherAssociationNode.parent.source.uniqueID))
                    {
                        //found one, return it.
                        foundMappings.Add(mapping);
                    }
                }
                return foundMappings;
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
                if (nameToFind.Equals(unCefactName, System.StringComparison.InvariantCultureIgnoreCase))
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
            if (this.structure == MP.ModelStructure.DataModel)
            {
                //it could also be that the mappingPath for an association is expressed as Class.RelatedClass, omitting the name of the association.
                // so if this association node has childnodes with the given then then we return this node
                if (mappingPathNames.Count == 1)
                {
                    foreach (var childNode in this.allChildNodes)
                    {
                        foundNode = childNode.findNode(mappingPathNames);
                        if (foundNode != null)
                        {
                            break;
                        }
                    }
                    //if the end class of the association is not loaded as a node, we check the target class name
                    if (this.sourceAssociation.targetElement.name == mappingPathNames[0])
                    {
                        foundNode = this;
                    }
                    //if we can't find it as the name of the target element we check the subclasses of the target element
                    if (((TSF_EA.ElementWrapper)this.sourceAssociation.targetElement).subClasses.Any(x => x.name == mappingPathNames[0]))
                    {
                        foundNode = this;
                    }
                }
                else if (mappingPathNames.Count == 2)
                {
                    //if the end class of the association is not loaded as a node, we check the target class name
                    if (this.sourceAssociation.targetElement.name == mappingPathNames[0]
                        && this.sourceAssociation.targetElement.ownedElements
                                        .OfType<UML.Classes.Kernel.Property>()
                                        .Any(x => x.name == mappingPathNames[1]))
                    {
                        foundNode = this;
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
                if (!this.existAsParent(targetElement)
                    && targetElement != null && !this.allChildNodes.Any(x => x.source?.uniqueID == targetElement.uniqueID))
                {
                    var childNode = new ElementMappingNode(targetElement, this, this.settings, this.structure, this.isTarget);
                }
            }
        }


    }
}
