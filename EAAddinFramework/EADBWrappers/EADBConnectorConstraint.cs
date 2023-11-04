using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBConnectorConstraint : EADBBase
    {
        internal static Dictionary<string, int> staticColumnNames;
        const string selectQuery = "select cc.* from t_connectorconstraint cc";
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

        public static List<EADBConnectorConstraint> getEADBConnectorConstraintsForConnectorIDs(IEnumerable<int> connectorIDs, Model model)
        {
            var elements = new List<EADBConnectorConstraint>();
            if (connectorIDs == null || connectorIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($"{selectQuery} where cc.ConnectorID in ({string.Join(",", connectorIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBConnectorConstraint(model, propertyValues));
            }
            return elements;
        }
        private global::EA.ConnectorConstraint _eaConnectorConstraint;
        private global::EA.ConnectorConstraint eaConnectorConstraint
        {
            get
            {
                if (this._eaConnectorConstraint == null)
                {
                    //first get element that owns the tagged value, then get the tagged value from the list of tagged values
                    var owner = this.model.wrappedModel.GetConnectorByID(this.ConnectorID);
                    foreach (global::EA.ConnectorConstraint constraint in owner.Constraints)
                    {
                        if (constraint.Name == this.Name)
                        {
                            this._eaConnectorConstraint = constraint;
                            break;//found it
                        }
                    }
                }
                return this._eaConnectorConstraint;
            }
            set => this._eaConnectorConstraint = value;
        }

        private EADBConnectorConstraint(Model model) : base(model) { }

        public EADBConnectorConstraint(Model model, List<string> propertyValues)
            : base(model, propertyValues)
        { }

        public EADBConnectorConstraint(Model model, global::EA.ConnectorConstraint attributeConstraint)
            : this(model)
        {
            this.eaConnectorConstraint = attributeConstraint;
            updateFromWrappedElement();
        }

        private void updateFromWrappedElement()
        {
            //update properties from eaConnector
            this.Name = this.eaConnectorConstraint.Name;
            this.Notes = this.eaConnectorConstraint.Notes;
            this.Type = this.eaConnectorConstraint.Type;
            this.ConnectorID = this.eaConnectorConstraint.ConnectorID;
        }
        public bool Update()
        {
            this.eaConnectorConstraint.Name = this.Name;
            this.eaConnectorConstraint.Notes = this.Notes;
            this.eaConnectorConstraint.Type = this.Type;
            this.eaConnectorConstraint.ConnectorID = this.ConnectorID;
            var updateResult = this.eaConnectorConstraint.Update();
            //aver saving we need to refresh the properties
            updateFromWrappedElement();
            return updateResult;
        }

        public string Name
        {
            get => this.properties["Constraint"];
            set => this.properties["Constraint"] = value;
        }
        public int ConnectorID
        {
            get
            {
                int result;
                if (int.TryParse(this.properties["ConnectorID"], out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            private set => this.properties["ConnectorID"] = value.ToString();
        }
        public string Type
        {
            get => this.properties["ConstraintType"];
            set => this.properties["ConstraintType"] = value;
        }
        public string Notes
        {
            get => this.properties["Notes"];
            set => this.properties["Notes"] = value;
        }
        public ObjectType ObjectType => ObjectType.otConnectorConstraint;



    }
}