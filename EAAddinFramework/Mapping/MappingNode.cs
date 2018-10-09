using System;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;

namespace EAAddinFramework.Mapping
{
    public abstract class MappingNode : MP.MappingNode
    {
        protected TSF_EA.Element _source;
        protected MappingNode(UML.Classes.Kernel.NamedElement source, MappingNode parent, MappingSettings settings, MP.ModelStructure structure)
        {
            this.source = source;
            this.parent = parent;
            this.settings = settings;
            this.parent?.addChildNode(this);
            this.structure = structure;
        }
        public string name
        {
            get { return this._source.name; }
        }
        public MappingSettings settings { get; set; }
        public List<string> getMappingPath()
        {
            if (this.parent == null) return new List<string>() { this.source.uniqueID };
            var path = ((MappingNode)this.parent).getMappingPath();
            path.Add(this.source.uniqueID);
            return path;
        }
        public MappingNode createMappingNode(List<string> mappingPath)
        {
            //if the path is empty we return this node
            if (!mappingPath.Any()) return this;
            //check if the first node corresponds to this node
            if (mappingPath[0] == this.source.uniqueID)
            {
                //pop the first node
                mappingPath.RemoveAt(0);
                //if this was the last guid then also return this
                if (!mappingPath.Any()) return this;
                //get the element corresponding to the (now) first guid.
                var subElement = this._source.model.getItemFromGUID(mappingPath[0]) as UML.Classes.Kernel.NamedElement;
                //TODO: check if subElement is actually somehow linked to this node?
                //check if a childNode for the given subElement is already present
                var childNode  = this.allChildNodes.FirstOrDefault(x => x.source.uniqueID == subElement.uniqueID) as MappingNode;
                //create new new node if not already present
                if (childNode == null) childNode = MappingFactory.createMappingNode(subElement, this, this.settings);
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
            get
            {
                return this._source as UML.Classes.Kernel.NamedElement;
            }
            set
            {
                this._source = (TSF_EA.Element)value;
            }
        }
        protected List<MappingNode> _allChildNodes = new List<MappingNode>();
        public IEnumerable<MP.MappingNode> allChildNodes
        {
            get
            {
                return _allChildNodes;
            }
            set
            {
                _allChildNodes = value.Cast<MappingNode>().ToList();
            }
        }
        
        public MP.MappingNode parent { get; set; }

        protected List<Mapping> _mappings = new List<Mapping>();
        public IEnumerable<MP.Mapping> mappings { get { return this._mappings; } }

        public MP.ModelStructure structure { get; set; }
        public IEnumerable<MP.MappingNode> mappedChildNodes
        {
            get
            {
                return this.allChildNodes.Where(x => x.isMapped);
            }
        }

        public IEnumerable<MP.MappingNode> childNodes
        {
            get => this.showAll ? this.allChildNodes: this.mappedChildNodes;
        }
        public bool showAll { get; set; }

        public bool isMapped
        {
            get
            {
                return this.mappings.Any() || this.mappedChildNodes.Any();
            }
        }

        public void addChildNode(MP.MappingNode childNode)
        {
            this._allChildNodes.Add((MappingNode)childNode);
        }

        public abstract IEnumerable<MP.Mapping> getOwnedMappings(MP.MappingNode targetRootNode);

        public void buildNodeTree()
        {
            //set immediate child nodes
            this.setChildNodes();
            //build deeper
            foreach (MappingNode childNode in this.allChildNodes)
            {
                childNode.buildNodeTree();
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

        public void addMapping(MP.Mapping mapping)
        {
            this._mappings.Add((Mapping)mapping);
        }

        public MP.Mapping mapTo(MP.MappingNode targetNode)
        {
            var mappingItem =  this.createMappingItem((MappingNode)targetNode);
            return MappingFactory.createMapping(mappingItem, this, (MappingNode)targetNode);
        }
        protected abstract UML.Extended.UMLItem createMappingItem(MappingNode targetNode);
        public UML.Profiles.TaggedValue createTaggedValueMappingItem(MappingNode targetNode)
        {
            var tagName = targetNode is AssociationMappingNode ?
                    this.settings.linkedAssociationTagName :
                    this.settings.linkedAttributeTagName;
            return ((TSF_EA.Element)this.source).addTaggedValue(tagName, targetNode.source);
        }
    }
}
