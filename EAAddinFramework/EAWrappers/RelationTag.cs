using System;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class RelationTag : TaggedValue
    {

        internal global::EA.ConnectorTag wrappedTaggedValue { get; set; }
        internal RelationTag(Model model, global::EA.ConnectorTag eaTag) : base(model)
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
            get => this.model.getRelationByID(this.wrappedTaggedValue.ConnectorID);
            set => throw new NotImplementedException();
        }

        public override string ea_guid => this.wrappedTaggedValue.TagGUID;

        public override void save()
        {
            this.wrappedTaggedValue.Update();
        }
        internal override bool equalsTagObject(object eaTag)
        {
            var otherTag = eaTag as global::EA.ConnectorTag;
            return otherTag != null && otherTag.TagGUID == this.uniqueID;
        }
    }
}
