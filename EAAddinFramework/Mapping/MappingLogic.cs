using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using MP = MappingFramework;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Mapping
{
    /// <summary>
    /// Description of MappingLogic.
    /// </summary>
    public class MappingLogic:MP.MappingLogic
	{
		private string _description;
		internal TSF_EA.ElementWrapper _wrappedElement;
        private TSF_EA.ElementWrapper _context;

		public UML.Classes.Kernel.Element mappingElement 
		{
			get{return _wrappedElement;}
			set{this._wrappedElement = value as TSF_EA.ElementWrapper; }
		}
        private MappingLogic(TSF_EA.ElementWrapper context)
        {
            this._context = context;
        }
		public MappingLogic(string logicDescription, TSF_EA.ElementWrapper context = null) :this(context)
		{
			_description = logicDescription;
		}
		public MappingLogic(TSF_EA.ElementWrapper wrappedElement, TSF_EA.ElementWrapper context):this(context)
		{
			this.mappingElement = wrappedElement;
		}

		#region MappingLogic implementation

		public string description {
			get 
			{
				if (string.IsNullOrEmpty(_description)
				   && this.mappingElement != null)
				{
					_description = _wrappedElement.notes;
				}
				return _description;
			}
			set 
			{
				if (mappingElement != null)
				{
					_wrappedElement.notes = value;
				}
				_description = value;
			}
		}

        public NamedElement context
        {
            get
            {
                return this._context;
            }
            set
            {
                this._context = (TSF_EA.ElementWrapper)value;
            }
        }

        public void delete()
        {
            //delete the element if present
            this.mappingElement?.delete();
        }
        public string logicString => this.context?.uniqueID + this.description;
        
        public static XElement getMappingLogicElement(List<MappingLogic> mappingLogics)
        {
            if (!mappingLogics.Any())
            {
                return null;
            }
            string logicString = string.Empty;
            var xdoc = new XDocument();
            var bodyNode = new XElement("mappingLogics");
            xdoc.Add(bodyNode);
            foreach(var mappingLogic in mappingLogics)
            {
                bodyNode.Add(new XElement("mappingLogic",
                                    new XElement("context", mappingLogic.context?.uniqueID),
                                    new XElement("description", mappingLogic.description))
                                    );
            }
            return bodyNode;
        }
        public static string getMappingLogicString(List<MappingLogic> mappingLogics)
        {
            return getMappingLogicElement(mappingLogics)?.ToString();
        }
        public static List<MappingLogic> getMappingLogicsFromString(string logicsString, TSF_EA.Model model)
        {
            var mappingLogics = new List<MappingLogic>();
            if (!string.IsNullOrEmpty(logicsString))
            {
                try
                {
                    XDocument xdoc = XDocument.Load(new System.IO.StringReader(logicsString));
                    foreach (var logicNode in xdoc.Descendants("mappingLogic"))
                    {
                        string contextID = logicNode.Elements("context").FirstOrDefault()?.Value;
                        TSF_EA.ElementWrapper contextElement = model.getElementWrapperByGUID(contextID);
                        string description = logicNode.Elements("description").FirstOrDefault()?.Value;
                        //only create mapping logic if the description exists, or the contextElement exists.
                        if (!string.IsNullOrEmpty(description) || contextElement != null)
                        {
                            mappingLogics.Add(new MappingLogic(description, contextElement));
                        }
                    }
                }
                catch (System.Xml.XmlException)
                {
                    //no xml found, just plain text
                    mappingLogics.Add(new MappingLogic(logicsString));
                }
            }
            return mappingLogics;
        }


        #endregion
    }
}
