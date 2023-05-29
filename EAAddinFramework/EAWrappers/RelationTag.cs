using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class RelationTag : TaggedValue
    {

        internal EADBConnectorTag wrappedTaggedValue { get; set; }
        internal RelationTag(Model model, Element owner, EADBConnectorTag eaTag) : base(model, owner)
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
            set
            {
                if (value?.Length > 255)
                {
                    throw new ArgumentOutOfRangeException("The provided string for this tagged value is too long");
                }
                this.wrappedTaggedValue.Value = value;
            }
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
                    this._owner = this.model.getRelationByID(this.wrappedTaggedValue.ElementID);
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
            var otherTag = eaTag as EADBConnectorTag;
            return otherTag != null && otherTag.PropertyGUID == this.uniqueID;
        }
    }
}
