using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBOperationTag : EADBTaggedValue
    {
        const string selectQuery = "select * from t_operationtag tv";

        public static List<EADBTaggedValue> getTaggedValuesForElementID(int elementID, Model model)
        {
            var elements = new List<EADBTaggedValue>();
            string sqlGetData = $"{selectQuery} tv where tv.ElementID = {elementID}";
            var results = model.getDataSetFromQuery(sqlGetData, false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBOperationTag(model, propertyValues));
            }
            return elements;
        }
        public EADBOperationTag(Model model, List<string> propertyValues)
            : base(model, propertyValues)
        { }
        public EADBOperationTag(Model model, global::EA.MethodTag taggedValue)
             : base(model)
        {
            //set wrapped property
            this.eaTaggedValue = taggedValue;
            //set properties
            updateFromWrappedElement();

        }
        private void updateFromWrappedElement()
        {
            this.PropertyGUID = this.eaTaggedValue.TagGUID;
            this.Name = this.eaTaggedValue.Name;
            this.Value = this.eaTaggedValue.Value;
            this.Notes = this.eaTaggedValue.Notes;
            this.ElementID = this.eaTaggedValue.MethodID;
            this.PropertyID = this.eaTaggedValue.TagID;
        }
        private global::EA.MethodTag _eaTaggedValue;
        private global::EA.MethodTag eaTaggedValue
        {
            get
            {
                if (this._eaTaggedValue == null)
                {
                    //first get element that owns the tagged value, then get the tagged value from the list of tagged values
                    var owner = this.model.wrappedModel.GetMethodByID(this.ElementID);
                    foreach (global::EA.MethodTag taggedValue in owner.TaggedValues)
                    {
                        if (taggedValue.TagGUID == this.PropertyGUID)
                        {
                            this._eaTaggedValue = taggedValue;
                            break;//found it
                        }
                    }
                }
                return this._eaTaggedValue;
            }
            set => this._eaTaggedValue = value;
        }
        public override bool Update()
        {
            //update properties
            this.eaTaggedValue.TagGUID = this.PropertyGUID;
            this.eaTaggedValue.Name = this.Name;
            this.eaTaggedValue.Value = this.Value;
            this.eaTaggedValue.Notes = this.Notes;
            //this.eaTaggedValue.MethodID = this.ElementID;
            //save wrapped tagged value
            var updateresult = this.eaTaggedValue.Update();
            //aver saving we need to refresh the properties
            updateFromWrappedElement();
            return updateresult;
        }
        public override string GetLastError()
        {
            return this.eaTaggedValue.GetLastError();
        }
        public override string GetAttribute(string PropName)
        {
            return this.eaTaggedValue.GetAttribute(PropName);
        }
        public override bool SetAttribute(string PropName, string PropValue)
        {
            return this.eaTaggedValue.SetAttribute(PropName, PropValue);
        }
        public override bool HasAttributes()
        {
            return this.eaTaggedValue.HasAttributes();
        }
        public override string FQName => this.eaTaggedValue.FQName;

        public override ObjectType ObjectType => ObjectType.otMethodTag;
    }
}
