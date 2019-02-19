
using System;
using System.Collections.Generic;
using System.Text;
using EAAddinFramework.Utilities;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
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
		private TSF_EA.Model model;
		private EASchema ownerSchema;
		private HashSet<SBF.SchemaProperty> _schemaProperties;
		private TSF_EA.ElementWrapper _sourceElement;
		private HashSet<SBF.SchemaAssociation> _schemaAssociations;
		private HashSet<SBF.SchemaLiteral> _schemaLiterals;
		
		public EASchemaElement(TSF_EA.Model model,EASchema owner, EA.SchemaType objectToWrap)
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
		private HashSet<EASchemaPropertyWrapper> _schemaPropertyWrappers;
		private HashSet<EASchemaPropertyWrapper> schemaPropertyWrappers
		{
			get
			{
				if (_schemaPropertyWrappers == null)
				{
					_schemaPropertyWrappers = new HashSet<EASchemaPropertyWrapper>();
					foreach (EA.SchemaProperty schemaProperty in this.getSchemaProperties())
					{
						EASchemaPropertyWrapper wrapper = EASchemaBuilderFactory.getInstance(this.model).createSchemaPropertyWrapper(this, schemaProperty);
						if (wrapper != null)
						{
							_schemaPropertyWrappers.Add(wrapper);
						}
					}
				}
				return _schemaPropertyWrappers;
			}
		}
		private List<EA.SchemaProperty> getSchemaProperties ()
		{
			List<EA.SchemaProperty> localSchemaProperties = new List<EA.SchemaProperty>();
			EA.SchemaPropEnum schemaPropEnumerator = this.wrappedSchemaType.Properties;
			EA.SchemaProperty schemaProperty = schemaPropEnumerator.GetFirst();
			while (	schemaProperty != null)
			{
				localSchemaProperties.Add(schemaProperty);
				schemaProperty = schemaPropEnumerator.GetNext();
			}
			return localSchemaProperties;
		}
		public HashSet<SBF.SchemaProperty> schemaProperties 
		{
			get 
			{
				if (this._schemaProperties == null)
				{
					this._schemaProperties = new HashSet<SBF.SchemaProperty>(this.schemaPropertyWrappers.OfType<SBF.SchemaProperty>());
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
					this._schemaLiterals = new HashSet<SBF.SchemaLiteral>(this.schemaPropertyWrappers.OfType<SBF.SchemaLiteral>());
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
					this._schemaAssociations = new HashSet<SBF.SchemaAssociation>(this.schemaPropertyWrappers.OfType<SBF.SchemaAssociation>());
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
            if (this.sourceElement == null)
            {
                return null;
            }
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
                                              ,((TSF_EA.ElementWrapper)sourceElement).id
                                              , LogTypeEnum.warning);						
				}
			}
			//stereotypes
			this.subsetElement.stereotypes = this.sourceElement.stereotypes;
            //abstract
            this.subsetElement.isAbstract = this.sourceElement.isAbstract;
			//alias
			//only copy alias is the alias in the subset is empty
			if(string.IsNullOrEmpty(((TSF_EA.ElementWrapper)subsetElement).alias))
			                        ((TSF_EA.ElementWrapper)subsetElement).alias = ((TSF_EA.ElementWrapper)sourceElement).alias;
			//Check if the subset alias is different from the source alias and issue warning if that is the case
			if (!string.Equals(((TSF_EA.ElementWrapper)subsetElement).alias,((TSF_EA.ElementWrapper)sourceElement).alias))
			{
					EAOutputLogger.log(this.model,this.owner.settings.outputName
                                              ,string.Format("Property '{0}' has alias '{1}' in the model and a different alias '{2}' in the subset"
                                  					,this.sourceElement.name
                                  					,((TSF_EA.ElementWrapper)subsetElement).alias
                                  					,((TSF_EA.ElementWrapper)sourceElement).alias)
                                              ,((TSF_EA.ElementWrapper)sourceElement).id
                                              , LogTypeEnum.warning);				
			}
			//genlinks
			((TSF_EA.ElementWrapper)subsetElement).genLinks = ((TSF_EA.ElementWrapper)sourceElement).genLinks;
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
			((TSF_EA.Element) this.subsetElement).save();
			//copy tagged values
			((EASchema) this.owner).copyTaggedValues((TSF_EA.Element)this.sourceElement, (TSF_EA.Element)this.subsetElement);
			//set reference to source element
			if (this.owner.settings.tvInsteadOfTrace)
			{
				//add a tagged value from the subset element to the source element
				((TSF_EA.ElementWrapper)this.subsetElement).addTaggedValue(this.owner.settings.elementTagName,this.sourceElement.uniqueID);
			}
			else
			{
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
			}
			//return the new element
			return this.subsetElement;
		}
		/// <summary>
		/// If all attributes are coming from the same source element, and the order of the existing attributes corresponds exactly to that of the
		/// source element, then we use the same order as the order of the source element. That means that attributes will be inserted in the sequence.
		/// If not all attributes are from the same source element, or if the order of the subset attributes does not corresponds 
		/// with the order of the attributes in the source element then all new attributes are added at the end in alphabetical order.
		/// </summary>
		public void orderAttributes()
		{
			//get all new attributes
			var newAttributes = this.schemaPropertyWrappers.Where(x => x.isNew && x.subsetAttributeWrapper != null);
			var existingAttributes = this.schemaPropertyWrappers.Where(x => ! x.isNew && x.subsetAttributeWrapper != null);
			//first figure out if all attributes are coming from the same sourceElement
			var firstProperty = newAttributes.FirstOrDefault(x => x.sourceAttributeWrapper != null);
			var firstSourceOwner = firstProperty != null ? firstProperty.sourceAttributeWrapper.owner : null;
			bool oneSourceElement = this.schemaPropertyWrappers
								.Where(x => x.sourceAttributeWrapper != null)
								.All(x => x.sourceAttributeWrapper.owner.Equals(firstSourceOwner));
			//order all existing subset attributes if ther is more then one subsetproperty with position 0
			List<TSF_EA.AttributeWrapper>orderedExistingAttributes;
			if (existingAttributes.Where(x => x.subsetAttributeWrapper.position == 0)
							    .Skip(1)
							    .Any())
			{
				orderedExistingAttributes = existingAttributes
											.Select(a => a.subsetAttributeWrapper)
											.OrderBy(y => y.name)
											.ToList();
				//order the existing attributes is they are not yet ordered
				for (int i = 0; i < orderedExistingAttributes.Count() ; i++)
				{
					orderedExistingAttributes[i].position = i ;
					orderedExistingAttributes[i].save();
				}
			}
			else
			{
				orderedExistingAttributes = existingAttributes
										.Select(a => a.subsetAttributeWrapper)
										.OrderBy(x => x.position)
										.ThenBy(y => y.name)
										.ToList();
			}
			//get the maximum position nbr from the existing attributes
			var lastAtribute = orderedExistingAttributes.LastOrDefault();
			int lastIndex = lastAtribute != null ? lastAtribute.position : -1;
			// if there is only one source element then we use the position,else we use the name for sorting.
			if (newAttributes.Any())
			{
				List<TSF_EA.AttributeWrapper> orderedNewAttributes;
				if (oneSourceElement)
				{
					orderedNewAttributes = newAttributes
										.Select(x => x.subsetAttributeWrapper)
										.OrderBy(y => y.position)
										.ThenBy(z => z.name)
										.ToList();
				}
				else
				{
					orderedNewAttributes = newAttributes
										.Select(x => x.subsetAttributeWrapper)
										.OrderBy(z => z.name)
										.ToList();
				}
				for (int i = 0; i < orderedNewAttributes.Count() ; i++)
				{
					orderedNewAttributes[i].position = lastIndex + 1 + i ;
					orderedNewAttributes[i].save();
				}
			}
		}

		/// <summary>
		/// orders the associations of this element alphabetically
		/// </summary>
		public void orderAssociationsAlphabetically()
		{
			if (this.subsetElement != null)
			{
				var orderedList = new List<TSF_EA.Element>();
                //add the associations
                var associations = this.subsetElement.getRelationships<Association>(true, false)
                                    .Cast<TSF_EA.Association>().OrderBy(x => x.orderingName); 
                IEnumerable<TSF_EA.Association> choiceAssociations = new List<TSF_EA.Association>();
                if (this.owner.settings.orderXmlChoiceBeforeAttributes)
                {
                    //get the associations to XmlChoice elements
                    choiceAssociations = associations.Where(x => x.targetElement.HasStereotype("XSDchoice"))
                                                    .OrderBy(x => x.orderingName);
                    //get the other associations
                    associations = associations.Where(x => !choiceAssociations.Contains(x)).OrderBy(x => x.orderingName); 
                }
                
                //add attributes depending on the settings
                if (associations.Any() || choiceAssociations.Any())
                {
                    //get the attributes
                    var attributes = this.subsetElement.attributes.Cast<TSF_EA.Attribute>().OrderBy(x => x.orderingName);
                    //add attributes and associations
                    orderedList.AddRange(attributes);
                    orderedList.AddRange(associations);
                    //order if needed
                    if (this.owner.settings.orderAssociationsAmongstAttributes)
                    {
                        //order again
                        orderedList = orderedList.OrderBy(x => x.orderingName).ToList();
                    }
                    //add choice associations if needed
                    if (this.owner.settings.orderXmlChoiceBeforeAttributes)
                    {
                        orderedList.InsertRange(0, choiceAssociations);
                    }
                    int i = 1;
                    //set the order for both associations and Attributes
                    foreach (var element in orderedList)
                    {
                        var association = element as TSF_EA.Association;
                        if (association != null)
                        {
                            association.sourceEnd.addTaggedValue("position", i.ToString());
                        }
                        else
                        {
                            var attribute = element as TSF_EA.Attribute;
                            if (attribute != null)
                            {
                                attribute.addTaggedValue("position", i.ToString());
                            }
                        }
                        //up the counter
                        i++;
                    }
                }
			}
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
						var newGeneralization = this.model.factory.createNewElement<TSF_EA.Generalization>(this.subsetElement,string.Empty);
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
			//start creating the subset attributes
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
		/// clean up any attribute type dependencies we don't neeed anymore
		/// </summary>
		public void cleanupAttributeDependencies()
		{
			if (this.subsetElement != null)
			{
				foreach (var dependency in this.subsetElement.getRelationships<TSF_EA.Dependency>().Where(x => x.source.Equals(this.subsetElement) 
				                                                                                          && ! x.stereotypes.Any()))
				{
					//if the settings say we don't need to create attribute type dependencies then we delete them all
					if (this.owner.settings.dontCreateAttributeDependencies)
					{
						dependency.delete();
					}
					else
					{
						//if there are not attributes that have the same name as the depencency and the same type as the target then
						//the dependency can be deleted
						if (!this.subsetElement.attributes.Any(x => dependency.target.Equals(x.type) && dependency.name == x.name))
						{
							dependency.delete();
						}
					}
				}
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
							    && ! subsetGeneralizations.Any(x => x.target.Equals(schemaParent.subsetElement))
							   	&& schemaParent.subsetElement != null)
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
						&& schemaParent != null
						&& schemaParent.subsetElement != null)
					{
					    if ( schemaParent.subsetElement != null 
					    && ! subsetGeneralizations.Any(x => x.target.Equals(schemaParent.subsetElement)))
						{
						//generalization doesn't exist yet. Add it
						var newGeneralization = this.model.factory.createNewElement<TSF_EA.Generalization>(this.subsetElement,string.Empty);
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
							var newGeneralization = this.model.factory.createNewElement<TSF_EA.Generalization>(this.subsetElement,string.Empty);
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
				foreach (TSF_EA.Attribute attribute in this.subsetElement.attributes) 
				{
					//tell the user what we are doing 
					EAOutputLogger.log(this.model,this.owner.settings.outputName,"Matching subset attribute: '" + attribute.name + "' to a schema property"
					                   ,((TSF_EA.ElementWrapper)subsetElement).id, LogTypeEnum.log);
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
				var subsetElementWrapper = subsetElement as TSF_EA.ElementWrapper;
				if (subsetElementWrapper != null)
				{
					foreach (TSF_EA.EnumerationLiteral literal in subsetElementWrapper.ownedLiterals) 
					{
						//tell the user what we are doing 
						EAOutputLogger.log(this.model,this.owner.settings.outputName,"Matching subset literal: '" + literal.name + "' to a schema property"
					                   ,subsetElementWrapper.id, LogTypeEnum.log);
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
		EASchemaLiteral getMatchingSchemaLiteral(TSF_EA.EnumerationLiteral literal)
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
					if (((TSF_EA.EnumerationLiteral)schemaLiteral.sourceLiteral)?.guid == tagReference)
					{
						result = schemaLiteral;
						break;
					}
				}
			}
			return result;
		}
        /// <summary>
        /// get the subset element that represents this element in the destination package
        /// </summary>
        /// <param name="destinationPackage"></param>
        public void matchSubsetElement(Package destinationPackage)
        {
            string sqlGetClassifiers;
            if (this.ownerSchema.settings.tvInsteadOfTrace)
            {
                //get the classifier in the subset that represents this element
                sqlGetClassifiers = "select distinct o.Object_ID from t_object o "
                                   + "  inner join t_objectproperties p on p.Object_ID = o.Object_ID"
                                   + $" where p.Property = '{this.ownerSchema.settings.elementTagName}'"
                                   + $"  and p.Value = '{this.sourceElement?.uniqueID}'"
                                   + $"  and o.Package_ID in ({((TSF_EA.Package)destinationPackage).packageTreeIDString})";
            }
            else
            {
                //get the classifier in the subset that represents this element
                sqlGetClassifiers = "select distinct o.Object_ID from ((t_object o"
                                       + " inner join t_connector c on(c.Start_Object_ID = o.Object_ID"
                                       + "                and c.Connector_Type = 'Abstraction'"
                                       + "               and c.Stereotype = 'trace'))"
                                       + "   inner join t_object ot on ot.Object_ID = c.End_Object_ID)"
                                       + $"    where ot.ea_guid = '{this.sourceElement?.uniqueID}'"
                                       + $"  and o.Package_ID in ({((TSF_EA.Package)destinationPackage).packageTreeIDString})";
            }
            //match with this classifier
            this.matchSubsetElement(this.model.getElementWrappersByQuery(sqlGetClassifiers).OfType<Classifier>().FirstOrDefault());
        }

        /// <summary>
        /// finds the corresponding Schema property for the given attribut
        /// </summary>
        /// <param name="attribute">attribute</param>
        /// <returns>the corresponding Schema property if one is found. Else null</returns>
        public EASchemaProperty getMatchingSchemaProperty(TSF_EA.Attribute attribute)
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
					if (((TSF_EA.Attribute)property.sourceProperty)?.guid == tagReference)
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
                //tell the user what we are doing 
                EAOutputLogger.log(this.model, this.owner.settings.outputName, "Matching relations of subset element: '" + subsetElement.name + "' to the schema"
                                   , ((TSF_EA.ElementWrapper)subsetElement).id, LogTypeEnum.log);
                foreach (TSF_EA.Association association in this.subsetElement.getRelationships<Association>()) 
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
		public EASchemaAssociation getMatchingSchemaAssociation(TSF_EA.Association association)
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
					if (((TSF_EA.Association)schemaAssociation.sourceAssociation).guid == tagReference)
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
