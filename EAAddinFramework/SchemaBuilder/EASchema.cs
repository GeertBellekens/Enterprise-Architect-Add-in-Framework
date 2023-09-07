
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
        //we only need classes, enumerations, datatypes and primitivetypes
        private List<string> classifierObjectTypes = new List<string>() { "Class", "Enumeration", "Datatype", "PrimitiveType", "Association" };

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

        private List<Package> _localSharedPackages = null;
        private List<Package> localSharedPackages
        {
            get
            {
                if (this._localSharedPackages == null)
                {
                    this._localSharedPackages = new List<Package>();
                    if (this.containerElement != null)
                    {
                        var sqlQuery = $@"select po.Object_ID from (( t_objectproperties tv
                                inner join t_object po on po.ea_guid = tv.Value)
                                inner join t_object o on o.Object_ID = tv.Object_ID)
                                where tv.Property = 'sharedPackage'
                                and o.ea_guid = '{this.containerElement.uniqueID}'";
                        this._localSharedPackages = this.model.getElementWrappersByQuery(sqlQuery).OfType<Package>().ToList();
                    }
                }
                return this._localSharedPackages;
            }
        }
        public bool isSharedLocalPackage(Package package)
        {
            return this.localSharedPackages.Any(x => x.uniqueID == package.uniqueID);
        }

        private HashSet<SBF.SchemaElement> _elements = null;
        /// <summary>
        /// the SchemaElements owned by this Schema
        /// </summary>
        public HashSet<SBF.SchemaElement> elements
        {
            get
            {
                if (this._elements == null)
                {
                    //reset elementGUIDString
                    this._elementGUIDstring = null;
                    //get elements
                    this._elements = new HashSet<SBF.SchemaElement>();
                    foreach (EA.SchemaType schemaType in getSchemaTypes())
                    {
                        this._elements.Add(EASchemaBuilderFactory.getInstance(this.model).createSchemaElement(this, schemaType));
                    }
                    //now load all source items for the whole schema
                    loadSourceItems();
                }
                return this._elements;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        private void loadSourceItems()
        {
            //tell the user what we are doing 
            EAOutputLogger.log(this.model, this.settings.outputName, "Loading schema source elements"
                               , 0, LogTypeEnum.log);
            var elementGuids = this.elements.OfType<EASchemaElement>().Select(x => x.TypeID).ToList();
            var elementWrappers = this.model.getElementWrapperByGUIDs(elementGuids);
            //create a dictionary based on the GUID of the element
            var sourceElementDictionary = new Dictionary<string, TSF_EA.ElementWrapper>();
            var elementDictionary = new Dictionary<int, TSF_EA.ElementWrapper>();
            foreach (var elementWrapper in elementWrappers)
            {
                if (! sourceElementDictionary.ContainsKey(elementWrapper.uniqueID))
                {
                    sourceElementDictionary.Add(elementWrapper.uniqueID, elementWrapper);
                }
                if (! elementDictionary.ContainsKey(elementWrapper.id))
                {
                    elementDictionary.Add(elementWrapper.id, elementWrapper);
                }
            }
            //loop all elements and link set the source element
            foreach(var schemaElement in this.elements.OfType<EASchemaElement>())
            {
               if ( sourceElementDictionary.TryGetValue(schemaElement.TypeID, out TSF_EA.ElementWrapper elementWrapper))
               {
                    schemaElement.sourceElement = elementWrapper as UML.Classes.Kernel.Classifier;
               }
            }
            //load tagged values
            TSF_EA.Constraint.loadConstraints(elementDictionary, model);
            //load tagged values
            TSF_EA.ElementTag.loadElementTags(elementDictionary, model);
            //tell the user what we are doing 
            EAOutputLogger.log(this.model, this.settings.outputName, "Loading schema source attributes and connectors"
                               , 0, LogTypeEnum.log);
            //attributes/Literals
            var attributeGUIDs = new List<string>();
            // connectors
            var connectorGUIDs = new List<string>();
            foreach (var schemaElement in this.elements.OfType<EASchemaElement>())
            {
                attributeGUIDs.AddRange(schemaElement.schemaPropertyWrappers.OfType<EASchemaProperty>().Select(x => x.GUID));//attributes and literals
                connectorGUIDs.AddRange(schemaElement.schemaAssociations.OfType<EASchemaAssociation>().Select(x => x.GUID));
            }
            var attributeWrappers = this.model.getAttributeWrapperByGUIDs(attributeGUIDs);
            //create dictionaries (both on id and GUID)
            var sourceAttributeDictionary = new Dictionary<string, TSF_EA.AttributeWrapper>();
            var attributeDictionary = new Dictionary<int, TSF_EA.AttributeWrapper>();
            foreach (var attributeWrapper in attributeWrappers)
            {
                if (!sourceAttributeDictionary.ContainsKey(attributeWrapper.uniqueID))
                {
                    sourceAttributeDictionary.Add(attributeWrapper.uniqueID, attributeWrapper);
                }
                if (! attributeDictionary.ContainsKey(attributeWrapper.id))
                {
                    attributeDictionary.Add(attributeWrapper.id, attributeWrapper);
                }
            }
            //load tagged values for attributes
            TSF_EA.AttributeTag.loadAttributeTags(attributeDictionary, this.model);
            //load constraints for attributes
            TSF_EA.AttributeConstraint.loadConstraints(attributeDictionary, model);
            //load source associations
            var associations = this.model.getRelationsByGUIDs(connectorGUIDs);
            //create dictionary
            var sourceAssociationDictionary = new Dictionary<string, TSF_EA.ConnectorWrapper>();
            var connectorDictionary = new Dictionary<int, TSF_EA.ConnectorWrapper>();
            foreach (var association in associations)
            {
                if (!sourceAssociationDictionary.ContainsKey(association.uniqueID))
                {
                    sourceAssociationDictionary.Add(association.uniqueID, association);
                }
                if (!connectorDictionary.ContainsKey(association.id))
                {
                    connectorDictionary.Add(association.id, association);
                }
            }
            //load connector tags
            TSF_EA.RelationTag.loadConnectorTags(connectorDictionary, model);
            //set attribute/literal/connector sources
            foreach (var schemaElement in this.elements.OfType<EASchemaElement>())
            {
                //Loop all attribute and link the source attribute
                foreach (var schemaProperty in schemaElement.schemaPropertyWrappers.OfType<EASchemaProperty>())
                {
                    if (sourceAttributeDictionary.TryGetValue(schemaProperty.GUID, out TSF_EA.AttributeWrapper attributeWrapper))
                    {
                        if (attributeWrapper is TSF_EA.Attribute)
                        {
                            schemaProperty.sourceProperty = attributeWrapper as TSF_EA.Attribute;
                        }
                        else
                        {
                            schemaProperty.sourceLiteral = attributeWrapper as TSF_EA.EnumerationLiteral;
                        }
                        
                    }
                }

                //and for all connectors
                foreach (var schemaAssociation in schemaElement.schemaAssociations.OfType<EASchemaAssociation>())
                {
                    if (sourceAssociationDictionary.TryGetValue(schemaAssociation.GUID, out TSF_EA.ConnectorWrapper association))
                    {
                        schemaAssociation.sourceAssociation = association as Association; ;
                    }
                }
            }
        }
        private string _elementGUIDstring = null;
        private string elementGUIDstring
        {
            get
            {
                if (this._elementGUIDstring == null)
                {
                    this._elementGUIDstring = "'" 
                                            + string.Join("','", this.elements.Where(x => x.sourceElement != null).Select(x => x.sourceElement.uniqueID).ToArray<string>()) 
                                            + "'" ;
                }
                return _elementGUIDstring;
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
        internal EASchemaElement getSchemaElementForSubsetElement(Classifier subsetElement, Dictionary<string, string> subsetDictionary)
        {
            
            
            var elementWrapperSubsetElement = subsetElement as TSF_EA.ElementWrapper;
            EASchemaElement result = null;
            if (elementWrapperSubsetElement == null)
            {
                return null;
            }
            //first check if we already have this subset element registered on one of the elements
            result = this.elements
                    .OfType<EASchemaElement>()
                    .FirstOrDefault(x => x.sourceElement?.uniqueID == subsetElement.uniqueID);
            if (result != null)
            {
                return result;
            }
            //first find the source element from which this subset element is derived
            //check on if there is a tagged value that references the source element
            string sourceElementUniqueID = null;
            //first try to find it using the subset dictionary
            if (subsetDictionary != null)
            {
                if (subsetDictionary.ContainsKey(elementWrapperSubsetElement.uniqueID))
                {
                    sourceElementUniqueID = subsetDictionary[elementWrapperSubsetElement.uniqueID];
                }
            }
            if (string.IsNullOrEmpty(sourceElementUniqueID))
            {
                if (this.settings.tvInsteadOfTrace)
                {
                    //use query to get source element unique ID
                    var sqlGetSourceUniqueID = $@"select tv.Value as uniqueID from t_objectproperties tv
                                        where tv.Object_ID = {elementWrapperSubsetElement.id}
                                        and tv.Property = '{this.settings.elementTagName}'";
                    sourceElementUniqueID = this.model.getFirstValueFromQuery(sqlGetSourceUniqueID, "uniqueID");
                }
                else
                {
                    //check if the subset element has a dependency to the source element of the schema
                    string sqlCheckTrace = $@"select c.End_Object_ID from t_connector c
												where 
												c.Connector_Type = 'Abstraction'
												and c.Stereotype = 'trace'
												and c.Start_Object_ID = {elementWrapperSubsetElement.id}";
                    var checkTraceXML = this.model.SQLQuery(sqlCheckTrace);
                    var endObjectIDNodes = checkTraceXML.SelectNodes(this.model.formatXPath("//End_Object_ID"));
                    foreach (System.Xml.XmlNode connectorIDNode in endObjectIDNodes)
                    {
                        int elementID;
                        if (int.TryParse(connectorIDNode.InnerText, out elementID))
                        {
                            //check if there is an schemaElement with this 
                            sourceElementUniqueID = this.elements
                                .OfType<EASchemaElement>()
                                .FirstOrDefault(x => x.eaSourceElement?.id == elementID)
                                ?.sourceElement.uniqueID;
                            break;
                        }
                    }
                }
            }
            //if we haven't found a source element, then this subset element can't have a schema element
            if (string.IsNullOrEmpty(sourceElementUniqueID))
            {
                return null;
            }
            //first check if the subset element can be matched to a redefined schemaElement
            if (this.settings.useAliasForRedefinedElements)
            {
                //find redefined element with same source element, and where the name corresponds to the alias of the sourceElement
                result = this.elements.OfType<EASchemaElement>().FirstOrDefault(x => x.isRedefined
                                                                        && x.name == elementWrapperSubsetElement.alias
                                                                        && x.sourceElement.uniqueID == sourceElementUniqueID
                                                                        && (x.subsetElement == null
                                                                            || x.subsetElement.uniqueID == subsetElement.uniqueID));
                //if not found with redefined elemnet then find as regular element
                if (result == null)
                {
                    result = this.elements.OfType<EASchemaElement>().FirstOrDefault(x => !x.isRedefined
                                                                        && x.name == subsetElement.name
                                                                        && x.sourceElement.uniqueID == sourceElementUniqueID
                                                                        && (x.subsetElement == null
                                                                            || x.subsetElement.uniqueID == subsetElement.uniqueID));
                }
            }
            else
            {
                //find based on name, regardless of redefined or not
                result = this.elements.OfType<EASchemaElement>().FirstOrDefault(x => x.name == subsetElement.name
                                                                        && x.sourceElement.uniqueID == sourceElementUniqueID
                                                                        && (x.subsetElement == null
                                                                            || x.subsetElement.uniqueID == subsetElement.uniqueID));
            }
            //if the subset element is renamed, then we won't find it based on the name, so we use the unique ID only
            if (result == null)
            {
                result = this.elements.OfType<EASchemaElement>().FirstOrDefault(x => x.sourceElement.uniqueID == sourceElementUniqueID
                                                                        && (x.subsetElement == null
                                                                            || x.subsetElement.uniqueID == subsetElement.uniqueID));
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
                EAOutputLogger.log(this.model, this.settings.outputName, "Creating subset element for: '" + schemaElement.name + "'"
                                   , 0, LogTypeEnum.log);
                //do not copy elements that are shared
                if (!schemaElement.isShared)
                {
                    //only create subset elements for classes, not for datatypes
                    if (schemaElement.sourceElement is UML.Classes.Kernel.Class
                       || schemaElement.sourceElement is UML.Classes.Kernel.Enumeration
                       || schemaElement.sourceElement is UML.Classes.Kernel.Association)
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
                    //only create subset elements for classes and enumerations and datatypes and n-ary associations
                    if (schemaElement.sourceElement is UML.Classes.Kernel.Class
                       || schemaElement.sourceElement is UML.Classes.Kernel.Enumeration
                        || schemaElement.sourceElement is UML.Classes.Kernel.DataType
                        || schemaElement.sourceElement is UML.Classes.Kernel.Association)
                    {
                        //tell the user what we are doing 
                        EAOutputLogger.log(this.model, this.settings.outputName, "Creating subset details for: '" + schemaElement.name + "'"
                                           , 0, LogTypeEnum.log);
                        schemaElement.createSubsetAssociations();
                        // and to resolve the attributes types to subset types if required
                        schemaElement.createSubsetAttributes();
                        if (this.settings.copyAllOperations)
                        {
                            schemaElement.createSubsetOperations();
                        }
                        //Logger.log("after EASchema::createSubsetAttributes ");
                        schemaElement.createSubsetLiterals();
                        //clean up attribute type dependencies we don't need anymore
                        schemaElement.cleanupAttributeDependencies();
                        if (!this.settings.dontCreateAttributeDependencies)
                        {
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
                    var subsetElement = schemaElement.subsetElement as TSF_EA.ElementWrapper;
                    if (subsetElement != null)
                    {
                        //remove those subset elements that don't have any attributes or associations
                        if (isSubsetElementNotUsed(subsetElement as TSF_EA.ElementWrapper))
                        {
                            //check if the subset element is used in a schema or subset downstream
                            if (isItemUsedInASchema(subsetElement, this.model))
                            {
                                //report error
                                EAOutputLogger.log(this.model, this.settings.outputName, $"Subset element '{subsetElement.name}' cannot be deleted as it is still used in one or more schemas"
                                       , subsetElement.id, LogTypeEnum.warning);
                            }
                            else
                            {
                                //tell the user what we are doing 
                                EAOutputLogger.log(this.model, this.settings.outputName, "Deleting subset element for: '" + schemaElement.name + "'"
                                               , 0, LogTypeEnum.log);
                                //actually delete the subset element
                                subsetElement.delete();
                            }
                        }
                    }
                }
            }
            //tell the user what we are doing 
            EAOutputLogger.log(this.model, this.settings.outputName, "Synchronizing tagged values", 0, LogTypeEnum.log);
            //then synchronize the tagged values where needed
            synchronizeTaggedValues();
            //tell the user what we are doing 
            EAOutputLogger.log(this.model, this.settings.outputName, "Saving Schema content", 0, LogTypeEnum.log);
            //save the new schema contents to the destination package
            this.saveSchemaContent(destinationPackage);

        }
        private bool isSubsetElementNotUsed(TSF_EA.ElementWrapper subsetElement)
        {
            return !subsetElement.hasAttributes()
                && !subsetElement.hasAssociations()
                && !subsetElement.hasGeneralizations()
                && !subsetElement.hasDependentTypedAttributes();
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
                        if (!string.IsNullOrEmpty(targetTagComment))
                        {
                            tuple.Item2.addTaggedValue(tuple.Item1.name, tagValueString, targetTagComment, false);
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
            return string.Join(",", targetGUIDs);
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
                targetItem = this.elements.FirstOrDefault(x => x.sourceElement.Equals(sourceItem))?.subsetElement;
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
            foreach (var schemaElement in this.elements)
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
            foreach (var schemaElement in this.elements)
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
                    DialogResult response;
                    try
                    {
                        response = MessageBox.Show(this.model.mainEAWindow,
                                                    $"Elements to update: {elementsToUpdate.Count}\n" +
                                                    $"Elements to create: {elementsToCreate.Count}\n" +
                                                    $"Elements to delete: {subsetElementsToDelete.Count}\n" +
                                                    "YES to generate only changes\n" +
                                                    "NO to generate the whole schema"
                                                    , "Generate only changes?"
                                                    , MessageBoxButtons.YesNoCancel
                                                    , MessageBoxIcon.Question);
                    }
                    catch (Exception) //sometimes the this.model.mainEAWindow handle is invalid
                    {
                        response = MessageBox.Show(
                                                    $"Elements to update: {elementsToUpdate.Count}\n" +
                                                    $"Elements to create: {elementsToCreate.Count}\n" +
                                                    $"Elements to delete: {subsetElementsToDelete.Count}\n" +
                                                    "YES to generate only changes\n" +
                                                    "NO to generate the whole schema"
                                                    , "Generate only changes?"
                                                    , MessageBoxButtons.YesNoCancel
                                                    , MessageBoxIcon.Question);
                    }

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
                    DialogResult response;
                    try
                    {
                        response = MessageBox.Show(this.model.mainEAWindow
                                                   , "Found no differences from the generated schema \n" +
                                                   "Would you like to re-generate anyway?"
                                                   , "Re-Generate?"
                                                   , MessageBoxButtons.YesNo
                                                   , MessageBoxIcon.Question);

                    }
                    catch (Exception) //sometimes the this.model.mainEAWindow handle is invalid
                    {
                        response = MessageBox.Show("Found no differences from the generated schema \n" +
                                                   "Would you like to re-generate anyway?"
                                                   , "Re-Generate?"
                                                   , MessageBoxButtons.YesNo
                                                   , MessageBoxIcon.Question);
                    }


                    if (response == DialogResult.Yes)
                        generateChangesOnly = false;
                    else return;
                }
            }
            EAOutputLogger.log(this.model, this.settings.outputName, $"Loading subsetmodel for package '{destinationPackage.name}'");
            
            if (generateChangesOnly)
            {
                // this will delete the subset elements that are no longer needed
                matchSubsetElements(destinationPackage, new HashSet<Classifier>(destinationPackage.getAllOwnedElements(classifierObjectTypes).OfType<Classifier>()));
                //then process only the elementsToUpdate and ElementsToCreate
                matchAndUpdateSubsetModel(destinationPackage, elementsToCreate, elementsToUpdate);
            }
            else
            {
                //regenerate completely
                matchSubsetElements(destinationPackage, new HashSet<Classifier>(destinationPackage.getAllOwnedElements(classifierObjectTypes).OfType<Classifier>()));
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
                //tell the user what we are doing 
                EAOutputLogger.log(this.model, this.settings.outputName, "Matching subset element '" + schemaElement.name + "' to the schema", 0, LogTypeEnum.log);
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
                //tell the user what we are doing 
                EAOutputLogger.log(this.model, this.settings.outputName, "Matching subset element '" + schemaElement.name + "' to the schema",0, LogTypeEnum.log);
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
            EAOutputLogger.log(this.model, this.settings.outputName
                               , $"Getting subset elements starting from root element in package '{messageElement.owningPackage.name}'", 0, LogTypeEnum.log);
            HashSet<UML.Classes.Kernel.Classifier> subsetElements = this.getSubsetElementsfromMessage(messageElement);
            //match each subset element to a schema element
            matchSubsetElements(messageElement.owningPackage, subsetElements);
        }

        private Dictionary<string, string> getSubsetElementDictionary(HashSet<Classifier> subsetElements)
        {
            //get a list comma separated objectID's of the subsetElements
            var subsetElementIDs = String.Join(",", subsetElements.OfType<TSF_EA.ElementWrapper>().Select(x => x.id).ToArray());
            if (subsetElementIDs == String.Empty)
            {
                subsetElementIDs = "0";
            }
            var schemaSourceElementGuids = String.Join(",", this.elements.Select(x => "'" + x.sourceElement?.uniqueID + "'").ToArray());
            if (schemaSourceElementGuids == String.Empty)
            {
                schemaSourceElementGuids = "0";
            }
            string sqlGetData;
            //build a query
            if (this.settings.tvInsteadOfTrace)
            {
                sqlGetData = $@"select o.ea_guid as subsetID, tv.Value as sourceID 
                                from ( t_objectproperties tv
                                inner join t_object o on o.Object_ID = tv.Object_ID)
                                where tv.Object_ID in ({subsetElementIDs})
                                and tv.Property = '{this.settings.elementTagName}'
                                and tv.Value in ({schemaSourceElementGuids})";
            }
            else
            {
                sqlGetData = $@"select o.ea_guid as subsetID, oo.ea_guid as sourceID
                                from ((t_connector c
                                inner join t_object o on o.Object_ID = c.Start_Object_ID)
                                inner join t_object oo on oo.Object_ID = c.End_Object_ID)
                                where 
                                c.Connector_Type in ('Abstraction', 'Dependency')
                                and c.Stereotype = 'trace'
                                and c.Start_Object_ID in ({subsetElementIDs})
                                and oo.ea_guid in ({schemaSourceElementGuids})";
            }
            //return dictionary based on query
            return this.model.getDictionaryFromQuery(sqlGetData);
        }

        void matchSubsetElements(Package destinationPackage, HashSet<Classifier> subsetElements)
        {
            EAOutputLogger.log(this.model, this.settings.outputName, $"Getting subset element dictionary'", 0, LogTypeEnum.log);
            //get a dictionary of subset element guid's with their source GUID
            var subsetDictionary = this.getSubsetElementDictionary(subsetElements);
            //loop subset elements ordered by name
            foreach (Classifier subsetElement in subsetElements.OrderBy(x => name))
            {
                //tell the user what we are doing 
                EAOutputLogger.log(this.model, this.settings.outputName, $"Matching subset element: '{subsetElement.name}' to a schema element"
                                   , ((TSF_EA.ElementWrapper)subsetElement).id, LogTypeEnum.log);
                //get the corrsponding schema element
                EASchemaElement schemaElement = this.getSchemaElementForSubsetElement(subsetElement, subsetDictionary);

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
            //now we loop all schemaElements that haven't been matched yet to figure out if they should be matched with a shared class
            foreach (var schemaElement in this.elements.Where( x=> x.subsetElement == null && x.isShared))
            {
                schemaElement.subsetElement = schemaElement.sourceElement;
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
            if (subsetElement is Class || subsetElement is Enumeration || subsetElement is Association )
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
            //a layered SQL approach in order to get all subset elements (similar to packageTreeID's)
            var subsetElements = new HashSet<UML.Classes.Kernel.Classifier>();
            this.addRelatedSubsetElements(messageElement, subsetElements) ;
            return subsetElements;
        }

        /// <summary>
        /// adds all the related subset elements to the list recursively, including all elements in the package of the element, and it's subpackages
        /// </summary>
        /// <param name="element">the element to start from </param>
        /// <param name="subsetElements">the HashSet of subset element to add to</param>
        private void addRelatedSubsetElements(UML.Classes.Kernel.Classifier element, HashSet<UML.Classes.Kernel.Classifier> subsetElements )
        {
            
            EAOutputLogger.log(this.model, this.settings.outputName
                               , $"Getting subsetElement IDs level 0", 0, LogTypeEnum.log);
            var allElementIDs = new List<string>();
            var parentElementIDs = new List<string>();

            var sqlGetData = $@"select o.Object_ID from t_object o
                            where o.Object_Type in ('Class', 'Enumeration', 'DataType', 'PrimitiveType', 'Association')
                            and o.Package_ID in ({((TSF_EA.Package)element.owningPackage).packageTreeIDString})";
            //start at the message root
            parentElementIDs.AddRange(this.model.getListFromQuery(sqlGetData));
            //get all related elementID's
            addAllRelatedElementIDs(allElementIDs, parentElementIDs, 0);
            EAOutputLogger.log(this.model, this.settings.outputName
                               , $"Getting {allElementIDs.Count} possible subsetElement objects", 0, LogTypeEnum.log);
            //get all elements from the IDs
            var eaDBElements = TSF_EA.EADBElement.getEADBElementsForElementIDs (allElementIDs, this.model);
            //create the EAElementWrappers for them
            var elementWrappers = this.model.factory.createElements(eaDBElements).OfType<TSF_EA.ElementWrapper>();
            TSF_EA.ElementWrapper.loadDetailsForElementWrappers(elementWrappers, model);
            foreach (var subsetElement in elementWrappers.OfType<Classifier>())
            {
                if (subsetElement != null
                  && !subsetElements.Contains(subsetElement))
                {
                    subsetElements.Add(subsetElement);
                }
            }
        }
        private void addAllRelatedElementIDs(List<string> allElementIDs, List<string> parentElementIDs, int level)
        {
            level ++;
            EAOutputLogger.log(this.model, this.settings.outputName
                               , $"Getting subsetElement IDs level {level}", 0, LogTypeEnum.log);
            if (!parentElementIDs.Any())
            {
                //no point in searching related elements for nothing
                return;
            }
            //add the parent elements to all elements
            allElementIDs.AddRange(parentElementIDs);
            var parentIDString = String.Join(",", parentElementIDs);
            var allElementIDString = String.Join(",", allElementIDs);
            var sqlGetData = $@"select o.Object_ID from  (
                    select o.Object_ID from ((t_object o
                    inner join t_connector c on (o.Object_ID = c.End_Object_ID
                                                and c.Connector_Type in ('Aggregation', 'Association')))
                    inner join t_object oo on (oo.Object_ID = c.Start_Object_ID
                                            and oo.Object_ID <> o.Object_ID))
                    where o.Object_Type in ('Class', 'Enumeration', 'DataType', 'PrimitiveType', 'Association')
                    and oo.Object_ID in ({parentIDString})
                    and o.Object_ID not in ({allElementIDString})
                    union
                    select o.Object_ID from ((t_object o
                    inner join t_attribute a on a.Classifier = o.Object_ID)
                    inner join t_object oo on oo.Object_ID = a.Object_ID)
                    where o.Object_Type in ('Class', 'Enumeration', 'DataType', 'PrimitiveType', 'Association')
                    and oo.Object_ID in ({parentIDString})
                    and o.Object_ID not in ({allElementIDString})
                    )o
                    where exists";
            if (this.settings.tvInsteadOfTrace)
            {
                sqlGetData += Environment.NewLine +
                    $@"(select tv.ea_guid
					from t_objectproperties tv 
					where tv.Object_ID = o.Object_ID
							and tv.Property = '{this.settings.elementTagName}'
							and tv.Value in ({this.elementGUIDstring})
					)";
            }
            else
            {
                sqlGetData += Environment.NewLine +
                    $@"(select tr.Connector_ID from (t_connector tr
					inner join t_object oso on (oso.Object_ID = tr.End_Object_ID
									and oso.ea_guid in ({this.elementGUIDstring})))
					where tr.Stereotype = 'trace'
					and tr.Start_Object_ID = o.Object_ID
					)";
            }
            var relatedElementIDs = this.model.getListFromQuery(sqlGetData);
            //go one level deeper
            addAllRelatedElementIDs(allElementIDs, relatedElementIDs, level);
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
                if (sourceTaggedValue.name.Equals(this.settings.elementTagName, StringComparison.InvariantCultureIgnoreCase)
                    || sourceTaggedValue.name.Equals(this.settings.sourceAttributeTagName, StringComparison.InvariantCultureIgnoreCase)
                    || sourceTaggedValue.name.Equals(this.settings.sourceAssociationTagName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;//skip if the tagged value is one of the configured traceability tagged values
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
