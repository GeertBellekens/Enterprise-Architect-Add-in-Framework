using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class ElementTag : TaggedValue
    {

        internal global::EA.TaggedValue wrappedTaggedValue { get; set; }
        internal ElementTag(Model model, Element owner, global::EA.TaggedValue eaTag) : base(model, owner)
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
            var otherTag = eaTag as global::EA.TaggedValue;
            return otherTag != null && otherTag.PropertyGUID == this.uniqueID;
        }
    }
}
