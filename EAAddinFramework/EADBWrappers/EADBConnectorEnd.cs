using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBConnectorEnd : EADBBase
    {
        private static Dictionary<String, int> staticColumnNames = null;
        const string selectQuery = "select c.* from t_connector c";
        protected override Dictionary<String, int> columnNames
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

        public bool isSource { get; private set; }
        public EADBConnectorEnd(Model model, IndexedList properties, bool isSource)
            : base(model)
        {
            this.properties = properties;
            this.isSource = isSource;
        }

        public EADBConnectorEnd(Model model, global::EA.ConnectorEnd connectorEnd)
            : base(model)
        {
            this.eaConnectorEnd = connectorEnd;
            this.updateFromWrappedElement();
        }

        private global::EA.ConnectorEnd _eaConnectorEnd;
        private global::EA.ConnectorEnd eaConnectorEnd
        {
            get
            {
                if (this._eaConnectorEnd == null)
                {
                    var connector = this.model.wrappedModel.GetConnectorByGuid(this.properties["ea_guid"]);
                    this._eaConnectorEnd = this.isSource ? connector.ClientEnd : connector.SupplierEnd;
                }
                return this._eaConnectorEnd;
            }
            set => this._eaConnectorEnd = value;
        }
        private void updateFromWrappedElement()
        {
            try
            {
                this.isSource = this.eaConnectorEnd.End == "Client";
                this.Cardinality = this.eaConnectorEnd.Cardinality;
                this.Visibility = this.eaConnectorEnd.Visibility;
                this.Role = this.eaConnectorEnd.Role;
                this.RoleType = this.eaConnectorEnd.RoleType;
                this.RoleNote = this.eaConnectorEnd.RoleNote;
                this.Containment = this.eaConnectorEnd.Containment;
                this.Aggregation = this.eaConnectorEnd.Aggregation;
                this.Ordering = this.eaConnectorEnd.Ordering;
                this.Qualifier = this.eaConnectorEnd.Qualifier;
                this.IsChangeable = this.eaConnectorEnd.IsChangeable;
                //this.StereotypeEx = this.eaConnectorEnd.StereotypeEx;
                this.Stereotype = this.eaConnectorEnd.Stereotype;
                this.Navigable = this.eaConnectorEnd.Navigable;
                this.AllowDuplicates = this.eaConnectorEnd.AllowDuplicates;
                this.OwnedByClassifier = this.eaConnectorEnd.OwnedByClassifier;
                this.Derived = this.eaConnectorEnd.Derived;
                this.DerivedUnion = this.eaConnectorEnd.DerivedUnion;
                this.Alias = this.eaConnectorEnd.Alias;
            }
            catch (Exception e)
            {
                Logger.logError($"{e.Message} {e.StackTrace}");
            }


        }
        public bool Update()
        {
            try
            {
                bool toUpdate = false;
                if (this.eaConnectorEnd.Cardinality != this.Cardinality)
                {
                    this.eaConnectorEnd.Cardinality = this.Cardinality;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.Visibility != this.Visibility)
                {
                    this.eaConnectorEnd.Visibility = this.Visibility;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.Role != this.Role)
                {
                    this.eaConnectorEnd.Role = this.Role;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.RoleType != this.RoleType)
                {
                    this.eaConnectorEnd.RoleType = this.RoleType;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.RoleNote != this.RoleNote)
                {
                    this.eaConnectorEnd.RoleNote = this.RoleNote;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.Containment != this.Containment)
                {
                    this.eaConnectorEnd.Containment = this.Containment;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.Aggregation != this.Aggregation)
                {
                    this.eaConnectorEnd.Aggregation = this.Aggregation;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.Ordering != this.Ordering)
                {
                    this.eaConnectorEnd.Ordering = this.Ordering;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.Qualifier != this.Qualifier)
                {
                    this.eaConnectorEnd.Qualifier = this.Qualifier;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.IsChangeable != this.IsChangeable)
                {
                    this.eaConnectorEnd.IsChangeable = this.IsChangeable;
                    toUpdate = true;
                }
                // commented out to prevent exceptions
                //if (this.eaConnectorEnd.StereotypeEx != this.StereotypeEx)
                //{
                //    this.eaConnectorEnd.StereotypeEx = this.StereotypeEx;
                //    toUpdate = true;
                //}
                if (this.eaConnectorEnd.Stereotype != this.Stereotype)
                {
                    this.eaConnectorEnd.Stereotype = this.Stereotype;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.Navigable != this.Navigable)
                {
                    this.eaConnectorEnd.Navigable = this.Navigable;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.AllowDuplicates != this.AllowDuplicates)
                {
                    this.eaConnectorEnd.AllowDuplicates = this.AllowDuplicates;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.OwnedByClassifier != this.OwnedByClassifier)
                {
                    this.eaConnectorEnd.OwnedByClassifier = this.OwnedByClassifier;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.Derived != this.Derived)
                {
                    this.eaConnectorEnd.Derived = this.Derived;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.DerivedUnion != this.DerivedUnion)
                {
                    this.eaConnectorEnd.DerivedUnion = this.DerivedUnion;
                    toUpdate = true;
                }
                if (this.eaConnectorEnd.Alias != this.Alias)
                {
                    this.eaConnectorEnd.Alias = this.Alias;
                    toUpdate = true;
                }
                // Only update the API object if needed
                var updateResult = true;
                if (toUpdate)
                {
                    updateResult = this.eaConnectorEnd.Update();
                }
                this.updateFromWrappedElement();
                return updateResult;
            }
            catch (Exception e)
            {
                Logger.logError($"{e.Message} {e.StackTrace}");
                return false;
            }
        }

        public string GetLastError()
        {
            return this.eaConnectorEnd.GetLastError();
        }

        public string End => this.isSource ? "Client" : "Supplier";

        public string Cardinality
        {
            get => this.properties[this.isSource ? "SourceCard" : "DestCard"];
            set => this.properties[this.isSource ? "SourceCard" : "DestCard"] = value;
        }
        public string Visibility
        {
            get => this.properties[this.isSource ? "SourceAccess" : "DestAccess"];
            set => this.properties[this.isSource ? "SourceAccess" : "DestAccess"] = value;
        }
        public string Role
        {
            get => this.properties[this.isSource ? "SourceRole" : "DestRole"];
            set => this.properties[this.isSource ? "SourceRole" : "DestRole"] = value;
        }
        public string RoleType
        {
            get => this.properties[this.isSource ? "SourceRoleType" : "DestRoleType"];
            set => this.properties[this.isSource ? "SourceRoleType" : "DestRoleType"] = value;
        }
        public string RoleNote
        {
            get => this.properties[this.isSource ? "SourceRoleNote" : "DestRoleNote"];
            set => this.properties[this.isSource ? "SourceRoleNote" : "DestRoleNote"] = value;
        }
        public string Containment
        {
            get => this.properties[this.isSource ? "SourceContainment" : "DestContainment"];
            set => this.properties[this.isSource ? "SourceContainment" : "DestContainment"] = value;
        }
        public int Aggregation
        {
            get => this.getIntFromProperty(this.isSource ? "SourceIsAggregate" : "DestIsAggregate");
            set => this.properties[this.isSource ? "SourceIsAggregate" : "DestIsAggregate"] = value.ToString();
        }
        public int Ordering
        {
            get => this.getIntFromProperty(this.isSource ? "SourceIsOrdered" : "DestIsOrdered");
            set => this.properties[this.isSource ? "SourceIsOrdered" : "DestIsOrdered"] = value.ToString();
        }
        public string Qualifier
        {
            get => this.properties[this.isSource ? "SourceQualifier" : "DestQualifier"];
            set => this.properties[this.isSource ? "SourceQualifier" : "DestQualifier"] = value;
        }
        public string Constraint
        {
            get => this.properties[this.isSource ? "SourceConstraint" : "DestConstraint"];
            set => this.properties[this.isSource ? "SourceConstraint" : "DestConstraint"] = value;
        }
        //public bool IsNavigable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); } //according to the documentation this property should not be used, so we don't have it here.
        public string IsChangeable
        {
            get => this.properties[this.isSource ? "SourceChangeable" : "DestChangeable"];
            set => this.properties[this.isSource ? "SourceChangeable" : "DestChangeable"] = value;
        }
        public string Stereotype
        {
            get => this.properties[this.isSource ? "SourceStereotype" : "DestStereotype"];
            set => this.properties[this.isSource ? "SourceStereotype" : "DestStereotype"] = value;
        }
        public string Navigable
        {
            get
            {
                var navigableValue = KeyValuePairsHelper.getValueForKey(this.properties[this.isSource ? "SourceStyle" : "DestStyle"], "Navigable");
                if (!string.IsNullOrEmpty(navigableValue))
                {
                    return navigableValue;
                }
                //If not specified in the database we use the value of the EAConnectorEnd.
                //There is no simple default as it changes between types of connectors
                return this.eaConnectorEnd.Navigable;
            }
            set => KeyValuePairsHelper.setValueForKey(this.properties[this.isSource ? "SourceStyle" : "DestStyle"], "Navigable", value);
        }


        public bool AllowDuplicates
        {
            get => KeyValuePairsHelper.getValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "AllowDuplicates")
                             == "1";
            set => this.properties[this.isSource ? "SourceStyle" : "DestStyle"] =
                    KeyValuePairsHelper.setValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "AllowDuplicates"
                            , value ? "1" : "0");
        }
        public bool OwnedByClassifier
        {
            get => KeyValuePairsHelper.getValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "Owned")
                            == "1";
            set => this.properties[this.isSource ? "SourceStyle" : "DestStyle"] =
                    KeyValuePairsHelper.setValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "Owned"
                            , value ? "1" : "0");
        }
        public bool Derived
        {
            get => KeyValuePairsHelper.getValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "Derived")
                            == "1";
            set => this.properties[this.isSource ? "SourceStyle" : "DestStyle"] =
                    KeyValuePairsHelper.setValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "Derived"
                            , value ? "1" : "0");
        }
        public bool DerivedUnion
        {
            get => KeyValuePairsHelper.getValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "Union")
                            == "1";
            set => this.properties[this.isSource ? "SourceStyle" : "DestStyle"] =
                    KeyValuePairsHelper.setValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "Union"
                            , value ? "1" : "0");
        }
        public string Alias
        {
            get => KeyValuePairsHelper.getValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "alias") ?? String.Empty;
            set => this.properties[this.isSource ? "SourceStyle" : "DestStyle"] =
                    KeyValuePairsHelper.setValueForKey(
                            this.properties[this.isSource ? "SourceStyle" : "DestStyle"]
                            , "alias"
                            , value);
        }

        private string _StereotypeEx = null;
        public string StereotypeEx
        {
            get
            {
                if (_StereotypeEx == null)
                {
                    var xrefType = this.isSource ? "connectorSrcEnd property" : "connectorDestEnd property";
                    var xrefDescription = this.model.getFirstValueFromQuery($@"select x.Description from t_xref x
                                                                            where x.Client = '{this.properties["ea_guid"]}'
                                                                            and Name = 'Stereotypes'
                                                                            and Type = '{xrefType}'", "Description");
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

        public Collection TaggedValues => this.eaConnectorEnd.TaggedValues;

        public ObjectType ObjectType => global::EA.ObjectType.otConnectorEnd;

    }
}
