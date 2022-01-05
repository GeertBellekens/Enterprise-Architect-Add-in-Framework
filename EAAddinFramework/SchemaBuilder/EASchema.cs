
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using SBF = SchemaBuilderFramework;
using UML = TSF.UmlToolingFramework.UML;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using EAAddinFramework.Utilities;
using System.Linq;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Windows.Forms;
using System.Diagnostics;

namespace EAAddinFramework.SchemaBuilder
{
    /// <summary>
    /// The EA Specific implementation of the Schema, wrapping the EA.SchemaComposer object
    /// </summary>
    public class EASchema : SBF.Schema
    {
        private TSF_EA.Model model;
        private EA.SchemaComposer wrappedComposer;
        private HashSet<SBF.SchemaElement> schemaElements = null;
        private List<Tuple<TSF_EA.TaggedValue, TSF_EA.Element>> taggedValuesToSynchronize = new List<Tuple<TSF_EA.TaggedValue, TSF_EA.Element>>();

        public SBF.SchemaSettings settings { get; set; }

        /// <summary>
        /// Constructor of the EASchema. Only to be used by the EASchemaBuilderFactory
        /// </summary>
        /// <param name="model">The model containing this Scheam</param>
        /// <param name="composer">The EA.SchemaComposer object to be wrapped</param>
        internal EASchema(TSF_EA.Model model, EA.SchemaComposer composer, SBF.SchemaSettings settings)
        {
            this.model = model;
            this.wrappedComposer = composer;
            this.settings = settings;
        }


        /// <summary>
        /// the name of the schema
        /// </summary>
        public string name
        {
            get
            {
                return model.EAVersion >= 1308 ?
                    getSchemaName() :
                    "Unknown Schema Name";
            }
        }
        /// <summary>
        /// can only be used as of version 13.0.1308
        /// this has to be in a separate operation that is only be called from the correct version.
        /// If not then you get a MissingMethodException, even if the actual property is not used.
        /// </summary>
        /// <returns></returns>
        private string getSchemaName()
        {
            return this.wrappedComposer.SchemaName;//only available from version 13.0.1308
        }

