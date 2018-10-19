using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class RoleTag : TaggedValue
    {

        internal global::EA.RoleTag wrappedTaggedValue { get; set; }
        internal RoleTag(Model model, Element owner,  global::EA.RoleTag eaTag) : base(model, owner)
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
                //apparently not no notes implemented in EA.RoleTag
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
                    var owningRelation = this.model.getRelationByGUID(this.wrappedTaggedValue.ElementGUID) as ConnectorWrapper;
                    if (this.wrappedTaggedValue.BaseClass == "ASSOCIATION_TARGET")
                    {
                        this._owner = owningRelation.targetEnd;
                    }
                    else
                    {
                        this._owner = owningRelation.sourceEnd;
                    }
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
            var otherTag = eaTag as global::EA.RoleTag;
            return otherTag != null && otherTag.PropertyGUID == this.uniqueID;
        }
    }
}
