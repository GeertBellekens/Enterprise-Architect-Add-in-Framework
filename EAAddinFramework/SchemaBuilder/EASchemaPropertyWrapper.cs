
using System;
using System.Collections.Generic;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
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
		
		public EASchemaPropertyWrapper(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap)
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
				this._owner = (EASchemaElement) value;
			}
		}
		public UTF_EA.Multiplicity multiplicity
		{
			get
			{
				if (this._multiplicity == null)
				{
					string restriction = this._owner.wrappedSchemaType.GetRestriction(this.wrappedProperty.GUID);
					//only use restriction if not empty string
					if ( restriction != string.Empty)
					{
						Dictionary<string,string> parsedRestriction = this.parseRestriction(restriction);
						string lower;
						string upper;
						
						if (parsedRestriction.TryGetValue("minOccurs",out lower)
						    && parsedRestriction.TryGetValue("maxOccurs",out upper))
						{
							this._multiplicity = new UTF_EA.Multiplicity(lower, upper);
						}
					}
					if (this._multiplicity == null)
					{
						//no restriction on multiplicity, use standard cardinality
						try
						{
							this._multiplicity = new UTF_EA.Multiplicity(this.wrappedProperty.Cardinality.Replace("...",".."));
						}
						catch(ArgumentException)
						{
							//if fro some reason the cardinality is invalid we return the default multiplicity
							this._multiplicity = this.defaultMultiplicity;
						}
					}
				}
				return this._multiplicity;
			}
		}
		protected abstract UTF_EA.Multiplicity defaultMultiplicity {get;}
		/// <summary>
		/// restriction string have a form like "byRef=0;inline=0;minOccurs=1;maxOccurs=7;"
		/// So we split by ";" and then by "=" to get the individual key-value pairs in a dictionary
		/// </summary>
		/// <param name="restriction">the restriction string</param>
		/// <returns>a dictionary with the individual key-value pairs</returns>
		private Dictionary<string,string> parseRestriction(string restriction)
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
					    && ! parsedRestriction.ContainsKey(key))
					{
						parsedRestriction.Add(key, value);
					}
				}
			}
			return parsedRestriction;
		}

	}
}
