using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBAttribute
    {
        internal static List<String> columnNames;
        private static void initializeColumnNames(Model model)
        {
            columnNames = model.getDataSetFromQuery("select top 1 * from t_attribute ", true).FirstOrDefault();
        }
        public static EADBAttribute getEADBAttributeForAttributeID(int attributeID, Model model)
        {
            return getEADBAttributesForAttributeIDs(new List<string>() { attributeID.ToString() }, model).FirstOrDefault();
        }

        public static List<EADBAttribute> getEADBAttributesForAttributeIDs(List<string> attributeIDs, Model model)
        {
            var elements = new List<EADBAttribute>();
            if (attributeIDs == null || attributeIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($"select * from t_attribute a where a.ID in ({string.Join(",", attributeIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBAttribute(model, propertyValues));
            }
            return elements;
        }
        public static EADBAttribute getEADBAttributeForAttributeGUID(string attributeGUID, Model model)
        {
            return getEADBAttributesForAttributeGUIDs(new List<string>() { attributeGUID }, model).FirstOrDefault();
        }
        public static List<EADBAttribute> getEADBAttributesForAttributeGUIDs(List<string> attributeGUIDs, Model model)
        {
            var elements = new List<EADBAttribute>();
            if (attributeGUIDs == null || attributeGUIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($"select * from t_attribute a where a.ea_guid in ('{string.Join("','", attributeGUIDs)}')", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBAttribute(model, propertyValues));
            }
            return elements;
        }
        public static List<EADBAttribute> getEADBAttributesForElementID(int elementID, Model model)
        {
            return getEADBAttributesForElementIDs(new List<int>() { elementID }, model);
        }
        public static List<EADBAttribute> getEADBAttributesForElementIDs(IEnumerable<int> elementIDs, Model model)
        {
            var elements = new List<EADBAttribute>();
            if (elementIDs == null || elementIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($"select * from t_attribute a where a.Object_ID in ({string.Join(",", elementIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBAttribute(model, propertyValues));
            }
            return elements;
        }
        public static List<EADBAttribute> getEADBAttributesForPackageIDs(List<string> packageIDs, Model model)
        {
            var elements = new List<EADBAttribute>();
            if (packageIDs == null || packageIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($@"select * from (t_attribute a 
                                                    inner join t_object o on o.Object_ID = a.Object_ID)
                                                    where o.Package_ID in ({string.Join(",", packageIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBAttribute(model, propertyValues));
            }
            return elements;
        }

        private Model model { get; set; }
        private global::EA.Attribute _eaAttribute;
        private global::EA.Attribute eaAttribute
        {
            get
            {
                if (this._eaAttribute == null)
                {
                    this._eaAttribute = this.model.wrappedModel.GetAttributeByGuid(this.properties["ea_guid"]);
                }
                return this._eaAttribute;
            }
            set => this._eaAttribute = value;
        }
        private Dictionary<string, string> properties { get; set; }

        private EADBAttribute(Model model)
        {
            if (columnNames == null)
            {
                initializeColumnNames(model);
            }
            this.model = model;
        }
        public EADBAttribute(Model model, List<string> propertyValues)
            : this(model)
        {
            this.properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < columnNames.Count; i++)
            {
                this.properties.Add(columnNames[i], propertyValues[i]);
            }
        }
        public EADBAttribute(Model model, int attributeID)
            : this(model, model.getDataSetFromQuery($"select * from t_attribute a where a.ID = {attributeID}", false).FirstOrDefault())
        { }
        public EADBAttribute(Model model, string uniqueID)
            : this(model, model.getDataSetFromQuery($"select * from t_attribute a where a.ea_guid = {uniqueID}", false).FirstOrDefault())
        { }
        public EADBAttribute(Model model, global::EA.Attribute attribute)
            : this(model)
        {
            this.eaAttribute = attribute;
            //initialize properties emtpy
            this.properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < columnNames.Count; i++)
            {
                this.properties.Add(columnNames[i], String.Empty);
            }
            updateFromWrappedElement();
        }

        private void updateFromWrappedElement()
        {
            //update properties from eaAttribute
            this.Name = this.eaAttribute.Name;
            this.Visibility = this.eaAttribute.Visibility;
            this.Stereotype = this.eaAttribute.Stereotype;
            this.Containment = this.eaAttribute.Containment;
            this.IsStatic = this.eaAttribute.IsStatic;
            this.IsCollection = this.eaAttribute.IsCollection;
            this.IsOrdered = this.eaAttribute.IsOrdered;
            this.AllowDuplicates = this.eaAttribute.AllowDuplicates;
            this.LowerBound = this.eaAttribute.LowerBound;
            this.UpperBound = this.eaAttribute.UpperBound;
            this.Container = this.eaAttribute.Container;
            this.Notes = this.eaAttribute.Notes;
            this.IsDerived = this.eaAttribute.IsDerived;
            this.Pos = this.eaAttribute.Pos;
            this.Length = this.eaAttribute.Length;
            this.Precision = this.eaAttribute.Precision;
            this.Scale = this.eaAttribute.Scale;
            this.IsConst = this.eaAttribute.IsConst;
            this.Style = this.eaAttribute.Style;
            this.ClassifierID = this.eaAttribute.ClassifierID;
            this.Default = this.eaAttribute.Default;
            this.Type = this.eaAttribute.Type;
            this.AttributeGUID = this.eaAttribute.AttributeGUID;
            this.StyleEx = this.eaAttribute.StyleEx;
            this.StereotypeEx = this.eaAttribute.StereotypeEx;
            this.IsID = this.eaAttribute.IsID;
            //add also ID
            this.AttributeID = this.eaAttribute.AttributeID;
        }

        public string Name
        {
            get => this.properties["Name"];
            set => this.properties["Name"] = value;
        }
        public string Visibility
        {
            get => this.properties["Scope"];
            set => this.properties["Scope"] = value;
        }
        public string Stereotype
        {
            get => this.properties["Stereotype"];
            set => this.properties["Stereotype"] = value;
        }
        public string Containment
        {
            get => this.properties["Containment"];
            set => this.properties["Containment"] = value;
        }
        public bool IsStatic
        {
            get => this.properties["IsStatic"] == "1" ? true : false;
            set => this.properties["IsStatic"] = value ? "1" : "0";
        }
        public bool IsCollection
        {
            get => this.properties["IsCollection"] == "1" ? true : false;
            set => this.properties["IsCollection"] = value ? "1" : "0";
        }
        public bool IsOrdered
        {
            get => this.properties["IsOrdered"] == "1" ? true : false;
            set => this.properties["IsOrdered"] = value ? "1" : "0";
        }
        public bool AllowDuplicates
        {
            get => this.properties["AllowDuplicates"] == "1" ? true : false;
            set => this.properties["AllowDuplicates"] = value ? "1" : "0";
        }
        public string LowerBound
        {
            get => this.properties["LowerBound"];
            set => this.properties["LowerBound"] = value;
        }
        public string UpperBound
        {
            get => this.properties["UpperBound"];
            set => this.properties["UpperBound"] = value;
        }
        public string Container
        {
            get => this.properties["Container"];
            set => this.properties["Container"] = value;
        }
        public string Notes
        {
            get => this.properties["Notes"];
            set => this.properties["Notes"] = value;
        }
        public bool IsDerived
        {
            get => this.properties["Derived"] == "1" ? true : false;
            set => this.properties["Derived"] = value ? "1" : "0";
        }
        public int AttributeID
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["ID"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            private set => this.properties["ID"] = value.ToString();
        }

        public int Pos
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["Pos"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            set => this.properties["Pos"] = value.ToString();
        }
        public string Length
        {
            get => this.properties["Length"];
            set => this.properties["Length"] = value;
        }
        public string Precision
        {
            get => this.properties["Precision"];
            set => this.properties["Precision"] = value;
        }
        public string Scale
        {
            get => this.properties["Precision"];
            set => this.properties["Precision"] = value;
        }
        public bool IsConst
        {
            get => this.properties["Const"] == "1" ? true : false;
            set => this.properties["Const"] = value ? "1" : "0";
        }
        public string Style
        {
            get => this.properties["Style"];
            set => this.properties["Style"] = value;
        }
        public int ClassifierID
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["Classifier"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            set => this.properties["Classifier"] = value.ToString();
        }
        public string Default
        {
            get => this.properties["Default"];
            set => this.properties["Default"] = value;
        }
        public string Type
        {
            get => this.properties["Type"];
            set => this.properties["Type"] = value;
        }
        public string AttributeGUID
        {
            get => this.properties["ea_guid"];
            set => this.properties["ea_guid"] = value;
        }
        public string StyleEx
        {
            get => this.properties["StyleEx"];
            set => this.properties["StyleEx"] = value;
        }

        public ObjectType ObjectType => ObjectType.otAttribute;

        public int ParentID
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["Object_ID"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            set => this.properties["Object_ID"] = value.ToString();
        }


        public string Alias
        {
            get => this.Style;
            set => this.Style = value;
        }
        private bool _isID;
        public bool IsID
        {
            get
            {
                if (this._eaAttribute != null)
                {
                    this._isID = this.eaAttribute.IsID;
                }
                else
                {
                    //getIsID property from database
                    var sqlGetData = $@"select count(x.xrefID) as isID
                                    from t_xref x
                                    where 1=1
                                    and x.Name = 'CustomProperties'
                                    and x.type = 'attribute property'
                                    and x.Description like '%@NAME=isID@ENDNAME;@TYPE=Boolean@ENDTYPE;@VALU=1@%' 
                                    and x.client = '{this.AttributeGUID}'";
                    this._isID = int.Parse(this.model.getFirstValueFromQuery(sqlGetData, "isID")) > 0;
                }
                return this._isID;
            }
            set => this._isID = value;
        }
        public string SubsettedProperty
        {
            get => this.eaAttribute.SubsettedProperty;
            set => this.eaAttribute.SubsettedProperty = value;
        }
        public string RedefinedProperty
        {
            get => this.eaAttribute.RedefinedProperty;
            set => this.eaAttribute.RedefinedProperty = value;
        }
        private string _StereotypeEx = null;
        public string StereotypeEx 
        {
            get
            {
                if (_StereotypeEx == null)
                {
                    var xrefDescription = this.model.getFirstValueFromQuery($"select x.Description from t_xref x where x.Name = 'Stereotypes' and x.Client = '{this.AttributeGUID}'", "Description");
                    if (string.IsNullOrEmpty(xrefDescription))
                    {
                        this._StereotypeEx = string.Empty;
                    }
                    else
                    {
                        this._StereotypeEx = String.Join(",", KeyValuePairsHelper.getValuesForKey("Name", xrefDescription));
                    }
                }
                return this._StereotypeEx;
            }
            set => this._StereotypeEx = value;
        }
        public Collection Constraints => this.eaAttribute.Constraints;
        public Collection TaggedValues => this.eaAttribute.TaggedValues;
        public Collection TaggedValuesEx => this.eaAttribute.TaggedValuesEx;
        public string FQStereotype => this.eaAttribute.FQStereotype;
 
        public TypeInfoProperties TypeInfoProperties => this.eaAttribute.TypeInfoProperties;

        public bool Update()
        {
            this.eaAttribute.Name = this.Name;
            this.eaAttribute.Visibility = this.Visibility;
            this.eaAttribute.Stereotype = this.Stereotype;
            this.eaAttribute.Containment = this.Containment;
            this.eaAttribute.IsStatic = this.IsStatic;
            this.eaAttribute.IsCollection = this.IsCollection;
            this.eaAttribute.IsOrdered = this.IsOrdered;
            this.eaAttribute.AllowDuplicates = this.AllowDuplicates;
            this.eaAttribute.LowerBound = this.LowerBound;
            this.eaAttribute.UpperBound = this.UpperBound;
            this.eaAttribute.Container = this.Container;
            this.eaAttribute.Notes = this.Notes;
            this.eaAttribute.IsDerived = this.IsDerived;
            this.eaAttribute.Pos = this.Pos;
            this.eaAttribute.Length = this.Length;
            this.eaAttribute.Precision = this.Precision;
            this.eaAttribute.Scale = this.Scale;
            this.eaAttribute.IsConst = this.IsConst;
            this.eaAttribute.Style = this.Style;
            this.eaAttribute.ClassifierID = this.ClassifierID;
            this.eaAttribute.Default = this.Default;
            this.eaAttribute.Type = this.Type;
            this.eaAttribute.AttributeGUID = this.AttributeGUID;
            this.eaAttribute.StyleEx = this.StyleEx;
            this.eaAttribute.StereotypeEx = this.StereotypeEx;
            this.eaAttribute.IsID = this.IsID;
            var updateResult = this.eaAttribute.Update();
            //aver saving we need to refresh the properties
            updateFromWrappedElement();
            return updateResult;
        }
        public string GetLastError()
        {
            return this.eaAttribute.GetLastError();
        }
        public string GetTXName(string txCode, int nFlag)
        {
            return this.eaAttribute.GetTXName(txCode, nFlag);
        }

        public string GetTXAlias(string txCode, int nFlag)
        {
            return this.eaAttribute.GetTXAlias(txCode, nFlag);
        }

        public string GetTXNote(string txCode, int nFlag)
        {
            return this.eaAttribute.GetTXNote(txCode, nFlag);
        }

        public void SetTXName(string txCode, string translation)
        {
            this.eaAttribute.SetTXName(txCode, translation);
        }

        public void SetTXAlias(string txCode, string translation)
        {
            this.eaAttribute.SetTXAlias(txCode, translation);
        }

        public void SetTXNote(string txCode, string translation)
        {
            this.eaAttribute.SetTXNote(txCode, translation);
        }
    }
}
