using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBAttribute: EADBBase
    {
        internal static Dictionary<string, int> staticColumnNames;
        const string selectQuery = @"select a.*, x.Description as StereotypesXref , x2.Description as CustomPropertiesXref
                from ((t_attribute a  
                left join t_xref x on (x.Client = a.ea_guid
				                and x.Name = 'Stereotypes'))
				left join t_xref x2 on (x2.Client = a.ea_guid
									and x2.Name = 'CustomProperties'))";
        
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
        public static EADBAttribute getEADBAttributeForAttributeID(int attributeID, Model model)
        {
            return getEADBAttributesForAttributeIDs(new List<string>() { attributeID.ToString() }, model).FirstOrDefault();
        }

        public static List<EADBAttribute> getEADBAttributesForAttributeIDs(List<string> attributeIDs, Model model)
        {
            var elements = new List<EADBAttribute>();
            if (attributeIDs == null || attributeIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($"{selectQuery} where a.ID in ({string.Join(",", attributeIDs)})", false);
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
            var results = model.getDataSetFromQuery($"{selectQuery} where a.ea_guid in ('{string.Join("','", attributeGUIDs)}')", false);
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
            var results = model.getDataSetFromQuery($"{selectQuery} where a.Object_ID in ({string.Join(",", elementIDs)})", false);
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
            var results = model.getDataSetFromQuery($@"{selectQuery}
                                                    where a.Object_ID in 
                                                        (select o.object_ID 
                                                        from t_object o 
                                                         where o.Package_ID in ({string.Join(",", packageIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBAttribute(model, propertyValues));
            }
            return elements;
        }

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
        
        public EADBAttribute(Model model, List<string> propertyValues)
            : base(model, propertyValues)
        { }
        public EADBAttribute(Model model, int attributeID)
            : this(model, model.getDataSetFromQuery($"{selectQuery} where a.ID = {attributeID}", false).FirstOrDefault())
        { }
        public EADBAttribute(Model model, string uniqueID)
            : this(model, model.getDataSetFromQuery($"{selectQuery} where a.ea_guid = {uniqueID}", false).FirstOrDefault())
        { }
        public EADBAttribute(Model model, global::EA.Attribute attribute)
            : base(model)
        {
            this.eaAttribute = attribute;
            //initialize properties emtpy 
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
            this.ParentID = this.eaAttribute.ParentID;
            //add also ID
            this.AttributeID = this.eaAttribute.AttributeID;
        }
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
            get => this.getBoolFromProperty("IsStatic");
            set => this.setBoolToProperty("IsStatic", value);
        }
        public bool IsCollection
        {
            get => this.getBoolFromProperty("IsCollection");
            set => this.setBoolToProperty("IsCollection", value);
        }
        public bool IsOrdered
        {
            get => this.getBoolFromProperty("IsOrdered");
            set => this.setBoolToProperty("IsOrdered", value);
        }
        public bool AllowDuplicates
        {
            get => this.getBoolFromProperty("AllowDuplicates");
            set => this.setBoolToProperty("AllowDuplicates", value);
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
            get => this.getBoolFromProperty("Derived");
            set => this.setBoolToProperty("Derived", value);
        }
        public int AttributeID
        {
            get => this.getIntFromProperty("ID");
            private set => this.properties["ID"] = value.ToString();
        }

        public int Pos
        {
            get => this.getIntFromProperty("Pos");
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
            get => this.properties["Scale"];
            set => this.properties["Scale"] = value;
        }
        public bool IsConst
        {
            get => this.getBoolFromProperty("Const");
            set => this.setBoolToProperty("Const", value);
        }
        public string Style
        {
            get => this.properties["Style"];
            set => this.properties["Style"] = value;
        }
        public int ClassifierID
        {
            get => this.getIntFromProperty("Classifier");
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
            get => this.getIntFromProperty("Object_ID");
            set => this.properties["Object_ID"] = value.ToString();
        }


        public string Alias
        {
            get => this.Style;
            set => this.Style = value;
        }
        private bool? _isID = null;
        public bool IsID
        {
            get
            {
                if (_isID == null)
                {
                    var xrefDescription = this.properties["CustomPropertiesXref"];
                    if (string.IsNullOrEmpty(xrefDescription))
                    {
                        this._isID = false;
                    }
                    else
                    {
                        this._isID = xrefDescription.Contains("@NAME=isID@ENDNAME;@TYPE=Boolean@ENDTYPE;@VALU=1");
                    }
                }
                return this._isID.Value;
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
                    var xrefDescription = this.properties["StereotypesXref"];
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
