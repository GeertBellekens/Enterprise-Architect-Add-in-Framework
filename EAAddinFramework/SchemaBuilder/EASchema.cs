
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
		/// finds the Schema Element for which the given element could be the subset element
		/// </summary>
		/// <param name="subsetElement">the element to search a match for</param>
		/// <returns>the corresponding SchemaElement</returns>
		internal EASchemaElement getSchemaElementSubsetElement(UML.Classes.Kernel.Class subsetElement)
		{
			EASchemaElement result = null;
			foreach (EASchemaElement schemaElement in this.elements) 
			{
				if (schemaElement.name == subsetElement.name)
				{
					//check if the subset element has a dependency to the source element of the schema
					foreach (var dependency in subsetElement.clientDependencies) 
					{
						if (schemaElement.sourceElement.Equals(dependency.supplier))
						{
							result = schemaElement;
							break;
						}
					}
					
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

			//loop the elements to create the subSetElements
			foreach (EASchemaElement schemaElement in this.elements) 
			{
				//only create subset elements for classes, not for datatypes
				if (schemaElement.sourceElement is UML.Classes.Kernel.Class)
				{
					schemaElement.createSubsetElement(destinationPackage);
					//Logger.log("after EASchema::creating single subset element");
				}
			}
			//Logger.log("after EASchema::creating subsetelements");
			// then loop them again to create the associations
			foreach (EASchemaElement schemaElement in this.elements) 
			{
				//only create subset elements for classes, not for datatypes
				if (schemaElement.sourceElement is UML.Classes.Kernel.Class)
				{
					schemaElement.createSubsetAssociations();
					//Logger.log("after EASchema::creating single subset association");
				}
				// and to resolve the attributes types to subset types if required
				schemaElement.resolveAttributetypes(this.schemaElements);
				//Logger.log("after EASchema::resolving attributes");
				//and add a dependency from the schemaElement to the type of the attributes
				schemaElement.addAttributeTypeDependencies();
				//Logger.log("after EASchema::adding attribuetypeDependencies");
			}

		}
		/// <summary>
		/// updates the subset model linked to given messageElement
		/// </summary>
		/// <param name="messageElement">The message element that is the root for the message subset model</param>
		public void updateSubsetModel(UML.Classes.Kernel.Class messageElement)
		{
			//match the subset existing subset elements
			matchSubsetElements(messageElement);
			
			foreach (EASchemaElement schemaElement in this.schemaElements) 
			{
				//match the attributes
				schemaElement.matchSubsetAttributes();
				//match the associations
				schemaElement.matchSubsetAssociations();
			}
			this.createSubsetModel(messageElement.owningPackage);
			//create missing subset elements
			//synchronize attributes
			//synchronize associations
		}
		/// <summary>
		/// Finds all subset elements linked to the given message element and links those to the schema elements.
		/// If a subset element could not be matched, and it is in the same package as the given messageElement, then it is deleted
		/// </summary>
		/// <param name="messageElement">the message element to start from</param>
		void matchSubsetElements(UML.Classes.Kernel.Class messageElement)
		{
			HashSet<UML.Classes.Kernel.Class> subsetElements = this.getSubsetElementsfromMessage(messageElement);
			//match each subset element to a schema element
			foreach (UML.Classes.Kernel.Class subsetElement in subsetElements) 
			{
				//get the corrsponding schema element
				EASchemaElement schemaElement = this.getSchemaElementSubsetElement(subsetElement);
				//found a corresponding schema element
				if (schemaElement != null) 
				{
					schemaElement.matchSubsetElement(subsetElement);
				} else 
				{
					//if it doesn't correspond with a schema element we delete it?
					//only if the subset element is located in the same folder as the message element
					if (subsetElement.owner.Equals(messageElement.owner)) 
					{
						subsetElement.delete();
					}
				}
			}
		}

		/// <summary>
		/// gets all the subset elements for a given message element
		/// </summary>
		/// <param name="messageElement">the message element</param>
		/// <returns>all subset elements in the subset model for this message element</returns>
		private HashSet<UML.Classes.Kernel.Class> getSubsetElementsfromMessage(UML.Classes.Kernel.Class messageElement)
		{
			var subsetElements = new HashSet<UML.Classes.Kernel.Class>();
			this.addRelatedSubsetElements(messageElement,subsetElements);
			//we also add all classes in the package of the subset element
			foreach (var element in messageElement.owningPackage.ownedElements) 
			{
				//we only do classes
				var classElement = element as UTF_EA.Class;
				this.addToSubsetElements(classElement, subsetElements);
			}
			return subsetElements;
		}
		/// <summary>
		/// adds all the related subset elements to the list recursively
		/// </summary>
		/// <param name="element">the element to start from </param>
		/// <param name="subsetElements">the HashSet of subset element to add to</param>
		private void addRelatedSubsetElements(UML.Classes.Kernel.Class element, HashSet<UML.Classes.Kernel.Class> subsetElements)
		{
			//follow the associations
			foreach (UTF_EA.Association association in element.getRelationships<UML.Classes.Kernel.Association>()) 
			{
				addToSubsetElements(association.target as UML.Classes.Kernel.Class, subsetElements);
			}
			//follow the attribute types
			foreach (UTF_EA.Attribute attribute in element.ownedAttributes) 
			{
				addToSubsetElements(attribute.type as UML.Classes.Kernel.Class, subsetElements);
			}
			
		}
		/// <summary>
		/// adds the given class to the list of subset elements and then adds all related
		/// </summary>
		/// <param name="element">the Class to add</param>
		/// <param name="subsetElements">the list of subset elements</param>
		private void addToSubsetElements(UML.Classes.Kernel.Class element, HashSet<UML.Classes.Kernel.Class> subsetElements)
		{
			//add element is not already in the list
			if (element != null 
			    && !subsetElements.Contains(element)) 
			{
				subsetElements.Add(element);
				//add related elements for this element
				this.addRelatedSubsetElements(element, subsetElements);
			}
		}
	}
}
