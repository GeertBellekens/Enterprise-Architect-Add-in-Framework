using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class RoleTag : TaggedValue
    {

        internal EADBRoleTag wrappedTaggedValue { get; set; }
        internal RoleTag(Model model, Element owner, EADBRoleTag eaTag) : base(model, owner)
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
            get => this.wrappedTaggedValue.Name;
            set => throw new NotImplementedException();
        }

        public override UML.Classes.Kernel.Element owner
        {
            get
            {
                if (this._owner == null)
                {
                    var owningRelation = this.model.getRelationByGUID(this.wrappedTaggedValue.ConnectorGUID) as ConnectorWrapper;
                    if (this.wrappedTaggedValue.isSource)
                    {
                        this._owner = owningRelation.sourceEnd;
                    }
                    else
                    {
                        this._owner = owningRelation.targetEnd;
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
            var otherEATag = eaTag as global::EA.RoleTag;
            if (otherEATag != null && otherEATag.PropertyGUID == this.uniqueID)
            {
                return true;
            }
            var otherTag = eaTag as EADBRoleTag;
            return otherTag != null && otherTag.PropertyGUID == this.uniqueID;
        }
    }
}
