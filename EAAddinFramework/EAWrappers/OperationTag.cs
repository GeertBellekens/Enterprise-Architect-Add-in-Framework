using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class OperationTag : TaggedValue
    {

        internal EADBOperationTag wrappedTaggedValue { get; set; }
        internal OperationTag(Model model, Element owner, EADBOperationTag eaTag) : base(model, owner)
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
                    this._owner = this.model.getOperationByID(this.wrappedTaggedValue.ElementID);
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

        #region implemented abstract members of TaggedValue

        public override string comment
        {
            get => this.wrappedTaggedValue.Notes;
            set => this.wrappedTaggedValue.Notes = value;
        }
        internal override bool equalsTagObject(object eaTag)
        {
            var otherEATag = eaTag as global::EA.MethodTag;
            if (otherEATag != null && otherEATag.TagGUID == this.uniqueID)
            {
                return true;
            }
            var otherTag = eaTag as EADBOperationTag;
            return otherTag != null && otherTag.PropertyGUID == this.uniqueID;
        }

        #endregion
    }
}
