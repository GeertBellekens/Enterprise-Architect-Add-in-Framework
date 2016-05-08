
using System;
using System.Collections.Generic;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using EAAddinFramework.Utilities;
using System.Linq;
using TSF.UmlToolingFramework.UML.Classes.Kernel;

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

		public SBF.SchemaSettings settings {get;set;}

		/// <summary>
		/// Constructor of the EASchema. Only to be used by the EASchemaBuilderFactory
		/// </summary>
		/// <param name="model">The model containing this Scheam</param>
		/// <param name="composer">The EA.SchemaComposer object to be wrapped</param>
		internal EASchema(UTF_EA.Model model, EA.SchemaComposer composer, SBF.SchemaSettings settings)
		{
			this.model = model;
			this.wrappedComposer = composer;
			this.settings = settings;
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
				if (schemaElement.sourceElement != null
					&& schemaElement.sourceElement.Equals(umlElement))
				{
					if (result == null)
					{
						result = schemaElement;
					}
					else if (schemaElement.name == umlElement.name)
					{
						result = schemaElement;
						break;
					}
				}
			}
			return result;
		}
		/// <summary>
		/// finds the Schema Element for which the given element could be the subset element
		/// </summary>
		/// <param name="subsetElement">the element to search a match for</param>
		/// <returns>the corresponding SchemaElement</returns>
		internal EASchemaElement getSchemaElementForSubsetElement(UML.Classes.Kernel.Classifier subsetElement)
		{
			EASchemaElement result = null;
			if (subsetElement != null)
			{
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
			}
			return result;
		}

	    /// <summary>
	    /// creates a subset of the source model with only the properties and associations used in this schema
	    /// </summary>
	    /// <param name="destinationPackage">the package to create the subset in</param>
	    /// <param name="copyDatatype"></param>
	    public void createSubsetModel(UML.Classes.Kernel.Package destinationPackage)
		{

			//loop the elements to create the subSetElements
			foreach (EASchemaElement schemaElement in this.elements) 
			{
				//only create subset elements for classes, not for datatypes
				if (schemaElement.sourceElement is UML.Classes.Kernel.Class
				   || schemaElement.sourceElement is UML.Classes.Kernel.Enumeration)
				{
                    schemaElement.createSubsetElement(destinationPackage);
                    //Logger.log("after EASchema::creating single subset element");
                }
                else if (schemaElement.sourceElement is UML.Classes.Kernel.DataType && this.settings.copyDataTypes)
                {
                	//if the datatypes are limited then only the ones in the list should be copied
                	if (!this.settings.limitDataTypes
                	    || this.settings.dataTypesToCopy.Contains(schemaElement.sourceElement.name))
                    schemaElement.createSubsetElement(destinationPackage);
                }
			}
			//Logger.log("after EASchema::creating subsetelements");
			// then loop them again to create the associations
			foreach (EASchemaElement schemaElement in this.elements) 
			{
				//only create subset elements for classes and enumerations and datatypes
				if (schemaElement.sourceElement is UML.Classes.Kernel.Class
				   || schemaElement.sourceElement is UML.Classes.Kernel.Enumeration
                    || schemaElement.sourceElement is UML.Classes.Kernel.DataType)
				{
					schemaElement.createSubsetAssociations();
					//Logger.log("after EASchema::creating single subset association");
					// and to resolve the attributes types to subset types if required
					schemaElement.createSubsetAttributes();
					//Logger.log("after EASchema::createSubsetAttributes ");
					schemaElement.createSubsetLiterals();
					//and add a dependency from the schemaElement to the type of the attributes
					schemaElement.addAttributeTypeDependencies();
					//Logger.log("after EASchema::addAttributeTypeDependencies");
					//add generalizations if both elements are in the subset
					schemaElement.addGeneralizations();
				}
			}
			//then loop them the last time to remove those subset elements that don't have any attributes or associations
			foreach (EASchemaElement schemaElement in this.elements) 
			{
				if (schemaElement.subsetElement != null)
				{
					//remove those subset elements that don't have any attributes or associations
					//reload the element because otherwise the API does not return any attributes or associations
			    	var reloadedElement = model.getElementByGUID(schemaElement.subsetElement.uniqueID) as Classifier;
					if  (reloadedElement != null
					   && reloadedElement.attributes.Count == 0
					   && reloadedElement.getRelationships<UML.Classes.Kernel.Association>().Count == 0
					   && reloadedElement.getDependentTypedElements<UML.Classes.Kernel.TypedElement>().Count == 0)
					{
						schemaElement.subsetElement.delete();
					}
				 }
			}
		}

	    /// <summary>
	    /// updates the subset model linked to given messageElement
	    /// </summary>
	    /// <param name="messageElement">The message element that is the root for the message subset model</param>
	    public void updateSubsetModel(Classifier messageElement)
		{
			//match the subset existing subset elements
			//Logger.log("starting EASchema::updateSubsetModel");
			matchSubsetElements(messageElement);
			//Logger.log("after EASchema::matchSubsetElements");
			
			foreach (EASchemaElement schemaElement in this.schemaElements) 
			{
				//match the attributes
				schemaElement.matchSubsetAttributes();
				//Logger.log("after EASchema::matchSubsetAttributes");
				schemaElement.matchSubsetLiterals();
				//match the associations
				schemaElement.matchSubsetAssociations();
				//Logger.log("after EASchema::matchSubsetAssociations");
			}
			this.createSubsetModel(messageElement.owningPackage);
			//Logger.log("after EASchema::createSubsetModel");
		}
		/// <summary>
		/// Finds all subset elements linked to the given message element and links those to the schema elements.
		/// If a subset element could not be matched, and it is in the same package as the given messageElement, then it is deleted
		/// </summary>
		/// <param name="messageElement">the message element to start from</param>
		void matchSubsetElements(UML.Classes.Kernel.Classifier messageElement)
		{
			HashSet<UML.Classes.Kernel.Classifier> subsetElements = this.getSubsetElementsfromMessage(messageElement);
			//match each subset element to a schema element
			foreach (UML.Classes.Kernel.Classifier subsetElement in subsetElements) 
			{
				//get the corrsponding schema element
				EASchemaElement schemaElement = this.getSchemaElementForSubsetElement(subsetElement);
				//found a corresponding schema element
				if (schemaElement != null 
				    && shouldElementExistAsDatatype(subsetElement))
				{
					schemaElement.matchSubsetElement(subsetElement);
				} 
				else
				{
					//if it doesn't correspond with a schema element we delete it?
					//only if the subset element is located in the same folder as the message element
					//and it doesn't have one of stereotypes to be ignored
					if (subsetElement.owner.Equals(messageElement.owner)
					    && ! this.settings.ignoredStereotypes.Intersect(((UTF_EA.Element)subsetElement).stereotypeNames).Any())
					{
						subsetElement.delete();
					}
				}
			}
		}
		
		private bool shouldElementExistAsDatatype(Classifier subsetElement)
		{
			if (subsetElement is Class || subsetElement is Enumeration)
			{
				return true;
			}
			else
			{
				var datatype = subsetElement as DataType;
				if (datatype != null && this.settings.copyDataTypes)
				{
					if (!this.settings.limitDataTypes
					    || this.settings.dataTypesToCopy.Contains(datatype.name))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
		}
		/// <summary>
		/// gets all the subset elements for a given message element
		/// </summary>
		/// <param name="messageElement">the message element</param>
		/// <returns>all subset elements in the subset model for this message element</returns>
		private HashSet<UML.Classes.Kernel.Classifier> getSubsetElementsfromMessage(UML.Classes.Kernel.Classifier messageElement)
		{
			var subsetElements = new HashSet<UML.Classes.Kernel.Classifier>();
			this.addRelatedSubsetElements(messageElement,subsetElements);
			//we also add all classes in the package of the subset element
			foreach (var element in messageElement.owningPackage.ownedElements) 
			{
				//we only do classes or enumerations
				var classElement = element as UML.Classes.Kernel.Classifier;
				
				this.addToSubsetElements(classElement, subsetElements);
			}
			return subsetElements;
		}
		/// <summary>
		/// adds all the related subset elements to the list recursively
		/// </summary>
		/// <param name="element">the element to start from </param>
		/// <param name="subsetElements">the HashSet of subset element to add to</param>
		private void addRelatedSubsetElements(UML.Classes.Kernel.Classifier element, HashSet<UML.Classes.Kernel.Classifier> subsetElements)
		{
			//follow the associations
			foreach (UTF_EA.Association association in element.getRelationships<UML.Classes.Kernel.Association>()) 
			{
				addToSubsetElements(association.target as UML.Classes.Kernel.Classifier, subsetElements);
			}
			//follow the attribute types
			foreach (UTF_EA.Attribute attribute in element.attributes) 
			{
				addToSubsetElements(attribute.type as UML.Classes.Kernel.Classifier, subsetElements);
			}
			
		}
		/// <summary>
		/// adds the given class to the list of subset elements and then adds all related
		/// </summary>
		/// <param name="element">the Class to add</param>
		/// <param name="subsetElements">the list of subset elements</param>
		private void addToSubsetElements(UML.Classes.Kernel.Classifier element, HashSet<UML.Classes.Kernel.Classifier> subsetElements)
		{
			//add element is not already in the list
			if (element != null 
			    && (element is Class || element is Enumeration|| (element is DataType && !(element is PrimitiveType)))
			    && !subsetElements.Contains(element)) 
			{
				subsetElements.Add(element);
				//add related elements for this element
				this.addRelatedSubsetElements(element, subsetElements);
			}
		}
		/// <summary>
		/// copies only the tagged values necesarry
		/// </summary>
		/// <param name="source">the source element</param>
		/// <param name="target">the target element</param>
		public void copyTaggedValues(UTF_EA.Element source, UTF_EA.Element target)
        {
        	 //copy tagged values
            foreach (UTF_EA.TaggedValue sourceTaggedValue in source.taggedValues)
            {
                bool updateTaggedValue = true;
                if (this.settings.ignoredTaggedValues.Contains(sourceTaggedValue.name))
                {
                    UTF_EA.TaggedValue targetTaggedValue =
                        target.getTaggedValue(sourceTaggedValue.name);
                    if (targetTaggedValue != null &&
                        targetTaggedValue.eaStringValue != string.Empty)
                    {
                        //don't update any of the tagged values of the ignoredTaggeValues if the value is already filled in.
                        updateTaggedValue = false;
                    }
                }
                if (updateTaggedValue)
                {
                    target.addTaggedValue(sourceTaggedValue.name,
                        sourceTaggedValue.eaStringValue);
                }
            }
        }
	}
}
