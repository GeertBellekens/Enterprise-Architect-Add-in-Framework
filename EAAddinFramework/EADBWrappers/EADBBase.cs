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

        protected abstract Dictionary<String, int> columnNames { get; }
        protected EADBBase(Model model)
        {
            this.model = model;
            this.properties = new IndexedList(columnNames);
        }
        protected EADBBase(Model model, List<string> propertyValues)
        {
            this.model = model;
            this.properties = new IndexedList(this.columnNames, propertyValues);
        }
        protected Model model { get; set; }
        protected IndexedList properties { get; set; }
        
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

        protected DateTime getDateTimeFromProperty(string propertyName)
        {
            DateTime result;
            if (DateTime.TryParse(this.properties[propertyName], out result))
            {
                return result;
            }
            else
            {
                return default;
            }
        }

        public Dictionary<string, int> getColumnNames(string selectQuery)
        {
            var newColumnNames = new Dictionary<String, int>(StringComparer.OrdinalIgnoreCase);
            var headers = model.getDataSetFromQuery(selectQuery.Replace("select ", "select top 1 "), true).FirstOrDefault();
            for (int i = 0; i < headers.Count; i++)
            {
                newColumnNames.Add(headers[i], i);
            }
            return newColumnNames;
        }

    }
}
