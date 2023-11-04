using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class IndexedList
    {
        private Dictionary<string, int> columnNames;
        private List<string> propertyValues;
        public IndexedList( Dictionary<string, int> columnNames, List<string> propertyValues)
        {
            this.columnNames = columnNames;
            this.propertyValues = propertyValues;
        }
        public IndexedList(Dictionary<string, int> columnNames)
        {
            this.columnNames = columnNames;
            this.propertyValues = Enumerable.Repeat(string.Empty, columnNames.Count).ToList();

        }
        public string this[string columnName]
        {
            get => propertyValues[columnNames[columnName]];
            set => propertyValues[columnNames[columnName]] = value;
        }
    }
}
