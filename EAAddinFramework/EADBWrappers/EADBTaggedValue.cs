using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public abstract class EADBTaggedValue
    {
        internal static List<String> columnNames;
        private static void initializeColumnNames(Model model)
        {
            columnNames = model.getDataSetFromQuery("select top 1 * from t_attributeTag ", true).FirstOrDefault();
        }
        //public static List<EADBTaggedValue> getTaggedValuesForElementID(int elementID, global::EA.ObjectType ownerType, Model model)
        //{
        //    var elements = new List<EADBTaggedValue>();
        //    string sqlGetData;
        //    switch ( ownerType)
        //    {
        //        case ObjectType.otElement:
        //            sqlGetData = $@"select tv.PropertyID, tv.Object_ID as ElementID, tv.Property, tv.Notes, tv.ea_guid 
        //                         from t_objectproperties tv 
        //                         where tv.Object_ID = {elementID}";
        //            break;
        //        case ObjectType.otAttribute:
        //            sqlGetData = $"select * from t_attributeTag tv where tv.ElementID = {elementID}";
        //            break;
        //        case ObjectType.otConnector:
        //            sqlGetData = "select * from t_connectorTag tv where tv.ElementID = {elementID}";
        //            break;
        //        case ObjectType.otMethod:
        //            sqlGetData = "select * from t_operationTag tv where tv.ElementID = { elementID}";
        //            break;
        //    }
        //    var results = model.getDataSetFromQuery(sqlGetData, false);
        //    foreach (var propertyValues in results)
        //    {
        //        elements.Add(new T(model, propertyValues));
        //    }
        //    return elements;

        //}
        protected Model model { get; set; }
  
        protected Dictionary<string, string> properties { get; set; }

        protected EADBTaggedValue(Model model)
        {
            if (columnNames == null)
            {
                initializeColumnNames(model);
            }
            this.model = model;
        }
        protected EADBTaggedValue(Model model, List<string> propertyValues)
            : this(model)
        {
            this.properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < columnNames.Count; i++)
            {
                this.properties.Add(columnNames[i], propertyValues[i]);
            }
        }
        public virtual int PropertyID  
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["PropertyID"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            protected set => this.properties["PropertyID"] = value.ToString();
        }

        public string PropertyGUID
        {
            get => this.properties["ea_guid"];
            set => this.properties["ea_guid"] = value;
        }
        public string Name
        {
            get => this.properties["Property"];
            set => this.properties["Property"] = value;
        }
        public string Value
        {
            get => this.properties["VALUE"];
            set => this.properties["VALUE"] = value;
        }
        public virtual string Notes
        {
            get => this.properties["NOTES"];
            set => this.properties["NOTES"] = value;
        }
        public virtual int ElementID
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["ElementID"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            set => this.properties["ElementID"] = Value.ToString();
        }

        public abstract bool Update();
        public abstract string GetLastError();
        public abstract string GetAttribute(string PropName);
        public abstract bool SetAttribute(string PropName, string PropValue);
        public abstract bool HasAttributes();
        public abstract ObjectType ObjectType { get; }
        public abstract string FQName { get; }
    }
}
