
using System;
using System.Collections.Generic;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;

namespace EAAddinFramework.SchemaBuilder
{
	/// <summary>
	/// Description of EASchemaElement.
	/// </summary>
	public class EASchemaElement : SBF.SchemaElement
	{
		private EA.SchemaType wrappedSchemaType;
		private UTF_EA.Model model;
		private EASchema ownerSchema;
		private HashSet<SBF.SchemaProperty> _schemaProperties;
		private UTF_EA.ElementWrapper _sourceElement;
		private HashSet<SBF.SchemaAssociation> _schemaAssociations;
		
		public EASchemaElement(UTF_EA.Model model,EASchema owner, EA.SchemaType objectToWrap)
		{
			this.model = model;
			this.ownerSchema = owner;
			this.wrappedSchemaType = objectToWrap;
		}
		/// <summary>
		/// the name of the Schema element. In most cases this is equal to the name of the source element, unless the element has been redefined.
		/// </summary>
		public string name 
		{
			get 
			{
				return this.wrappedSchemaType.TypeName;
			}
			set {
				throw new NotImplementedException();
			}
		}
		public UML.Classes.Kernel.Classifier sourceElement 
		{
			get 
			{
				if (_sourceElement == null)
				{
					this._sourceElement = this.model.getElementWrapperByID(this.wrappedSchemaType.TypeID);
				}
				return this._sourceElement as UML.Classes.Kernel.Classifier;
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		public UML.Classes.Kernel.Classifier subsetElement {get;set;}
		
		public SchemaBuilderFramework.Schema owner 
		{
			get 
			{
				return this.ownerSchema;
			}
			set 
			{
				this.ownerSchema = (EASchema) value;
			}
		}

		private HashSet<EASchemaPropertyWrapper> schemaPropertyWrappers
		{
			get
			{
				HashSet<EASchemaPropertyWrapper> propertyWrappers = new HashSet<EASchemaPropertyWrapper>();
				foreach (EA.SchemaProperty schemaProperty in this.getSchemaProperties())
				{
					EASchemaPropertyWrapper wrapper = EASchemaBuilderFactory.getInstance(this.model).createSchemaPropertyWrapper(this, schemaProperty);
					if (wrapper != null)
					{
						propertyWrappers.Add(wrapper);
					}
				}
				return propertyWrappers;
			}
		}
		private List<EA.SchemaProperty> getSchemaProperties ()
		{
			List<EA.SchemaProperty> schemaProperties = new List<EA.SchemaProperty>();
			EA.SchemaPropEnum schemaPropEnumerator = this.wrappedSchemaType.Properties;
			EA.SchemaProperty schemaProperty = schemaPropEnumerator.GetFirst();
			while (	schemaProperty != null)
			{
				schemaProperties.Add(schemaProperty);
				schemaProperty = schemaPropEnumerator.GetNext();
			}
			return schemaProperties;
		}
		public HashSet<SBF.SchemaProperty> schemaProperties 
		{
			get 
			{
				if (this._schemaProperties == null)
				{
					this._schemaProperties = new HashSet<SBF.SchemaProperty>();
					foreach (EASchemaPropertyWrapper wrapper in this.schemaPropertyWrappers) 
					{
						if (wrapper is EASchemaProperty)
						{
							this._schemaProperties.Add((SBF.SchemaProperty)wrapper );
						}
					}
				}
				return this._schemaProperties;
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		
		public HashSet<SBF.SchemaAssociation> schemaAssociations 
		{
			get 
			{
				if (this._schemaAssociations == null)
				{
					this._schemaAssociations = new HashSet<SBF.SchemaAssociation>();
					foreach (EASchemaPropertyWrapper wrapper in this.schemaPropertyWrappers) 
					{
						if (wrapper is EASchemaAssociation)
						{
							this._schemaAssociations.Add((SBF.SchemaAssociation)wrapper );
						}
					}
				}
				return this._schemaAssociations;			
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		
		public UML.Classes.Kernel.Classifier createSubsetElement(UML.Classes.Kernel.Package destinationPackage)
		{
			//first create the element in the destination Package
			if (this.subsetElement == null)
			{
				this.subsetElement = this.model.factory.createNewElement<UML.Classes.Kernel.Class>(destinationPackage, this.wrappedSchemaType.TypeName);
			}
			//stereotypes
			this.subsetElement.stereotypes = this.sourceElement.stereotypes;
			//notes
			this.subsetElement.ownedComments = this.sourceElement.ownedComments;
			//loop the properties
			foreach (EASchemaProperty property in this.schemaProperties) 
			{
				//create the subset property
				property.createSubsetProperty();
			}
			//save the new subset element
			((UTF_EA.Element) this.subsetElement).save();
			//copy tagged values
			foreach (UTF_EA.TaggedValue sourceTaggedValue in sourceElement.taggedValues) 
			{
				bool updateTaggedValue = true;
				if (this.owner.ignoredTaggedValues.Contains(sourceTaggedValue.name))
				{
					UTF_EA.TaggedValue targetTaggedValue = ((UTF_EA.Element) this.subsetElement).getTaggedValue(sourceTaggedValue.name);
					if (targetTaggedValue.eaStringValue != string.Empty)
					{
						//don't update any of the tagged values of the ignoredTaggeValues if the value is already filled in.
						updateTaggedValue = false;
					}
				}
				if (updateTaggedValue)
				{
					((UTF_EA.Element) this.subsetElement).addTaggedValue(sourceTaggedValue.name, sourceTaggedValue.eaStringValue);
				}
			}
			//add a trace relation from the subset element to the source element
			// check if trace already exists?
			var trace = this.subsetElement.getRelationships<UML.Classes.Dependencies.Abstraction>()
						.FirstOrDefault(x => this.sourceElement.Equals(x.supplier)) as UML.Classes.Dependencies.Abstraction;
			if (trace == null)
			{
				trace = this.model.factory.createNewElement<UML.Classes.Dependencies.Abstraction>(this.subsetElement, string.Empty);
				trace.addStereotype(this.model.factory.createStereotype(trace, "trace"));
				trace.target = this.sourceElement;
				trace.save();
			}
			//return the new element
			return this.subsetElement;
		}		
		
		/// <summary>
		/// duplicates the asociations in the schema to associations between schema elements
		/// </summary>
		public void createSubsetAssociations()
		{
			foreach (SBF.SchemaAssociation schemaAssociation in this.schemaAssociations) 
			{
				schemaAssociation.createSubsetAssociation();
			}
		}
		/// <summary>
		/// Checks if the attribute type is present as the source element of one of the schema elements
		/// If it finds a match the type is set to the subset elemnt of this schema element
		/// </summary>
		/// <param name="schemaElements"></param>
		public void resolveAttributetypes(HashSet<SBF.SchemaElement> schemaElements)
		{
			foreach (EASchemaProperty schemaProperty in this.schemaProperties) 
			{
				schemaProperty.resolveAttributeType(schemaElements);
			}
		}
		/// <summary>
		/// adds a dependency from the attributes owner to the type of the attributes
		/// </summary>
		public void addAttributeTypeDependencies()
		{
			foreach (EASchemaProperty schemaProperty in this.schemaProperties) 
			{
				schemaProperty.addAttributeTypeDependency();
			}
		}
		/// <summary>
		/// matches the given subset element with the schema element, matching attributes and association
		/// all attributes or associations that do not exist in the schema are deleted
		/// </summary>
		/// <param name="subsetElement">the subset element to match</param>
		public void matchSubsetElement(UML.Classes.Kernel.Class subsetElement)
		{
			//set the subset element
			this.subsetElement = subsetElement;
		}
		/// <summary>
		/// checks all attributes of the subset element and tries to match it with a SchemaProperty.
		/// It it can't be matches te subset attribute is deleted.
		/// </summary>
		public void matchSubsetAttributes()
		{
			if (this.subsetElement != null)
			{
				foreach (UTF_EA.Attribute attribute in this.subsetElement.attributes) 
				{
					EASchemaProperty matchingProperty = this.getMatchingSchemaProperty(attribute);
					if (matchingProperty != null)
					{
						//found a match
						matchingProperty.subSetProperty = attribute;
					}
					else
					{
						//no match, delete the attribute
						attribute.delete();
					}
				}
			}
		}
		/// <summary>
		/// finds the corresponding Schema property for the given attribut
		/// </summary>
		/// <param name="attribute">attribute</param>
		/// <returns>the corresponding Schema property if one is found. Else null</returns>
		public EASchemaProperty getMatchingSchemaProperty(UTF_EA.Attribute attribute)
		{
			EASchemaProperty result = null;
			var sourceAttributeTag = attribute.getTaggedValue(EASchemaBuilderFactory.sourceAttributeTagName);
			if (sourceAttributeTag != null)
			{
				string tagReference = sourceAttributeTag.eaStringValue;
			
				foreach (EASchemaProperty property in this.schemaProperties) 
				{
					//we have the same attribute if the given attribute has a tagged value 
					//called sourceAttribute that refences the source attribute of the schema Property
					if (((UTF_EA.Attribute)property.sourceProperty).guid == tagReference)
					{
						result = property;
						break;
					}
				}
			}
			return result;
		}
		
		/// <summary>
		/// matches the association of the subset element with the schema associations.
		/// If an association cannot be matched, it is deleted.
		/// </summary>
		public void matchSubsetAssociations()
		{
			if (this.subsetElement != null)
			{
				foreach (UTF_EA.Association association in this.subsetElement.getRelationships<UML.Classes.Kernel.Association>()) 
				{
					//we are only interested in the outgoing associations
					if (this.subsetElement.Equals(association.source))
					{
						EASchemaAssociation matchingAssociation = this.getMatchingSchemaAssociation(association);
						if (matchingAssociation != null)
						{
							//found a match
							matchingAssociation.subsetAssociation = association;
						}
						else
						{
							//no match, delete the association
							association.delete();
						}
					}
				}
			}
		}
		/// <summary>
		/// returns the matching SchemaAssociation for the given Association.
		/// the match is made based on the tagged value sourceAssociation on the subset assocation, which references the source association fo the SchemaAssociation
		/// </summary>
		/// <param name="association">the association to match</param>
		/// <returns>the matching SchemaAssociation</returns>
		public EASchemaAssociation getMatchingSchemaAssociation(UTF_EA.Association association)
		{
			EASchemaAssociation result = null;
			var sourceAssociationTag = association.getTaggedValue(EASchemaBuilderFactory.sourceAssociationTagName);
			if (sourceAssociationTag != null)
			{
				string tagReference = sourceAssociationTag.eaStringValue;
			
				foreach (EASchemaAssociation schemaAssociation in this.schemaAssociations) 
				{
					//we have the same attribute if the given attribute has a tagged value 
					//called sourceAssociation that refences the source association of the schema Association
					if (((UTF_EA.Association)schemaAssociation.sourceAssociation).guid == tagReference)
					{
						result = schemaAssociation;
						break;
					}
				}
			}
			return result;
		}
	}
}