        /// <summary>
        /// the SchemaElements owned by this Schema
        /// </summary>
        public HashSet<SBF.SchemaElement> elements
        {
            get
            {
                if (schemaElements == null)
                {
                    schemaElements = new HashSet<SBF.SchemaElement>();
                    foreach (EA.SchemaType schemaType in getSchemaTypes())
                    {
                        schemaElements.Add(EASchemaBuilderFactory.getInstance(this.model).createSchemaElement(this, schemaType));
                    }
                }
                return schemaElements;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        private TSF_EA.ElementWrapper _containerElement;
        public PackageableElement containerElement
        {
            get
            {
                if (this._containerElement is null)
                {
                    string sqlGetSchemaArtifact = "select o.Object_ID from t_object o "
                                                  + " inner join t_document d on d.ElementID = o.ea_guid "
                                                  + $"where d.StrContent like '%<description%name=\"{this.name}\"%</description>%'";
                    var schemaArtifacts = this.model.getElementWrappersByQuery(sqlGetSchemaArtifact);
                    //only safe if only one element found.
                    if (schemaArtifacts.Count == 1) this._containerElement = schemaArtifacts.First();
                    else
                    {
                        //in case of EA version pre v13 the schema name is not available, so this method to get the artifact will not work.
                        //as a workaround we let the user select the artifact
                        //this also applies if two (or more) schema's with the same name exist in the model
                        this._containerElement = this.model.getUserSelectedElement(new List<string> { "Artifact" }, null, this.model.selectedTreePackage?.uniqueID) as TSF_EA.ElementWrapper;
                    }
                }
                return _containerElement as PackageableElement;
            }
            set
            {
                this._containerElement = value as TSF_EA.ElementWrapper;
            }
        }

        /// <summary>
        /// gets the EA.SchemaTypes from the enumerator
        /// </summary>
        /// <returns>all the EA.Schematypes in this schema</returns>
        private HashSet<EA.SchemaType> getSchemaTypes()
        {
            HashSet<EA.SchemaType> schemaTypes = new HashSet<EA.SchemaType>();
            EA.SchemaTypeEnum schemaTypeEnumerator = wrappedComposer.SchemaTypes;
            EA.SchemaType schemaType = schemaTypeEnumerator.GetFirst();
            while (schemaType != null)
            {
                schemaTypes.Add(schemaType);
                schemaType = schemaTypeEnumerator.GetNext();
            }
            return schemaTypes;
        }
        /// <summary>
        /// returns the SchemaElement that corresponds with the given UML element
        /// </summary>
        /// <param name="umlElement">the source UMLElement</param>
        /// <returns></returns>
        internal EASchemaElement getSchemaElementForUMLElement(UML.Classes.Kernel.Element umlElement)
        {
            EASchemaElement result = null;
            foreach (EASchemaElement schemaElement in this.elements)
            {
                if (schemaElement.sourceElement != null
                    && schemaElement.sourceElement.Equals(umlElement))
                {
                    if (result == null)
                    {
                        result = schemaElement;
                    }
                    else if (schemaElement.name == umlElement.name)
                    {
                        result = schemaElement;
                        break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// finds the Schema Element for which the given element could be the subset element
        /// </summary>
        /// <param name="subsetElement">the element to search a match for</param>
        /// <returns>the corresponding SchemaElement</returns>
        internal EASchemaElement getSchemaElementForSubsetElement(Classifier subsetElement)
        {
            EASchemaElement result = null;
            if (subsetElement != null)
            {
                //first find the source element from which this subset element is derived
                //check on if there is a tagged value that references the source element
                Classifier sourceElement = null;
                if (this.settings.tvInsteadOfTrace)
                {
                    sourceElement = subsetElement.taggedValues.FirstOrDefault(x => x.name == this.settings.elementTagName)?.tagValue as Classifier;
                }
                else
                {
                    //check if the subset element has a dependency to the source element of the schema
                    string sqlCheckTrace = @"select c.End_Object_ID from t_connector c
													where 
													c.Connector_Type = 'Abstraction'
													and c.Stereotype = 'trace'
													and c.Start_Object_ID = " + ((TSF_EA.ElementWrapper)subsetElement).id;
                    var checkTraceXML = this.model.SQLQuery(sqlCheckTrace);
                    var endObjectIDNodes = checkTraceXML.SelectNodes(this.model.formatXPath("//End_Object_ID"));
                    foreach (System.Xml.XmlNode connectorIDNode in endObjectIDNodes)
                    {
                        int elementID;
                        if (int.TryParse(connectorIDNode.InnerText, out elementID))
                        {
                            //check if there is an schemaElement with this 
                            sourceElement = this.elements
                                .FirstOrDefault(x => ((TSF_EA.ElementWrapper)x.sourceElement)?.id == elementID)
                                ?.sourceElement;
                            break;
                        }
                    }
                }
                //if we haven't found a source element, then this subset element can't have a schema element
                if (sourceElement == null) return null;
                //first check if the subset element can be matched to a redefined schemaElement
                if (this.settings.useAliasForRedefinedElements)
                {
                    //find redefined element with same source element, and where the name corresponds to the alias of the sourceElement
                    result = this.elements.OfType<EASchemaElement>().FirstOrDefault(x => x.isRedefined
                                                                          && x.name == ((TSF_EA.ElementWrapper)subsetElement).alias
                                                                          && x.sourceElement.uniqueID == sourceElement.uniqueID
                                                                          && (x.subsetElement == null 
                                                                              || x.subsetElement.uniqueID == subsetElement.uniqueID ));
                    //if not found with redefined elemnet then find as regular element
                    if (result == null)
                    {
                        result = this.elements.OfType<EASchemaElement>().FirstOrDefault(x => !x.isRedefined
                                                                          && x.name == subsetElement.name
                                                                          && x.sourceElement.uniqueID == sourceElement.uniqueID
                                                                          && (x.subsetElement == null
                                                                              || x.subsetElement.uniqueID == subsetElement.uniqueID));
                    }
                }
                else
                {
                    //find based on name, regardless of redefined or not
                    result = this.elements.OfType<EASchemaElement>().FirstOrDefault(x => x.name == subsetElement.name
                                                                          && x.sourceElement.uniqueID == sourceElement.uniqueID
                                                                          && (x.subsetElement == null
                                                                              || x.subsetElement.uniqueID == subsetElement.uniqueID));
                }
            }
            return result;
        }
        internal EASchemaElement getSchemaElementForSchemaType(EA.SchemaType schemaType)
        {
            foreach (var schemaElement in this.elements.OfType<EASchemaElement>())
            {
                if (schemaElement.wrappedSchemaType.GUID == schemaType?.GUID)
                {
                    return schemaElement;
                }
            }
            return null; //not found
        }
        /// <summary>
        /// creates a subset of the source model with only the properties and associations used in this schema
        /// </summary>
        /// <param name="destinationPackage">the package to create the subset in</param>
        /// <param name="copyDatatype"></param>
        public void createSubsetModel(UML.Classes.Kernel.Package destinationPackage, HashSet<SBF.SchemaElement> elements)
        {
            //loop the elements to create the subSetElements
            foreach (EASchemaElement schemaElement in elements)
            {
                //tell the user what we are doing 
                EAOutputLogger.log(this.model, this.settings.outputName, "Creating subset element for : '" + schemaElement.name + "'"
                                   , 0, LogTypeEnum.log);
                //do not copy elements that are shared
                if (!schemaElement.isShared)
                {
                    //only create subset elements for classes, not for datatypes
                    if (schemaElement.sourceElement is UML.Classes.Kernel.Class
                       || schemaElement.sourceElement is UML.Classes.Kernel.Enumeration)
                    {
                        schemaElement.createSubsetElement(destinationPackage);
                        //Logger.log("after EASchema::creating single subset element");
                    }
                    else if (schemaElement.sourceElement is UML.Classes.Kernel.DataType && this.settings.copyDataTypes)
                    {
                        //if the datatypes are limited then only the ones in the list should be copied
                        if (!this.settings.limitDataTypes
                            || this.settings.dataTypesToCopy.Contains(schemaElement.sourceElement.name))
                            schemaElement.createSubsetElement(destinationPackage);
                    }
                }
            }
            //Logger.log("after EASchema::creating subsetelements");
            // then loop them again to create the associations
            foreach (EASchemaElement schemaElement in elements)
            {
                if (!schemaElement.isShared)
                {
                    //only create subset elements for classes and enumerations and datatypes
                    if (schemaElement.sourceElement is UML.Classes.Kernel.Class
                       || schemaElement.sourceElement is UML.Classes.Kernel.Enumeration
                        || schemaElement.sourceElement is UML.Classes.Kernel.DataType)
                    {
                        //tell the user what we are doing 
                        EAOutputLogger.log(this.model, this.settings.outputName, "Creating subset associations for: '" + schemaElement.name + "'"
                                           , 0, LogTypeEnum.log);
                        schemaElement.createSubsetAssociations();
                        //Logger.log("after EASchema::creating single subset association");
                        //tell the user what we are doing 
                        EAOutputLogger.log(this.model, this.settings.outputName, "Creating subset attributes for: '" + schemaElement.name + "'"
                                           , 0, LogTypeEnum.log);
                        // and to resolve the attributes types to subset types if required
                        schemaElement.createSubsetAttributes();
                        //tell the user what we are doing 
                        EAOutputLogger.log(this.model, this.settings.outputName, "Creating subset literals for: '" + schemaElement.name + "'"
                                           , 0, LogTypeEnum.log);
                        //Logger.log("after EASchema::createSubsetAttributes ");
                        schemaElement.createSubsetLiterals();
                        //clean up attribute type dependencies we don't need anymore
                        schemaElement.cleanupAttributeDependencies();
                        if (!this.settings.dontCreateAttributeDependencies)
                        {
                            //tell the user what we are doing 
                            EAOutputLogger.log(this.model, this.settings.outputName, "Creating attribute depencendies for: '" + schemaElement.name + "'"
                                               , 0, LogTypeEnum.log);
                            //and add a dependency from the schemaElement to the type of the attributes
                            schemaElement.addAttributeTypeDependencies();
                        }
                        //order the attributes
                        if (!this.settings.keepOriginalAttributeOrder
                            && !this.settings.setAttributeOrderZero
                            && !(this.settings.orderAssociationsAlphabetically && this.settings.orderAssociationsAmongstAttributes))
                        {
                            schemaElement.orderAttributes();
                        }
                        //order the associations
                        if (this.settings.orderAssociationsAlphabetically)
                        {
                            //tell the user what we are doing 
                            EAOutputLogger.log(this.model, this.settings.outputName, "Ordering associations for: '" + schemaElement.name + "'"
                                               , 0, LogTypeEnum.log);
                            //and add a dependency from the schemaElement to the type of the attributes
                            schemaElement.orderAssociationsAlphabetically();
                        }
                        //Logger.log("after EASchema::addAttributeTypeDependencies");
                        //tell the user what we are doing 
                        EAOutputLogger.log(this.model, this.settings.outputName, "Creating generalizations for: '" + schemaElement.name + "'"
                                           , 0, LogTypeEnum.log);
                        //add generalizations if both elements are in the subset
                        schemaElement.addGeneralizations();
                    }
                }
            }
            //then loop them againe to remove those subset elements that don't have any attributes or associations 
            // or generalizations, or are used as type
            if (this.settings.deleteUnusedSchemaElements)
            {
                foreach (EASchemaElement schemaElement in elements)
                {
                    if (schemaElement.subsetElement != null)
                    {
                        //remove those subset elements that don't have any attributes or associations
                        //reload the element because otherwise the API does not return any attributes or associations
                        var reloadedElement = model.getElementByGUID(schemaElement.subsetElement.uniqueID) as Classifier;
                        if (reloadedElement != null
                             && !reloadedElement.attributes.Any()
                           && !reloadedElement.getRelationships<UML.Classes.Kernel.Association>().Any()
                           && !reloadedElement.getRelationships<UML.Classes.Kernel.Generalization>().Any()
                           && !reloadedElement.getDependentTypedElements<UML.Classes.Kernel.TypedElement>().Any()
                           //&& (reloadedElement is TSF_EA.ElementWrapper && !((TSF_EA.ElementWrapper)reloadedElement).primitiveParentNames.Any())
                           )
                        {
                            //check if the subset element is used in a schema or subset downstream
                            if (isItemUsedInASchema(reloadedElement, this.model))
                            {
                                //report error
                                EAOutputLogger.log(this.model, this.settings.outputName, $"Subset element '{reloadedElement.name}' cannot be deleted as it is still used in one or more schemas"
                                       , ((TSF_EA.ElementWrapper)reloadedElement).id, LogTypeEnum.warning);
                            }
                            else
                            {
                                //tell the user what we are doing 
                                EAOutputLogger.log(this.model, this.settings.outputName, "Deleting subset element for: '" + schemaElement.name + "'"
                                               , 0, LogTypeEnum.log);
                                schemaElement.subsetElement.delete();
                            }
                        }
                    }
                }
            }
            //then synchronize the tagged values where needed
            synchronizeTaggedValues();
            //save the new schema contents to the destination package
            this.saveSchemaContent(destinationPackage);

        }
        /// <summary>
        /// creates subset tagged values for each of the tagged values that need to be synchronized.
        /// TODO: handle case where a single source element has multiple subset elements (e.g in case of inherited attributes)
        /// </summary>
        private void synchronizeTaggedValues()
        {
            foreach (var tuple in this.taggedValuesToSynchronize)
            {
                var sourceItem = tuple.Item1.tagValue as UML.Classes.Kernel.Element;
                if (sourceItem != null)
                {
                    var targetItem = this.getTargetItem(sourceItem);
                    if (targetItem != null)
                    {
                        //create the tagged value
                        tuple.Item2.addTaggedValue(tuple.Item1.name, targetItem.uniqueID, null, true);
                    }
                }
                else
                {
                    //check if the tagvalue contains a list of guid's
                    var tagValueString = tuple.Item1.eaStringValue;
                    //handle <memo> cases => guidList in the comment
                    if (tagValueString.Equals("<memo>", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var targetTagComment = getTargetGUIDs(tuple.Item1.comment);
                        if (! string.IsNullOrEmpty(targetTagComment))
                        {
                            tuple.Item2.addTaggedValue(tuple.Item1.name, tagValueString,targetTagComment, false);
                        }
                        
                    }
                    //guidList in the tag value
                    var targetTagString = getTargetGUIDs(tagValueString);
                    {
                        if (!string.IsNullOrEmpty(targetTagString))
                        {
                            tuple.Item2.addTaggedValue(tuple.Item1.name, targetTagString, null, false);
                        }
                    }
                }
            }
            //delete any duplicate empty tagged values. It could happen that there are 3 separate BusinessKeyAttribute tags
            //TODO
            //clear the taggedValues to be synchronized
            this.taggedValuesToSynchronize.Clear();
        }
        private string getTargetGUIDs(string guidsStringValue)
        {
            var sourceGUIDs = this.getGUIDsFromString(guidsStringValue);
            var targetGUIDs = new List<string>();
            foreach (var sourceGUID in sourceGUIDs)
            {
                var sourceItem = this.model.getElementByGUID(sourceGUID);
                if (sourceItem != null)
                {
                    var targetItem = this.getTargetItem(sourceItem);
                    if (targetItem != null)
                    {
                        targetGUIDs.Add(targetItem.uniqueID);
                    }
                }
            }
            return string.Join(",",  targetGUIDs);
        }
        private List<string> getGUIDsFromString(string guidListString)
        {          
            return guidListString.Split(',') // split by comma ","
                .Select(x => x.Trim()) //remove spaces
                .Where(x => this.isGUID(x)) //check for GUID values only
                .ToList(); //return list
        }
        private bool isGUID(string guidString)
        {
            Guid dummy;
            return Guid.TryParse(guidString, out dummy);
        }
        private UML.Classes.Kernel.Element getTargetItem(UML.Classes.Kernel.Element sourceItem)
        {
            UML.Classes.Kernel.Element targetItem = null;
            if (sourceItem is TSF_EA.ElementWrapper)
            {
                targetItem = this.schemaElements.FirstOrDefault(x => x.sourceElement.Equals(sourceItem))?.subsetElement;
            }
            else if (sourceItem is TSF_EA.AttributeWrapper)
            {
                targetItem = getSubsetAttributeWrapper((TSF_EA.AttributeWrapper)sourceItem);
            }
            else if (sourceItem is TSF_EA.ConnectorWrapper)
            {
                //get corresponding schema element
                targetItem = getSubsetConnector((TSF_EA.ConnectorWrapper)sourceItem);
            }
            return targetItem;
        }
        private UML.Classes.Kernel.Relationship getSubsetConnector(TSF_EA.ConnectorWrapper sourceConnector)
        {
            foreach (var schemaElement in this.schemaElements)
            {
                foreach (var schemaAssociation in schemaElement.schemaAssociations)
                {
                    if (schemaAssociation.sourceAssociation.Equals(sourceConnector)
                        && schemaAssociation.subsetAssociations.Any())
                    {
                        return schemaAssociation.subsetAssociations.First();
                    }
                }
            }
            //not found, return null
            return null;
        }
        private TSF_EA.AttributeWrapper getSubsetAttributeWrapper(TSF_EA.AttributeWrapper sourceAttribute)
        {
            foreach (var schemaElement in this.schemaElements)
            {
                foreach (var schemaAttribute in schemaElement.schemaProperties)
                {
                    if (schemaAttribute.sourceProperty.Equals(sourceAttribute)
                        && schemaAttribute.subSetProperty != null)
                    {
                        return (TSF_EA.AttributeWrapper)schemaAttribute.subSetProperty;
                    }
                }
                foreach (var schemaLiteral in schemaElement.schemaLiterals)
                {
                    if (schemaLiteral.sourceLiteral.Equals(sourceAttribute)
                        && schemaLiteral.subSetLiteral != null)
                    {
                        return (TSF_EA.AttributeWrapper)schemaLiteral.subSetLiteral;
                    }
                }
            }
            //nothing found, return null
            return null;
        }


        /// <summary>
        /// updates the subset model linked to given messageElement
        /// </summary>
        /// <param name="messageElement">The message element that is the root for the message subset model</param>
        public void updateSubsetModel(Classifier messageElement)
        {
            //TODO: compare with stored schema (if any)?
            //match the subset existing subset elements
            matchSubsetElements(messageElement);
            matchAndUpdateSubsetModel(messageElement.owningPackage);
        }
        /// <summary>
        /// updates the subset model linked to given messageElement
        /// </summary>
        /// <param name="destinationPackage">the package to create the subset in</param>
        public void updateSubsetModel(Package destinationPackage)
        {
            List<EASchemaElement> elementsToCreate;
            List<EASchemaElement> elementsToUpdate;
            HashSet<Classifier> subsetElementsToDelete;
            bool generateChangesOnly = false;
            if (this.compareSchemas(destinationPackage, out elementsToCreate, out elementsToUpdate, out subsetElementsToDelete))
            {
                if (elementsToCreate.Any() || elementsToUpdate.Any() || subsetElementsToDelete.Any())
                {
                    //Inform user and request feedback
                    var response = MessageBox.Show(this.model.mainEAWindow,
                                                    $"Elements to update: {elementsToUpdate.Count}\n" +
                                                    $"Elements to create: {elementsToCreate.Count}\n" +
                                                    $"Elements to delete: {subsetElementsToDelete.Count}\n" +
                                                    "YES to generate only changes\n" +
                                                    "NO to generate the whole schema"
                                                    , "Generate only changes?"
                                                    , MessageBoxButtons.YesNoCancel
                                                    , MessageBoxIcon.Question);
                    switch (response)
                    {
                        case DialogResult.Yes:
                            generateChangesOnly = true;
                            break;
                        case DialogResult.No:
                            generateChangesOnly = false;
                            break;
                        default:
                            return;//if the user presses cancel then we stop immediately
                    }

                }
                else
                {
                    var response = MessageBox.Show(this.model.mainEAWindow
                                                   , "Found no differences from the generated schema \n" +
                                                   "Would you like to re-generate anyway?"
                                                   , "Re-Generate?"
                                                   , MessageBoxButtons.YesNo
                                                   , MessageBoxIcon.Question);

                    if (response == DialogResult.Yes)
                        generateChangesOnly = false;
                    else return;
                }
            }
            if (generateChangesOnly)
            {
                // this will delete the subset elements that are no longer needed
                matchSubsetElements(destinationPackage, new HashSet<Classifier>(destinationPackage.getAllOwnedElements().OfType<Classifier>()));
                //then process only the elementsToUpdate and ElementsToCreate
                matchAndUpdateSubsetModel(destinationPackage, elementsToCreate, elementsToUpdate);
            }
            else
            {
                //regenerate completely
                matchSubsetElements(destinationPackage, new HashSet<Classifier>(destinationPackage.getAllOwnedElements().OfType<Classifier>()));
                matchAndUpdateSubsetModel(destinationPackage);
            }
        }
        /// <summary>
        /// compares the saved existing schema with the current schema and determines the 
        /// - ElementsToCreate: elements that exist in the current schema but not in the existing schema
        /// - ElementsToUpdate : elements that have been changed somehow and should be re-generated
        /// - subsetElementsToDelete: elements of the existing schema that are no longer in the current schema. For each of these elements we get the subset elements
        /// </summary>
        /// <param name="destinationPackage">the package where the subset should be generated to</param>
        /// <param name="elementsToCreate">elements that exist in the current schema but not in the existing schema</param>
        /// <param name="elementsToUpdate">elements that have been changed somehow and should be re-generated</param>
        /// <param name="subsetElementsToDelete">the classifiers in the subset that should be deleted</param>
        /// <returns>returns true if was able to compare the schemas. False otherwise</returns>
        private bool compareSchemas(Package destinationPackage, out List<EASchemaElement> elementsToCreate
                                , out List<EASchemaElement> elementsToUpdate, out HashSet<Classifier> subsetElementsToDelete)
        {
            elementsToCreate = new List<EASchemaElement>();
            elementsToUpdate = new List<EASchemaElement>();
            subsetElementsToDelete = new HashSet<Classifier>();
            var elementsToCreateIDs = new List<string>();
            var elementsToUpdateIDs = new List<string>();
            var subsetElementsToDeleteIDs = new List<string>();
            //get the own XmlSchemaContent
            XDocument ownSchema = null;
            XDocument existingSchema = null;
            try
            {
                //parse own schema
                var xmlSchemacontent = this.getXMLSchemaContent();
                if (!string.IsNullOrEmpty(xmlSchemacontent)) ownSchema = XDocument.Parse(getXMLSchemaContent());
                //parse existing schema
                existingSchema = this.getXmlSchemaFromPackage(destinationPackage);
            }
            catch (Exception)
            {
                //if for some reason we can't parse the own or existing schema then we process the whole schema
                return false;
            }
            if (ownSchema == null || existingSchema == null) return false; //could not compare
            //compare the schema's
            //loop all class nodes in own schema
            foreach (var node in ownSchema.XPathSelectElements("//class"))
            {
                var classGUID = node.Attribute("guid").Value;
                //find corresponding node in existingSchema
                var correspondingNode = existingSchema.XPathSelectElement($"//class[@guid='{classGUID}']");
                if (correspondingNode == null)
                {
                    //add node to elementsToCreate
                    var elementToCreate = this.elements.FirstOrDefault(x => x.sourceElement?.uniqueID == classGUID) as EASchemaElement;
                    if (elementToCreate != null) elementsToCreate.Add(elementToCreate);
                }
                else
                {
                    //compare both nodes
                    if (!XNode.DeepEquals(node, correspondingNode))
                    {
                        var elementToUpdate = this.elements.FirstOrDefault(x => x.sourceElement?.uniqueID == classGUID) as EASchemaElement;
                        if (elementToUpdate != null) elementsToUpdate.Add(elementToUpdate);
                    }
                }
            }
            //loop all classNodes in existingSchema
            foreach (var node in existingSchema.XPathSelectElements("//class"))
            {
                var classGUID = node.Attribute("guid").Value;
                //find corresponding node in ownschema
                //debug
                string xpath = $"//class[@guid='{classGUID}']";
                string ownSchemaString = ownSchema.ToString();
                var correspondingNode = ownSchema.XPathSelectElement($"//class[@guid='{classGUID}']");
                if (correspondingNode == null)
                {
                    string sqlGetClassifiers;
                    if (this.settings.tvInsteadOfTrace)
                    {
                        //get the classifier in the subset that represents this element
                        sqlGetClassifiers = "select distinct o.Object_ID from t_object o "
                                           + "  inner join t_objectproperties p on p.Object_ID = o.Object_ID"
                                           + $" where p.Property = '{this.settings.elementTagName}'"
                                           + $"  and p.Value = '{classGUID}'"
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
                                               + $"    where ot.ea_guid = '{classGUID}'"
                                               + $"  and o.Package_ID in ({((TSF_EA.Package)destinationPackage).packageTreeIDString})";
                    }
                    //get the elements
                    subsetElementsToDelete = new HashSet<Classifier>(this.model.getElementWrappersByQuery(sqlGetClassifiers).OfType<Classifier>());
                }
            }
            return true;
        }
        private XDocument getXmlSchemaFromPackage(Package destinationPackage)
        {
            var sqlGetSchemaContent = "select tv.Notes from t_objectproperties tv"
                                    + " inner join t_object o on o.Object_ID = tv.Object_ID"
                                    + " where tv.Property = 'schema'"
                                    + $" and o.ea_guid = '{destinationPackage.uniqueID}'";
            var xmlSchemaContent = this.model.SQLQuery(sqlGetSchemaContent).SelectSingleNode(this.model.formatXPath("//Notes"))?.InnerText;
            if (!string.IsNullOrEmpty(xmlSchemaContent))
                return XDocument.Parse(xmlSchemaContent);
            return null;
        }
        /// <summary>
        /// The xml schema content is being saved to the tagged value "schema"
        /// </summary>
        /// <param name="destinationPackage">the package to store the content into</param>
        private void saveSchemaContent(Package destinationPackage)
        {

            var schemaContent = getXMLSchemaContent();
            if (!string.IsNullOrEmpty(schemaContent))
            {
                ((TSF_EA.Package)destinationPackage).addTaggedValue("schema", "<memo>", schemaContent);
            }
        }

        public string getXMLSchemaContent()
        {
            if (this.containerElement != null)
            {
                var sqlGetXMLSchemaContent = "select doc.StrContent from t_document doc "
                                             + " where doc.DocType = 'SC_MessageProfile'"
                                             + $"and doc.ElementID = '{this.containerElement.uniqueID}'";
                return this.model.SQLQuery(sqlGetXMLSchemaContent).SelectSingleNode(this.model.formatXPath("//StrContent")).InnerText;
            }
            return null;
        }

        void matchAndUpdateSubsetModel(Package destinationPackage, List<EASchemaElement> elementsToCreate, List<EASchemaElement> elementsToUpdate)
        {
            //first match the elementsToUpdate with their subset model equivalent
            foreach (var schemaElement in elementsToUpdate)
            {
                schemaElement.matchSubsetElement(destinationPackage);
                //match the attributes
                schemaElement.matchSubsetAttributes();
                //Logger.log("after EASchema::matchSubsetAttributes");
                schemaElement.matchSubsetLiterals();
                //match the associations
                schemaElement.matchSubsetAssociations();
                //Logger.log("after EASchema::matchSubsetAssociations");
            }
            //then create all elements
            var joinedElementsList = new List<SBF.SchemaElement>(elementsToCreate);
            joinedElementsList.AddRange(elementsToUpdate);
            //create the subset model for this set of elements
            this.createSubsetModel(destinationPackage, new HashSet<SBF.SchemaElement>(joinedElementsList));
        }
        void matchAndUpdateSubsetModel(Package destinationPackage)
        {
            foreach (EASchemaElement schemaElement in this.elements)
            {
                //match the attributes
                schemaElement.matchSubsetAttributes();
                //Logger.log("after EASchema::matchSubsetAttributes");
                schemaElement.matchSubsetLiterals();
                //match the associations
                schemaElement.matchSubsetAssociations();
                //Logger.log("after EASchema::matchSubsetAssociations");
            }
            this.createSubsetModel(destinationPackage, this.elements);
            //Logger.log("after EASchema::createSubsetModel");
        }

        /// <summary>
        /// Finds all subset elements linked to the given message element and links those to the schema elements.
        /// If a subset element could not be matched, and it is in the same package as the given messageElement, then it is deleted
        /// </summary>
        /// <param name="messageElement">the message element to start from</param>
        void matchSubsetElements(UML.Classes.Kernel.Classifier messageElement)
        {
            HashSet<UML.Classes.Kernel.Classifier> subsetElements = this.getSubsetElementsfromMessage(messageElement);
            //match each subset element to a schema element
            matchSubsetElements(messageElement.owningPackage, subsetElements);
        }

        void matchSubsetElements(Package destinationPackage, HashSet<Classifier> subsetElements)
        {

            //loop subset elements ordered by name
            foreach (Classifier subsetElement in subsetElements.OrderBy(x => name))
            {
                //tell the user what we are doing 
                EAOutputLogger.log(this.model, this.settings.outputName, "Matching subset element: '" + subsetElement.name + "' to a schema element"
                                   , ((TSF_EA.ElementWrapper)subsetElement).id, LogTypeEnum.log);
                //get the corrsponding schema element
                EASchemaElement schemaElement = this.getSchemaElementForSubsetElement(subsetElement);

                //found a corresponding schema element
                if (schemaElement != null && shouldElementExistAsDatatype(subsetElement))
                {
                    schemaElement.matchSubsetElement(subsetElement);
                }
                else
                {
                    //if it doesn't correspond with a schema element we delete it?
                    //only if the subset element is located in the same folder as the message element
                    //and it doesn't have one of stereotypes to be ignored
                    if (destinationPackage.getNestedPackageTree(true).Any(x => x.Equals(subsetElement.owningPackage))
                        && !this.settings.ignoredStereotypes.Intersect(((TSF_EA.Element)subsetElement).stereotypeNames).Any())
                    {
                        //check if the subset element is used in a schema or subset downstream
                        if (isItemUsedInASchema(subsetElement, this.model))
                        {
                            //report error
                            EAOutputLogger.log(this.model, this.settings.outputName, $"Subset element '{subsetElement.name}' cannot be deleted as it is still used in one or more schemas"
                                   , ((TSF_EA.ElementWrapper)subsetElement).id, LogTypeEnum.warning);
                        }
                        else
                        {
                            subsetElement.delete();
                        }
                    }
                }
            }
        }
        public static bool isItemUsedInASchema(UML.Classes.Kernel.Element element, TSF_EA.Model model)
        {
            var sqlGetCountSchemas = @"select count(*) as countresult from t_document d 
                                        inner join t_object o on o.ea_guid = d.ElementID
                                        where d.DocType = 'SC_MessageProfile' "
                                        + $"and d.StrContent like '%{element.uniqueID}%'";
            var resultXml = model.SQLQuery(sqlGetCountSchemas);
            var countNode = resultXml.SelectSingleNode(model.formatXPath("//countresult"));
            return countNode.InnerText != "0";
        }

        private bool shouldElementExistAsDatatype(Classifier subsetElement)
        {
            if (subsetElement is Class || subsetElement is Enumeration)
            {
                return true;
            }
            else
            {
                var datatype = subsetElement as DataType;
                if (datatype != null && this.settings.copyDataTypes)
                {
                    if (!this.settings.limitDataTypes
                        || this.settings.dataTypesToCopy.Contains(datatype.name))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// gets all the subset elements for a given message element
        /// </summary>
        /// <param name="messageElement">the message element</param>
        /// <returns>all subset elements in the subset model for this message element</returns>
        private HashSet<UML.Classes.Kernel.Classifier> getSubsetElementsfromMessage(Classifier messageElement)
        {
            var subsetElements = new HashSet<UML.Classes.Kernel.Classifier>();
            this.addRelatedSubsetElements(messageElement, subsetElements);
            //we also add all classes in the package of the subset element
            foreach (var element in getSubsetElementsfromPackage(messageElement.owningPackage))
            {
                this.addToSubsetElements(element, subsetElements);
            }
            return subsetElements;
        }
        private HashSet<Classifier> getSubsetElementsfromPackage(Package destinationPackage)
        {
            var subsetElements = new HashSet<Classifier>();
            foreach (var element in destinationPackage.getAllOwnedElements().OfType<Classifier>())
            {
                this.addToSubsetElements(element, subsetElements);
            }
            return subsetElements;
        }
        /// <summary>
        /// adds all the related subset elements to the list recursively
        /// </summary>
        /// <param name="element">the element to start from </param>
        /// <param name="subsetElements">the HashSet of subset element to add to</param>
        private void addRelatedSubsetElements(UML.Classes.Kernel.Classifier element, HashSet<UML.Classes.Kernel.Classifier> subsetElements)
        {
            //follow the associations
            foreach (TSF_EA.Association association in element.getRelationships<UML.Classes.Kernel.Association>())
            {
                addToSubsetElements(association.target as UML.Classes.Kernel.Classifier, subsetElements);
            }
            //follow the attribute types
            foreach (TSF_EA.Attribute attribute in element.attributes)
            {
                addToSubsetElements(attribute.type as UML.Classes.Kernel.Classifier, subsetElements);
            }

        }
        /// <summary>
        /// adds the given class to the list of subset elements and then adds all related
        /// </summary>
        /// <param name="element">the Class to add</param>
        /// <param name="subsetElements">the list of subset elements</param>
        private void addToSubsetElements(UML.Classes.Kernel.Classifier element, HashSet<UML.Classes.Kernel.Classifier> subsetElements)
        {
            //add element is not already in the list
            if (element != null
                && (element is Class || element is Enumeration || (element is DataType && !(element is PrimitiveType)))
                && !subsetElements.Contains(element))
            {
                subsetElements.Add(element);
                //add related elements for this element
                this.addRelatedSubsetElements(element, subsetElements);
            }
        }
        /// <summary>
        /// copies only the tagged values necesarry
        /// </summary>
        /// <param name="source">the source element</param>
        /// <param name="target">the target element</param>
        public void copyTaggedValues(TSF_EA.Element source, TSF_EA.Element target)
        {
            //build a dictionary of tagged value pairs
            var matchedPairs = new Dictionary<TSF_EA.TaggedValue, TSF_EA.TaggedValue>();
            //get a copy of the list of target tagged values
            var targetTaggedValues = target.taggedValues.OfType<TSF_EA.TaggedValue>().ToList();
            //copy tagged values
            foreach (TSF_EA.TaggedValue sourceTaggedValue in source.taggedValues)
            {
                //if it's a tagged value to be synchronized then add it to the list and skip the rest
                if (this.settings.synchronizedTaggedValues.Contains(sourceTaggedValue.name))
                {
                    this.taggedValuesToSynchronize.Add(new Tuple<TSF_EA.TaggedValue, TSF_EA.Element>(sourceTaggedValue, target));
                    continue;
                }
                bool updateTaggedValue = true;
                var targetTaggedValue = this.popTargetTaggedValue(targetTaggedValues, sourceTaggedValue);
                if (this.settings.ignoredTaggedValues.Contains(sourceTaggedValue.name)
                    || sourceTaggedValue.name.Equals(this.settings.customPositionTag, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (targetTaggedValue != null &&
                        targetTaggedValue.eaStringValue != string.Empty)
                    {
                        //don't update any of the tagged values of the ignoredTaggeValues if the value is already filled in.
                        updateTaggedValue = false;
                    }
                }
                if (updateTaggedValue)
                {
                    if (targetTaggedValue != null)
                    {
                        //check if neededs to be updated
                        if (targetTaggedValue.eaStringValue != sourceTaggedValue.eaStringValue
                            || targetTaggedValue.comment != sourceTaggedValue.comment)
                        {
                            targetTaggedValue.eaStringValue = sourceTaggedValue.eaStringValue;
                            targetTaggedValue.comment = sourceTaggedValue.comment;
                            try
                            {
                                targetTaggedValue.save();
                            }
                            catch (Exception) //saving a tagged value sometimes fails. It happened when saving an attribute's tagged value that was problably just created by EA itself because of the attribute stereotype.
                            {
                                EAOutputLogger.log($"Error saving tagged value {targetTaggedValue.name} on item {target.name} with GUID {target.uniqueID}. Try to update the target element first, making sure it has the correct stereotype and all associated tagged values.", 0, LogTypeEnum.error);
                            }
                            
                        }
                    }
                    else
                    {
                        try
                        {
                            //create new tagged value
                            target.addTaggedValue(sourceTaggedValue.name, sourceTaggedValue.eaStringValue, sourceTaggedValue.comment, true);
                        }
                        catch (Exception) //saving a tagged value sometimes fails. It happened when saving an attribute's tagged value that was problably just created by EA itself because of the attribute stereotype.
                        {
                            EAOutputLogger.log($"Error creating new tagged value {sourceTaggedValue.name} on item {target.name} with GUID {target.uniqueID}. Try to update the target element first, making sure it has the correct stereotype and all associated tagged values.", 0, LogTypeEnum.error);
                        }
                        
                    }
                }
            }
            //clear the tagged values to be synchronized from the target element
            var taggedValuesClear = target.taggedValues.Where(x => this.settings.synchronizedTaggedValues.Contains(x.name)).ToList();
            foreach (var targetTag in taggedValuesClear)
            {
                targetTag.tagValue = string.Empty;
                targetTag.comment = string.Empty;
                targetTag.save();
            }
        }
        private TSF_EA.TaggedValue popTargetTaggedValue(List<TSF_EA.TaggedValue> targetTaggedValues, TSF_EA.TaggedValue sourceTaggedValue)
        {
            TSF_EA.TaggedValue targetTaggedValue = null;
            var nameMatches = targetTaggedValues.Where(x => x.name.Equals(sourceTaggedValue.name, StringComparison.InvariantCultureIgnoreCase));
            if (!nameMatches.Any())
            {
                return null;
            }
            //found some matches by name
            //check if only one found
            if (nameMatches.Count() == 1)
            {
                targetTaggedValue = nameMatches.First();
            }
            if (targetTaggedValue == null)
            {
                //multiple found, check if any of them as the same value
                var valueMatches = nameMatches.Where(x => x.eaStringValue == sourceTaggedValue.eaStringValue);
                if (!valueMatches.Any())
                {
                    targetTaggedValue = nameMatches.First();
                }
                else
                {
                    if (valueMatches.Count() == 1)
                    {
                        targetTaggedValue = valueMatches.First();
                    }
                    else
                    {
                        //multiple value matches, check for comments
                        var commentMatches = valueMatches.Where(x => x.comment == sourceTaggedValue.comment);
                        if (!commentMatches.Any())
                        {
                            targetTaggedValue = valueMatches.First();
                        }
                        else
                        {
                            targetTaggedValue = commentMatches.First();
                        }
                    }
                }
            }
            //pop from the list of target tagged values
            if (targetTaggedValue != null)
            {
                targetTaggedValues.Remove(targetTaggedValue);
            }
            //return
            return targetTaggedValue;
        }
    }
}
