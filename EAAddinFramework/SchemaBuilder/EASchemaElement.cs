
using System;
using System.Collections.Generic;
using EAAddinFramework.Utilities;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using TSF.UmlToolingFramework.UML.Classes.Kernel;

namespace EAAddinFramework.SchemaBuilder
{
	/// <summary>
	/// Description of EASchemaElement.
	/// </summary>
	public class EASchemaElement : SBF.SchemaElement
	{
		internal EA.SchemaType wrappedSchemaType;
		private UTF_EA.Model model;
		private EASchema ownerSchema;
		private HashSet<SBF.SchemaProperty> _schemaProperties;
		private UTF_EA.ElementWrapper _sourceElement;
		private HashSet<SBF.SchemaAssociation> _schemaAssociations;
		private HashSet<SBF.SchemaLiteral> _schemaLiterals;
		
		public EASchemaElement(UTF_EA.Model model,EASchema owner, EA.SchemaType objectToWrap)
		{
			this.model = model;
			this.ownerSchema = owner;
			this.wrappedSchemaType = objectToWrap;
		}

		public bool isShared 
		{
			get
			{
				if (sourceElement != null)
				{
					if (isSharedElement(sourceElement))
					{
						return true;
					}
					//if not on element level we check on package level
					return isSharedElement(sourceElement.owningPackage);                                
				}
				return false;
			}
		}
		private bool isSharedElement(Element element)
		{
			return element.taggedValues.Any(x => x.name.Equals("shared",StringComparison.InvariantCultureIgnoreCase)
			                                && x.tagValue.ToString().Equals("true",StringComparison.InvariantCultureIgnoreCase));
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

	    public string TypeID
	    {
	        get { return this.wrappedSchemaType.GUID; }
            set
            {
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
		
		public HashSet<SBF.SchemaLiteral> schemaLiterals 
		{
			get 
			{
				if (this._schemaLiterals == null)
				{
					this._schemaLiterals = new HashSet<SBF.SchemaLiteral>();
					foreach (EASchemaPropertyWrapper wrapper in this.schemaPropertyWrappers) 
					{
						if (wrapper is EASchemaLiteral)
						{
							this._schemaLiterals.Add((SBF.SchemaLiteral)wrapper );
						}
					}
				}
				return this._schemaLiterals;
			}
			set {throw new NotImplementedException();}
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
				if (this.sourceElement is UML.Classes.Kernel.Enumeration)
				{
					this.subsetElement = this.model.factory.createNewElement<UML.Classes.Kernel.Enumeration>(destinationPackage, this.wrappedSchemaType.TypeName);
				}
				else if (this.sourceElement is UML.Classes.AssociationClasses.AssociationClass)
				{
					this.subsetElement = this.model.factory.createNewElement<UML.Classes.AssociationClasses.AssociationClass>(destinationPackage, this.wrappedSchemaType.TypeName);
				}
				else if (this.sourceElement is UML.Classes.Kernel.Class)
				{
					this.subsetElement = this.model.factory.createNewElement<UML.Classes.Kernel.Class>(destinationPackage, this.wrappedSchemaType.TypeName);
				}
                else if (this.sourceElement is UML.Classes.Kernel.DataType)
                {
                    this.subsetElement = this.model.factory.createNewElement<UML.Classes.Kernel.DataType>(destinationPackage, this.wrappedSchemaType.TypeName);
                }
            }
			else
			{
				//report change of name of the element
				if (this.sourceElement.name != this.subsetElement.name)
				{
					EAOutputLogger.log(this.model,this.owner.settings.outputName
                                              ,string.Format("Element '{0}' has changed name from '{1}' since the last schema generation"
                                  					,this.sourceElement.name
                                  					,this.subsetElement.name)
                                              ,((UTF_EA.ElementWrapper)sourceElement).id
                                              , LogTypeEnum.warning);						
				}
			}
			//stereotypes
			this.subsetElement.stereotypes = this.sourceElement.stereotypes;
			//alias
			((UTF_EA.ElementWrapper)subsetElement).alias = ((UTF_EA.ElementWrapper)sourceElement).alias;
			//genlinks
			((UTF_EA.ElementWrapper)subsetElement).genLinks = ((UTF_EA.ElementWrapper)sourceElement).genLinks;
			//notes only update them if they are empty
			if (this.subsetElement.ownedComments.Count == 0 || ! this.subsetElement.ownedComments.Any(x => x.body.Length > 0))
			{
				this.subsetElement.ownedComments = this.sourceElement.ownedComments;
				if (this.owner.settings.prefixNotes
				    && this.owner.settings.prefixNotesText.Length > 0
				    && this.subsetElement.ownedComments.Any(x => x.body.Length > 0))
				{
					foreach (var comment in subsetElement.ownedComments) 
					{
						comment.body = this.owner.settings.prefixNotesText + Environment.NewLine + comment.body;
					}	
				}
				
			}
			//save the new subset element
			((UTF_EA.Element) this.subsetElement).save();
			//copy tagged values
			((EASchema) this.owner).copyTaggedValues((UTF_EA.Element)this.sourceElement, (UTF_EA.Element)this.subsetElement);
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
			//copy the generatlizations for enumerations and for datatypes if that setting is set
//			if (this.sourceElement is UML.Classes.Kernel.Enumeration
//			   || this.sourceElement is UML.Classes.Kernel.DataType && this.owner.settings.copyDataTypeGeneralizations)
//			{
//				this.copyGeneralizations();
//			}
			//return the new element
			return this.subsetElement;
		}
		
		internal void setAssociationClassProperties()
		{
			//TODO
			//create the link tot he association if the association is in the subset
			//this.owner.
		}
		/// <summary>
		/// copy the generalizations from the source elemnt to the subset element, 
		/// removing the generalizations that are not present in the source element
		/// </summary>
		private void copyGeneralizations()
		{
			if (this.sourceElement != null && this.subsetElement != null)
			{
				var sourceGeneralizations = this.sourceElement.getRelationships<UML.Classes.Kernel.Generalization>()
					.Where(x => x.source.Equals(this.sourceElement));
				var subsetGeneralizations = this.subsetElement.getRelationships<UML.Classes.Kernel.Generalization>()
					.Where(x => x.source.Equals(this.subsetElement));
				//remove generalizations that shouldn't be there
				foreach ( var subsetGeneralization in subsetGeneralizations)
				{
					if (! sourceGeneralizations.Any(x => x.target.Equals(subsetGeneralization.target)))
				    {
						//if it doesn't exist in the set of source generalizations then we delete the generalization
						subsetGeneralization.delete();
				    }
				}
				//add the generalizations that don't exist at the subset yet
				foreach (var sourceGeneralization in sourceGeneralizations) 
				{
					if (! subsetGeneralizations.Any(x => x.target.Equals(sourceGeneralization.target)))
					{
						//generalization doesn't exist yet. Add it
						var newGeneralization = this.model.factory.createNewElement<UTF_EA.Generalization>(this.subsetElement,string.Empty);
						newGeneralization.addRelatedElement(sourceGeneralization.target);
						newGeneralization.save();
					}
				}
			}
			
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
		/// duplicates the attributes in the schema as attributes in the subset element
		/// </summary>
		public void createSubsetAttributes()
		{
			foreach (EASchemaProperty schemaProperty in this.schemaProperties) 
			{
				schemaProperty.createSubsetProperty();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public void createSubsetLiterals()
		{
			foreach (EASchemaLiteral schemaLiteral in this.schemaLiterals) 
			{
				schemaLiteral.createSubsetLiteral();
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
		public void matchSubsetElement(UML.Classes.Kernel.Classifier subsetElement)
		{
			//set the subset element
			this.subsetElement = subsetElement;
		}
		/// <summary>
		/// Adds generalizations if the parent of the generalization is in the subset elements as well
		/// or if the settings allow us to copy generalizations
		/// </summary>
		public void addGeneralizations()
		{
			if (this.sourceElement != null && this.subsetElement != null)
			{
				var sourceGeneralizations = this.sourceElement.getRelationships<UML.Classes.Kernel.Generalization>()
					.Where(x => x.source.Equals(this.sourceElement));
				var subsetGeneralizations = this.subsetElement.getRelationships<UML.Classes.Kernel.Generalization>()
					.Where(x => x.source.Equals(this.subsetElement));
				//remove generalizations that shouldn't be there
				foreach ( var subsetGeneralization in subsetGeneralizations)
				{
					var schemaParent = ((EASchema)this.owner).getSchemaElementForSubsetElement(subsetGeneralization.target as Classifier);
					if (schemaParent == null) //the generalization does not target an element in the schema
					{
						schemaParent = ((EASchema)this.owner).getSchemaElementForUMLElement(subsetGeneralization.target);
						if (schemaParent != null
						    && schemaParent.subsetElement != null)
						{
							//if the settings redirectGeneralizationsToSubset is set and
							//the generalization exists on the source, but there is a subset element for the target
							//we move the generalization to the element in the subset.
							if (this.owner.settings.redirectGeneralizationsToSubset
							    && ! subsetGeneralizations.Any(x => x.target.Equals(schemaParent.subsetElement)))
						    {
								//if the generalization doesn't exist yet we move it tot he subset element
						    	subsetGeneralization.target = schemaParent.subsetElement;
								subsetGeneralization.save();
						    }
							else
							{
								//Enumerations should always keep their external Generalizations
								//Datatypes should only keep their external Generalizatons if the settings is on
								//classes can keep their external generalizations if the setting allows it
								if (! (this.sourceElement is UML.Classes.Kernel.Enumeration)
								    && !(this.sourceElement is UML.Classes.Kernel.DataType && this.owner.settings.copyDataTypeGeneralizations)
								    && !(this.sourceElement is UML.Classes.Kernel.Class && this.owner.settings.copyGeneralizations))
								//The generalization should not be there
								subsetGeneralization.delete();
							}
						}
					}
					else //the generalization targets an element in the schema. 
					{
						// Make sure the setting to redirect to the subset is on and
						// and the source element has an equivalent generalization.
						if (! this.owner.settings.redirectGeneralizationsToSubset
							|| ! sourceGeneralizations.Any(x => x.target.Equals(schemaParent.sourceElement)))
					    {
							//the source doesn't have a generalization like this
							subsetGeneralization.delete();
					    }
					}
				}
				//add the generalizations that don't exist at the subset yet
				foreach (var sourceGeneralization in sourceGeneralizations) 
				{
					var schemaParent = ((EASchema)this.owner).getSchemaElementForUMLElement(sourceGeneralization.target);
					if (this.owner.settings.redirectGeneralizationsToSubset
						&& schemaParent != null)
					{
					    if ( schemaParent.subsetElement != null 
					    && ! subsetGeneralizations.Any(x => x.target.Equals(schemaParent.subsetElement)))
						{
						//generalization doesn't exist yet. Add it
						var newGeneralization = this.model.factory.createNewElement<UTF_EA.Generalization>(this.subsetElement,string.Empty);
						newGeneralization.addRelatedElement(schemaParent.subsetElement);
						newGeneralization.save();
						}
					}
					else
					{
						//the target is not in the schema.
						//Enumerations should always get their external Generalizations
						//Datatypes should only get their external Generalizatons if the settings is on
						if ((this.sourceElement is UML.Classes.Kernel.Enumeration
						    || (this.sourceElement is UML.Classes.Kernel.DataType && this.owner.settings.copyDataTypeGeneralizations)
						   || this.sourceElement is UML.Classes.Kernel.Class && this.owner.settings.copyGeneralizations)
						&& ! subsetGeneralizations.Any(x => x.target != null && x.target.Equals(sourceGeneralization.target)))
						{
							//generalization doesn't exist yet. Add it
							var newGeneralization = this.model.factory.createNewElement<UTF_EA.Generalization>(this.subsetElement,string.Empty);
							newGeneralization.addRelatedElement(sourceGeneralization.target);
							newGeneralization.save();
						}
					}
				}
			}
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
						//only delete if stereotype not in list of ignored stereotypes
						
						if (attribute.stereotypes.Count(x => this.owner.settings.ignoredStereotypes.Contains(x.name)) <= 0)
						{
							//no match, delete the attribute
							attribute.delete();
						}
					}
				}
			}
		}

		public void matchSubsetLiterals()
		{
			if (this.subsetElement != null)
			{
				var subsetElementWrapper = subsetElement as UTF_EA.ElementWrapper;
				if (subsetElementWrapper != null)
				{
					foreach (UTF_EA.EnumerationLiteral literal in subsetElementWrapper.ownedLiterals) 
					{
						EASchemaLiteral matchingLiteral = this.getMatchingSchemaLiteral(literal);
						if (matchingLiteral != null)
						{
							//found a match
							matchingLiteral.subSetLiteral = literal;
						}
						else
						{
							//only delete if stereotype not in list of ignored stereotypes
							if (literal.stereotypes.Count(x => this.owner.settings.ignoredStereotypes.Contains(x.name)) <= 0)
							{
								//no match, delete the literal
								literal.delete();
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// gets the SchemaLiteral that corresponds with the given literal value
		/// </summary>
		/// <param name="literal">the literal to match</param>
		/// <returns>the corresponding SchemaLiteral</returns>
		EASchemaLiteral getMatchingSchemaLiteral(UTF_EA.EnumerationLiteral literal)
		{
			EASchemaLiteral result = null;
			var sourceAttributeTag = literal.getTaggedValue(this.owner.settings.sourceAttributeTagName);
			if (sourceAttributeTag != null)
			{
				string tagReference = sourceAttributeTag.eaStringValue;
			
				foreach (EASchemaLiteral schemaLiteral in this.schemaLiterals) 
				{
					//we have the same attribute if the given attribute has a tagged value 
					//called sourceAttribute that refences the source attribute of the schema Property
					if (((UTF_EA.EnumerationLiteral)schemaLiteral.sourceLiteral).guid == tagReference)
					{
						result = schemaLiteral;
						break;
					}
				}
			}
			return result;
		}
		/// <summary>
		/// finds the corresponding Schema property for the given attribut
		/// </summary>
		/// <param name="attribute">attribute</param>
		/// <returns>the corresponding Schema property if one is found. Else null</returns>
		public EASchemaProperty getMatchingSchemaProperty(UTF_EA.Attribute attribute)
		{
			EASchemaProperty result = null;
			var sourceAttributeTag = attribute.getTaggedValue(this.owner.settings.sourceAttributeTagName);
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
						    if (matchingAssociation.subsetAssociations == null)
						    {
                                matchingAssociation.subsetAssociations = new List<Association>();

                            }
						    matchingAssociation.subsetAssociations.Add(association);
						}
						else
						{
							//only delete if the target does not have a stereotype in the list of ignored stereotypes or if this association does not have an ignored stereotype
							if (association.target.stereotypes.Count(x => this.owner.settings.ignoredStereotypes.Contains(x.name)) <= 0
							    && association.stereotypes.Count(x => this.owner.settings.ignoredStereotypes.Contains(x.name)) <= 0)
							{
								//no match, delete the association
								association.delete();
							}
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
			var sourceAssociationTag = association.getTaggedValue(this.owner.settings.sourceAssociationTagName);
			if (sourceAssociationTag != null)
			{
				string tagReference = sourceAssociationTag.eaStringValue;
			
				foreach (EASchemaAssociation schemaAssociation in this.schemaAssociations) 
				{
					//we have the same attribute if the given attribute has a tagged value 
					//called sourceAssociation that refences the source association of the schema Association
					if (((UTF_EA.Association)schemaAssociation.sourceAssociation).guid == tagReference)
					{
						//if the schema association has choiceElements then the target of the association should match one of the choice elements
						if ((schemaAssociation.choiceElements != null 
						     && (schemaAssociation.choiceElements.Count == 0 
						         || schemaAssociation.choiceElements.Exists(x => association.target.Equals(x.subsetElement))))
						    || schemaAssociation.choiceElements == null)
						{
							result = schemaAssociation;
							break;
						}
					}
				}
			}
			return result;
		}

	}
}
