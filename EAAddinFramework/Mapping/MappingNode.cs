using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using MP = MappingFramework;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Mapping
{
    public abstract class MappingNode : MP.MappingNode
    {
        protected TSF_EA.Element _source;
        protected TSF_EA.Element _virtualOwner;
        protected MappingNode(NamedElement source, MappingNode parent, MappingSettings settings, MP.ModelStructure structure, NamedElement virtualOwner)
        {
            this.source = source;
            this.parent = parent;
            this.settings = settings;
            this.parent?.addChildNode(this);
            this.structure = structure;
            this.virtualOwner = virtualOwner;
        }

        public virtual string name => this._source.name;
        public MappingSettings settings { get; set; }
        private List<string> _mappingPath;
        public List<string> getMappingPath()
        {
            if (this._mappingPath == null)
            {
                if (this.parent == null)
                {
                    this._mappingPath = new List<string>() { this.source.uniqueID };
                }
                else
                {
                    this._mappingPath = new List<string>( ((MappingNode)this.parent).getMappingPath());
                    this._mappingPath.Add(this.source.uniqueID);
                }
            }
            return this._mappingPath;
        }
        public string getMappingPathString()
        {
            return string.Join(".", this.getMappingPath());
        }
        public MappingNode createMappingNode(List<string> mappingPath)
        {
            //if the path is empty we return this node
            if (!mappingPath.Any())
            {
                return this;
            }
            //check if the first node corresponds to this node
            if (mappingPath[0] == this.source.uniqueID)
            {
                //pop the first node
                mappingPath.RemoveAt(0);
                //if this was the last guid then also return this
                if (!mappingPath.Any())
                {
                    return this;
                }
                //get the element corresponding to the (now) first guid.
                var subElement = this._source.model.getItemFromGUID(mappingPath[0]) as UML.Classes.Kernel.NamedElement;
                //TODO: check if subElement is actually somehow linked to this node?
                //check if a childNode for the given subElement is already present
                var childNode = this.allChildNodes.FirstOrDefault(x => x.source.uniqueID == subElement.uniqueID) as MappingNode;
                //create new new node if not already present
                if (childNode == null)
                {
                    childNode = MappingFactory.createMappingNode(subElement, this, this.settings);
                }

                return childNode?.createMappingNode(mappingPath);
            }
            else
            {
                //the first GUID indicates that it does not below in this node
                return null;
            }
        }

        public UML.Classes.Kernel.NamedElement source
        {
            get => this._source as UML.Classes.Kernel.NamedElement;
            set => this._source = (TSF_EA.Element)value;
        }

        protected List<MappingNode> _allChildNodes = new List<MappingNode>();
        public IEnumerable<MP.MappingNode> allChildNodes
        {
            get => this._allChildNodes;
            set => this._allChildNodes = value.Cast<MappingNode>().ToList();
        }

        public MP.MappingNode parent { get; set; }

        protected List<Mapping> _mappings = new List<Mapping>();
        public IEnumerable<MP.Mapping> mappings => this._mappings;

        public MP.ModelStructure structure { get; set; }
        public IEnumerable<MP.MappingNode> mappedChildNodes => this.allChildNodes.Where(x => x.isMapped);

        public IEnumerable<MP.MappingNode> childNodes => this.showAll ? this.allChildNodes : this.mappedChildNodes;
        public bool showAll { get; set; }

        public bool isMapped => this.mappings.Any() || this.mappedChildNodes.Any();

        public void addChildNode(MP.MappingNode childNode)
        {
            this._allChildNodes.Add((MappingNode)childNode);
        }

        public virtual IEnumerable<MP.Mapping> getOwnedMappings(MP.MappingNode targetRootNode)
        {
            var foundMappings = new List<MP.Mapping>();
            //Mappings are stored in tagged values
            foreach (var mappingTag in this.sourceTaggedValues.Where(x => x.name == this.settings.linkedAttributeTagName
                                                                                 || x.name == this.settings.linkedAssociationTagName
                                                                                 || x.name == this.settings.linkedElementTagName))
            {
                var mapping = MappingFactory.getMapping(this, (TSF_EA.TaggedValue)mappingTag, (MappingNode)targetRootNode);
                if (mapping != null) foundMappings.Add(mapping);
            }
            //loop subnodes
            foreach (MappingNode childNode in this.allChildNodes)
            {
                foundMappings.AddRange(childNode.getOwnedMappings(targetRootNode));
            }
            return foundMappings;
        }

        public void buildNodeTree()
        {
            //set immediate child nodes
            this.setChildNodes();
            //build deeper
            foreach (MappingNode childNode in this.allChildNodes)
            {
                //check if childnode is not already somewhere in the parents to avoid infinite loops
                if (!this.getMappingPath().Any(x => x == childNode.source.uniqueID))
                {
                    childNode.buildNodeTree();
                }
            }
        }
        public abstract void setChildNodes();

        public IEnumerable<MP.Mapping> getMappings(MP.MappingNode targetRootNode)
        {
            //first build tree node
            this.buildNodeTree();
            //then get mappings
            return this.getOwnedMappings(targetRootNode);
        }
        protected abstract List<UML.Profiles.TaggedValue> sourceTaggedValues { get; }
        public NamedElement virtualOwner
        {
            get => this._virtualOwner as NamedElement;
            set => this._virtualOwner = (TSF_EA.Element)value;
        }

        public bool isVirtual => this.virtualOwner != null;

        public void addMapping(MP.Mapping mapping)
        {
            this._mappings.Add((Mapping)mapping);
        }

        public void removeMapping(MP.Mapping mapping)
        {
            this._mappings.Remove((Mapping)mapping);
        }

        public MP.Mapping mapTo(MP.MappingNode targetNode)
        {
            var mappingItem = this.createMappingItem((MappingNode)targetNode);
            var mapping = MappingFactory.createMapping(mappingItem, this, (MappingNode)targetNode);
            mapping.save();
            return mapping;
        }
        protected abstract UML.Extended.UMLItem createMappingItem(MappingNode targetNode);
        public UML.Profiles.TaggedValue createTaggedValueMappingItem(MappingNode targetNode)
        {
            //get the appropriate tagname
            string tagName;
            if (targetNode is AssociationMappingNode)
                tagName = this.settings.linkedAssociationTagName;
            else if (targetNode is AttributeMappingNode)
                tagName = this.settings.linkedAttributeTagName;
            else
                tagName = this.settings.linkedElementTagName;
            //add the tagged value;
            return ((TSF_EA.Element)this.source).addTaggedValue(tagName, targetNode.source, null, true);
        }

        public bool isChildOf(MP.MappingNode parentNode)
        {
            return this.parent != null &&
                ( parentNode == this.parent || this.parent.isChildOf(parentNode));
        }
    }
}
