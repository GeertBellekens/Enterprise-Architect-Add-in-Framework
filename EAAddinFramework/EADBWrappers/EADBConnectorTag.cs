﻿using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBConnectorTag : EADBTaggedValue
    {
        const string selectQuery = "select * from t_connectorTag tv";
        public static List<EADBTaggedValue> getTaggedValuesForElementIDs(IEnumerable<int> elementIDs, Model model)
        {
            var elements = new List<EADBTaggedValue>();
            if (elementIDs == null || elementIDs.Count() == 0) return elements;
            string sqlGetData = $"{selectQuery} where tv.ElementID in ({string.Join(",", elementIDs)})";
            var results = model.getDataSetFromQuery(sqlGetData, false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBConnectorTag(model, propertyValues));
            }
            return elements;
        }
        public static List<EADBTaggedValue> getTaggedValuesForElementID(int elementID, Model model)
        {
            return getTaggedValuesForElementIDs(new List<int>() { elementID }, model);
        }
        public EADBConnectorTag(Model model, List<string> propertyValues)
            : base(model, propertyValues)
        { }
        public EADBConnectorTag(Model model, global::EA.ConnectorTag taggedValue)
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
            this.ElementID = this.eaTaggedValue.ConnectorID;
            this.PropertyID = this.eaTaggedValue.TagID;
        }

        private global::EA.ConnectorTag _eaTaggedValue;
        private global::EA.ConnectorTag eaTaggedValue
        {
            get
            {
                if (this._eaTaggedValue == null)
                {
                    //first get element that owns the tagged value, then get the tagged value from the list of tagged values
                    var owner = this.model.wrappedModel.GetConnectorByID(this.ElementID);
                    foreach (global::EA.ConnectorTag taggedValue in owner.TaggedValues)
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
            //this.eaTaggedValue.ConnectorID = this.ElementID;
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

        public override ObjectType ObjectType => ObjectType.otConnectorTag;
    }
}
