using System;
using System.Collections.Generic;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class ElementTag : TaggedValue
    {

        internal EADBElementTag wrappedTaggedValue { get; set; }
        internal ElementTag(Model model, Element owner, EADBElementTag eaTag) : base(model, owner)
        {
            this.wrappedTaggedValue = eaTag;
        }

        /// <summary>
        /// return the unique ID of this element
        /// </summary>
        public override string uniqueID => this.wrappedTaggedValue.PropertyGUID;
        public override string eaStringValue
        {
            get => this.wrappedTaggedValue.Value;
            set => this.wrappedTaggedValue.Value = value;
        }
        public override string comment
        {
            get => this.wrappedTaggedValue.Notes;
            set => this.wrappedTaggedValue.Notes = value;
        }
        public override string name
        {
            get => this.wrappedTaggedValue.Name;
            set => throw new NotImplementedException();
        }

        public override UML.Classes.Kernel.Element owner
        {
            get
            {
                if (this._owner == null)
                {
                    this._owner = this.model.getElementWrapperByID(this.wrappedTaggedValue.ElementID);
                }
                return this._owner;
            }
            set => throw new NotImplementedException();
        }

        public override string ea_guid => this.wrappedTaggedValue.PropertyGUID;

        public override void save()
        {
            this.wrappedTaggedValue.Update();
        }
        internal override bool equalsTagObject(object eaTag)
        {
            var otherTag = eaTag as EADBElementTag;
            return otherTag != null && otherTag.PropertyGUID == this.uniqueID;
        }
        public int elementID => this.wrappedTaggedValue.ElementID;
        public static void loadElementTags(Dictionary<int, ElementWrapper> elementDictionary, Model model)
        {
            var eaDBElementTags = EADBElementTag.getTaggedValuesForElementIDs(elementDictionary.Keys, model);

            //add the tags to their respective elements
            foreach (var eaDBElementTag in eaDBElementTags)
            {
                if (elementDictionary.TryGetValue(eaDBElementTag.ElementID, out ElementWrapper elementWrapper))
                {
                    var elementTag = model.factory.createTaggedValue(elementWrapper, eaDBElementTag);
                    elementWrapper.addExistingTaggedValue(elementTag);
                }
            }
        }
    }
}
