
using System;
using System.Collections.Generic;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.SchemaBuilder
{
	/// <summary>
	/// Description of Schema.
	/// </summary>
	public class EASchema: SBF.Schema
	{
		private UTF_EA.Model model;
		private EA.SchemaComposer wrappedComposer;
		public EASchema(UTF_EA.Model model, EA.SchemaComposer composer)
		{
			this.model = model;
			this.wrappedComposer = composer;
		}
		
		public HashSet<SBF.SchemaElement> elements {
			get 
			{
				HashSet<SBF.SchemaElement> schemaElements = new HashSet<SchemaBuilderFramework.SchemaElement>();
				foreach (EA.SchemaType schemaType in getSchemaTypes ()) 
				{
					schemaElements.Add(EASchemaBuilderFactory.getInstance(this.model).createSchemaElement(this,schemaType));
				}
				return schemaElements;
			}
			set {
				throw new NotImplementedException();
			}
		}
		private HashSet<EA.SchemaType> getSchemaTypes ()
		{
			HashSet<EA.SchemaType> schemaTypes = new HashSet<EA.SchemaType>();
			EA.SchemaTypeEnum schemaTypeEnumerator = wrappedComposer.SchemaTypes;
            EA.SchemaType schemaType = schemaTypeEnumerator.GetFirst();
            while (schemaType != null)
            {
                schemaTypes.Add(schemaType);
                schemaType = schemaTypeEnumerator.GetNext();
            }
            return schemaTypes;
		}
		internal EASchemaElement getSchemaElementForUMLElement(UML.Classes.Kernel.Element umlElement)
		{
			EASchemaElement result = null;
			foreach (EASchemaElement schemaElement in this.elements) 
			{
				if (schemaElement.sourceElement.Equals(umlElement))
				{
					result = schemaElement;
				}
			}
			return result;
		}
	}
}
