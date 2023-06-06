using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBAttributeTag : EADBTaggedValue
    {
        public static List<EADBTaggedValue> getTaggedValuesForElementIDs(IEnumerable<int> elementIDs, Model model)
        {
            var elements = new List<EADBTaggedValue>();
            if (elementIDs == null || elementIDs.Count() == 0) return elements;
            string sqlGetData = $"select * from t_attributeTag tv where tv.ElementID in ({string.Join(",",elementIDs)})";
            var results = model.getDataSetFromQuery(sqlGetData, false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBAttributeTag(model, propertyValues));
            }
            return elements;
        }
        public static List<EADBTaggedValue> getTaggedValuesForElementID(int elementID, Model model)
        {
            return getTaggedValuesForElementIDs(new List<int> { elementID }, model);
        }
        public EADBAttributeTag(Model model, List<string> propertyValues)
            : base(model, propertyValues)
        { }
        public EADBAttributeTag(Model model, global::EA.AttributeTag taggedValue)
             : base(model)
        {
            //set wrapped property
            this.eaTaggedValue = taggedValue;
            //Initialize empty
            this.properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < columnNames.Count; i++)
            {
                this.properties.Add(columnNames[i], string.Empty);
            }
            //set properties
            updateFromWrappedElement();

        }
        private void updateFromWrappedElement()
        {
            this.PropertyGUID = this.eaTaggedValue.TagGUID;
            this.Name = this.eaTaggedValue.Name;
            this.Value = this.eaTaggedValue.Value;
            this.Notes = this.eaTaggedValue.Notes;
            this.ElementID = this.eaTaggedValue.AttributeID;
            this.PropertyID = this.eaTaggedValue.TagID;
        }
        private global::EA.AttributeTag _eaTaggedValue;
        private global::EA.AttributeTag eaTaggedValue
        {
            get
            {
                if (this._eaTaggedValue == null)
                {
                    //first get element that owns the tagged value, then get the tagged value from the list of tagged values
                    var owner = this.model.wrappedModel.GetAttributeByID(this.ElementID);
                    foreach (global::EA.AttributeTag taggedValue in owner.TaggedValues)
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
            //this.eaTaggedValue.AttributeID = this.ElementID;
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

        public override ObjectType ObjectType => ObjectType.otAttributeTag;
    }
}
