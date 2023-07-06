using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace TSF.UmlToolingFramework.Wrappers.EA
{

    public class EADBElement: EADBBase
    {
        internal static Dictionary<string, int> staticColumnNames;
        const string selectQuery = @"select o.*, x.Description as StereotypesXref 
                                    from t_object o 
                                    left join t_xref x on x.Client = o.ea_guid
				                                    and x.Name = 'Stereotypes'";
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
        public static EADBElement getEADBElementsForElementGUID(string elementGUID, Model model)
        {
            return getEADBElementsForElementGUIDs(new List<string>() { elementGUID }, model).FirstOrDefault();
        }
        public static List<EADBElement> getEADBElementsForElementGUIDs(List<string> elementGUIDs, Model model)
        {
            var elements = new List<EADBElement>();
            if (elementGUIDs == null || elementGUIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($@"{selectQuery}                
                                                    where o.ea_guid in ('{String.Join("','", elementGUIDs)}')", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBElement(model, propertyValues));
            }
            return elements;
        }
        public static EADBElement getEADBElementsForElementID(int elementID, Model model)
        {
            return getEADBElementsForElementIDs(new List<string>() { elementID.ToString() }, model).FirstOrDefault();
        }
        public static List<EADBElement> getEADBElementsForElementIDs(List<string> elementIDs, Model model)
        {
            var elements = new List<EADBElement>();
            if (elementIDs == null || ! elementIDs.Any()) return elements;
            var results = model.getDataSetFromQuery($@"{selectQuery}  
                                                    where o.Object_ID in ({String.Join(",", elementIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBElement(model, propertyValues));
            }
            return elements;
        }
        public static List<EADBElement> getEADBElementsForPackageIDs(List<string> PackageIDs, Model model, IEnumerable<string> objectTypes = null)
        {
            var elements = new List<EADBElement>();
            if (PackageIDs == null || ! PackageIDs.Any()) return elements;
            var sqlGetData = $@"{selectQuery}  
                              where 1=1
                              and o.Package_ID in ({String.Join(",", PackageIDs)})";
            if (objectTypes != null && objectTypes.Any())
            {
                sqlGetData += $"{Environment.NewLine} and o.Object_Type in ('{string.Join("','", objectTypes)}')";
            }
            var results = model.getDataSetFromQuery(sqlGetData, false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBElement(model, propertyValues));
            }
            return elements;
        }
        
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



        public EADBElement(Model model, List<string> propertyValues)
            : base(model, propertyValues)
        { }
        public EADBElement(Model model, int elementID)
            : this(model, model.getDataSetFromQuery($@"{selectQuery}
                                                    where o.Object_ID = {elementID}", false).FirstOrDefault())
        { }
        public EADBElement(Model model, string uniqueID)
            : this(model, model.getDataSetFromQuery($@"{selectQuery}  
                                                    where o.ea_guid = {uniqueID}", false).FirstOrDefault())
        { }
        public EADBElement(Model model, global::EA.Element element)
            : base(model)
        {
            this.eaElement = element;
            updateFromWrappedElement();

        }
        public bool Update()
        {
            this.eaElement.Type = this.Type;
            this.eaElement.Name = this.Name;
            this.eaElement.Alias = this.Alias;
            this.eaElement.Author = this.Author;
            this.eaElement.Version = this.Version;
            this.eaElement.Notes = this.Notes;
            this.eaElement.PackageID = this.PackageID;
            this.eaElement.Stereotype = this.Stereotype;
            this.eaElement.Subtype = this.Subtype;
            this.eaElement.Complexity = this.Complexity;
            this.eaElement.Created = this.Created;
            this.eaElement.Modified = this.Modified;
            this.eaElement.Status = this.Status;
            this.eaElement.Abstract = this.Abstract;
            this.eaElement.Visibility = this.Visibility;
            this.eaElement.Persistence = this.Persistence;
            this.eaElement.Gentype = this.Gentype;
            this.eaElement.Genfile = this.Genfile;
            this.eaElement.Header1 = this.Header1;
            this.eaElement.Header2 = this.Header2;
            this.eaElement.Phase = this.Phase;
            this.eaElement.Genlinks = this.Genlinks;
            this.eaElement.ClassifierID = this.ClassifierID;
            this.eaElement.ParentID = this.ParentID;
            this.eaElement.RunState = this.RunState;
            this.eaElement.TreePos = this.TreePos;
            this.eaElement.IsLeaf = this.IsLeaf;
            this.eaElement.IsSpec = this.IsSpec;
            this.eaElement.IsActive = this.IsActive;
            this.eaElement.Multiplicity = this.Multiplicity;
            this.eaElement.StyleEx = this.StyleEx;
            this.eaElement.EventFlags = this.EventFlags;
            this.eaElement.ActionFlags = this.ActionFlags;
            this.eaElement.Stereotype = this.StereotypeEx;
            this.eaElement.Tag = this.Tag;
            var updateresult = this.eaElement.Update();
            //after saving we need to refresh the properties
            updateFromWrappedElement();
            return updateresult;
        }
        private void updateFromWrappedElement()
        {
            //update properties from eaElement
            this.Type = this.eaElement.Type;
            this.Name = this.eaElement.Name;
            this.Alias = this.eaElement.Alias;
            this.Author = this.eaElement.Author;
            this.Version = this.eaElement.Version;
            this.Notes = this.eaElement.Notes;
            this.PackageID = this.eaElement.PackageID;
            this.Stereotype = this.eaElement.Stereotype;
            this.Subtype = this.eaElement.Subtype;
            this.Complexity = this.eaElement.Complexity;
            this.Created = this.eaElement.Created;
            this.Modified = this.eaElement.Modified;
            this.Status = this.eaElement.Status;
            this.Abstract = this.eaElement.Abstract;
            this.Visibility = this.eaElement.Visibility;
            this.Persistence = this.eaElement.Persistence;
            this.Gentype = this.eaElement.Gentype;
            this.Genfile = this.eaElement.Genfile;
            this.Header1 = this.eaElement.Header1;
            this.Header2 = this.eaElement.Header2;
            this.Phase = this.eaElement.Phase;
            this.Genlinks = this.eaElement.Genlinks;
            this.ClassifierID = this.eaElement.ClassifierID;
            this.ParentID = this.eaElement.ParentID;
            this.RunState = this.eaElement.RunState;
            this.TreePos = this.eaElement.TreePos;
            this.IsLeaf = this.eaElement.IsLeaf;
            this.IsSpec = this.eaElement.IsSpec;
            this.IsActive = this.eaElement.IsActive;
            this.Multiplicity = this.eaElement.Multiplicity;
            this.StyleEx = this.eaElement.StyleEx;
            this.EventFlags = this.eaElement.EventFlags;
            this.ActionFlags = this.eaElement.ActionFlags;
            this.StereotypeEx = this.eaElement.StereotypeEx;
            this.FQStereotype = this.eaElement.FQStereotype;
            this.Tag = this.eaElement.Tag;
            //add also id
            this.ElementID = this.eaElement.ElementID;
            this.ElementGUID = this.eaElement.ElementGUID;
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
            get => this.getBoolFromProperty("IsActive");
            set => this.setBoolToProperty("IsActive", value);
        }
        public bool IsLeaf
        {
            get => this.getBoolFromProperty("IsLeaf");
            set => this.setBoolToProperty("IsLeaf", value);

        }
        public bool IsSpec
        {
            get => this.getBoolFromProperty("IsSpec");
            set => this.setBoolToProperty("IsSpec", value);
        }
        public int Subtype
        {
            get => this.getIntFromProperty("NType");
            set => this.properties["NType"] = value.ToString();
        }
        public string Type
        {
            get => this.properties["Object_Type"];
            set => this.properties["Object_Type"] = value;
        }
        public int ClassifierID
        {
            get => getIntFromProperty("Classifier");
            set => this.properties["Classifier"] = value.ToString();
        }
        public int ClassfierID
        {
            get => this.ClassifierID;
            set => this.ClassifierID = value;
        }

        public DateTime Created
        {
            get => getDateTimeFromProperty("CreatedDate");
            set => this.properties["CreatedDate"] = value.ToString();//TODO: check if this format is OK?
        }
        public DateTime Modified
        {
            get => getDateTimeFromProperty("ModifiedDate");
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
            get => this.getIntFromProperty("Object_ID");
            private set => this.properties["Object_ID"] = value.ToString();
        }

        public int PackageID
        {
            get => this.getIntFromProperty("Package_ID");
            set => this.properties["Package_ID"] = value.ToString();
        }
        public string ElementGUID
        {
            get => this.properties["ea_guid"];
            set => this.properties["ea_guid"] = value;
        }
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
            get => this.getIntFromProperty("TPos");
            set => this.properties["TPos"] = value.ToString();
        }
        public int ParentID
        {
            get => this.getIntFromProperty("ParentID");
            set => this.properties["ParentID"] = value.ToString();
        }
        public string RunState
        {
            get => this.properties["RunState"];
            set => this.properties["RunState"] = value;
        }
        public string Tag
        {
            get => this.properties["PDATA5"];
            set => this.properties["PDATA5"] = value;
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
        public Collection Elements => this.eaElement.Elements;
        public Collection Diagrams => this.eaElement.Diagrams;
        public Collection Partitions => this.eaElement.Partitions;
        public Collection CustomProperties => this.eaElement.CustomProperties;
        public Collection StateTransitions => this.eaElement.StateTransitions;
        public Collection EmbeddedElements => this.eaElement.EmbeddedElements;
        public Collection BaseClasses => this.eaElement.BaseClasses;
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
        private string _FQStereotype = null;
        public string FQStereotype
        {
            get
            {
                if (_FQStereotype == null)
                {
                    var xrefDescription = this.properties["StereotypesXref"];
                    if (string.IsNullOrEmpty(xrefDescription))
                    {
                        this._FQStereotype = string.Empty;
                    }
                    else
                    {
                        this._FQStereotype =  KeyValuePairsHelper.getValueForKey("FQName", xrefDescription);
                        //if FQName is not found then try name
                        if (string.IsNullOrEmpty(this._FQStereotype))
                        {
                            this._FQStereotype = KeyValuePairsHelper.getValueForKey("Name", xrefDescription);
                        } 
                        //if still null then return empty string
                        if (this._FQStereotype == null)
                        {
                            this._FQStereotype = String.Empty;
                        }
                    }
                }
                return this._FQStereotype;
            }
            private set => this._FQStereotype = value ?? String.Empty;
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

        

        public bool IsRoot { set => this.eaElement.IsRoot = value; }

        public string FQName => this.eaElement.FQName;

        public TypeInfoProperties TypeInfoProperties => this.eaElement.TypeInfoProperties;

        public Collection FeatureLinks => this.eaElement.FeatureLinks;
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
            this.eaElement.SetTXName(txCode, translation);
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
