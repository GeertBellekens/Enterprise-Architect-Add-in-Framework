using EAAddinFramework.Utilities;
using System;
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
        protected MappingNode(NamedElement source, MappingNode parent, MappingSettings settings, MP.ModelStructure structure, NamedElement virtualOwner, bool isTarget)
        {
            this.source = source;
            this.parent = parent;
            this.mappingSet = parent?.mappingSet;
            this.settings = settings;
            this.parent?.addChildNode(this);
            this.structure = structure;
            this.virtualOwner = virtualOwner;
            this.isTarget = isTarget;
        }
        public TSF_EA.Model model => ((TSF_EA.Element)this.source)?.EAModel;
        public virtual string name => this._source?.name;
        public virtual string displayName => this.name;
        public string displayMappingCount
        {
            get
            {
                var subClassMappingCount = this.subClassMappings.Count();
                return subClassMappingCount > 0 ?
                    $"{this.mappingCount} ({subClassMappingCount})"
                    : $"{this.mappingCount}";
            }
            
        }
        public MappingSettings settings { get; set; }
        private List<string> _mappingPath;
        public List<string> mappingPath
        {
            get
            {
                if (this._mappingPath == null)
                {
                    if (this.parent == null)
                    {
                        this._mappingPath = new List<string>() { this.source.uniqueID };
                    }
                    else
                    {
                        this._mappingPath = new List<string>(((MappingNode)this.parent).mappingPath);
                        this._mappingPath.Add(this.source.uniqueID);
                    }
                }
                return this._mappingPath;
            }
        }
        public string getMappingPathString()
        {
            return string.Join(".", this.mappingPath);
        }
        protected bool existAsParent(UML.Extended.UMLItem umlItem)
        {
            return umlItem != null 
                && ((MappingNode)this.parent)?.mappingPath.Contains(umlItem.uniqueID) == true;
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
                
                //check if a childNode for the given subElement is already present
                var childNode = this.allChildNodes.FirstOrDefault(x => x.source.uniqueID == subElement?.uniqueID) as MappingNode;
                //create new new node if not already present
                if (childNode == null)
                {
                    //TODO: check if subElement is actually somehow linked to this node?
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

        protected List<Mapping> _mappings;
        public IEnumerable<MP.Mapping> mappings
        {
            get
            {
                if (this._mappings == null)
                {
                    return new List<Mapping>();
                }
                return this._mappings.Where(x => x.source == this && !x.isReverseEmpty || x.target == this && !(x.isEmpty && !x.isReverseEmpty));
            }
        }
            
        public int mappingCount => this.mappings.Count();
        public abstract IEnumerable<MP.Mapping> subClassMappings { get; }

        public MP.ModelStructure structure { get; set; }
        public IEnumerable<MP.MappingNode> mappedChildNodes => this.allChildNodes.Where(x => x.isMapped);

        public IEnumerable<MP.MappingNode> childNodes => this.showAll ? this.allChildNodes : this.mappedChildNodes;
        public bool showAll { get; set; }

        public bool isMapped => this.mappings.Any() || this.mappedChildNodes.Any();

        public void addChildNode(MP.MappingNode childNode)
        {
            this._allChildNodes.Add((MappingNode)childNode);
        }

        public virtual IEnumerable<MP.Mapping> getOwnedMappings()
        {
            //get my mappings
            var foundMappings = this.getMyMappings().ToList();
            //loop subnodes
            foreach (MappingNode childNode in this.allChildNodes)
            {
                foundMappings.AddRange(childNode.getOwnedMappings());
            }
            return foundMappings;
        }

        public void buildNodeTree()
        {
            //log progress
            //EAOutputLogger.log($"Loading '{this.name}'");
            //set immediate child nodes
            this.setChildNodes();
            //build deeper
            foreach (MappingNode childNode in this.allChildNodes)
            {
                //check if childnode is not already somewhere in the parents to avoid infinite loops
                if (!this.isChildOf(childNode.source.uniqueID))
                {
                    childNode.buildNodeTree();
                }
            }
        }
        public abstract void setChildNodes();

        public IEnumerable<MP.Mapping> getMappings()
        {
            //first build tree node
            this.buildNodeTree();
            //then get mappings
            return this.getOwnedMappings();
        }
        public IEnumerable<MP.Mapping> getMyMappings( )
        {
            if (this._mappings == null)
            {
                this._mappings = new List<Mapping>();

                var targetRootNode = this.mappingSet.target;
                //clear mappings before starting

                var foundMappings = new List<MP.Mapping>();
                //Mappings are stored in tagged values
                foreach (var mappingTag in this.sourceTaggedValues.Where(x => x.name == this.settings.linkedAttributeTagName
                                                                                     || x.name == this.settings.linkedAssociationTagName
                                                                                     || x.name == this.settings.linkedElementTagName))
                {
                    var mapping = MappingFactory.getMapping(this, (TSF_EA.TaggedValue)mappingTag, (MappingNode)targetRootNode);
                    if (mapping != null) foundMappings.Add(mapping);
                }
                return foundMappings;
            }
            else
            {
                return this._mappings;
            }
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
            if (this._mappings == null)
            {
                this._mappings = new List<Mapping>();
            }
            this._mappings.Add((Mapping)mapping);
            //also add to mappingSet if this is a sourceNode
            if(! this.isTarget)
            {
                this.mappingSet.addMapping(mapping);
            }
        }

        public void removeMapping(MP.Mapping mapping)
        {
            if (this._mappings != null)
            {
                this._mappings.Remove((Mapping)mapping);
            }
        }
        public bool isReadOnly => this.source != null ? this.source.isReadOnly : false;

        public MP.MappingSet mappingSet { get; set; }
        public bool isTarget { get; set; }

        public MP.Mapping mapTo(MP.MappingNode targetNode)
        {
            //check if not already mapped
            var mapping = this.mappings.FirstOrDefault(x => x.target != null
                                                        && x.target.getMappingPathString() == targetNode?.getMappingPathString());
            if (mapping == null && targetNode.source != null)
            {
                //check if not read-only
                if (this.isReadOnly)
                {
                    throw new InvalidOperationException($"Element {this.source.name} is read-only");
                }
                var mappingItem = this.createTaggedValueMappingItem((MappingNode)targetNode);
                mapping = MappingFactory.createMapping(mappingItem, this, (MappingNode)targetNode);
                //save the mapping
                mapping.save();
            }
            return mapping;
        }

        public MP.Mapping createEmptyMapping(bool reverse)
        {
            MP.Mapping newMapping;
            if (reverse)
            {
                newMapping = this.mappingSet.source.mapTo(this);
            }
            else
            {
                newMapping = this.mapTo(this.mappingSet.target);
            }
            newMapping.isEmpty = true;
            newMapping.isReverseEmpty = reverse;
            newMapping.addMappingLogic(new MappingLogic("Empty mapping"));
            newMapping.save();
            //remove from target node
            this.mappingSet.target.removeMapping(newMapping);
            return newMapping;
        }

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
        public bool isChildOf(string uniqueID)
        {
            return this.parent != null &&
                (this.parent.source?.uniqueID == uniqueID || this.parent.isChildOf(uniqueID));
        }

        public string getMappingPathExportString()
        {
            if (this.parent == null)
                return this.name;
            else
                return ((MappingNode)this.parent).getMappingPathExportString() + "." + this.name;
        }

        public virtual MP.MappingNode findNode(List<string> mappingPathNames)
        {
            //first try to find the node based on the full set of mapping path names 
            //the mappingPathNames should start with this nodes name
            //check for UN/CEFACT convention in case of message structure
            if (this.name.Equals(mappingPathNames.FirstOrDefault(), StringComparison.InvariantCultureIgnoreCase)
                || (this.structure == MP.ModelStructure.Message 
                    && this.name.Replace("_", string.Empty).Equals(mappingPathNames.FirstOrDefault()
                    , StringComparison.InvariantCultureIgnoreCase)))
            {
                //if there is only one item then return this node
                if (mappingPathNames.Count == 1 )
                {
                    return this;
                }
                else
                {
                    //remove the first name
                    var reducedPathNames = mappingPathNames.Where((v, i) => i != 0).ToList();
                    //loop child nodes
                    foreach (var childNode in this.allChildNodes)
                    {
                        var foundNode = childNode.findNode(reducedPathNames);
                        if (foundNode != null )
                        {
                            return foundNode;
                        }
                    }
                }
            }
            //return null if nothing found
            return null;
        }
    }
}
