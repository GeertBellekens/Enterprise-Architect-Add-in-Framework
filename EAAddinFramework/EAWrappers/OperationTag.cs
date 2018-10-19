using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class OperationTag : TaggedValue
    {

        internal global::EA.MethodTag wrappedTaggedValue { get; set; }
        internal OperationTag(Model model, Element owner, global::EA.MethodTag eaTag) : base(model, owner)
        {
            this.wrappedTaggedValue = eaTag;
        }

        /// <summary>
        /// return the unique ID of this element
        /// </summary>
        public override string uniqueID => this.wrappedTaggedValue.TagGUID;
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
                    this._owner = this.model.getOperationByID(this.wrappedTaggedValue.MethodID);
                }
                return this._owner;
            }
            set => throw new NotImplementedException();
        }

        public override string ea_guid => this.wrappedTaggedValue.TagGUID;

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
            var otherTag = eaTag as global::EA.MethodTag;
            return otherTag != null && otherTag.TagGUID == this.uniqueID;
        }

        #endregion
    }
}
