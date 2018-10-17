using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class RoleTag : TaggedValue
    {

        internal global::EA.RoleTag wrappedTaggedValue { get; set; }
        internal RoleTag(Model model, global::EA.RoleTag eaTag) : base(model)
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
                var owningRelation = this.model.getRelationByGUID(this.wrappedTaggedValue.ElementGUID) as ConnectorWrapper;
                if (this.wrappedTaggedValue.BaseClass == "ASSOCIATION_TARGET")
                {
                    return owningRelation.targetEnd;
                }
                else
                {
                    return owningRelation.sourceEnd;
                }
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
