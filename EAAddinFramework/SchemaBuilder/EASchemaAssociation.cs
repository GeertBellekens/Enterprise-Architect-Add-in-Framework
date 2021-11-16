using System;
using System.Collections.Generic;
using EAAddinFramework.Utilities;
using SBF = SchemaBuilderFramework;
using UML = TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using Association = TSF.UmlToolingFramework.UML.Classes.Kernel.Association;

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

        public EASchemaAssociation(UTF_EA.Model model, EASchemaElement owner, EA.SchemaProperty objectToWrap) : base(model, owner, objectToWrap)
        {
        	this.subsetAssociations = new List<Association>();
        }

        #region implemented abstract members of EASchemaPropertyWrapper
        protected override UTF_EA.Multiplicity defaultMultiplicity
        {
            get
            {
                return new UTF_EA.Multiplicity("1..1");
            }
        }

		#region implemented abstract members of EASchemaPropertyWrapper
		internal override UTF_EA.AttributeWrapper sourceAttributeWrapper {
			get {
				return null;
			}
		}
		internal override UTF_EA.AttributeWrapper subsetAttributeWrapper {
			get {
				return null;
			}
		}
		#endregion
		#region implemented abstract members of EASchemaPropertyWrapper


		protected override UTF_EA.Multiplicity sourceMultiplicity 
		{
			get 
			{
				return (otherEnd != null ? otherEnd.multiplicity : defaultMultiplicity) as UTF_EA.Multiplicity;
			}
		}


		#endregion

        #endregion
        public UML.Classes.Kernel.Association sourceAssociation
        {
            get
            {
                if (this._sourceAssociation == null)
                {
                    this._sourceAssociation = (UTF_EA.Association)this.model.getRelationByGUID(this.wrappedProperty.GUID);
                }
                return this._sourceAssociation;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<UML.Classes.Kernel.Association> subsetAssociations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SBF.SchemaElement otherElement
        {
            get
            {
                if (this._otherElement == null)
                {
                	if (this.redefinedElement != null)
                	{
                		this._otherElement = this.redefinedElement;
                	}
                	else
                	{
                    	this._otherElement = ((EASchema)this.owner.owner).getSchemaElementForUMLElement(this.otherEnd.type);
                	}
                }
                return this._otherElement;
            }
            set
            {
                this._otherElement = (EASchemaElement)value;
            }
        }
        public UML.Classes.Kernel.Property thisEnd
        {
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
        public UML.Classes.Kernel.Property otherEnd
        {
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
            UTF_EA.AssociationEnd sourceEnd = ((UTF_EA.Association)this.sourceAssociation)?.sourceEnd;
            UTF_EA.AssociationEnd targetEnd = ((UTF_EA.Association)this.sourceAssociation)?.targetEnd;
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
                        && sourceEnd.EAMultiplicity == new UTF_EA.Multiplicity(this.wrappedProperty.Cardinality.Length > 0 ? this.wrappedProperty.Cardinality : UTF_EA.AssociationEnd.defaultMultiplicity)
                        && UTF_EA.AggregationKind.getEAAggregationKind(sourceEnd.aggregation) == this.wrappedProperty.Aggregation)
                    {
                        //the source end properties corresponds with the wrappedProperty properties
                        this._thisEnd = targetEnd;
                        this._otherEnd = sourceEnd;
                    }
                    else
                    {
                        //source end properties don't correspond, so we assume the targetEnd to be this end
                        this._thisEnd = sourceEnd;
                        this._otherEnd = targetEnd;
                    }
                }
                else
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
        public void createSubsetAssociation()
        {
            //find the other schemaElement
            if (this.otherElement != null)
            {
                //found the other element
                //if this schemaElement has a subsetElement 
                if (this.owner.subsetElement != null)
                {
                    UML.Classes.Kernel.Classifier associationTarget;

                    if (this.choiceElements != null)
                    {
                        foreach (EASchemaElement choiceElement in this.choiceElements)
                        {
                            if (null != choiceElement.subsetElement)
                            {
                                associationTarget = choiceElement.subsetElement;
                            }
                            else
                            {
                                associationTarget = choiceElement.sourceElement;
                            }
                            //add the actual subset association if needed
                            addSubsetAssociation( associationTarget);
                        }

                    }
                    else
                    {
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
                        //add the actual subset association if needed
                        addSubsetAssociation( associationTarget);
                    }
                }
            }
        
        }
        /// <summary>
        /// add the subset association to the list if it does not exist
        /// </summary>
        /// <param name="associationTarget">the target for the association</param>
        private void addSubsetAssociation(UML.Classes.Kernel.Classifier associationTarget)
        {
        	Association existingAssociation = this.subsetAssociations.FirstOrDefault(x => ((UTF_EA.Association)x).target.Equals(associationTarget));
        	Association subsetAssociation = CreateSubSetAssociation(associationTarget,existingAssociation);
	        if (existingAssociation == null
        	    && subsetAssociation != null)
        	{
        		this.subsetAssociations.Add(subsetAssociation);
        	}
        	//if the association is part of an associationclass then create the link
        	this.setAssociationClassProperties();
        	
        }
        private void setAssociationClassProperties()
        {
        	UTF_EA.AssociationClass sourceAssociationClass = ((UTF_EA.Association)this.sourceAssociation).associationClass;
        	if (sourceAssociationClass != null)
        	{
        		//find subset association class
        		EASchemaElement associationClassSchemaElement = ((EASchema) this.owner.owner).getSchemaElementForUMLElement(sourceAssociationClass);
        		if (associationClassSchemaElement != null 
        			&& associationClassSchemaElement.subsetElement != null)
        		{
        			//link the corresponding subset associationclass to the subset associations
		        	foreach (var subsetAssociation in this.subsetAssociations) 
		        	{
	        			((UTF_EA.AssociationClass) associationClassSchemaElement.subsetElement).relatedAssociation = subsetAssociation as UTF_EA.Association;
	        			associationClassSchemaElement.subsetElement.save();
	        			break; //we can only link one association to the association class, so we break after the first one
		        	}
        		}
        	}
        }
        private Association CreateSubSetAssociation(UML.Classes.Kernel.Classifier associationTarget, Association existingAssociation)
        {
        	Association subSetAssociation;
        	if (existingAssociation == null)
        	{
           		subSetAssociation = this.model.factory.createNewElement<UML.Classes.Kernel.Association>
           		 					(this.owner.subsetElement,this.sourceAssociation.name);
           		subSetAssociation.addRelatedElement(associationTarget);
        	}
        	else
        	{
        		//subset association already exists
        		subSetAssociation = existingAssociation;
        		//report differences
        		//different multiplicity
        		if (((UTF_EA.Association)subSetAssociation).targetEnd.EAMultiplicity != this.multiplicity)
        		{
        			EAOutputLogger.log(this.model,this.owner.owner.settings.outputName
						                                              ,string.Format("Multiplicity of association between '{0}' and '{1}' has changed from '{2}' to '{3}'"
						                                  					,((UTF_EA.Association)subSetAssociation).source.name
						                                  					,((UTF_EA.Association)subSetAssociation).target.name
						                                  					,((UTF_EA.Association)subSetAssociation).targetEnd.EAMultiplicity
						                                  					,this.multiplicity)
						                                              ,((UTF_EA.ElementWrapper)((UTF_EA.Association)subSetAssociation).source).id
						                                              , LogTypeEnum.warning);
        		}
        		//different target rolname
        		if (((UTF_EA.Association)subSetAssociation).targetEnd.name != this.otherEnd.name)
        		{
        			EAOutputLogger.log(this.model,this.owner.owner.settings.outputName
						                                              ,string.Format("Target rolename of association between '{0}' and '{1}' has changed from '{2}' to '{3}'"
						                                  					,((UTF_EA.Association)subSetAssociation).source.name
						                                  					,((UTF_EA.Association)subSetAssociation).target.name
						                                  					,((UTF_EA.Association)subSetAssociation).targetEnd.name
						                                  					,this.otherEnd.name)
						                                              ,((UTF_EA.ElementWrapper)((UTF_EA.Association)subSetAssociation).source).id
						                                              , LogTypeEnum.warning);
        		}
        	}
           	//update name
            subSetAssociation.name = this.sourceAssociation.name;
            //alias only if alias in the subset is emtpy
            if (string.IsNullOrEmpty(((UTF_EA.Association)subSetAssociation).alias))
            	((UTF_EA.Association)subSetAssociation).alias = ((UTF_EA.Association)sourceAssociation).alias;
            //Check if the subset alias is different from the source alias and issue warning if that is the case
			if (!string.Equals(((UTF_EA.Association)subSetAssociation).alias,((UTF_EA.Association)sourceAssociation).alias))
			{
					EAOutputLogger.log(this.model,this.owner.owner.settings.outputName
                                              ,string.Format("Association between '{0}' and '{1}' has alias {2} in the model and a different alias '{3}' in the subset"
                                  					,((UTF_EA.Association)subSetAssociation).source.name
                                  					,((UTF_EA.Association)subSetAssociation).target.name
                                  					,((UTF_EA.Association)subSetAssociation).alias
                                  					,((UTF_EA.Association)sourceAssociation).alias)
                                              ,((UTF_EA.ElementWrapper)((UTF_EA.Association)subSetAssociation).source).id
                                              , LogTypeEnum.warning);				
			}
            //notes only update them if they are empty or if settings
			if (subSetAssociation.ownedComments.Count == 0 || ! subSetAssociation.ownedComments.Any(x => x.body.Length > 0)
                || this.owner.owner.settings.keepNotesInSync)
			{
				subSetAssociation.ownedComments = this.sourceAssociation.ownedComments;
				if (!this.owner.owner.settings.keepNotesInSync
                    && this.owner.owner.settings.prefixNotes
				    && this.owner.owner.settings.prefixNotesText.Length > 0
				    && subSetAssociation.ownedComments.Any(x => x.body.Length > 0))
				{
					foreach (var comment in subSetAssociation.ownedComments) 
					{
						comment.body = this.owner.owner.settings.prefixNotesText + Environment.NewLine + comment.body;
					}	
				}
			}
            //stereotype
            subSetAssociation.stereotypes = this.sourceAssociation.stereotypes;
            //derived
            subSetAssociation.isDerived = this.sourceAssociation.isDerived;
            //save all changes
            //subSetAssociation.save();
            //copy the association end properties
            //copy source end properties
            this.copyAssociationEndProperties((UTF_EA.AssociationEnd)this.thisEnd,
                ((UTF_EA.Association)subSetAssociation).sourceEnd);
            //copy target end properties
            this.copyAssociationEndProperties((UTF_EA.AssociationEnd)this.otherEnd,
                ((UTF_EA.Association)subSetAssociation).targetEnd);
            //set the target multiplicity to a possible redefined multiplicity from the schema
            ((UTF_EA.Association)subSetAssociation).targetEnd.EAMultiplicity = this.multiplicity;
            //save all changes
            subSetAssociation.save();
            //copy tagged values
            ((EASchema) this.owner.owner).copyTaggedValues((UTF_EA.Element)this.sourceAssociation,(UTF_EA.Element)subSetAssociation);
            //((UTF_EA.Association)this.subsetAssociation).copyTaggedValues((UTF_EA.Association)this.sourceAssociation);
            //add tagged value with reference to source association
            ((UTF_EA.Association)subSetAssociation).addTaggedValue(
                this.owner.owner.settings.sourceAssociationTagName, ((UTF_EA.Association)this.sourceAssociation).guid);

            //subSetAssociation.save();
            return subSetAssociation;
        }

        private void copyAssociationEndProperties(UTF_EA.AssociationEnd source, UTF_EA.AssociationEnd target)
        {
            target.name = source.name;
            target.EAMultiplicity = source.EAMultiplicity;
            target.isNavigable = source.isNavigable;
            target.aggregation = source.aggregation;
            target.ownedComments = source.ownedComments;
            //TODO: copy alias of the AssociationEnd if the alias in the subset is empty.
            //target.save();
        }
    }
}
