using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class EADBAttributeConstraint
    {
        internal static List<String> columnNames;
        private static void initializeColumnNames(Model model)
        {
            columnNames = model.getDataSetFromQuery("select top 1 * from t_attributeconstraints ", true).FirstOrDefault();
        }


        public static List<EADBAttributeConstraint> getEADBAttributeConstraintsForAttributeIDs(IEnumerable<string> attributeIDs, Model model)
        {
            var elements = new List<EADBAttributeConstraint>();
            if (attributeIDs == null || attributeIDs.Count() == 0) return elements;
            var results = model.getDataSetFromQuery($"select * from t_attributeconstraints a where a.ID in ({string.Join(",", attributeIDs)})", false);
            foreach (var propertyValues in results)
            {
                elements.Add(new EADBAttributeConstraint(model, propertyValues));
            }
            return elements;
        }
        private Model model { get; set; }
        private global::EA.AttributeConstraint _eaAttributeConstraint;
        private global::EA.AttributeConstraint eaAttributeConstraint
        {
            get
            {
                if (this._eaAttributeConstraint == null)
                {
                    //first get element that owns the tagged value, then get the tagged value from the list of tagged values
                    var owner = this.model.wrappedModel.GetAttributeByID(this.AttributeID);
                    foreach (global::EA.AttributeConstraint constraint in owner.Constraints)
                    {
                        if (constraint.Name == this.Name)
                        {
                            this._eaAttributeConstraint = constraint;
                            break;//found it
                        }
                    }
                }
                return this._eaAttributeConstraint;
            }
            set => this._eaAttributeConstraint = value;
        }
        private Dictionary<string, string> properties { get; set; }

        private EADBAttributeConstraint(Model model)
        {
            if (columnNames == null)
            {
                initializeColumnNames(model);
            }
            this.model = model;
        }
        public EADBAttributeConstraint(Model model, List<string> propertyValues)
            : this(model)
        {
            this.properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < columnNames.Count; i++)
            {
                this.properties.Add(columnNames[i], propertyValues[i]);
            }
        }

        public EADBAttributeConstraint(Model model, global::EA.AttributeConstraint attributeConstraint)
            : this(model)
        {
            this.eaAttributeConstraint = attributeConstraint;
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
            this.Name = this.eaAttributeConstraint.Name;
            this.Notes = this.eaAttributeConstraint.Notes;
            this.Type = this.eaAttributeConstraint.Type;
            this.AttributeID = this.eaAttributeConstraint.AttributeID;
        }

        public string Name
        {
            get => this.properties["Constraint"];
            set => this.properties["Constraint"] = value;
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
        public string Type
        {
            get => this.properties["Type"];
            set => this.properties["Type"] = value;
        }
        public string Notes
        {
            get => this.properties["Notes"];
            set => this.properties["Notes"] = value;
        }
        public ObjectType ObjectType => ObjectType.otAttributeConstraint;

      
        public bool Update()
        {
            this.eaAttributeConstraint.Name= this.Name;
            this.eaAttributeConstraint.Notes = this.Notes;
            this.eaAttributeConstraint.Type = this.Type;
            this.eaAttributeConstraint.AttributeID = this.AttributeID ;
            var updateResult = this.eaAttributeConstraint.Update();
            //aver saving we need to refresh the properties
            updateFromWrappedElement();
            return updateResult;
        }
    }
}