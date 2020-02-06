using System;
using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.Wrappers.EA;
using MP = MappingFramework;

namespace EAAddinFramework.Mapping
{
    /// <summary>
    /// Not used anymore. All mappings are stored as tagged values.
    /// </summary>
    public class ConnectorMapping : Mapping
    {
        internal ConnectorWrapper wrappedConnector { get; private set; }
        public ConnectorMapping(ConnectorWrapper wrappedConnector, MappingNode sourceEnd, MappingNode targetEnd) : base(sourceEnd, targetEnd)
        {
            this.wrappedConnector = wrappedConnector;
        }

        #region implemented abstract members of Mapping


        private bool? _isEmpty = null;
        public override bool isEmpty
        {
            get
            {
                if (!this._isEmpty.HasValue)
                {
                    var emptyTag = this.wrappedConnector.taggedValues
                        .FirstOrDefault(x => x.name.Equals(MappingFactory.isEmptyMappingName, StringComparison.InvariantCultureIgnoreCase));
                    return "True".Equals(emptyTag?.tagValue.ToString(), StringComparison.InvariantCultureIgnoreCase); 
                }
                return this._isEmpty.Value;
            }
            set
            {
                this.wrappedConnector.addTaggedValue(MappingFactory.isEmptyMappingName, value.ToString());
            }
        }
        private bool? _isReverseEmpty = null;
        public override bool isReverseEmpty
        {
            get
            {
                if (!this._isReverseEmpty.HasValue)
                {
                    var emptyTag = this.wrappedConnector.taggedValues
                        .FirstOrDefault(x => x.name.Equals(MappingFactory.isReverseEmptyName, StringComparison.InvariantCultureIgnoreCase));
                    return "True".Equals(emptyTag?.tagValue.ToString(), StringComparison.InvariantCultureIgnoreCase);
                }
                return this._isReverseEmpty.Value;
            }
            set
            {
                this.wrappedConnector.addTaggedValue(MappingFactory.isReverseEmptyName, value.ToString());
            }
        }

        public override MP.MappingSet mappingSet { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region implemented abstract members of Mapping

        protected override void saveMe()
        {

            var logicString = MappingLogic.getMappingLogicString(this.EAMappingLogics);
            addTaggedValueSafe(MappingFactory.mappingLogicName, logicString);
            
            //set mapping path
            if (this.source.structure == MP.ModelStructure.Message || this.source.isVirtual)
            {
                addTaggedValueSafe(MappingFactory.mappingSourcePathName, string.Join(".", ((MappingNode)this.source).mappingPath));
            }
            if (this.target.structure == MP.ModelStructure.Message || this.target.isVirtual)
            {
                addTaggedValueSafe(MappingFactory.mappingTargetPathName, string.Join(".", ((MappingNode)this.target).mappingPath));
            }
            this.wrappedConnector.save();
        }

        public override void deleteWrappedItem()
        {
            this.wrappedConnector?.delete();
        }
        private void addTaggedValueSafe(string tagName, string value)
        {
            value = value ?? string.Empty;//avoid nullpointer exceptions
            if (value.Length < 255)
            {
                this.wrappedConnector.addTaggedValue(tagName, value);
            }
            else
            {
                this.wrappedConnector.addTaggedValue(tagName, "<memo>", value);
            }
        }
        private string getTaggedValueSafe(string tagName)
        {
            //get the tag
            var tag = this.wrappedConnector.taggedValues
                        .FirstOrDefault(x => x.name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase));
            //return string value
            return tag?.tagValue?.ToString() == "<memo>" ? tag?.comment : tag?.tagValue?.ToString();
        }

        protected override List<MappingLogic> loadMappingLogics()
        {
            var logicString = this.getTaggedValueSafe(MappingFactory.mappingLogicName);
            return MappingLogic.getMappingLogicsFromString(logicString, this.wrappedConnector.EAModel);
        }

        #endregion
    }
}
