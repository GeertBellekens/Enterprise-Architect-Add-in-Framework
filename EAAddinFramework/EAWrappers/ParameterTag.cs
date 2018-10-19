using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class ParameterTag : TaggedValue
    {

        internal global::EA.ParamTag wrappedTaggedValue { get; set; }
        internal ParameterTag(Model model, Element owner, global::EA.ParamTag eaTag) : base(model, owner)
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
            get =>
                //apparently not no notes implemented in EA.ParamTag
                string.Empty;
            set
            {
                //do nothing
            }
        }
        public override string name
        {
            get => this.wrappedTaggedValue.Tag;
            set => throw new NotImplementedException();
        }

        public override UML.Classes.Kernel.Element owner
        {
            get
            {
                if (this._owner == null)
                {
                    this._owner = this.model.getParameterByGUID(this.wrappedTaggedValue.ElementGUID);
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
            var otherTag = eaTag as global::EA.ParamTag;
            return otherTag != null && otherTag.PropertyGUID == this.uniqueID;
        }

    }
}
