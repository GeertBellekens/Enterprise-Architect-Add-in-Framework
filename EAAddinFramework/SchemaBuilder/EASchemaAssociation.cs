using System;
using System.Collections.Generic;
using TSF.UmlToolingFramework.UML;
using SBF = SchemaBuilderFramework;
using UML = TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.SchemaBuilder
{
	/// <summary>
	/// Description of EASchemaAssociation.
	/// </summary>
	public class EASchemaAssociation : EASchemaPropertyWrapper, SBF.SchemaAssociation
	{
		private List<SBF.SchemaElement> _relatedElements;
		private UTF_EA.Association _sourceAssociation;

		public EASchemaAssociation(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap):base(model,owner, objectToWrap)
		{
		}
		
		public UML.Classes.Kernel.Association sourceAssociation {
			get 
			{
				if (this._sourceAssociation == null)
				{
					this._sourceAssociation = (UTF_EA.Association)this.model.getRelationByGUID(this.wrappedProperty.GUID);
				}
				return this._sourceAssociation;
			}
			set {
				throw new NotImplementedException();
			}
		}
		public UML.Classes.Kernel.Association subsetAssociation {get;set;}
		
		public List<SBF.SchemaElement> relatedElements 
		{
			get 
			{
				if (this._relatedElements == null)
				{
					this._relatedElements= new List<SchemaBuilderFramework.SchemaElement>();
					foreach (UML.Classes.Kernel.Element element in this.sourceAssociation.relatedElements)
					{
						SBF.SchemaElement relatedSchemaElement = ((EASchema)this.owner.owner).getSchemaElementForUMLElement(element);
						if (relatedSchemaElement != null)
						{
							this._relatedElements.Add(relatedSchemaElement);
						}
					}
				}
				return this._relatedElements;
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		
		
		/// <summary>
		/// Creates the association in the subset. This is a copy of the source association but now between the subsetelements
		/// </summary>
		/// <returns>the new subset association</returns>
		public UML.Classes.Kernel.Association createSubsetAssociation()
		{
			EASchemaElement otherElement = null;
			this.subsetAssociation = null;
			//find the other schemaElement
			foreach (EASchemaElement schemaElement in this.relatedElements) 
			{
				if (!schemaElement.sourceElement.Equals(this.owner.sourceElement))
				{
					otherElement = schemaElement;
					break;
				}
			}
			if (otherElement != null)
			{
				//found the other element
				//if this schemaElement has a subsetElement 
				if (this.owner.subsetElement != null)
				{
					UML.Classes.Kernel.Classifier associationTarget;
					if (otherElement.subsetElement != null)
					{
						//and the otherElement has a subsetElement we create an association from this subsetElement to the other element subsetElement
						associationTarget = otherElement.subsetElement;
					}
					else
					{
						//if the other elemnt doesn't have a subset element then we create an association to the source element
						associationTarget = otherElement.sourceElement;
					}
					//create the association
					this.subsetAssociation = this.model.factory.createNewElement<UML.Classes.Kernel.Association>(this.owner.subsetElement,this.sourceAssociation.name);
					this.subsetAssociation.addRelatedElement(associationTarget);
					//copy association properties
					this.subsetAssociation.name = this.sourceAssociation.name;
					this.subsetAssociation.stereotypes = this.sourceAssociation.stereotypes;
					//save all changes
					this.subsetAssociation.save();
					//copy the association end properties
					//find the source associations ends that correspond with the subset association source and end
					UTF_EA.AssociationEnd sourceAssociationSourceEnd = null;
					UTF_EA.AssociationEnd sourceAssociationTargetEnd = null; 

					int sourceID = ((UTF_EA.ElementWrapper)((UTF_EA.Association) this.sourceAssociation).source).id;
					if (this.wrappedProperty.Parent == sourceID)
					{
						sourceAssociationSourceEnd = (UTF_EA.AssociationEnd) ((UTF_EA.Association)this.sourceAssociation).sourceEnd;
						sourceAssociationTargetEnd = (UTF_EA.AssociationEnd) ((UTF_EA.Association)this.sourceAssociation).targetEnd;
					}
					else
					{
						sourceAssociationSourceEnd = (UTF_EA.AssociationEnd) ((UTF_EA.Association)this.sourceAssociation).targetEnd;
						sourceAssociationTargetEnd = (UTF_EA.AssociationEnd) ((UTF_EA.Association)this.sourceAssociation).sourceEnd;
					}
					//copy source end properties
					this.copyAssociationEndProperties(sourceAssociationSourceEnd,((UTF_EA.Association) this.subsetAssociation).sourceEnd);
					//copy target end properties
					this.copyAssociationEndProperties(sourceAssociationTargetEnd,((UTF_EA.Association) this.subsetAssociation).targetEnd);
					//save all changes
					this.subsetAssociation.save();
					//copy tagged values
					((UTF_EA.Association)this.subsetAssociation).copyTaggedValues((UTF_EA.Association)this.sourceAssociation);
					//add tagged value with reference to source association
					((UTF_EA.Association)this.subsetAssociation).addTaggedValue(EASchemaBuilderFactory.sourceAssociationTagName,((UTF_EA.Association)this.sourceAssociation).guid);
					
				}
			}
			return this.subsetAssociation as UML.Classes.Kernel.Association;
		}
		private void copyAssociationEndProperties(UTF_EA.AssociationEnd source, UTF_EA.AssociationEnd target)
		{
			target.name = source.name;
			target.multiplicity = source.multiplicity;
			target._isNavigable = source._isNavigable;
			target.aggregation = source.aggregation;
		}			
	}
}
