using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TSF.UmlToolingFramework.Wrappers.EA;
using MP = MappingFramework;

namespace EAAddinFramework.Mapping
{
    /// <summary>
    /// Description of TaggedValueMapping.
    /// </summary>
    public class TaggedValueMapping : Mapping
    {
        internal TaggedValue wrappedTaggedValue { get; private set; }
        private XDocument _xdoc = null;
        public TaggedValueMapping(TaggedValue wrappedTaggedValue, MappingNode source, MappingNode target) : base(source, target)
        {
            this.wrappedTaggedValue = wrappedTaggedValue;
            //make sure the mappingset is defined for all tagged value mappings.
            if (! this.hasMappingSetDefined
                && ! this.isReadOnly)
            {
                var temp = this.EAMappingLogics; //load mapping logics to make sure they get saved
                this.saveMe();
            }
        }
        private bool hasMappingSetDefined
        {
            get
            {
                return this.xdoc.Descendants(MappingFactory.mappingSetName).Any();
            }
        }
        private XDocument xdoc
        {
            get
            {
                if (this._xdoc == null)
                {
                    try
                    {
                        this._xdoc = XDocument.Load(new System.IO.StringReader(this.wrappedTaggedValue.comment));
                    }
                    catch (System.Xml.XmlException)
                    {
                        this._xdoc = new XDocument();
                    }
                }
                return this._xdoc;
            }
            set => this._xdoc = value;
        }

        #region implemented abstract members of Mapping

        private bool? _isEmpty;
        public override bool isEmpty
        {
            get
            {
                if (!this._isEmpty.HasValue)
                {
                    bool emptyResult;
                    if (bool.TryParse(xdoc.Descendants(MappingFactory.isEmptyMappingName).FirstOrDefault()?.Value, out emptyResult))
                    {
                        _isEmpty = emptyResult;
                    }
                    else
                    {
                        _isEmpty = false;
                    }
                }
                return _isEmpty.Value;
            }
            set
            {
                this._isEmpty = value;
            }
        }
        private bool? _isReverseEmpty = null;
        public override bool isReverseEmpty
        {
            get
            {
                if (!this._isReverseEmpty.HasValue)
                {
                    bool reverseEmptyResult;
                    if (bool.TryParse(xdoc.Descendants(MappingFactory.isReverseEmptyName).FirstOrDefault()?.Value, out reverseEmptyResult))
                    {
                        _isReverseEmpty = reverseEmptyResult;
                    }
                    else
                    {
                        _isReverseEmpty = false;
                    }
                }
                return _isReverseEmpty.Value;
            }
            set
            {
                this._isReverseEmpty = value;
            }
        }
        private MappingSet _mappingSet;
        public override MP.MappingSet mappingSet
        {
            get
            {
                return this._mappingSet;
            }
            set
            {
                this._mappingSet = (MappingSet) value;
            }
        }

        protected override void saveMe()
        {
            //create XDocument 
            this.xdoc = new XDocument();
            var bodyNode = new XElement("mapping");
            this.xdoc.Add(bodyNode);
            bodyNode.Add(MappingLogic.getMappingLogicElement(this.EAMappingLogics));
            //add mappingSet node
            if (this.mappingSet != null)
            {
                bodyNode.Add(new XElement(MappingFactory.mappingSetName, 
                    new XElement(MappingFactory.mappingSetSourceName ,this.mappingSet.source.source.uniqueID),
                    new XElement(MappingFactory.mappingSetTargetName, this.mappingSet.target.source.uniqueID)));
            }
            //add isEmpty Node
            bodyNode.Add(new XElement(MappingFactory.isEmptyMappingName, this.isEmpty.ToString()));
            //add isReverseEmptyNode
            bodyNode.Add(new XElement(MappingFactory.isReverseEmptyName, this.isReverseEmpty.ToString()));
            //set mapping path
            if (this.source.structure == MP.ModelStructure.Message || this.source.isVirtual)
            {
                bodyNode.Add(new XElement(MappingFactory.mappingSourcePathName, ((MappingNode)this.source).getMappingPathString()));
            }
            if (this.target.structure == MP.ModelStructure.Message || this.target.isVirtual)
            {
                bodyNode.Add(new XElement(MappingFactory.mappingTargetPathName, ((MappingNode)this.target).getMappingPathString()));
            }
            //set comment
            this.wrappedTaggedValue.comment = this.xdoc.ToString();
            this.wrappedTaggedValue.save();
        }

        public override void deleteWrappedItem()
        {
            this.wrappedTaggedValue?.delete();
        }

        protected override List<MappingLogic> loadMappingLogics()
        {
            var mappingLogicString = this.xdoc.ToString();
            return MappingLogic.getMappingLogicsFromString(mappingLogicString, this.wrappedTaggedValue.model);
        }

        #endregion
    }
}
