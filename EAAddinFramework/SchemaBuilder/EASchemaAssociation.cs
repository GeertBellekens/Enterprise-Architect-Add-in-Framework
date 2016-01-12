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

		public EASchemaAssociation(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap):base(model,owner, objectToWrap)
		{
		}
		
		public UML.Classes.Kernel.Association sourceAssociation {
			get 
			{
				return this.model.getRelationByGUID(this.wrappedProperty.GUID) as UML.Classes.Kernel.Association;
			}
			set {
				throw new NotImplementedException();
			}
		}
		public TSF.UmlToolingFramework.UML.Classes.Kernel.Association subsetAssociation {get;set;}
		
		public List<SBF.SchemaElement> relatedElements {
			get 
			{
				List<SBF.SchemaElement> elements = new List<SchemaBuilderFramework.SchemaElement>();
				foreach (UML.Classes.Kernel.Element element in this.sourceAssociation.relatedElements)
				{
					SBF.SchemaElement relatedSchemaElement = ((EASchema)this.owner.owner).getSchemaElementForUMLElement(element);
					if (relatedSchemaElement != null)
					{
						elements.Add(relatedSchemaElement);
					}
				}
				return elements;
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
					//copy the association end properties
					//find the source associations ends that correspond with the subset association source and end
					UTF_EA.AssociationEnd sourceAssociationSourceEnd = null;
					UTF_EA.AssociationEnd sourceAssociationTargetEnd = null; 
					foreach (UML.Classes.Kernel.Property memberEnd in this.sourceAssociation.memberEnds)
					{
						if (this.owner.sourceElement.Equals(memberEnd.type))
						{
							sourceAssociationSourceEnd = (UTF_EA.AssociationEnd) memberEnd;
						}
						else
						{
							sourceAssociationTargetEnd = (UTF_EA.AssociationEnd) memberEnd;
						}
					}
					//copy source end properties
					this.copyAssociationEndProperties(sourceAssociationSourceEnd,((UTF_EA.Association) this.subsetAssociation).sourceEnd);
					//copy target end properties
					this.copyAssociationEndProperties(sourceAssociationTargetEnd,((UTF_EA.Association) this.subsetAssociation).targetEnd);
					this.subsetAssociation.save();
				}
			}
			return this.subsetAssociation as UML.Classes.Kernel.Association;
		}
		private void copyAssociationEndProperties(UTF_EA.AssociationEnd source, UTF_EA.AssociationEnd target)
		{
			target.name = source.name;
			target.multiplicity = source.multiplicity;
			target._isNavigable = source._isNavigable;
			target.isComposite = source.isComposite;
			//target.save(); (should be included in the association save
			
		}
			
	}
}
