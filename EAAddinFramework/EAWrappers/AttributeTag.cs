using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class AttributeTag : TaggedValue
    {

        internal EADBAttributeTag wrappedTaggedValue { get; set; }
        internal AttributeTag(Model model, Element owner, EADBAttributeTag eaTag) : base(model, owner)
        {
            this.wrappedTaggedValue = eaTag;
        }
        /// <summary>
        /// return the unique ID of this element
        /// </summary>
        public override string uniqueID => this.wrappedTaggedValue.PropertyGUID;
        public override string comment
        {
            get => this.wrappedTaggedValue.Notes;
            set => this.wrappedTaggedValue.Notes = value;
        }
        public override string eaStringValue
        {
            get => this.wrappedTaggedValue.Value;
            set => this.wrappedTaggedValue.Value = value;
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
                    this._owner = this.model.getAttributeWrapperByID(this.wrappedTaggedValue.ElementID);
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
            var otherTag = eaTag as EADBAttributeTag;
            return otherTag != null && otherTag.PropertyGUID == this.uniqueID;
        }
        public int attributeID => this.wrappedTaggedValue.ElementID;
    }
}
