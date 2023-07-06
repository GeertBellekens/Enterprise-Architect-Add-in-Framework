using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBRoleTag : EADBTaggedValue
    {
        
        internal static new Dictionary<string, int> staticColumnNames;
        const string selectQuery = @"select 0 as PropertyID, c.ea_guid as ElementID , tv.TagValue as Property
                            , tv.Notes as VALUE, '' as NOTES, tv.PropertyID as ea_guid, tv.BaseClass
                            from t_taggedvalue tv
                            inner join t_connector c on c.ea_guid = tv.ElementID
                            where tv.BaseClass like 'ASSOCIATION_%' ";
        protected override Dictionary<string, int> columnNames
        {
            get
            {
                if (staticColumnNames == null)
                {
                    staticColumnNames = this.getColumnNames(selectQuery);
                }
                return staticColumnNames;
            }
        }
        public static List<EADBTaggedValue> getTaggedValuesForElementIDs(IEnumerable<int> elementIDs, Model model)
        {
            var elements = new List<EADBTaggedValue>();
            if (elementIDs == null || elementIDs.Count() == 0) return elements;
            string sqlGetData = $@"{selectQuery}
                            and c.Connector_ID in ({string.Join(",", elementIDs)})";
            var results = model.getDataSetFromQuery(sqlGetData, false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBRoleTag(model, propertyValues));
            }
            return elements;
        }
        public static List<EADBTaggedValue> getTaggedValuesForElementID(int elementID, Model model)
        {
            return getTaggedValuesForElementIDs(new List<int>() { elementID }, model);
        }
        public EADBRoleTag(Model model, List<string> propertyValues)
            : base(model,  propertyValues)
        { }
        public EADBRoleTag(Model model, global::EA.RoleTag taggedValue)
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
            this.Name = this.eaTaggedValue.Tag;
            this.Value = this.eaTaggedValue.Value;
            //this.ConnectorGUID = this.eaTaggedValue.ElementGUID;
            this.BaseClass = this.eaTaggedValue.BaseClass;
        }
        public override bool Update()
        {
            //update properties
            this.eaTaggedValue.PropertyGUID = this.PropertyGUID;
            this.eaTaggedValue.Tag = this.Name;
            this.eaTaggedValue.Value = this.Value;
            //this.eaTaggedValue.ConnectorID = this.ElementID;
            //save wrapped tagged value
            var updateresult = this.eaTaggedValue.Update();
            //aver saving we need to refresh the properties
            updateFromWrappedElement();
            return updateresult;
        }
        public string ConnectorGUID
        {
            get => this.properties["ElementID"];
            set => this.properties["ElementID"] = value;
        }
        public bool isSource => this.BaseClass.Equals("ASSOCIATION_SOURCE", StringComparison.InvariantCultureIgnoreCase);
        public string BaseClass
        {
            get => this.properties["BaseClass"];
            set => this.properties["BaseClass"] = value;
        }

        private global::EA.RoleTag _eaTaggedValue;
        private global::EA.RoleTag eaTaggedValue
        {
            get
            {
                if (this._eaTaggedValue == null)
                {
                    //first get element that owns the tagged value, then get the tagged value from the list of tagged values
                    var connector = this.model.wrappedModel.GetConnectorByGuid(this.ConnectorGUID);
                    var owner = connector.ClientEnd;
                    foreach (global::EA.RoleTag taggedValue in owner.TaggedValues)
                    {
                        if (taggedValue.PropertyGUID == this.PropertyGUID)
                        {
                            this._eaTaggedValue = taggedValue;
                            break;//found it
                        }
                    }
                    //if still not found try again with supplierEnd
                    if (this._eaTaggedValue == null)
                    {
                        owner = connector.SupplierEnd;
                        foreach (global::EA.RoleTag taggedValue in owner.TaggedValues)
                        {
                            if (taggedValue.PropertyGUID == this.PropertyGUID)
                            {
                                this._eaTaggedValue = taggedValue;
                                break;//found it
                            }
                        }
                    }
                }
                return this._eaTaggedValue;
            }
            set => this._eaTaggedValue = value;
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

        public override ObjectType ObjectType => ObjectType.otRoleTag;
    }
}
