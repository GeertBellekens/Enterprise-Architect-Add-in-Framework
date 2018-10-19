using System;
using EAAddinFramework.Utilities;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;

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
                if (_mappingLogic == null)
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
                        if (mappingLogicElement != null) _mappingLogic = new MappingLogic(mappingLogicElement);
                    }
                    if (_mappingLogic == null && !string.IsNullOrEmpty(mappingLogicString))
                    {
                        _mappingLogic = new MappingLogic(mappingLogicString);
                    }
                }
                return _mappingLogic;
            }
            set
            {
                string logicString = value.description;
                if (value.mappingElement != null) logicString = value.mappingElement.uniqueID;
                this.wrappedTaggedValue.comment = KeyValuePairsHelper.setValueForKey(MappingFactory.mappingLogicName, logicString, this.wrappedTaggedValue.comment);
            }
        }
        #endregion

        #region implemented abstract members of Mapping

        public override void save()
        {
            if (this._mappingLogic != null)
                this.mappingLogic = this._mappingLogic; //make sure to set the mapping logic value correctly
            //set mapping path
            if (this.source.structure == MP.ModelStructure.Message)
            {
                this.wrappedTaggedValue.comment = KeyValuePairsHelper.setValueForKey(MappingFactory.mappingSourcePathName, ((MappingNode)this.source).getMappingPathString(), this.wrappedTaggedValue.comment);
            }
            if (this.target.structure == MP.ModelStructure.Message)
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
