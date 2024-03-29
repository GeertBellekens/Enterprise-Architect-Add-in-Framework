﻿using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBElementTag : EADBTaggedValue
    {
        const string selectQuery = "select * from t_objectproperties tv";
        public static List<EADBTaggedValue> getTaggedValuesForElementID(int elementID, Model model)
        {
            return getTaggedValuesForElementIDs(new List<int> { elementID }, model);
        }
        public static List<EADBTaggedValue> getTaggedValuesForElementIDs(IEnumerable<int> elementIDs, Model model)
        {
            var elements = new List<EADBTaggedValue>();
            if (elementIDs == null || elementIDs.Count() == 0) return elements;
            string sqlGetData = $"{selectQuery} where tv.Object_ID in ({string.Join(",", elementIDs)})";
            var results = model.getDataSetFromQuery(sqlGetData, false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBElementTag(model, propertyValues));
            }
            return elements;
        }
        public EADBElementTag(Model model, List<string> propertyValues)
            : base(model, propertyValues)
        { }
        public EADBElementTag(Model model, global::EA.TaggedValue taggedValue)
             : base(model)
        {
            //set wrapped property
            this.eaTaggedValue = taggedValue;
            //set properties
            updateFromWrappedElement();
        }
        private void updateFromWrappedElement()
        {
            this.PropertyGUID = this.eaTaggedValue.PropertyGUID;
            this.Name = this.eaTaggedValue.Name;
            this.Value = this.eaTaggedValue.Value;
            this.Notes = this.eaTaggedValue.Notes;
            this.ElementID = this.eaTaggedValue.ElementID;
            this.PropertyID = this.eaTaggedValue.PropertyID;
        }
        private global::EA.TaggedValue _eaTaggedValue;
        private global::EA.TaggedValue eaTaggedValue
        {
            get
            {
                if (this._eaTaggedValue == null)
                {
                    //first get element that owns the tagged value, then get the tagged value from the list of tagged values
                    var owner = this.model.wrappedModel.GetElementByID(this.ElementID);
                    foreach (global::EA.TaggedValue taggedValue in owner.TaggedValues)
                    {
                        if (taggedValue.PropertyGUID == this.PropertyGUID)
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
            this.eaTaggedValue.PropertyGUID = this.PropertyGUID;
            this.eaTaggedValue.Name = this.Name;
            this.eaTaggedValue.Value = this.Value;
            this.eaTaggedValue.Notes = this.Notes;
            //this.eaTaggedValue.ElementID = this.ElementID;
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

        public override ObjectType ObjectType => ObjectType.otTaggedValue;
    }
}
