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
                        mappingLogicString = KeyValuePairsHelper.getValueForKey("mappingLogic", this.wrappedTaggedValue.comment);
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
                this.wrappedTaggedValue.comment = KeyValuePairsHelper.setValueForKey("mappingLogic", logicString, this.wrappedTaggedValue.comment);
            }
        }
        #endregion

        #region implemented abstract members of Mapping

        public override void save()
        {
            if (this._mappingLogic != null)
                this.mappingLogic = this._mappingLogic; //make sure to set the mapping logic value correctly
            this.wrappedTaggedValue.save();
        }

        #endregion
    }
}
