
using System;
using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using SBF = SchemaBuilderFramework;
using UML = TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.SchemaBuilder
{
    /// <summary>
    /// Description of EASchemaProperty.
    /// </summary>
    public abstract class EASchemaPropertyWrapper
    {
        protected UTF_EA.Model model;
        protected EA.SchemaProperty wrappedProperty;
        protected EASchemaElement _owner;
        protected UTF_EA.Multiplicity _multiplicity;
        private EASchemaElement _redefinedElement;
        private Dictionary<string, string> _restriction;
        private List<EASchemaElement> _choiceElements;
        List<UML.Classes.Kernel.Classifier> _choiceTypes;

        public EASchemaPropertyWrapper(UTF_EA.Model model, EASchemaElement owner, EA.SchemaProperty objectToWrap)
        {
            this._owner = owner;
            this.model = model;
            this.wrappedProperty = objectToWrap;
        }
		
        
        public SBF.SchemaElement owner
        {
            get
            {
                return this._owner;
            }
            set
            {
                this._owner = (EASchemaElement)value;
            }
        }
        public EASchemaElement redefinedElement
        {
        	get
        	{
        		if (this._redefinedElement == null)
        		{
	        		string redefinedName;
	        		if (this.restriction.TryGetValue("redefines", out redefinedName))
	        		{
	        			//redefinitions are defined by their name. The name has to be unique in the schema, so it is safe to use it to find the element.
	        			this._redefinedElement = this.owner.owner.elements.FirstOrDefault(x => x.name == redefinedName) as EASchemaElement;
	        		}
	        	}
        		return this._redefinedElement;
        	}
        }
        /// <summary>
        /// choiceElements
        /// </summary>
        public List<EASchemaElement> choiceElements
        {
            get
            {
                string choiceString;
                if (this.restriction.TryGetValue("choice", out choiceString))
                {
                    var choices = choiceString.Split(new[] { ',' }, StringSplitOptions.None);

                    if (_choiceElements == null)
                    {
                        _choiceElements = new List<EASchemaElement>();
                    }
                    else
                    {
                        _choiceElements.Clear();
                    }
                    var schemaElements = ((EASchema)owner.owner).elements;
                    if (schemaElements != null)
                    {
                        foreach (var schemaElement in schemaElements.ToList())
                        {
                            var element = (EASchemaElement) schemaElement;
                            if (null != element && choices.Contains(element.TypeID))
                            {
                                _choiceElements.Add(element);
                            }
                        }                  
                    }
                }
                return _choiceElements;
            }
        }
        public List<UML.Classes.Kernel.Classifier> choiceTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        protected Dictionary<string, string> restriction
        {
            get
            {
                if (this._restriction == null)
                {
                    string restrictionString = this._owner.wrappedSchemaType.GetRestriction(this.wrappedProperty.GUID);
                    //only use restriction if not empty string
                    if (restrictionString != string.Empty)
                    {
                        this._restriction = this.parseRestriction(restrictionString);
                    }
                    else
                    {
                        //no restriction found, set empty dictionary
                        this._restriction = new Dictionary<string, string>();
                    }
                }
                return this._restriction;
            }
        }
        public UTF_EA.Multiplicity multiplicity
        {
            get
            {
                if (this._multiplicity == null)
                {
                    string lower;
                    string upper;

                    if (this.restriction.TryGetValue("minOccurs", out lower)
                        && this.restriction.TryGetValue("maxOccurs", out upper))
                    {
                        this._multiplicity = new UTF_EA.Multiplicity(lower, upper);
                    }

                    if (this._multiplicity == null)
                    {
                        //no restriction on multiplicity, use standard cardinality
                        try
                        {
                            this._multiplicity = new UTF_EA.Multiplicity(this.wrappedProperty.Cardinality.Replace("...", ".."));
                        }
                        catch (ArgumentException)
                        {
                            //if fro some reason the cardinality is invalid we return the default multiplicity
                            this._multiplicity = this.defaultMultiplicity;
                        }
                    }
                }
                return this._multiplicity;
            }
        }
        protected abstract UTF_EA.Multiplicity defaultMultiplicity { get; }
        /// <summary>
        /// restriction string have a form like "byRef=0;inline=0;minOccurs=1;maxOccurs=7;"
        /// So we split by ";" and then by "=" to get the individual key-value pairs in a dictionary
        /// </summary>
        /// <param name="restriction">the restriction string</param>
        /// <returns>a dictionary with the individual key-value pairs</returns>
        private Dictionary<string, string> parseRestriction(string restriction)
        {
            var parsedRestriction = new Dictionary<string, string>();
            foreach (string keyValuePair in restriction.Split(';'))
            {
                string[] splittedKeyValue = keyValuePair.Split('=');
                if (splittedKeyValue.Length == 2)
                {
                    string key = splittedKeyValue[0];
                    string value = splittedKeyValue[1];
                    if (key != string.Empty
                        && !parsedRestriction.ContainsKey(key))
                    {
                        parsedRestriction.Add(key, value);
                    }
                }
            }
            return parsedRestriction;
        }


    }
}
