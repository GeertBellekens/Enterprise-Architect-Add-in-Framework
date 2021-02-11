using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using SBF = SchemaBuilderFramework;
using UML = TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.SchemaBuilder
{
    /// <summary>
    /// Description of EASchemaProperty.
    /// </summary>
    public class EASchemaProperty : EASchemaPropertyWrapper, SBF.SchemaProperty
    {
        private UTF_EA.Attribute _sourceProperty;
        
        /// <summary>
        /// constructor. Nothing specific, just calling base constructor
        /// </summary>
        /// <param name="model">the model</param>
        /// <param name="owner">the owner Schema Element</param>
        /// <param name="objectToWrap">the EA.SchemaProperty object to wrap</param>
        public EASchemaProperty(UTF_EA.Model model, EASchemaElement owner, EA.SchemaProperty objectToWrap) : base(model, owner, objectToWrap) { }



        #region implemented abstract members of EASchemaPropertyWrapper
        protected override UTF_EA.Multiplicity defaultMultiplicity => new UTF_EA.Multiplicity("1..1");

        #region implemented abstract members of EASchemaPropertyWrapper


        protected override UTF_EA.Multiplicity sourceMultiplicity => (this.sourceProperty != null ? this.sourceProperty.multiplicity : this.defaultMultiplicity) as UTF_EA.Multiplicity;


        #endregion

        #endregion
        /// <summary>
        /// The property int he model where this Schema property was derived from
        /// </summary>
        public UML.Classes.Kernel.Property sourceProperty
        {
            get
            {
                if (this._sourceProperty == null)
                {
                    this._sourceProperty = this.model.getAttributeWrapperByGUID(this.wrappedProperty.GUID) as UTF_EA.Attribute;
                }
                return this._sourceProperty;
            }
            set => throw new NotImplementedException();
        }
        /// <summary>
        /// the property in the subset model generated from this property
        /// </summary>
        public UML.Classes.Kernel.Property subSetProperty { get; set; }

        internal override UTF_EA.AttributeWrapper sourceAttributeWrapper => this.sourceProperty as UTF_EA.AttributeWrapper;
        internal override UTF_EA.AttributeWrapper subsetAttributeWrapper => this.subSetProperty as UTF_EA.AttributeWrapper;
        /// <summary>
        /// Checks if the attribute type is present as the source element of one of the schema elements
        /// If it finds a match the type is set to the subset elemnt of this schema element
        /// </summary>
        public void createSubsetProperty()
        {
            //no need to do anything if the subset element does not exist
            if (this.owner.subsetElement != null && this.sourceProperty != null)
            {
                HashSet<SBF.SchemaElement> schemaElements = this.owner.owner.elements;
                if (this.subSetProperty == null)
                {
                    this.isNew = true;
                    this.subSetProperty = this.model.factory.createNewElement<UML.Classes.Kernel.Property>(this.owner.subsetElement, this.sourceProperty.name);
                }
                else
                {
                    //report rename
                    if (this.subSetProperty.name != this.sourceProperty.name)
                    {
                        EAOutputLogger.log(this.model, this.settings.outputName
                                                                      , string.Format("Attribute '{0}' has been renamed from '{1}' since the last schema generation"
                                                                              , this.sourceProperty.owner.name + "." + this.sourceProperty.name
                                                                              , this.subSetProperty.name)
                                                                      , ((UTF_EA.ElementWrapper)this.sourceProperty.owner).id
                                                                      , LogTypeEnum.warning);
                    }
                    //report different type
                    string newTypeName = (this.redefinedElement != null) ? this.redefinedElement.name : this.sourceProperty.type.name;
                    if (this.subSetProperty.type.name != newTypeName)
                    {
                        EAOutputLogger.log(this.model, this.settings.outputName
                                                                      , string.Format("Attribute '{0}' has changed type from '{1}' to '{2}' since the last schema generation"
                                                                              , this.sourceProperty.owner.name + "." + this.sourceProperty.name
                                                                              , this.subSetProperty.type.name
                                                                              , newTypeName)
                                                                      , ((UTF_EA.ElementWrapper)this.sourceProperty.owner).id
                                                                      , LogTypeEnum.warning);
                    }
                    //report report different multiplicity
                    if (((UTF_EA.Attribute)this.subSetProperty).EAMultiplicity != this.multiplicity)
                    {
                        EAOutputLogger.log(this.model, this.settings.outputName
                                                                      , string.Format("Attribute '{0}' has changed multiplicity from '{1}' to '{2}' since the last schema generation"
                                                                              , this.sourceProperty.owner.name + "." + this.sourceProperty.name
                                                                              , this.subSetProperty.multiplicity
                                                                              , this.sourceProperty.multiplicity)
                                                                      , ((UTF_EA.ElementWrapper)this.sourceProperty.owner).id
                                                                      , LogTypeEnum.warning);
                    }
                }
                this.subSetProperty.name = this.sourceProperty.name;
                this.subSetProperty.type = this.sourceProperty.type;
                this.subSetProperty.stereotypes = this.sourceProperty.stereotypes;
                this.subSetProperty.multiplicity = this.multiplicity;
                this.subSetProperty.isID = this.sourceProperty.isID;
                this.subSetProperty.visibility = this.sourceProperty.visibility;
                this.subSetProperty.isDerived = this.sourceProperty.isDerived;
                //Set position
                if (this.settings.keepOriginalAttributeOrder)
                {
                    this.subSetProperty.position = this.sourceProperty.position;
                }
                if (this.settings.setAttributeOrderZero)
                {
                    this.subSetProperty.position = 0;
                }
                //alias (only if source alias is not empty)
                if (!string.IsNullOrEmpty(((UTF_EA.Attribute)this.sourceProperty).alias))
                {
                    ((UTF_EA.Attribute)this.subSetProperty).alias = ((UTF_EA.Attribute)this.sourceProperty).alias;
                }
                //Check if the subset alias is different from the source alias and issue warning if that is the case
                if (!string.Equals(((UTF_EA.Attribute)this.subSetProperty).alias, ((UTF_EA.Attribute)this.sourceProperty).alias))
                {
                    EAOutputLogger.log(this.model, this.owner.owner.settings.outputName
                                              , string.Format("Property '{0}' has alias '{1}' in the model and a different alias '{2}' in the subset"
                                                      , this.sourceProperty.owner.name + "." + this.sourceProperty.name
                                                      , ((UTF_EA.Attribute)this.subSetProperty).alias
                                                      , ((UTF_EA.Attribute)this.sourceProperty).alias)
                                              , ((UTF_EA.ElementWrapper)this.sourceProperty.owner).id
                                              , LogTypeEnum.warning);
                }
                //notes only update them if they are empty
                if (this.subSetProperty.ownedComments.Count == 0 || !this.subSetProperty.ownedComments.Any(x => x.body.Length > 0)
                    || this.owner.owner.settings.keepNotesInSync)
                {
                    this.subSetProperty.ownedComments = this.sourceProperty.ownedComments;
                    if (! this.owner.owner.settings.keepNotesInSync
                        && this.owner.owner.settings.prefixNotes
                        && this.owner.owner.settings.prefixNotesText.Length > 0
                        && this.subSetProperty.ownedComments.Any(x => x.body.Length > 0))
                    {
                        foreach (var comment in this.subSetProperty.ownedComments)
                        {
                            comment.body = this.owner.owner.settings.prefixNotesText + Environment.NewLine + comment.body;
                        }
                    }
                }
                //resolve the type
                foreach (EASchemaElement element in schemaElements)
                {
                    if (this.redefinedElement != null
                       && this.redefinedElement.subsetElement != null
                       && this.subSetProperty != null)
                    {
                        //replace the type by the subset element of the redefined type
                        this.subSetProperty.type = this.redefinedElement.subsetElement;
                    }
                    //check for restriction of type choice because that can contain the type that is used to redefine this attribute
                    else if (this.choiceElements != null
                        && this.choiceElements.Count > 0
                       && this.subSetProperty != null)
                    {
                        var redefinedTypeElement = this.choiceElements[0];
                        if (redefinedTypeElement.subsetElement != null)
                        {
                            this.subSetProperty.type = redefinedTypeElement.subsetElement;
                        }
                        else
                        {
                            this.subSetProperty.type = redefinedTypeElement.sourceElement;
                        }
                    }
                    else if (element.sourceElement != null &&
                        element.subsetElement != null &&
                        this.subSetProperty != null &&
                        element.sourceElement.Equals(this.subSetProperty.type)
                       && element.name == this.subSetProperty.type.name)
                    {
                        //replace the type if it matches the source element
                        this.subSetProperty.type = element.subsetElement;
                    }
                }
                //save
                ((UTF_EA.Element)this.subSetProperty).save();
                //copy constraints
                this.copyConstraints();
                //copy tagged values
                ((EASchema)this.owner.owner).copyTaggedValues((UTF_EA.Element)this.sourceProperty, (UTF_EA.Element)this.subSetProperty);
                //add tagged value with reference to source attribute
                ((UTF_EA.Element)this.subSetProperty).addTaggedValue(this.owner.owner.settings.sourceAttributeTagName, ((UTF_EA.Element)this.sourceProperty).guid);
            }
        }

       

        /// <summary>
        /// adds a dependency from the attributes owner to the type of the attributes
        /// </summary>
        public void addAttributeTypeDependency()
        {
            if (this.owner.subsetElement != null
                && this.subSetProperty != null
                && this.subSetProperty.type != null
                && !(this.subSetProperty.type is UML.Classes.Kernel.PrimitiveType))
            {
                //add a trace relation from the subset element to the source element
                //check if trace already exists?
                var dependency = this.owner.subsetElement.getRelationships<UTF_EA.Dependency>()
                            .FirstOrDefault(x => this.subSetProperty.type.Equals(x.supplier) && x.name == this.subSetProperty.name);
                if (dependency == null)
                {
                    dependency = this.model.factory.createNewElement<UTF_EA.Dependency>(this.owner.subsetElement, this.subSetProperty.name);
                    dependency.addRelatedElement(this.subSetProperty.type);
                    dependency.targetEnd.multiplicity = this.subSetProperty.multiplicity;
                    dependency.save();
                }
                else
                {
                    //check if the multiplicity is still ok
                    if (dependency.targetEnd.multiplicity != this.subSetProperty.multiplicity)
                    {
                        dependency.targetEnd.multiplicity = this.subSetProperty.multiplicity;
                        dependency.save();
                    }
                }
            }
        }
    }
}
