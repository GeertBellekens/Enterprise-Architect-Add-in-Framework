using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBElementConstraint : EADBBase
    {
        internal static Dictionary<string, int> staticColumnNames;
        const string selectQuery = "select * from t_objectconstraint oc ";
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


        public static List<EADBElementConstraint> getEADBElementConstraintsForElementIDs(IEnumerable<int> elementIDs, Model model)
        {
            var elements = new List<EADBElementConstraint>();
            if (elementIDs == null || elementIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($"{selectQuery} where oc.Object_ID in ({string.Join(",", elementIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBElementConstraint(model, propertyValues));
            }
            return elements;
        }
        private global::EA.Constraint _eaElementConstraint;
        private global::EA.Constraint eaElementConstraint
        {
            get
            {
                if (this._eaElementConstraint == null)
                {
                    //first get element that owns the tagged value, then get the tagged value from the list of tagged values
                    var owner = this.model.wrappedModel.GetElementByID(this.ParentID);
                    foreach (global::EA.Constraint constraint in owner.Constraints)
                    {
                        if (constraint.Name == this.Name)
                        {
                            this._eaElementConstraint = constraint;
                            break;//found it
                        }
                    }
                }
                return this._eaElementConstraint;
            }
            set => this._eaElementConstraint = value;
        }


        public EADBElementConstraint(Model model, List<string> propertyValues)
            : base(model, propertyValues)
        { }

        public EADBElementConstraint(Model model, global::EA.Constraint elementConstraint)
            : base(model)
        {
            this.eaElementConstraint = elementConstraint;
            updateFromWrappedElement();


        }
        public bool Update()
        {
            this.eaElementConstraint.Name = this.Name;
            this.eaElementConstraint.Notes = this.Notes;
            this.eaElementConstraint.Type = this.Type;
            this.eaElementConstraint.Weight = this.Weight;
            this.eaElementConstraint.Status = this.Status;


            var updateResult = this.eaElementConstraint.Update();
            //aver saving we need to refresh the properties
            updateFromWrappedElement();
            return updateResult;
        }
        private void updateFromWrappedElement()
        {
            //update properties from eaElement
            this.Name = this.eaElementConstraint.Name;
            this.Notes = this.eaElementConstraint.Notes;
            this.Type = this.eaElementConstraint.Type;
            this.Weight = this.eaElementConstraint.Weight;
            this.Status = this.eaElementConstraint.Status;
            this.ParentID = this.eaElementConstraint.ParentID;
        }

        public string Name
        {
            get => this.properties["Constraint"];
            set => this.properties["Constraint"] = value;
        }
        public int ParentID
        {
            get => this.getIntFromProperty("Object_ID");
            private set => this.properties["Object_ID"] = value.ToString();
        }
        public string Type
        {
            get => this.properties["ConstraintType"];
            set => this.properties["ConstraintType"] = value;
        }
        public int Weight
        {
            get => this.getIntFromProperty("Weight");
            set => this.properties["Weight"] = value.ToString();
        }
        public string Notes
        {
            get => this.properties["Notes"];
            set => this.properties["Notes"] = value;
        }
        public string Status
        {
            get => this.properties["Status"];
            set => this.properties["Status"] = value;
        }
        public ObjectType ObjectType => ObjectType.otConstraint;


        public string GetLastError()
        {
            return this.eaElementConstraint.GetLastError();
        }


    }
}