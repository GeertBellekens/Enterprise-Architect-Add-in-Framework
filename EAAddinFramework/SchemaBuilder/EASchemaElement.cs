
using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using SBF = SchemaBuilderFramework;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;

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

        public EASchemaElement(TSF_EA.Model model, EASchema owner, EA.SchemaType objectToWrap)
        {
            this.model = model;
            this.ownerSchema = owner;
            this.wrappedSchemaType = objectToWrap;
        }

        public bool isShared
        {
            get
            {
                if (this.sourceElement != null)
                {
                    if (this.isSharedElement(this.sourceElement))
                    {
                        return true;
                    }
                    //if not on element level we check on package level
                    if (this.isSharedElement(this.sourceElement.owningPackage))
                    {
                        return true;
                    }
                    //if not on package level, we check on local shared property (package GUID in a tagged value with name "sharedPackage" on the schema container element
                    return this.ownerSchema.isSharedLocalPackage(this.sourceElement.owningPackage);
                }
                return false;
            }
        }

        private bool isSharedElement(Element element)
        {
            return element.taggedValues.Any(x => x.name.Equals("shared", StringComparison.InvariantCultureIgnoreCase)
                                            && x.tagValue.ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// the name of the Schema element. In most cases this is equal to the name of the source element, unless the element has been redefined.
        /// </summary>
        public string name
        {
            get => this.wrappedSchemaType.TypeName;
        }
        /// <summary>
        /// There is no direct way to know if an element is redefined, so we assume that it is when the typename differs from the source elements name
        /// </summary>
        public bool isRedefined
        {
            get => this.sourceElement?.name != this.wrappedSchemaType.TypeName;
        }

        public string TypeID
        {
            get => this.wrappedSchemaType.GUID;
        }
        public UML.Classes.Kernel.Classifier sourceElement
        {
            get
            {
                if (this._sourceElement == null)
                {
                    this._sourceElement = this.model.getElementWrapperByID(this.wrappedSchemaType.TypeID);
                }
                return this._sourceElement as UML.Classes.Kernel.Classifier;
            }
            set => this._sourceElement = value as TSF_EA.ElementWrapper;
        }
        internal TSF_EA.ElementWrapper eaSourceElement => this.sourceElement as TSF_EA.ElementWrapper;
        public UML.Classes.Kernel.Classifier subsetElement { get; set; }
        internal TSF_EA.ElementWrapper eaSubsetElement => this.subsetElement as TSF_EA.ElementWrapper;

        public SchemaBuilderFramework.Schema owner
        {
            get => this.ownerSchema;
            set => this.ownerSchema = (EASchema)value;
        }
        EASchemaElement _parentSchemaElement;
        private EASchemaElement parentSchemaElement
        {
            get
            {
                if (this._parentSchemaElement == null)
                {
                    this._parentSchemaElement = ((EASchema)this.owner).getSchemaElementForSchemaType(this.wrappedSchemaType.Parent);
                }
                return this._parentSchemaElement;
            }
        }
        private HashSet<EASchemaPropertyWrapper> _schemaPropertyWrappers;
        private HashSet<EASchemaPropertyWrapper> schemaPropertyWrappers
        {
            get
            {
                if (this._schemaPropertyWrappers == null)
                {
                    this._schemaPropertyWrappers = new HashSet<EASchemaPropertyWrapper>();
                    foreach (EA.SchemaProperty schemaProperty in this.getSchemaProperties())
                    {
                        EASchemaPropertyWrapper wrapper = EASchemaBuilderFactory.getInstance(this.model).createSchemaPropertyWrapper(this, schemaProperty);
                        if (wrapper != null)
                        {
                            this._schemaPropertyWrappers.Add(wrapper);
                        }
                    }
                }
                return this._schemaPropertyWrappers;
            }
        }
        private List<EA.SchemaProperty> getSchemaProperties()
        {
            List<EA.SchemaProperty> localSchemaProperties = new List<EA.SchemaProperty>();
            EA.SchemaPropEnum schemaPropEnumerator = this.wrappedSchemaType.Properties;
            EA.SchemaProperty schemaProperty = schemaPropEnumerator.GetFirst();
            while (schemaProperty != null)
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
            set => throw new NotImplementedException();
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
            set => throw new NotImplementedException();
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
            set => throw new NotImplementedException();
        }

        public Classifier createSubsetElement(Package destinationPackage)
        {
            if (this.sourceElement == null)
            {
                return null;
            }
            //first create the element in the destination Package
            if (this.subsetElement == null)
            {
                if (this.sourceElement is Enumeration)
                {
                    this.subsetElement = this.model.factory.createNewElement<Enumeration>(destinationPackage, this.wrappedSchemaType.TypeName);
                }
                else if (this.sourceElement is UML.Classes.AssociationClasses.AssociationClass)
                {
                    this.subsetElement = this.model.factory.createNewElement<UML.Classes.AssociationClasses.AssociationClass>(destinationPackage, this.wrappedSchemaType.TypeName);
                }
                else if (this.sourceElement is Class)
                {
                    this.subsetElement = this.model.factory.createNewElement<Class>(destinationPackage, this.wrappedSchemaType.TypeName);
                }
                else if (this.sourceElement is PrimitiveType)
                {
                    this.subsetElement = this.model.factory.createNewElement<PrimitiveType>(destinationPackage, this.wrappedSchemaType.TypeName);
                }
                else if (this.sourceElement is DataType)
                {
                    this.subsetElement = this.model.factory.createNewElement<DataType>(destinationPackage, this.wrappedSchemaType.TypeName);
                }
            }
            else
            {
                //report change of name of the element
                if (this.sourceElement.name != this.subsetElement.name
                    && !this.isRedefined)
                {
                    EAOutputLogger.log(this.model, this.owner.settings.outputName
                                              , string.Format("Element '{0}' has changed name from '{1}' since the last schema generation"
                                                      , this.sourceElement.name
                                                      , this.subsetElement.name)
                                              , ((TSF_EA.ElementWrapper)this.sourceElement).id
                                              , LogTypeEnum.warning);
                }
            }
            //stereotypes
            if (this.sourceElement.stereotypes.Count == 1)
            {
                ((TSF_EA.ElementWrapper)this.subsetElement).fqStereotype = ((TSF_EA.ElementWrapper)this.sourceElement).fqStereotype;
            }
            else
            {
                this.subsetElement.stereotypes = this.sourceElement.stereotypes;
            }
            //abstract
            this.subsetElement.isAbstract = this.sourceElement.isAbstract;
            //alias
            if (this.isRedefined && this.owner.settings.useAliasForRedefinedElements)
            {
                //if redefined, and we use the useAliasForRedefinedElements setting, the name is kept like the source element, but the typename is used in the alias
                this.subsetElement.name = this.sourceElement.name;
                ((TSF_EA.ElementWrapper)this.subsetElement).alias = this.name;
            }
            else
            {
                //set the name
                this.subsetElement.name = this.name;
                //only copy alias if the alias in the source element is not empty
                if (!string.IsNullOrEmpty(((TSF_EA.ElementWrapper)this.sourceElement).alias))
                {
                    ((TSF_EA.ElementWrapper)this.subsetElement).alias = ((TSF_EA.ElementWrapper)this.sourceElement).alias;
                }

                //Check if the subset alias is different from the source alias and issue warning if that is the case
                if (!string.Equals(((TSF_EA.ElementWrapper)this.subsetElement).alias, ((TSF_EA.ElementWrapper)this.sourceElement).alias))
                {
                    EAOutputLogger.log(this.model, this.owner.settings.outputName
                                              , string.Format("Property '{0}' has alias '{1}' in the model and a different alias '{2}' in the subset"
                                                      , this.sourceElement.name
                                                      , ((TSF_EA.ElementWrapper)this.subsetElement).alias
                                                      , ((TSF_EA.ElementWrapper)this.sourceElement).alias)
                                              , ((TSF_EA.ElementWrapper)this.sourceElement).id
                                              , LogTypeEnum.warning);
                }
            }
            //genlinks
            ((TSF_EA.ElementWrapper)this.subsetElement).genLinks = ((TSF_EA.ElementWrapper)this.sourceElement).genLinks;
            //gentype, used to store the datatype type in Tables
            ((TSF_EA.ElementWrapper)this.subsetElement).genType = ((TSF_EA.ElementWrapper)this.sourceElement).genType;
            //pdata2 stored the database name
            ((TSF_EA.ElementWrapper)this.subsetElement).pdata2 = ((TSF_EA.ElementWrapper)this.sourceElement).pdata2;
            //keywords
            ((TSF_EA.ElementWrapper)this.subsetElement).keywords = ((TSF_EA.ElementWrapper)this.sourceElement).keywords;
            //notes only update them if they are empty
            if (this.subsetElement.ownedComments.Count == 0 || !this.subsetElement.ownedComments.Any(x => x.body.Length > 0)
                || this.owner.settings.keepNotesInSync)
            {
                this.subsetElement.ownedComments = this.sourceElement.ownedComments;
                if (!this.owner.settings.keepNotesInSync
                    && this.owner.settings.prefixNotes
                    && this.owner.settings.prefixNotesText.Length > 0
                    && this.subsetElement.ownedComments.Any(x => x.body.Length > 0))
                {
                    foreach (var comment in this.subsetElement.ownedComments)
                    {
                        comment.body = this.owner.settings.prefixNotesText + Environment.NewLine + comment.body;
                    }
                }

            }
            //copy default color
            ((TSF_EA.ElementWrapper)this.subsetElement).defaulBackColor = ((TSF_EA.ElementWrapper)this.sourceElement).defaulBackColor;
            //copy constraints
            this.copyConstraints();
            //save the new subset element
            ((TSF_EA.Element)this.subsetElement).save();
            //copy tagged values
            ((EASchema)this.owner).copyTaggedValues((TSF_EA.Element)this.sourceElement, (TSF_EA.Element)this.subsetElement);
            //set reference to source element
            if (this.owner.settings.tvInsteadOfTrace)
            {
                //add a tagged value from the subset element to the source element
                ((TSF_EA.ElementWrapper)this.subsetElement).addTaggedValue(this.owner.settings.elementTagName, this.sourceElement.uniqueID);
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

        private void copyConstraints()
        {
            //return if no subset elemnet exists
            if (this.subsetElement == null || this.sourceElement == null)
            {
                return;
            }
            //get the subset element constraints
            var tempSubsetContraints = new List<Constraint>(this.subsetElement.constraints);
            //copy constraints
            foreach (var constraint in this.sourceElement?.constraints)
            {
                //compare each of the constraints with the source element constraints
                var subsetConstraint = tempSubsetContraints.FirstOrDefault(x => x.name == constraint.name);
                if (subsetConstraint != null)
                {
                    tempSubsetContraints.Remove(subsetConstraint);
                }
                else
                {
                    subsetConstraint = this.model.factory.createNewElement<Constraint>(this.subsetElement, constraint.name);
                }
                //only update if not in the list of ignored constraint types
                if (!this.ownerSchema.settings.ignoredConstraintTypes.Contains(subsetConstraint.constraintType))
                {
                    //synch notes
                    subsetConstraint.specification = constraint.specification;
                    //save
                    subsetConstraint.save();
                }
            }
            //remove all remaining subset constraints
            foreach (var subsetConstraint in tempSubsetContraints)
            {
                if (!this.ownerSchema.settings.ignoredConstraintTypes.Contains(subsetConstraint.constraintType))
                {
                    subsetConstraint.delete();
                }
            }
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
            var existingAttributes = this.schemaPropertyWrappers.Where(x => !x.isNew && x.subsetAttributeWrapper != null);
            //first figure out if all attributes are coming from the same sourceElement
            var firstProperty = newAttributes.FirstOrDefault(x => x.sourceAttributeWrapper != null);
            var firstSourceOwner = firstProperty != null ? firstProperty.sourceAttributeWrapper.owner : null;
            bool oneSourceElement = this.schemaPropertyWrappers
                                .Where(x => x.sourceAttributeWrapper != null)
                                .All(x => x.sourceAttributeWrapper.owner.Equals(firstSourceOwner));
            //order all existing subset attributes if ther is more then one subsetproperty with position 0
            List<TSF_EA.AttributeWrapper> orderedExistingAttributes;
            if (existingAttributes.Where(x => x.subsetAttributeWrapper.position == 0)
                                .Skip(1)
                                .Any())
            {
                orderedExistingAttributes = existingAttributes
                                            .Select(a => a.subsetAttributeWrapper)
                                            .OrderBy(y => y.name)
                                            .ToList();
                //order the existing attributes is they are not yet ordered
                for (int i = 0; i < orderedExistingAttributes.Count(); i++)
                {
                    orderedExistingAttributes[i].position = i;
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
                for (int i = 0; i < orderedNewAttributes.Count(); i++)
                {
                    orderedNewAttributes[i].position = lastIndex + 1 + i;
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
                    choiceAssociations = associations.Where(x => x.targetElement.hasStereotype("XSDchoice"))
                                                    .OrderBy(x => x.orderingName);
                    //get the other associations
                    associations = associations.Where(x => !choiceAssociations.Contains(x)).OrderBy(x => x.orderingName);
                }

                //add attributes depending on the settings
                //get the attributes
                var attributesAndLiterals = new List<TSF_EA.AttributeWrapper>();
                attributesAndLiterals.AddRange(this.subsetElement.attributes.Cast<TSF_EA.Attribute>());
                //get the enumerationLiterals
                var subsetEnum = this.subsetElement as TSF_EA.Enumeration;
                if (subsetEnum != null)
                {
                    attributesAndLiterals.AddRange(subsetEnum.ownedLiterals.Cast<TSF_EA.EnumerationLiteral>());
                }
                //add attributes and associations
                orderedList.AddRange(attributesAndLiterals.OrderBy(x => x.orderingName));
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
                //keep track of all custom positions to detect conflicts
                var customPositions = new Dictionary<string, TSF_EA.Element>();
                //set the order for both associations and Attributes
                foreach (var element in orderedList)
                {
                    var customPosition = this.getCustomPosition(element);
                    string positionValue;
                    if (customPosition.HasValue)
                    {
                        //use customposition as position
                        positionValue = customPosition.Value.ToString();
                        //add custom position to list
                        if (!customPositions.ContainsKey(customPosition.Value.ToString()))
                        {
                            customPositions.Add(customPosition.Value.ToString(), element);
                        }
                        else
                        {
                            //conflict in custom positions
                            EAOutputLogger.log(this.model, this.owner.settings.outputName, $"Conflict with custom position '{positionValue}' on element '{this.subsetElement.name}'"
                                               , ((TSF_EA.ElementWrapper)this.subsetElement).id, LogTypeEnum.error);
                        }
                    }
                    else
                    {
                        //use i as position
                        positionValue = i.ToString();
                        //check if position value conflicts with existing custom position
                        if (customPositions.ContainsKey(positionValue))
                        {
                            //conflict in custom positions
                            EAOutputLogger.log(this.model, this.owner.settings.outputName, $"Conflict with custom position '{positionValue}' on element '{this.subsetElement.name}'"
                                               , ((TSF_EA.ElementWrapper)this.subsetElement).id, LogTypeEnum.error);
                        }
                    }
                    var association = element as TSF_EA.Association;
                    if (association != null)
                    {
                        association.sourceEnd.addTaggedValue("position", positionValue);
                    }
                    else
                    {
                        if (this.owner.settings.orderAssociationsAmongstAttributes)
                        {
                            var attribute = element as TSF_EA.AttributeWrapper;
                            if (attribute != null)
                            {
                                attribute.addTaggedValue("position", positionValue);
                            }
                        }
                    }
                    //up the counter
                    i++;
                }
            }
        }

        internal int? getCustomPosition(TSF_EA.Element element)
        {
            int? customPosition = null;
            //check if setting is filled in
            if (!string.IsNullOrEmpty(this.owner.settings.customPositionTag))
            {
                //check if tag exists
                var customPositionTag = element.taggedValues
                    .FirstOrDefault(x => x.name.Equals(this.owner.settings.customPositionTag, StringComparison.InvariantCultureIgnoreCase));
                int foundCustomPosition;
                //get custom position value from tag
                if (customPositionTag != null
                    && !string.IsNullOrWhiteSpace(customPositionTag.tagValue.ToString())
                    && int.TryParse(customPositionTag.tagValue.ToString(), out foundCustomPosition))
                {
                    customPosition = foundCustomPosition;
                }
            }
            //return
            return customPosition;
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
                foreach (var subsetGeneralization in subsetGeneralizations)
                {
                    if (!sourceGeneralizations.Any(x => x.target.Equals(subsetGeneralization.target)))
                    {
                        //if it doesn't exist in the set of source generalizations then we delete the generalization
                        subsetGeneralization.delete();
                    }
                }
                //add the generalizations that don't exist at the subset yet
                foreach (var sourceGeneralization in sourceGeneralizations)
                {
                    if (!subsetGeneralizations.Any(x => x.target.Equals(sourceGeneralization.target)))
                    {
                        //generalization doesn't exist yet. Add it
                        var newGeneralization = this.model.factory.createNewElement<TSF_EA.Generalization>(this.subsetElement, string.Empty);
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
        private void deleteInvalidSubsetOperations()
        {
            var sqlGetData = $@"select op.OperationID from t_operation op
                            left join t_operationtag tv on tv.ElementID = op.OperationID

                                                           and tv.Property = '{this.owner.settings.sourceOperationTagName}'
                            left join t_operation oop on oop.ea_guid = tv.VALUE

                                                    and oop.Object_ID = {this.eaSourceElement.id}
                            where 1 = 1
                            and oop.OperationID is null
                            and op.Object_ID = {this.eaSubsetElement.id}";
            var operationsToDelete = this.model.getOperationsByQuery(sqlGetData);
            foreach (var operation in operationsToDelete)
            {
                operation.delete();
            }
        }
        internal void createSubsetOperations()
        {
            //first delete the operations in the subset element that are not supposed to be there
            deleteInvalidSubsetOperations();
            //then create or update the operations
            foreach (var operation in this.eaSourceElement.ownedOperations.OfType<TSF_EA.Operation>())
            {
                //check if an operation referencing the original operation already exists
                var subsetOperation = this.eaSubsetElement.ownedOperations
                                    .OfType<TSF_EA.Operation>()
                                    .FirstOrDefault(x => x.taggedValues
                                                    .OfType<TSF_EA.OperationTag>()
                                                    .Any(y => y.name == this.owner.settings.sourceOperationTagName
                                                           && y.eaStringValue == operation.uniqueID));
                //clone the existing operation is that is not the case
                if (subsetOperation == null)
                {
                    subsetOperation = operation.clone(this.subsetElement as TSF_EA.ElementWrapper);
                    subsetOperation.addTaggedValue(this.owner.settings.sourceOperationTagName, operation);
                }
                else
                {
                    //update properties
                    var updated = false;
                    if (subsetOperation.name != operation.name)
                    {
                        subsetOperation.name = operation.name;
                        updated = true;
                    }
                    if (subsetOperation.notes != operation.notes) ;
                    {
                        subsetOperation.notes = operation.notes;
                        updated = true;
                    }
                    if (subsetOperation.visibility != operation.visibility)
                    {
                        subsetOperation.visibility = operation.visibility;
                        updated = true;
                    }
                    if (!subsetOperation.stereotypeNames.SequenceEqual(operation.stereotypeNames))
                    {
                        subsetOperation.stereotypes = operation.stereotypes;
                        updated = true;
                    }
                    if (updated)
                    {
                        subsetOperation.save();
                    }
                    //check if signature of the operation is still the same
                    if (subsetOperation.signature != operation.signature)
                    {
                        //first remove all parameters
                        subsetOperation.ownedParameters = new HashSet<Parameter>();
                        //then copy all parameters
                        foreach (var parameter in operation.ownedParameters.OfType<TSF_EA.ParameterWrapper>())
                        {

                            var newParam = subsetOperation.addOwnedParameter(parameter.name);
                            newParam.direction = parameter.direction;
                            newParam.type = parameter.type;
                            newParam.notes = parameter.notes;
                            newParam.save();
                        }
                        //set the return type
                        subsetOperation.type = operation.type;
                        subsetOperation.save();
                    }
                }
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
                                                                                                          && !x.stereotypes.Any()))
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
                foreach (var subsetGeneralization in subsetGeneralizations)
                {
                    var schemaParent = ((EASchema)this.owner).getSchemaElementForSubsetElement(subsetGeneralization.target as Classifier, null);
                    if (schemaParent == null) //the generalization does not target an element in the schema
                    {
                        schemaParent = ((EASchema)this.owner).getSchemaElementForUMLElement(subsetGeneralization.target);
                        if (schemaParent != null)
                        {
                            //if the settings redirectGeneralizationsToSubset is set and
                            //the generalization exists on the source, but there is a subset element for the target
                            //we move the generalization to the element in the subset.
                            if (schemaParent.subsetElement != null
                                && this.owner.settings.redirectGeneralizationsToSubset
                                && !subsetGeneralizations.Any(x => x.target.Equals(schemaParent.subsetElement))
                                && schemaParent.subsetElement != null
                                // the inheritance checkbox should be checked, or the setting "Ignore inheritance checkbox" should be used to allow the generalization to exist.
                                && (this.parentSchemaElement == schemaParent || this.owner.settings.copyAllGeneralizationsForElement(this)))
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
                                if (!(this.sourceElement is UML.Classes.Kernel.Enumeration)
                                    && !(this.sourceElement is UML.Classes.Kernel.DataType && this.owner.settings.copyDataTypeGeneralizations)
                                    && !(this.sourceElement is UML.Classes.Kernel.Class && this.owner.settings.copyAllGeneralizationsForElement(this)))
                                {
                                    //The generalization should not be there
                                    subsetGeneralization.delete();
                                }

                            }
                        }
                        else
                        {
                            //Enumerations should always keep their external Generalizations
                            //Datatypes should only keep their external Generalizatons if the settings is on
                            //classes can keep their external generalizations if the setting allows it
                            if (!(this.sourceElement is UML.Classes.Kernel.Enumeration)
                                && !(this.sourceElement is UML.Classes.Kernel.DataType && this.owner.settings.copyDataTypeGeneralizations)
                                && !(this.sourceElement is UML.Classes.Kernel.Class && this.owner.settings.copyAllGeneralizationsForElement(this)))
                            {
                                //The generalization should not be there
                                subsetGeneralization.delete();
                            }
                        }
                    }
                    else //the generalization targets an element in the schema. 
                    {
                        // Make sure the setting to redirect to the subset is on and
                        // and the source element has an equivalent generalization.
                        if (!this.owner.settings.redirectGeneralizationsToSubset
                            || !sourceGeneralizations.Any(x => x.target.Equals(schemaParent.sourceElement))
                            || (this.parentSchemaElement != schemaParent && !this.owner.settings.copyAllGeneralizationsForElement(this)))
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
                        && schemaParent.subsetElement != null
                        && (this.parentSchemaElement == schemaParent || this.owner.settings.copyAllGeneralizationsForElement(this)))
                    {
                        if (schemaParent.subsetElement != null
                        && !subsetGeneralizations.Any(x => x.target.Equals(schemaParent.subsetElement)))
                        {
                            //generalization doesn't exist yet. Add it
                            var newGeneralization = this.model.factory.createNewElement<TSF_EA.Generalization>(this.subsetElement, string.Empty);
                            newGeneralization.addRelatedElement(schemaParent.subsetElement);
                            newGeneralization.save();
                        }
                    }
                    else
                    {
                        //the target is not in the schema.
                        //Enumerations should always get their external Generalizations
                        //Datatypes should only get their external Generalizatons if the settings is on
                        if (schemaParent == null
                            &&(this.sourceElement is UML.Classes.Kernel.Enumeration
                            || (this.sourceElement is UML.Classes.Kernel.DataType && this.owner.settings.copyDataTypeGeneralizations)
                            || this.sourceElement is UML.Classes.Kernel.Class && this.owner.settings.copyAllGeneralizationsForElement(this))
                             && !subsetGeneralizations.Any(x => x.target != null && x.target.Equals(sourceGeneralization.target))
                            )
                        {
                            //generalization doesn't exist yet. Add it
                            var newGeneralization = this.model.factory.createNewElement<TSF_EA.Generalization>(this.subsetElement, string.Empty);
                            newGeneralization.addRelatedElement(sourceGeneralization.target);
                            newGeneralization.save();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// checks all attributes of the subset element and tries to match it with a SchemaProperty.
        /// If it can't be matched to a subset attribute it is deleted.
        /// </summary>
        public void matchSubsetAttributes()
        {
            if (!this.isShared && this.subsetElement != null)
            {
                //tell the user what we are doing 
                EAOutputLogger.log(this.model, this.owner.settings.outputName, "Matching attributes of subset element: '" + this.subsetElement.name + "' to the schema"
                                   , ((TSF_EA.ElementWrapper)this.subsetElement).id, LogTypeEnum.log);
                foreach (TSF_EA.Attribute attribute in this.subsetElement.attributes.ToList())
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
                            //check if the subset element is used in a schema or subset downstream
                            if (EASchema.isItemUsedInASchema(attribute, this.model))
                            {
                                //report error
                                EAOutputLogger.log(this.model, this.owner.settings.outputName, $"Subset attribute '{subsetElement.name}.{attribute.name}' cannot be deleted as it is still used in one or more schemas"
                                       , attribute.id, LogTypeEnum.warning);
                            }
                            else
                            {
                                //
                                this.subsetElement.attributes.Remove(attribute);
                                //no match, delete the attribute
                                attribute.delete();
                            }
                        }
                    }
                }
            }
        }

        public void matchSubsetLiterals()
        {
            if (!this.isShared && this.subsetElement != null)
            {
                //tell the user what we are doing 
                EAOutputLogger.log(this.model, this.owner.settings.outputName, "Matching literals of subset element: '" + this.subsetElement.name + "' to the schema"
                                   , ((TSF_EA.ElementWrapper)this.subsetElement).id, LogTypeEnum.log);
                var subsetElementWrapper = this.subsetElement as TSF_EA.ElementWrapper;
                if (subsetElementWrapper != null)
                {
                    foreach (TSF_EA.EnumerationLiteral literal in subsetElementWrapper.ownedLiterals)
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
                                //check if the subset element is used in a schema or subset downstream
                                if (EASchema.isItemUsedInASchema(literal, this.model))
                                {
                                    //report error
                                    EAOutputLogger.log(this.model, this.owner.settings.outputName, $"Subset literal '{subsetElement.name}.{literal.name}' cannot be deleted as it is still used in one or more schemas"
                                           , literal.id, LogTypeEnum.warning);
                                }
                                else
                                {
                                    //no match, delete the literal
                                    literal.delete();
                                }
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
                result = this.schemaLiterals.OfType<EASchemaLiteral>().FirstOrDefault(x => x.sourceLiteral?.uniqueID == tagReference);
            }
            return result;
        }
        /// <summary>
        /// get the subset element that represents this element in the destination package
        /// </summary>
        /// <param name="destinationPackage"></param>
        public void matchSubsetElement(Package destinationPackage)
        {
            string sqlGetClassifiersBase;
            if (this.ownerSchema.settings.tvInsteadOfTrace)
            {
                //get the classifier in the subset that represents this element
                sqlGetClassifiersBase = $@"select top(1) o.Object_ID from t_object o 
                                     inner join t_objectproperties p on p.Object_ID = o.Object_ID
                                     where p.Property = '{this.ownerSchema.settings.elementTagName}'
                                     and p.Value = '{this.sourceElement?.uniqueID}'
                                     and o.Package_ID in ({((TSF_EA.Package)destinationPackage).packageTreeIDString})";
            }
            else
            {
                //get the classifier in the subset that represents this element
                sqlGetClassifiersBase = $@"select top(1) o.Object_ID from ((t_object o
                                         inner join t_connector c on(c.Start_Object_ID = o.Object_ID
                                                       and c.Connector_Type = 'Abstraction'
                                                      and c.Stereotype = 'trace'))
                                         inner join t_object ot on ot.Object_ID = c.End_Object_ID)
                                         where ot.ea_guid = '{this.sourceElement?.uniqueID}'
                                         and o.Package_ID in ({((TSF_EA.Package)destinationPackage).packageTreeIDString})";
            }
            string sqlGetClassifiers;
            //if the schemaElement is redefined, we need to find the subset element with the correct name (or alias)
            if (this.isRedefined && this.owner.settings.useAliasForRedefinedElements)
            {
                sqlGetClassifiers = $@"{sqlGetClassifiersBase}{Environment.NewLine} and o.Alias = '{this.name}'";
            }
            else
            {
                sqlGetClassifiers = $@"{sqlGetClassifiersBase}{Environment.NewLine} and o.Name = '{this.name}'";
            }
            var subsetCandidates = this.model.getElementWrappersByQuery(sqlGetClassifiers);
            //if we haven't found any candidates, and this schema element has not been redefined in this schema, then we look without checking the name or alias
            if (!subsetCandidates.Any()
                && !this.isRedefined
                && !this.ownerSchema.elements.OfType<EASchemaElement>().Any(x => x.sourceElement != null
                                                                       && x.sourceElement.uniqueID == this.sourceElement?.uniqueID
                                                                       && x.isRedefined))
            {
                subsetCandidates = this.model.getElementWrappersByQuery(sqlGetClassifiersBase);
            }
            //match with this classifier
            this.matchSubsetElement(subsetCandidates.OfType<Classifier>().FirstOrDefault());
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
            if (!this.isShared && this.subsetElement != null)
            {
                //tell the user what we are doing 
                EAOutputLogger.log(this.model, this.owner.settings.outputName, "Matching relations of subset element: '" + this.subsetElement.name + "' to the schema"
                                   , ((TSF_EA.ElementWrapper)this.subsetElement).id, LogTypeEnum.log);
                //get outgoing associations of the subset element
                foreach (TSF_EA.Association association in this.subsetElement.getRelationships<Association>(true, false))
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
                            //check if the subset element is used in a schema or subset downstream
                            if (EASchema.isItemUsedInASchema(association, this.model))
                            {
                                //report error
                                EAOutputLogger.log(this.model, this.owner.settings.outputName, $"Subset association '{subsetElement.name}.{association.name}.{association.targetName}' cannot be deleted as it is still used in one or more schemas"
                                       , ((TSF_EA.ElementWrapper)subsetElement).id, LogTypeEnum.warning);
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

                foreach (var schemaAssociation in this.schemaAssociations
                                    .OfType<EASchemaAssociation>()
                                    .Where(x => tagReference.Equals(x.sourceAssociation?.uniqueID, StringComparison.InvariantCultureIgnoreCase)))
                {
                    //we have the same attribute if the given attribute has a tagged value 
                    //called sourceAssociation that refences the source association of the schema Association
                    //if the schema association has choiceElements then the target of the association should match one of the choice elements
                    //if it has a redefinedElement, then the target of the association should be the subset element of the redefinedElement
                    if (schemaAssociation.choiceElements?.Exists(x => association.target?.Equals(x.subsetElement) == true) == true
                       || association.target?.Equals(schemaAssociation.redefinedElement?.subsetElement) == true
                        || schemaAssociation.choiceElements?.Any() != true && schemaAssociation.redefinedElement == null
                           && association.target?.Equals(schemaAssociation.otherElement?.subsetElement) == true)
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
