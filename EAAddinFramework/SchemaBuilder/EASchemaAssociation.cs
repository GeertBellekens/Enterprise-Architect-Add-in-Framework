using System;
using System.Collections.Generic;
using TSF.UmlToolingFramework.Wrappers.EA;
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
		private UTF_EA.AssociationEnd _thisEnd;
		private UTF_EA.AssociationEnd _otherEnd;
		private EASchemaElement _otherElement;

		public EASchemaAssociation(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap):base(model,owner, objectToWrap)
		{
		}

		#region implemented abstract members of EASchemaPropertyWrapper
		protected override Multiplicity defaultMultiplicity 
		{
			get 
			{
				return new Multiplicity("0..*");
			}
		}
		#endregion		
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
		
		/// <summary>
		/// 
		/// </summary>
		public SBF.SchemaElement otherElement 
		{
			get 
			{
				if (this._otherElement == null)
				{
					this._otherElement = ((EASchema)this.owner.owner).getSchemaElementForUMLElement(this.otherEnd.type);
				}
				return this._otherElement;
			}
			set 
			{
				this._otherElement = (EASchemaElement)value;
			}
		}
		public UML.Classes.Kernel.Property thisEnd {
			get 
			{
				if (this._thisEnd == null)
				{
					this.setEnds();
				}
				return this._thisEnd;
			}
			set 
			{
				this._thisEnd = value as UTF_EA.AssociationEnd;
			}
		}
		public UML.Classes.Kernel.Property otherEnd {
			get 
			{
				if (this._otherEnd == null)
				{
					this.setEnds();
				}
				return this._otherEnd;
			}
			set 
			{
				this._otherEnd = value as UTF_EA.AssociationEnd;
			}
		}		
		
		private void setEnds()
		{
			UTF_EA.AssociationEnd sourceEnd = ((UTF_EA.Association)this.sourceAssociation).sourceEnd;
			UTF_EA.AssociationEnd targetEnd = ((UTF_EA.Association)this.sourceAssociation).targetEnd;	
			//check the source end
			var endType = sourceEnd.type as UTF_EA.ElementWrapper;
			//check if the source end is linked to the element with the parent ID of the wrappedProperty
			if (endType != null 
			    && endType.id == this.wrappedProperty.Parent)
			{
				//check if the other end is linked to the same element
				if (endType.Equals(targetEnd.type))
				{
					//the association is linked to the same element on both sides
					//check the other properties to figure out which one we need as this end
					if (sourceEnd.name == this.wrappedProperty.Name
					    && sourceEnd.multiplicity == new Multiplicity(this.wrappedProperty.Cardinality.Length > 0 ? this.wrappedProperty.Cardinality : AssociationEnd.defaultMultiplicity) 
					    && AggregationKind.getEAAggregationKind(sourceEnd.aggregation) == this.wrappedProperty.Aggregation)
					{
						//the source end properties corresponds with the wrappedProperty properties
						this._thisEnd = sourceEnd;
						this._otherEnd = targetEnd;
					}
					else
					{
						//source end properties don't correspond, so we assume the targetEnd to be this end
						this._thisEnd = targetEnd;
						this._otherEnd = sourceEnd;
					}
				}else
				{
					//both sides are different
					this._thisEnd = sourceEnd;
					this._otherEnd = targetEnd;
				}
			}	
			else
			{
				//this end doesn't correspond with sourceEnd, so we assume the targetEnd to be this end
				this._thisEnd = targetEnd;
				this._otherEnd = sourceEnd;
			}
		}
		
		/// <summary>
		/// the related elements are the owner element and the element on the other side
		/// </summary>
		public List<SBF.SchemaElement> relatedElements 
		{
			get 
			{
				if (this._relatedElements == null)
				{
					this._relatedElements = new List<SBF.SchemaElement>();
					//first add the owner
					this._relatedElements.Add(this.owner);
					//then add the other side
					this._relatedElements.Add(this.otherElement);
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
			//find the other schemaElement

			if (this.otherElement != null)
			{
				//found the other element
				//if this schemaElement has a subsetElement 
				if (this.owner.subsetElement != null)
				{
					UML.Classes.Kernel.Classifier associationTarget;
					if (this.otherElement.subsetElement != null)
					{
						//and the otherElement has a subsetElement we create an association from this subsetElement to the other element subsetElement
						associationTarget = this.otherElement.subsetElement;
					}
					else
					{
						//if the other elemnt doesn't have a subset element then we create an association to the source element
						associationTarget = this.otherElement.sourceElement;
					}
					//create the association
					if (this.subsetAssociation == null)
					{
						this.subsetAssociation = this.model.factory.createNewElement<UML.Classes.Kernel.Association>(this.owner.subsetElement,this.sourceAssociation.name);
						this.subsetAssociation.addRelatedElement(associationTarget);
					}
					//copy association properties
					this.subsetAssociation.name = this.sourceAssociation.name;
					this.subsetAssociation.ownedComments = this.sourceAssociation.ownedComments;
					this.subsetAssociation.stereotypes = this.sourceAssociation.stereotypes;
					//save all changes
					this.subsetAssociation.save();
					//copy the association end properties
					//copy source end properties
					this.copyAssociationEndProperties((UTF_EA.AssociationEnd)this.thisEnd,((UTF_EA.Association) this.subsetAssociation).sourceEnd);
					//copy target end properties
					this.copyAssociationEndProperties((UTF_EA.AssociationEnd)this.otherEnd,((UTF_EA.Association) this.subsetAssociation).targetEnd);
					//set the target multiplicity to a possible redefined multiplicity from the schema
					((UTF_EA.Association) this.subsetAssociation).targetEnd.multiplicity = this.multiplicity;
					//save all changes
					this.subsetAssociation.save();
					//copy tagged values
					foreach (UTF_EA.TaggedValue sourceTaggedValue in this.sourceAssociation.taggedValues) 
					{
						bool updateTaggedValue = true;
						if (this.owner.owner.ignoredTaggedValues.Contains(sourceTaggedValue.name))
						{
							UTF_EA.TaggedValue targetTaggedValue = ((UTF_EA.Element) this.subsetAssociation).getTaggedValue(sourceTaggedValue.name);
							if (targetTaggedValue.eaStringValue != string.Empty)
							{
								//don't update any of the tagged values of the ignoredTaggeValues if the value is already filled in.
								updateTaggedValue = false;
							}
						}
						if (updateTaggedValue)
						{
							((UTF_EA.Element) this.subsetAssociation).addTaggedValue(sourceTaggedValue.name, sourceTaggedValue.eaStringValue);
						}
					}
					
					//((UTF_EA.Association)this.subsetAssociation).copyTaggedValues((UTF_EA.Association)this.sourceAssociation);
					//add tagged value with reference to source association
					((UTF_EA.Association)this.subsetAssociation).addTaggedValue(EASchemaBuilderFactory.sourceAssociationTagName,((UTF_EA.Association)this.sourceAssociation).guid);
				}
			}
			return this.subsetAssociation;
		}
		private void copyAssociationEndProperties(UTF_EA.AssociationEnd source, UTF_EA.AssociationEnd target)
		{
			target.name = source.name;
			target.multiplicity = source.multiplicity;
			target._isNavigable = source._isNavigable;
			target.aggregation = source.aggregation;
			target.ownedComments = source.ownedComments;
		}			
	}
}
