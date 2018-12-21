using System.Collections.Generic;
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

        public override bool isEmpty { get; set; } = false;

        protected override void saveMe()
        {
            //create XDocument 
            this.xdoc = new XDocument();
            var bodyNode = new XElement("mapping");
            this.xdoc.Add(bodyNode);
            bodyNode.Add(MappingLogic.getMappingLogicElement(this.EAMappingLogics));
            bodyNode.Add(new XElement(MappingFactory.isEmptyMappingName, this.isEmpty.ToString()));
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
