using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public abstract class EADBTaggedValue: EADBBase
    {
        internal static Dictionary<String, int> staticColumnNames;
        protected override Dictionary<String, int> columnNames
        {
            get
            {
                if (staticColumnNames == null)
                {
                    staticColumnNames = this.getColumnNames("select * from t_attributeTag ");
                }
                return staticColumnNames;
            }

        }

        protected EADBTaggedValue(Model model): base(model)
        {
        }
        
        protected EADBTaggedValue(Model model, List<string> propertyValues)
            : base(model,  propertyValues)
        {
        }
        public virtual int PropertyID  
        {
            get => this.getIntFromProperty("PropertyID");
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
            get => this.getIntFromProperty("ElementID");
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
