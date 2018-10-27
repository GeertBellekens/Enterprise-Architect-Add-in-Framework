using EAAddinFramework.Utilities;
using System;
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
        public TaggedValueMapping(TaggedValue wrappedTaggedValue, MappingNode source, MappingNode target) : base(source, target)
        {
            this.wrappedTaggedValue = wrappedTaggedValue;
        }

        #region implemented abstract members of Mapping
        public override MP.MappingLogic mappingLogic
        {
            get
            {
                if (this._mappingLogic == null)
                {
                    Guid mappingElementGUID;
                    string mappingLogicString = "";
                    if (this.wrappedTaggedValue != null)
                    {
                        mappingLogicString = KeyValuePairsHelper.getValueForKey(MappingFactory.mappingLogicName, this.wrappedTaggedValue.comment);
                    }
                    if (Guid.TryParse(mappingLogicString, out mappingElementGUID))
                    {
                        ElementWrapper mappingLogicElement = this.wrappedTaggedValue.model.getElementByGUID(mappingLogicString) as ElementWrapper;
                        if (mappingLogicElement != null)
                        {
                            this._mappingLogic = new MappingLogic(mappingLogicElement);
                        }
                    }
                    if (this._mappingLogic == null && !string.IsNullOrEmpty(mappingLogicString))
                    {
                        this._mappingLogic = new MappingLogic(mappingLogicString);
                    }
                }
                return this._mappingLogic;
            }
            set
            {
                string logicString = value.description;
                if (value.mappingElement != null)
                {
                    logicString = value.mappingElement.uniqueID;
                }

                this.wrappedTaggedValue.comment = KeyValuePairsHelper.setValueForKey(MappingFactory.mappingLogicName, logicString, this.wrappedTaggedValue.comment);
            }
        }

        private bool? _isEmpty = null;
        public override bool isEmpty
        {
            get
            {
                if (!this._isEmpty.HasValue)
                {
                    if (this.wrappedTaggedValue != null)
                    {
                        var isEmptyString = KeyValuePairsHelper.getValueForKey(MappingFactory.isEmptyMappingName, this.wrappedTaggedValue.comment);
                        this._isEmpty = "True".Equals(isEmptyString, StringComparison.InvariantCultureIgnoreCase);
                    }
                }
                return this._isEmpty.Value;
            }
            set
            {
                this.wrappedTaggedValue.comment = KeyValuePairsHelper.setValueForKey(MappingFactory.isEmptyMappingName, value.ToString(), this.wrappedTaggedValue.comment);
            }
        }

        public override void save()
        {
            if (this._mappingLogic != null)
            {
                this.mappingLogic = this._mappingLogic; //make sure to set the mapping logic value correctly
            }
            //set mapping path
            if (this.source.structure == MP.ModelStructure.Message || this.source.isVirtual)
            {
                this.wrappedTaggedValue.comment = KeyValuePairsHelper.setValueForKey(MappingFactory.mappingSourcePathName, ((MappingNode)this.source).getMappingPathString(), this.wrappedTaggedValue.comment);
            }
            if (this.target.structure == MP.ModelStructure.Message || this.target.isVirtual)
            {
                this.wrappedTaggedValue.comment = KeyValuePairsHelper.setValueForKey(MappingFactory.mappingTargetPathName, ((MappingNode)this.target).getMappingPathString(), this.wrappedTaggedValue.comment);
            }
            this.wrappedTaggedValue.save();
        }

        public override void deleteWrappedItem()
        {
            this.wrappedTaggedValue?.delete();
        }

        #endregion
    }
}
