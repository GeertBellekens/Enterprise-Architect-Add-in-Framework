using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBConnector : EADBBase
    {
        internal static List<String> columnNames;
        private static List<String> getColumnNames(Model model)
        {
            if (columnNames == null)
            {
                columnNames = model.getDataSetFromQuery("select top 1 * from t_connector ", true).FirstOrDefault();
            }
            return columnNames;
        }
        protected override void initializeColumnNames()
        {
            getColumnNames(this.model);
        }
        public static EADBConnector getEADBConnectorForConnectorID(int connectorID, Model model)
        {
            return getEADBConnectorsForConnectorIDs(new List<string>() { connectorID.ToString() }, model).FirstOrDefault();
        }

        public static List<EADBConnector> getEADBConnectorsForConnectorIDs(List<string> connectorIDs, Model model)
        {
            var elements = new List<EADBConnector>();
            if (connectorIDs == null || connectorIDs.Count == 0)
            {
                return elements; //return emtpy
            }
            var results = model.getDataSetFromQuery($"select * from t_connector c where c.Connector_ID in ({string.Join(",", connectorIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBConnector(model, propertyValues));
            }
            return elements;
        }
        public static EADBConnector getEADBConnectorForConnectorGUID(string connectorGUID, Model model)
        {
            return getEADBConnectorsForConnectorGUIDs(new List<string>() { connectorGUID }, model).FirstOrDefault();
        }
        public static List<EADBConnector> getEADBConnectorsForConnectorGUIDs(List<string> connectorGUIDs, Model model)
        {
            var elements = new List<EADBConnector>();
            if (connectorGUIDs == null || connectorGUIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($"select * from t_connector c where c.ea_guid in ('{string.Join("','", connectorGUIDs)}')", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBConnector(model, propertyValues));
            }
            return elements;
        }
        public static List<EADBConnector> getEADBConnectorsForElementID(int elementID, Model model)
        {
            return getEADBConnectorsForElementIDs(new List<string>() { elementID.ToString() }, model);
        }
        public static List<EADBConnector> getEADBConnectorsForElementIDs(List<string> elementIDs, Model model)
        {
            var elements = new List<EADBConnector>();
            var results = model.getDataSetFromQuery($@"select * from t_connector c where c.Start_Object_ID in ({string.Join(",", elementIDs)})
                                                    union
                                                    select * from t_connector c where c.End_Object_ID in ({string.Join(",", elementIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBConnector(model, propertyValues));
            }
            return elements;
        }
        public static List<EADBConnector> getEADBConnectorsForPackageIDs(List<string> PackageIDs, Model model)
        {
            var elements = new List<EADBConnector>();
            var results = model.getDataSetFromQuery($@"select c.* from t_connector c 
                                                    inner join t_object o on o.Object_ID = c.Start_Object_ID
                                                    where o.Package_ID in ({string.Join(",", PackageIDs)})
                                                    union
                                                    select c.* from t_connector c 
                                                    inner join t_object o on o.Object_ID = c.End_Object_ID
                                                    where o.Package_ID in ({string.Join(",", PackageIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBConnector(model, propertyValues));
            }
            return elements;
        }


        private global::EA.Connector _eaConnector;
        public global::EA.Connector eaConnector
        {
            get
            {
                if (this._eaConnector == null)
                {
                    this._eaConnector = this.model.wrappedModel.GetConnectorByGuid(this.ConnectorGUID);
                }
                return this._eaConnector;
            }
            private set => this._eaConnector = value;
        }



        public EADBConnector(Model model, List<string> propertyValues)
            : base(model, getColumnNames(model), propertyValues)
        { }
        public EADBConnector(Model model, int connectorID)
            : this(model, model.getDataSetFromQuery($"select * from t_connector c where c.Connector_ID = {connectorID}", false).FirstOrDefault())
        { }
        public EADBConnector(Model model, string uniqueID)
            : this(model, model.getDataSetFromQuery($"select * from t_connector c where c.ea_guid = {uniqueID}", false).FirstOrDefault())
        { }
        public EADBConnector(Model model, global::EA.Connector connector)
            : base(model)
        {
            this.eaConnector = connector;
            //initialize properties empty
            this.fillPropertiesEmpty(columnNames);
            updateFromWrappedElement();
        }

        private void updateFromWrappedElement()
        {
            this.ConnectorID = this.eaConnector.ConnectorID;

            //update properties from eaAttribute
            
            this.Name = this.eaConnector.Name;
            this.Direction = this.eaConnector.Direction;
            this.Notes = this.eaConnector.Notes;
            this.Type = this.eaConnector.Type;
            this.Subtype = this.eaConnector.Subtype;
            this.ClientID = this.eaConnector.ClientID;
            this.SupplierID = this.eaConnector.SupplierID;
            this.SequenceNo = this.eaConnector.SequenceNo;
            this.Stereotype = this.eaConnector.Stereotype;
            this.VirtualInheritance = this.eaConnector.VirtualInheritance;
            this.ConnectorGUID = this.eaConnector.ConnectorGUID;
            this.IsRoot = this.eaConnector.IsRoot;
            this.IsLeaf = this.eaConnector.IsLeaf;
            this.IsSpec = this.eaConnector.IsSpec;
            this.RouteStyle = this.eaConnector.RouteStyle;
            this.StyleEx = this.eaConnector.StyleEx;
            this.EventFlags = this.eaConnector.EventFlags;
            this.DiagramID = this.eaConnector.DiagramID;
            this.StartPointX = this.eaConnector.StartPointX;
            this.StartPointY = this.eaConnector.StartPointY;
            this.EndPointX = this.eaConnector.EndPointX;
            this.EndPointY = this.eaConnector.EndPointY;
            this.StateFlags = this.eaConnector.StateFlags;
            this.StereotypeEx = this.eaConnector.StereotypeEx;

            //add also ID
            this.ConnectorID = this.eaConnector.ConnectorID;
        }

        public int ConnectorID
        {
            get => this.getIntFromProperty("Connector_ID");
            private set => this.properties["Connector_ID"] = value.ToString();
        }

        public string Name
        {
            get => this.properties["Name"];
            set => this.properties["Name"] = value;
        }
        public string Direction
        {
            get => this.properties["Direction"];
            set => this.properties["Direction"] = value;
        }
        public string Notes
        {
            get => this.properties["Notes"];
            set => this.properties["Notes"] = value;
        }
        public string Type
        {
            get => this.properties["Connector_Type"];
            set => this.properties["Connector_Type"] = value;
        }

        public string Subtype
        {
            get => this.properties["SubType"];
            set => this.properties["SubType"] = value;
        }

        public int ClientID
        {
            get => this.getIntFromProperty("Start_Object_ID");
            set => this.properties["Start_Object_ID"] = value.ToString();
        }
        public int SupplierID
        {
            get => this.getIntFromProperty("End_Object_ID");
            set => this.properties["End_Object_ID"] = value.ToString();
        }
        public int SequenceNo
        {
            get => this.getIntFromProperty("SeqNo");
            set => this.properties["SeqNo"] = value.ToString();
        }
        public string Stereotype
        {
            get => this.properties["Stereotype"];
            set => this.properties["Stereotype"] = value;
        }
        public string VirtualInheritance
        {
            get => this.properties["VirtualInheritance"];
            set => this.properties["VirtualInheritance"] = value;
        }
        public string ConnectorGUID
        {
            get => this.properties["ea_guid"];
            private set => this.properties["ea_guid"] = value;
        }
        public bool IsRoot
        {
            get => this.getBoolFromProperty("IsRoot");
            set => this.setBoolToProperty("IsRoot", value);
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

        public int RouteStyle
        {
            get => this.getIntFromProperty("RouteStyle");
            set => this.properties["RouteStyle"] = value.ToString();
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
        public int DiagramID
        {
            get => this.getIntFromProperty("DiagramID");
            set => this.properties["DiagramID"] = value.ToString();
        }
        public int StartPointX
        {
            get => this.getIntFromProperty("PtStartX");
            set => this.properties["PtStartX"] = value.ToString();
        }
        public int StartPointY
        {
            get => this.getIntFromProperty("PtStartY");
            set => this.properties["PtStartY"] = value.ToString();
        }
        public int EndPointX
        {
            get => this.getIntFromProperty("PtEndX");
            set => this.properties["PtEndX"] = value.ToString();
        }
        public int EndPointY
        {
            get => this.getIntFromProperty("PtEndY");
            set => this.properties["PtEndY"] = value.ToString();
        }
        public string StateFlags
        {
            get => this.properties["StateFlags"];
            set => this.properties["StateFlags"] = value;
        }
        //alias is stored as part of the StyleEx property
        public string Alias
        {
            get => KeyValuePairsHelper.getValueForKey("alias", this.StyleEx) ?? String.Empty;
            set => this.StyleEx = KeyValuePairsHelper.setValueForKey("alias", value, this.StyleEx);
        }
        private string _StereotypeEx = null;
        public string StereotypeEx
        {
            get
            {
                if (_StereotypeEx == null)
                {
                    var xrefDescription = this.model.getFirstValueFromQuery($"select x.Description from t_xref x where x.Name = 'Stereotypes' and x.Client = '{this.ConnectorGUID}'", "Description");
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
        public bool Update()
        {
            this.eaConnector.Name = this.Name;
            this.eaConnector.Direction = this.Direction;
            this.eaConnector.Notes = this.Notes;
            this.eaConnector.Type = this.Type;
            this.eaConnector.Subtype = this.Subtype;
            this.eaConnector.ClientID = this.ClientID;
            this.eaConnector.SupplierID = this.SupplierID;
            this.eaConnector.SequenceNo = this.SequenceNo;
            this.eaConnector.Stereotype = this.Stereotype;
            this.eaConnector.VirtualInheritance = this.VirtualInheritance;
            this.eaConnector.IsRoot = this.IsRoot;
            this.eaConnector.IsLeaf = this.IsLeaf;
            this.eaConnector.IsSpec = this.IsSpec;
            this.eaConnector.RouteStyle = this.RouteStyle;
            this.eaConnector.StyleEx = this.StyleEx;
            this.eaConnector.EventFlags = this.EventFlags;
            this.eaConnector.DiagramID = this.DiagramID;
            this.eaConnector.StartPointX = this.StartPointX;
            this.eaConnector.StartPointY = this.StartPointY;
            this.eaConnector.EndPointX = this.EndPointX;
            this.eaConnector.EndPointY = this.EndPointY;
            this.eaConnector.StateFlags = this.StateFlags;
            this.eaConnector.StereotypeEx = this.StereotypeEx;
            //isderived is a bit special because it's stored in the custom properties
            this.updateIsDerived();
            var updateResult = this.eaConnector.Update();
            this.updateFromWrappedElement();
            return updateResult;

        }
        private void updateIsDerived()
        {
            foreach (global::EA.CustomProperty property in this.eaConnector.CustomProperties)
            {
                if (property.Name == "isDerived")
                {
                    property.Value = this.isDerived ? "-1" : "0";
                    break;
                }
            }
        }
        public bool IsConnectorValid()
        {
            return this.eaConnector.IsConnectorValid();
        }
        public string GetLastError()
        {
            return this.eaConnector.GetLastError();
        }
        public string GetTXName(string txCode, int nFlag)
        {
            return this.eaConnector.GetTXName(txCode, nFlag);
        }

        public string GetTXAlias(string txCode, int nFlag)
        {
            return this.eaConnector.GetTXAlias(txCode, nFlag);
        }

        public string GetTXNote(string txCode, int nFlag)
        {
            return this.eaConnector.GetTXNote(txCode, nFlag);
        }

        public void SetTXName(string txCode, string translation)
        {
            this.eaConnector.SetTXName(txCode, translation);
        }

        public void SetTXAlias(string txCode, string translation)
        {
            this.eaConnector.SetTXAlias(txCode, translation);
        }

        public void SetTXNote(string txCode, string translation)
        {
            this.eaConnector.SetTXNote(txCode, translation);
        }

        public bool SetFeatureLink(EnumFeatureLinkType startType, string guidStart, EnumFeatureLinkType endType, string guidEnd)
        {
            throw new NotImplementedException();
        }


        public EADBConnectorEnd ClientEnd => new EADBConnectorEnd(this.model, this.properties, true);

        public EADBConnectorEnd SupplierEnd => new EADBConnectorEnd(this.model, this.properties, false);


        public Collection Constraints => this.eaConnector.Constraints;

        public Collection TaggedValues => this.eaConnector.TaggedValues;
              
        public string TransitionEvent
        {
            get => this.eaConnector.TransitionEvent;
            set => this.eaConnector.TransitionEvent = value;
        }
        public string TransitionGuard
        {
            get => this.eaConnector.TransitionGuard;
            set => this.eaConnector.TransitionGuard = value;
        }
        public string TransitionAction
        {
            get => this.eaConnector.TransitionAction;
            set => this.eaConnector.TransitionAction = value;
        }

        public int Color
        {
            get => this.eaConnector.Color;
            set => this.eaConnector.Color = value;
        }
        public int Width
        {
            get => this.eaConnector.Width;
            set => this.eaConnector.Width = value;
        }
        public Collection CustomProperties => this.eaConnector.CustomProperties;
        private bool? _isDerived;
        public bool isDerived
        {
            get
            {
                if (this._isDerived == null)
                {
                    if (this._eaConnector != null)
                    {
                        //get it from the customproperties
                        foreach (global::EA.CustomProperty property in this.eaConnector.CustomProperties)
                        {
                            if (property.Name == "isDerived")
                            {
                                this._isDerived = property.Value != "0"
                                    && !property.Value.Equals("false", StringComparison.InvariantCultureIgnoreCase);
                                break;
                            }
                        }
                        //if still not set, then it's false
                        if (this._isDerived == null)
                        {
                            this._isDerived = false;
                        }
                    }
                    else
                    {
                        //get it from the database
                        var sqlGetData = $@"select count(x.xrefID) as isDerived
                                    from t_xref x
                                    where 1=1
                                    and x.Name = 'CustomProperties'
                                    and x.type = 'connector property'
                                    and x.Description like '%@NAME=isDerived@ENDNAME;@TYPE=Boolean@ENDTYPE;@VALU=1@%' 
                                    and x.client = '{this.ConnectorGUID}'";
                        this._isDerived = int.Parse(this.model.getFirstValueFromQuery(sqlGetData, "isDerived")) > 0;
                    }
                }
                
                return this._isDerived.Value;
            }
            set
            {
                this._isDerived = value;
            }
        }
        public Properties Properties => this.eaConnector.Properties;

        public string MetaType
        {
            get => this.eaConnector.MetaType;
            set => this.eaConnector.MetaType = value;
        }
        public Collection ConveyedItems => this.eaConnector.ConveyedItems;

        public Collection TemplateBindings => this.eaConnector.TemplateBindings;

        public string ReturnValueAlias => this.eaConnector.ReturnValueAlias;

        public string MessageArguments => this.eaConnector.MessageArguments;

        public string ForeignKeyInformation => this.eaConnector.ForeignKeyInformation;

        public string FQStereotype => this.eaConnector.FQStereotype;

        public global::EA.Element AssociationClass => this.eaConnector.AssociationClass;

        public TypeInfoProperties TypeInfoProperties => this.eaConnector.TypeInfoProperties;

        public ObjectType ObjectType => global::EA.ObjectType.otConnector;

        public List<string> MiscData => new List<string>
            {this.properties["PDATA1"],
            this.properties["PDATA2"],
            this.properties["PDATA3"],
            this.properties["PDATA4"],
            this.properties["PDATA5"] };

    }
}