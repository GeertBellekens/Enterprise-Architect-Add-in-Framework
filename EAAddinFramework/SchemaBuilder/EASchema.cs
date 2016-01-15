
using System;
using System.Collections.Generic;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using EAAddinFramework.Utilities;

namespace EAAddinFramework.SchemaBuilder
{
	/// <summary>
	/// The EA Specific implementation of the Schema, wrapping the EA.SchemaComposer object
	/// </summary>
	public class EASchema: SBF.Schema
	{
		private UTF_EA.Model model;
		private EA.SchemaComposer wrappedComposer;
		private HashSet<SBF.SchemaElement> schemaElements = null;
		/// <summary>
		/// Constructor of the EASchema. Only to be used by the EASchemaBuilderFactory
		/// </summary>
		/// <param name="model">The model containing this Scheam</param>
		/// <param name="composer">The EA.SchemaComposer object to be wrapped</param>
		internal EASchema(UTF_EA.Model model, EA.SchemaComposer composer)
		{
			this.model = model;
			this.wrappedComposer = composer;
		}
		/// <summary>
		/// the SchemaElements owned by this Schema
		/// </summary>
		public HashSet<SBF.SchemaElement> elements {
			get 
			{
				if (schemaElements == null)
				{
					schemaElements = new HashSet<SBF.SchemaElement>();
					foreach (EA.SchemaType schemaType in getSchemaTypes ()) 
					{
						schemaElements.Add(EASchemaBuilderFactory.getInstance(this.model).createSchemaElement(this,schemaType));
					}
				}
				return schemaElements;
			}
			set {
				throw new NotImplementedException();
			}
		}
		/// <summary>
		/// gets the EA.SchemaTypes from the enumerator
		/// </summary>
		/// <returns>all the EA.Schematypes in this schema</returns>
		private HashSet<EA.SchemaType> getSchemaTypes()
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
		/// <summary>
		/// returns the SchemaElement that corresponds with the given UML element
		/// </summary>
		/// <param name="umlElement">the source UMLElement</param>
		/// <returns></returns>
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
		/// <summary>
		/// creates a subset of the source model with only the properties and associations used in this schema
		/// </summary>
		/// <param name="destinationPackage">the package to create the subset in</param>
		public void createSubsetModel(UML.Classes.Kernel.Package destinationPackage)
		{
			//loop the elemets to create the subSetElements
			foreach (EASchemaElement schemaElement in this.elements) 
			{
				//only create subset elements for classes, not for datatypes
				if (schemaElement.sourceElement is UML.Classes.Kernel.Class)
				{
					schemaElement.createSubsetElement(destinationPackage);
					Logger.log("after EASchema::creating single subset element");
				}
			}
			Logger.log("after EASchema::creating subsetelements");
			// then loop them again to create the associations
			foreach (EASchemaElement schemaElement in this.elements) 
			{
				//only create subset elements for classes, not for datatypes
				if (schemaElement.sourceElement is UML.Classes.Kernel.Class)
				{
					schemaElement.createSubsetAssociations();
					Logger.log("after EASchema::creating single subset association");
				}
				// and to resolve the attributes types to subset types if required
				schemaElement.resolveAttributetypes(this.schemaElements);
				Logger.log("after EASchema::resolving attributes");
				//and add a dependency from the schemaElement to the type of the attributes
				schemaElement.addAttributeTypeDependencies();
				Logger.log("after EASchema::adding attribuetypeDependencies");
			}

		}
	}
}
