using EA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace TSF.UmlToolingFramework.Wrappers.EA
{

    public class EADBElementWrapper
    {
        internal static List<String> columnNames;
        private static void initializeColumnNames(Model model)
        {
            columnNames = model.getDataSetFromQuery("select top 1 * from t_object o", true).FirstOrDefault();
        }
        public static List<EADBElementWrapper> GetEADBElementWrappersForElementIDs(List<string> elementIDs, Model model)
        {
            var elements = new List<EADBElementWrapper>();
            var results = model.getDataSetFromQuery($"select * from t_object o where o.Object_ID in ({String.Join(",",elementIDs)})", false);
            foreach(var propertyValues in results)
            {
                elements.Add(new EADBElementWrapper(model, propertyValues));
            }
            return elements;
        }
        public static List<EADBElementWrapper> GetEADBElementWrappersForPackageIDs(List<string> PackageIDs, Model model)
        {
            var elements = new List<EADBElementWrapper>();
            var results = model.getDataSetFromQuery($"select * from t_object o where o.Package_ID in ({String.Join(",", PackageIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBElementWrapper(model, propertyValues));
            }
            return elements;
        }
        private Model model { get; set; }
        private global::EA.Element _eaElement;
        private global::EA.Element eaElement
        {
            get
            {
                if (this._eaElement == null)
                {
                    this._eaElement = this.model.wrappedModel.GetElementByGuid(this.properties["ea_guid"]);
                }
                return this._eaElement;
            }
            set => this._eaElement = value;
        }
        private Dictionary<string, string> properties { get; set; }

        private EADBElementWrapper(Model model)
        {
            if (columnNames == null)
            {
                initializeColumnNames(model);
            }
            this.model = model;
        }
        public EADBElementWrapper(Model model, List<string> propertyValues)
            : this(model)
        {
            this.properties = new Dictionary<string, string>();
            for (int i = 0; i < columnNames.Count; i++)
            {
                this.properties.Add(columnNames[i], propertyValues[i]);
            }
        }
        public EADBElementWrapper(Model model, int elementID)
            : this(model)
        {
            this.properties = model.getDictionaryFromQuery($"select * from t_object o where o.Object_ID = {elementID}");
        }
        public EADBElementWrapper(Model model, string uniqueID)
            : this(model)
        {
            this.properties = model.getDictionaryFromQuery($"select * from t_object o where o.ea_guid = {uniqueID}");
        }
        public string Name
        {
            get => this.properties["Name"];
            set => this.properties["Name"] = value;
        }
        public string Notes
        {
            get => this.properties["Note"];
            set => this.properties["Note"] = value;
        }
        
        public string Version
        {
            get => this.properties["Note"];
            set => this.properties["Note"] = value;
        }
        public string Stereotype
        {
            get => this.properties["Stereotype"];
            set => this.properties["Stereotype"] = value;
        }
        public string Multiplicity
        {
            get => this.properties["Multiplicity"];
            set => this.properties["Multiplicity"] = value;
        }
        public string Genlinks
        {
            get => this.properties["GenLinks"];
            set => this.properties["GenLinks"] = value;
        }
        public string Abstract
        {
            get => this.properties["Abstract"];
            set => this.properties["Abstract"] = value;
        }
        public string Alias
        {
            get => this.properties["Alias"];
            set => this.properties["Alias"] = value;
        }
        public string Author
        {
            get => this.properties["Author"];
            set => this.properties["Author"] = value;
        }
        public string Complexity
        {
            get => this.properties["Complexity"];
            set => this.properties["Complexity"] = value;
        }
        public string Visibility
        {
            get => this.properties["Visibility"];
            set => this.properties["Visibility"] = value;
        }

        public string Phase
        {
            get => this.properties["Phase"];
            set => this.properties["Phase"] = value;
        }
        public string Persistence
        {
            get => this.properties["Persistence"];
            set => this.properties["Persistence"] = value;
        }
        public bool IsActive
        {
            get => this.properties["IsActive"] == "1" ? true : false;
            set => this.properties["IsActive"] = value ? "1" : "0";
        }
        public bool IsLeaf
        {
            get => this.properties["IsLeaf"] == "1" ? true : false;
            set => this.properties["IsLeaf"] = value ? "1" : "0";
        }
        public bool IsSpec
        {
            get => this.properties["IsSpec"] == "1" ? true : false;
            set => this.properties["IsSpec"] = value ? "1" : "0";
        }
        public int Subtype
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["NType"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            set => this.properties["NType"] = value.ToString();
        }
        public string Type
        {
            get => this.properties["Object_Type"];
            set => this.properties["Object_Type"] = value;
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
        public int ClassfierID
        {
            get => this.ClassifierID;
            set => this.ClassifierID = value;
        }

        public DateTime Created
        {
            get
            {
                DateTime result;
                if (DateTime.TryParse(this.properties["CreatedDate"], out result))
                {
                    return result;
                }
                else
                {
                    return default(DateTime);
                }
            }
            set => this.properties["CreatedDate"] = value.ToString();//TODO: check if this format is OK?
        }
        public DateTime Modified
        {
            get
            {
                DateTime result;
                if (DateTime.TryParse(this.properties["ModifiedDate"], out result))
                {
                    return result;
                }
                else
                {
                    return default(DateTime);
                }
            }
            set => this.properties["ModifiedDate"] = value.ToString();//TODO: check if this format is OK?
        }
        
        public string Genfile
        {
            get => this.properties["GenFile"];
            set => this.properties["GenFile"] = value;
        }
        public string Gentype
        {
            get => this.properties["GenType"];
            set => this.properties["GenType"] = value;
        }


        public int ElementID
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
        }

        public int PackageID
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["Package_ID"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            set => this.properties["Package_ID"] = value.ToString();
        }
        public string ElementGUID => this.properties["ea_guid"];
        public string StyleEx
        {
            get => this.properties["StyleEx"];
            set => this.properties["StyleEx"] = value;
        }
        public string EventFlags
        {
            get => this.properties["EventFlags"];
            set => this.properties["EventFlags"] = value;
        }
        public string ActionFlags
        {
            get => this.properties["ActionFlags"];
            set => this.properties["ActionFlags"] = value;
        }
        public string Status
        {
            get => this.properties["Status"];
            set => this.properties["Status"] = value;
        }
        public List<string> MiscData => new List<string> 
            {this.properties["PDATA1"], 
            this.properties["PDATA2"], 
            this.properties["PDATA3"], 
            this.properties["PDATA4"], 
            this.properties["PDATA5"] };
        public int TreePos
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["TPos"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            set => this.properties["TPos"] = value.ToString();
        }
        public int ParentID
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["ParentID"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            set => this.properties["ParentID"] = value.ToString();
        }
        public string RunState
        {
            get => this.properties["RunState"];
            set => this.properties["RunState"] = value;
        }
        public ObjectType ObjectType => ObjectType.otElement;
        public Collection Requirements => this.eaElement.Requirements;
        public Collection Constraints => this.eaElement.Constraints;
        public Collection Scenarios => this.eaElement.Scenarios;
        public Collection Files => this.eaElement.Files;
        public Collection Efforts => this.eaElement.Efforts;
        public Collection Risks => this.eaElement.Risks;
        public Collection Metrics => this.eaElement.Metrics;
        public Collection Issues => this.eaElement.Issues;
        public Collection Tests => this.eaElement.Tests;
        public Collection TaggedValues => this.eaElement.TaggedValues;
        public Collection Connectors => this.eaElement.Connectors;
        public Collection Methods => this.eaElement.Methods;
        public Collection Attributes => this.eaElement.Attributes;
        public Collection Resources => this.eaElement.Resources;
        public Collection Elements => this.eaElement.Elements; //TODO: rework to EADBElementWrappers?
        public Collection Diagrams => this.eaElement.Diagrams;
        public Collection Partitions => this.eaElement.Partitions;
        public Collection CustomProperties => this.eaElement.CustomProperties;
        public Collection StateTransitions => this.eaElement.StateTransitions;
        public Collection EmbeddedElements => this.eaElement.EmbeddedElements; //TODO: rework to EADBElementWrappers?
        public Collection BaseClasses => this.eaElement.BaseClasses; //TODO: rework to EADBElementWrappers?
        public Collection Realizes => this.eaElement.Realizes;
        public Collection TaggedValuesEx => this.eaElement.TaggedValuesEx;
        public Collection AttributesEx => this.eaElement.AttributesEx;
        public Collection MethodsEx => this.eaElement.MethodsEx;
        public Collection ConstraintsEx => this.eaElement.ConstraintsEx;
        public Collection RequirementsEx => this.eaElement.RequirementsEx;
        public Collection TemplateParameters => this.eaElement.TemplateParameters;
        public bool Locked
        {
            get => this.eaElement.Locked;
            set => this.eaElement.Locked = value;
        }
        public string MetaType
        {
            get => this.eaElement.MetaType;
            set => this.eaElement.MetaType = value;
        }
        public string ExtensionPoints
        {
            get => this.eaElement.ExtensionPoints; 
            set => this.eaElement.ExtensionPoints = value;
        }
        public string Tablespace
        {
            get => this.eaElement.Tablespace;
            set => this.eaElement.Tablespace = value;
        }
        public string Tag
        {
            get => this.eaElement.Tag;
            set => this.eaElement.Tag = value;
        }
        public string Priority
        {
            get => this.eaElement.Priority;
            set => this.eaElement.Priority = value;
        }
        public bool IsNew
        {
            get => this.eaElement.IsNew;
            set => this.eaElement.IsNew = value;
        }
        public string ClassifierName
        {
            get => this.eaElement.ClassifierName;
            set => this.eaElement.ClassifierName = value;
        }
        public string ClassifierType => this.eaElement.ClassifierType;
        public string Difficulty
        {
            get => this.eaElement.Difficulty;
            set => this.eaElement.Difficulty = value;
        }
        public object Header1
        {
            get => this.eaElement.Header1;
            set => this.eaElement.Header1 = value;
        }
        public object Header2
        {
            get => this.eaElement.Header2;
            set => this.eaElement.Header2 = value;
        }
        public Properties Properties => this.eaElement.Properties;
        public string StereotypeEx
        {
            get => this.eaElement.StereotypeEx;
            set => this.eaElement.StereotypeEx = value;
        }
        public int PropertyType
        {
            get => this.eaElement.PropertyType;
            set => this.eaElement.PropertyType = value;
        }

        public object CompositeDiagram => this.eaElement.CompositeDiagram;

        public bool IsComposite
        {
            get => this.eaElement.IsComposite;
            set => this.eaElement.IsComposite = value;
        }
        public int AssociationClassConnectorID => this.eaElement.AssociationClassConnectorID;



        public object PropertyTypeName => this.eaElement.PropertyTypeName;

        public bool IsInternalDocArtifact => this.eaElement.IsInternalDocArtifact;

        public string FQStereotype => this.eaElement.FQStereotype;

        public bool IsRoot { set => this.eaElement.IsRoot = value; }

        public string FQName => this.eaElement.FQName;

        public TypeInfoProperties TypeInfoProperties => this.eaElement.TypeInfoProperties;

        public Collection FeatureLinks => this.eaElement.FeatureLinks;

        public bool Update()
        {
            return this.eaElement.Update();
        }

        public string GetLastError()
        {
            return this.eaElement.GetLastError();
        }

        public void Refresh()
        {
            this.eaElement.Refresh();
        }

        public void SetAppearance(int Scope, int Item, int Value)
        {
            this.eaElement.SetAppearance(Scope, Item, Value);
        }

        public string GetRelationSet(EnumRelationSetType Type)
        {
            return this.eaElement.GetRelationSet(Type);
        }

        public string GetStereotypeList()
        {
            return this.eaElement.GetStereotypeList();
        }

        public string GetLinkedDocument()
        {
            return this.eaElement.GetLinkedDocument();
        }

        public bool LoadLinkedDocument(string Filename)
        {
            return this.eaElement.LoadLinkedDocument(Filename);
        }

        public bool SaveLinkedDocument(string Filename)
        {
            return this.eaElement.SaveLinkedDocument(Filename);
        }

        public bool ApplyUserLock()
        {
            return this.eaElement.ApplyUserLock();
        }

        public bool ReleaseUserLock()
        {
            return this.eaElement.ReleaseUserLock();
        }

        public bool ApplyGroupLock(string aGroupName)
        {
            return this.eaElement.ApplyGroupLock(aGroupName);
        }

        public bool CreateAssociationClass(int ConnectorID)
        {
            return this.eaElement.CreateAssociationClass(ConnectorID);
        }

        public bool UnlinkFromAssociation()
        {
            return this.eaElement.UnlinkFromAssociation();
        }

        public bool IsAssociationClass()
        {
            return this.eaElement.IsAssociationClass();
        }

        public bool SynchTaggedValues(string sProfile, string sStereotype)
        {
            return this.eaElement.SynchTaggedValues(sProfile, sStereotype);
        }

        public bool SynchConstraints(string sProfile, string sStereotype)
        {
            return this.eaElement.SynchConstraints(sProfile, sStereotype);
        }

        public string GetBusinessRules()
        {
            return this.eaElement.GetBusinessRules();
        }

        public bool DeleteLinkedDocument()
        {
            return this.eaElement.DeleteLinkedDocument();
        }

        public bool SetCompositeDiagram(string sGUID)
        {
            return this.eaElement.SetCompositeDiagram(sGUID);
        }

        public bool HasStereotype(string stereo)
        {
            return this.eaElement.HasStereotype(stereo);
        }

        public bool ImportInternalDocumentArtifact(string filenamne)
        {
            return this.eaElement.ImportInternalDocumentArtifact(filenamne);
        }

        public bool ExportInternalDocumentArtifact(string filenamne)
        {
            return this.eaElement.ExportInternalDocumentArtifact(filenamne);
        }

        public global::EA.Element Clone()
        {
            return this.eaElement.Clone();
        }

        public string GetDecisionTable()
        {
            return this.eaElement.GetDecisionTable();
        }

        public ElementGrid GetElementGrid()
        {
            return this.eaElement.GetElementGrid();
        }

        public string GetTXName(string txCode, int nFlag)
        {
            return this.eaElement.GetTXName(txCode, nFlag);
        }

        public string GetTXAlias(string txCode, int nFlag)
        {
            return this.eaElement.GetTXAlias(txCode, nFlag);
        }

        public string GetTXNote(string txCode, int nFlag)
        {
            return this.eaElement.GetTXNote(txCode, nFlag);
        }

        public void SetTXName(string txCode, string translation)
        {
            this.eaElement.SetTXName( txCode, translation);
        }

        public void SetTXAlias(string txCode, string translation)
        {
            this.eaElement.SetTXAlias(txCode, translation);
        }

        public void SetTXNote(string txCode, string translation)
        {
            this.eaElement.SetTXNote(txCode, translation);
        }

        public Chart GetChart()
        {
            return this.eaElement.GetChart();
        }


    }
}
