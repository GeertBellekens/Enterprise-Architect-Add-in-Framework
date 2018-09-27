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
        protected MappingNode(UML.Classes.Kernel.NamedElement source, MappingNode parent, MappingSettings settings)
        {
            this.source = source;
            this.parent = parent;
            this.settings = settings;
            this.parent?.addChildNode(this);
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
                var childNode = MappingFactory.createMappingNode(subElement, this, this.settings);
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
        protected List<MappingNode> _childNodes = new List<MappingNode>();
        public IEnumerable<MP.MappingNode> childNodes
        {
            get
            {
                return _childNodes;
            }
            set
            {
                _childNodes = value.Cast<MappingNode>().ToList();
            }
        }
        public MP.MappingNode parent { get; set; }
        public void addChildNode(MP.MappingNode childNode)
        {
            this._childNodes.Add((MappingNode)childNode);
        }

        public abstract IEnumerable<MP.Mapping> getOwnedMappings(MP.MappingNode targetRootNode);

        public void buildNodeTree()
        {
            //set immediate child nodes
            this.setChildNodes();
            //build deeper
            foreach (MappingNode childNode in this.childNodes)
            {
                childNode.buildNodeTree();
            }
        }
        protected abstract void setChildNodes();

        public IEnumerable<MP.Mapping> getMappings(MP.MappingNode targetRootNode)
        {
            //first build tree node
            this.buildNodeTree();
            //then get mappings
            return this.getOwnedMappings(targetRootNode);
        }

        //public IEnumerable<Mapping> getOwnedMappings(MappingModel targetMappingModel)
        //{
        //    //List<Mapping> returnedMappings = new List<Mapping>();
        //    ////connectors to an attribute
        //    //foreach (ConnectorWrapper mappedConnector in this.source.relationships.OfType<ConnectorWrapper>()
        //    //         .Where(y => y.targetElement is AttributeWrapper))
        //    //{
        //    //    //create a mappingNode
        //    //    var connectorNode = new 
        //    //    //figure out if the mapping is actually going to the targetMappingModel

        //    //    string connectorPath = basepath + "." + getConnectorString(mappedConnector);
        //    //    var connectorMapping = new ConnectorMapping(mappedConnector, connectorPath, targetRootElement);
        //    //    returnedMappings.Add(connectorMapping);
        //    //}
        //    ////loop owned attributes
        //    //foreach (TSF.UmlToolingFramework.Wrappers.EA.Attribute ownedAttribute in ownerElement.ownedAttributes)
        //    //{
        //    //    returnedMappings.AddRange(createNewMappings(ownedAttribute, basepath, targetRootElement));
        //    //}
        //    ////loop owned Elements
        //    //{
        //    //    foreach (var ownedElement in ownerElement.ownedElements.OfType<ElementWrapper>())
        //    //    {
        //    //        returnedMappings.AddRange(createOwnedMappings(ownedElement, basepath + "." + ownedElement.name, targetRootElement));
        //    //    }
        //    //}
        //    //return returnedMappings;
        //}
    }
}
