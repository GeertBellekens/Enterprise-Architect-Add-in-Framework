
using System;
using System.Collections.Generic;
using System.Xml;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;

namespace EAAddinFramework.Utilities
{
	/// <summary>
	/// Description of EASearchOutput.
	/// </summary>
	public class EASearchOutput : UML.Extended.UMLModelOutput
    {
        public List<UML.Extended.UMLModelOutPutItem> output { get; set; }
        public List<string> fields { get; set; }
        public string name { get; set; }
        private UTF_EA.Model _model;
        private XmlDocument formattedResults;

        public EASearchOutput(string name,List<string> fields,List<UML.Extended.UMLModelOutPutItem> output, UTF_EA.Model model)
        {
            this.name = name;
            //add the default fields for EA Searches
            this.fields = new List<string> { "CLASSGUID", "CLASSTYPE" };
            this.fields.AddRange(fields);
            this.output = output;
            if (this.output == null)
            {
                this.output = new List<UML.Extended.UMLModelOutPutItem>();
            }
            this._model = model;
        }
        public void show()
        {
            if (formattedResults == null)
            {
                this.formatResults();
            }
            //actually show the results in EA
            this._model.wrappedModel.RunModelSearch(this.name, "searchTerm", "searchOptions", this.formattedResults.InnerXml);
        }
        private void formatResults()
        {
            //make search xml format string
            //show as output in the search window
            XmlDocument xmlDom = new XmlDocument();
            //set root
            XmlNode xmlRoot = xmlDom.CreateElement("ReportViewData" );
            XmlAttribute uidAttr = xmlDom.CreateAttribute("UID");
            uidAttr.Value = "Valiation Results";    
            xmlRoot.Attributes.SetNamedItem(uidAttr);
            xmlDom.AppendChild(xmlRoot);
            //set fields
            XmlNode xmlFields = xmlDom.CreateElement( "Fields" );
		    xmlRoot.AppendChild(xmlFields);
            //add the fields to represent the titles
            foreach (var field in this.fields)
            {
			    XmlNode xmlField = xmlDom.CreateElement( "Field" );
			    XmlAttribute nameAttr = xmlDom.CreateAttribute("name");
			    nameAttr.Value = field;
			    xmlField.Attributes.SetNamedItem(nameAttr);
			    xmlFields.AppendChild(xmlField);
            }
            //add the rows with the actual output
            XmlNode xmlRows = xmlDom.CreateElement( "Rows" );
		    xmlRoot.AppendChild(xmlRows);
            foreach (var row in this.output)
            {
                //add default fields guid and type to the outputfields
                List<string> resultFields = new List<string>();
                if (row.outputElement != null)
                {
                    resultFields.Add(row.outputElement.uniqueID);
                    resultFields.Add(row.outputElement.GetType().Name); //TODO: find a way to effectively translate that to an EA known type
                }
                else
                {
                    resultFields.Add(string.Empty);
                    resultFields.Add(string.Empty);
                }
                //add the actual output
                resultFields.AddRange(row.outputFields);
                //create the row node
                XmlNode xmlRow = xmlDom.CreateElement( "Row" );
			    xmlRows.AppendChild (xmlRow);
                for (int i = 0; i < resultFields.Count && i < this.fields.Count; i++)
                {
                    string outputField = resultFields[i];
                    string field = fields[i];
                    //add the fields to the row
                    //field attribute
				    XmlNode xmlField = xmlDom.CreateElement( "Field" );
				    XmlAttribute nameAttr = xmlDom.CreateAttribute("name");
				    nameAttr.Value = field;
                    xmlField.Attributes.SetNamedItem(nameAttr);
				    //value attribute
				    XmlAttribute valueAttr =  xmlDom.CreateAttribute("value");
				    valueAttr.Value = outputField;
				    xmlField.Attributes.SetNamedItem(valueAttr);
				    //add the field to the row
				    xmlRow.AppendChild(xmlField);
                }
            }
            this.formattedResults = xmlDom;
        }

    }
}
