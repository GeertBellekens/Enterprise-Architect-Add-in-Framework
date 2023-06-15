using EA;
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public abstract class EADBBase
    {
        protected abstract List<String> columnNames { get; }
        protected EADBBase(Model model)
        {
            this.model = model;
            this.properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        protected EADBBase(Model model, List<string> propertyValues)
        :this(model)
        {
            this.fillProperties( propertyValues);
        }
        protected Model model { get; set; }
        protected Dictionary<string, string> properties { get; set; }
        protected void fillProperties(List<string> propertyValues)
        {
            for (int i = 0; i < columnNames.Count; i++)
            {
                this.properties.Add(columnNames[i], propertyValues[i]);
            }
        }
        protected void fillPropertiesEmpty()
        {
            for (int i = 0; i < columnNames.Count; i++)
            {
                this.properties.Add(columnNames[i], String.Empty);
            }
        }
        protected int getIntFromProperty(string propertyName)
        {
            int result;
            if (int.TryParse(this.properties[propertyName], out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
        protected bool getBoolFromProperty(string propertyName)
        {
            return this.properties[propertyName] == "1" ? true : false;
        }
        protected void setBoolToProperty(string propertyName, bool value)
        {
            this.properties[propertyName] = value ? "1" : "0";
        }


    }
}
