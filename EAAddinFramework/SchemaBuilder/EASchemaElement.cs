﻿
using System;
using System.Collections.Generic;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

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
		
		public EASchemaElement(UTF_EA.Model model,EASchema owner, EA.SchemaType objectToWrap)
		{
			this.model = model;
			this.ownerSchema = owner;
			this.wrappedSchemaType = objectToWrap;
		}
		
		public TSF.UmlToolingFramework.UML.Classes.Kernel.Classifier sourceElement 
		{
			get 
			{
				return this.model.getElementWrapperByID(this.wrappedSchemaType.TypeID) as UML.Classes.Kernel.Classifier;
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
				HashSet<SBF.SchemaAssociation> associations = new HashSet<SBF.SchemaAssociation>();
				foreach (EASchemaPropertyWrapper wrapper in this.schemaPropertyWrappers) 
				{
					if (wrapper is EASchemaAssociation)
					{
						associations.Add((SBF.SchemaAssociation)wrapper );
					}
				}
				return associations;				
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		
		public UML.Classes.Kernel.Classifier createSubsetElement(UML.Classes.Kernel.Package destinationPackage)
		{
			//first create the element in the destination Package
			this.subsetElement = this.model.factory.createNewElement<UML.Classes.Kernel.Class>(destinationPackage, this.sourceElement.name);
			//stereotypes
			this.subsetElement.stereotypes = this.sourceElement.stereotypes;
			//loop the properties
			foreach (EASchemaProperty property in this.schemaProperties) 
			{
				property.createSubsetProperty();
				UML.Classes.Kernel.Property sourceProperty = property.sourceProperty;
				string propertyName = sourceProperty.name;
			}
			//loop the associations
			foreach (EASchemaAssociation schemaAssociation in this.schemaAssociations) 
			{
				foreach (EASchemaElement relatedSchemaElement in schemaAssociation.relatedElements) 
				{
					string relatedSchemaElementName = relatedSchemaElement.sourceElement.name;
				}
				UML.Classes.Kernel.Association association = schemaAssociation.sourceAssociation;
				foreach (UML.Classes.Kernel.Element relatedElement in association.relatedElements) 
				{
					string relatedElementName = relatedElement.name;	
				}
			}
			((UTF_EA.Element) this.subsetElement).save();
			//copy tagged values
			((UTF_EA.Element) this.subsetElement).copyTaggedValues((UTF_EA.Element)this.sourceElement);

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
	}
}