using System.Collections.Generic;
using System.Linq;
using System;
using EAAddinFramework.Utilities;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using MP = MappingFramework;
using CSV = CsvHelper;
using System.IO;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingFactory.
	/// </summary>
	public static class MappingFactory
	{
		public const string mappingSourcePathName = "mappingSourcePath";
        public const string mappingTargetPathName = "mappingTargetPath";
        public const string mappingLogicName = "mappingLogic";
        public const string isEmptyMappingName = "isEmptyMapping";
        public static MappingSet createMappingSet(ElementWrapper sourceRoot, ElementWrapper targetRoot, MappingSettings settings )
        {
            //first create the root nodes
            var sourceRootNode = createNewRootNode(sourceRoot, settings);
            var targetRootNode = createNewRootNode(targetRoot, settings);
            //then create the new mappingSet
            var mappingSet = new MappingSet(sourceRootNode, targetRootNode, settings);
            return mappingSet;
        }
        public static ClassifierMappingNode createNewRootNode(ElementWrapper rootElement, MappingSettings settings)
        {
            //depending on the type of root we create a dataModel structure (Package) or a Message structure (class).
            return rootElement is Package ?
                    new ClassifierMappingNode(rootElement, settings, MP.ModelStructure.DataModel) :
                    new ClassifierMappingNode(rootElement, settings, MP.ModelStructure.Message);
        }
        public static MappingSet createMappingSet(ElementWrapper sourceRoot, MappingSettings settings)
        {

            //log progress
            var startTime = DateTime.Now;
            EAOutputLogger.log($"Start creating mapping set for {sourceRoot.name}", sourceRoot.id);
            //get target mapping root
            ElementWrapper targetRootElement = null;
            var packageTrace = sourceRoot.getRelationships<Abstraction>().FirstOrDefault(x => x.source.uniqueID == sourceRoot.uniqueID
                                                                                        && x.target is ElementWrapper 
                                                                                        && x.stereotypes.Any(y => y.name == "trace"));
            if (packageTrace != null) targetRootElement = packageTrace.target as ElementWrapper;
            var mappingSet =  createMappingSet(sourceRoot, targetRootElement, settings);
            //log progress
            var endTime = DateTime.Now;
            var processingTime = (endTime - startTime).TotalSeconds;
            EAOutputLogger.log($"Finished creating mapping set for {sourceRoot.name} in {processingTime.ToString("N0")} seconds", sourceRoot.id);
            return mappingSet;
        }

        private static List<string> getMappingPath(Element tagOwner, bool target)
        {
            var mappingPath = new List<String>();
            var taggedValue = target ? tagOwner.getTaggedValue(mappingTargetPathName) : tagOwner.getTaggedValue(mappingSourcePathName);
            if (taggedValue != null)
            {
                mappingPath = taggedValue.eaStringValue.Split('.').ToList();
            }
            return mappingPath;
        }
        private static List<string> getMappingPath(TaggedValue mappingTag, bool target)
        {
            var mappingPath = new List<String>();
            var pathString = target ? 
                KeyValuePairsHelper.getValueForKey(mappingTargetPathName, mappingTag.comment) : 
                KeyValuePairsHelper.getValueForKey(mappingSourcePathName, mappingTag.comment);
            if (! string.IsNullOrEmpty(pathString)) mappingPath = pathString.Split('.').ToList();
            return mappingPath;
        }

        private static List<string> getMappingPath(Element nodeSource, MappingNode targetRootNode)
        {
            if (nodeSource.owner == null || targetRootNode.source == null || nodeSource.uniqueID == targetRootNode.source.uniqueID) return new List<string>() { nodeSource.uniqueID };
            var path = getMappingPath((Element)nodeSource.owner, targetRootNode);
            path.Add(nodeSource.uniqueID);
            return path;
        }
        public static Mapping getMapping(MappingNode startNode, TaggedValue mappingTag, MappingNode targetRootNode)
        {
            var sourceMappingPath = getMappingPath(mappingTag, false);
            var targetMappingPath = getMappingPath(mappingTag, true);
            var targetElement = mappingTag.tagValue as Element;
            return getMapping(mappingTag, targetElement, startNode, sourceMappingPath, targetMappingPath, targetRootNode);
        }
        public static Mapping getMapping(MappingNode startNode, ConnectorWrapper mappingRelation, MappingNode targetRootNode)
        {
            var sourceMappingPath = getMappingPath(mappingRelation, false);
            var targetMappingPath = getMappingPath(mappingRelation, true);
            Element targetElement = mappingRelation.targetElement;
            return getMapping(mappingRelation, targetElement, startNode, sourceMappingPath, targetMappingPath, targetRootNode);
        }
        private static Mapping getMapping(UML.Extended.UMLItem mappingItem, Element mappingTarget, MappingNode startNode, List<string> sourceMappingPath, List<string> targetMappingPath, MappingNode targetRootNode)
        {
            //do nothing if mappign target is null
            if (mappingTarget == null) return null;
            //check if the mappingPath of the source corresponds with the path of the node
            var startNodeMappingPath = startNode.getMappingPath();
            //source is OK if mapping corresponds, or no mappingPath found and it is not virtual
            var sourceOK = sourceMappingPath.SequenceEqual(startNodeMappingPath) || (!sourceMappingPath.Any() && ! startNode.isVirtual);
            // if no targetMapping found then we try to build a Mapping up to the target root node source element
            if (!targetMappingPath.Any())
            {
                targetMappingPath = getMappingPath(mappingTarget, targetRootNode);
            }
            //target is OK if the first item of the targetMappignPath corresponds to the targetRootNode
            var targetOK = targetMappingPath.FirstOrDefault() == targetRootNode.source?.uniqueID;
            //if target or source is not OK then we return null
            if (!sourceOK || !targetOK) return null;
            //first create the targetMappingNode
            var targetMappingNode = targetRootNode.createMappingNode(targetMappingPath);
            //return the actual mapping
            return createMapping(mappingItem, startNode, targetMappingNode);
        }
        public static Mapping createMapping(UML.Extended.UMLItem mappingItem, MappingNode startNode, MappingNode targetNode)
        {
            var connector = mappingItem as ConnectorWrapper;
            if (connector != null) return new ConnectorMapping(connector, startNode, targetNode);
            var taggedValue = mappingItem as TaggedValue;
            if (taggedValue != null) return new TaggedValueMapping(taggedValue, startNode, targetNode);
            throw new ArgumentException("MappingItem should be Connector or TaggedValue");
        }

        

        public static MappingNode createMappingNode(UML.Classes.Kernel.NamedElement source, MappingNode parent, MappingSettings settings)
        {
            //AttributeMappingNode
            var attributeSource = source as AttributeWrapper;
            if (attributeSource != null) return new AttributeMappingNode(attributeSource, parent as ClassifierMappingNode, settings, parent.structure);

            //AssociationMappingNode
            var associationSource = source as Association;
            if (associationSource != null) return new AssociationMappingNode(associationSource, parent as ClassifierMappingNode, settings, parent.structure);

            //ClassifierMappingNode
            var classifierSource = source as ElementWrapper;
            if (classifierSource != null) return new ClassifierMappingNode(classifierSource, parent, settings, parent.structure);

            //not a valid source type, return null
            return null;
        }


        public static List<Mapping> createNewMappings(TSF.UmlToolingFramework.Wrappers.EA.Attribute attribute,string basepath,ElementWrapper targetRootElement)
		{
			List<Mapping> returnedMappings = new List<Mapping>();
			//connectors from owned attributes
			foreach (ConnectorWrapper mappedConnector in attribute.relationships.OfType<ConnectorWrapper>())
			{
				if (! mappedConnector.taggedValues.Any( x => x.name == mappingSourcePathName && x.tagValue.ToString() != basepath))
				{
					//get the target base path
					ConnectorMapping connectorMapping;
					var targetTV = mappedConnector.taggedValues.FirstOrDefault(x => x.name == mappingTargetPathName);
					string targetBasePath = string.Empty;
					if (targetTV != null) targetBasePath = targetTV.tagValue.ToString();
					if (! string.IsNullOrEmpty(targetBasePath))
					{
						//connectorMapping = new ConnectorMapping(mappedConnector,basepath,targetBasePath);
					}
					else
					{
						//connectorMapping = new ConnectorMapping(mappedConnector,basepath,targetRootElement);
					}
					//returnedMappings.Add(connectorMapping);
				}
			}
			//tagged value references from owned attributes	
			foreach (TaggedValue mappedTaggedValue in attribute.taggedValues.Where(x => x.tagValue is Element) )
			{
				string mappingSourcePath = KeyValuePairsHelper.getValueForKey(mappingSourcePathName,mappedTaggedValue.comment);
				string targetBasePath = KeyValuePairsHelper.getValueForKey(mappingTargetPathName,mappedTaggedValue.comment);

				//if not filled in or corresponds to the attributeBasePath or the attributeBasePath + the name of the attribute
				if (string.IsNullOrEmpty(mappingSourcePath) || mappingSourcePath == basepath 
				    || mappingSourcePath == basepath + "." + attribute.name)
				{
					TaggedValueMapping tagMapping;
					if (! string.IsNullOrEmpty(targetBasePath))
					{
						//tagMapping = new TaggedValueMapping(mappedTaggedValue,basepath,targetBasePath);
					}
					else
					{
						//tagMapping = new TaggedValueMapping(mappedTaggedValue,basepath,targetRootElement);
					}	
					//returnedMappings.Add(tagMapping);
				}
			}
			//add the mappings for the type of the attribute
			var attributeType = attribute.type as ElementWrapper;
			if (attributeType != null) returnedMappings.AddRange(createOwnedMappings(attributeType,basepath + "." + attribute.name,false));
			return returnedMappings;
		}
		public static List<Mapping> createOwnedMappings(ElementWrapper ownerElement,string basepath,ElementWrapper targetRootElement,bool includeOwnedElements)
		{
			List<Mapping> returnedMappings = new List<Mapping>();
			//connectors to an attribute
			foreach (ConnectorWrapper mappedConnector in ownerElement.relationships.OfType<ConnectorWrapper>()
			         .Where(y => y.targetElement is AttributeWrapper)) 
			{
				
				string connectorPath = basepath + "." + getConnectorString(mappedConnector);
				//var connectorMapping = new ConnectorMapping(mappedConnector,connectorPath,targetRootElement);
				//returnedMappings.Add(connectorMapping);
			}			
			//loop owned attributes
			foreach (TSF.UmlToolingFramework.Wrappers.EA.Attribute ownedAttribute in ownerElement.ownedAttributes) 
			{
				returnedMappings.AddRange(createNewMappings(ownedAttribute,basepath, targetRootElement));
			}
			//loop owned Elements
			if (includeOwnedElements)
			{
				foreach (var ownedElement in ownerElement.ownedElements.OfType<ElementWrapper>()) 
				{
					returnedMappings.AddRange(createOwnedMappings(ownedElement,basepath + "." + ownedElement.name,targetRootElement,includeOwnedElements));
				}
			}
			return returnedMappings;
		}
		public static List<Mapping> createOwnedMappings(ElementWrapper ownerElement,string basepath,bool includeOwnedElements )
		{
			return createOwnedMappings( ownerElement, basepath,null,includeOwnedElements);
		}
		public static List<Mapping> createRootMappings(ElementWrapper root,string basepath)
		{
			//get the owned mappings
			var returnedMappings =  MappingFactory.createOwnedMappings(root,basepath,false);
			//get the mappings of all associated elements
			foreach (var assocation in root.relationships
			         					.OfType<Association>()
			         					.Where (x => x.source.Equals(root))) 
			{
				var targetElement = assocation.target as ElementWrapper;
				if (targetElement != null) returnedMappings.AddRange(createRootMappings(targetElement,basepath + "." + getConnectorString(assocation)));
			}
			//get the owned mappings of all owned elements
			foreach (var ownedElement in root.ownedElements.OfType<ElementWrapper>()) 
			{
				returnedMappings.AddRange(createRootMappings(ownedElement,basepath + "." + ownedElement.name));
			}
			return returnedMappings;
		}
		public static string getConnectorString(ConnectorWrapper mappedConnector)
		{
			//check if the connector has a target role name
			string connectorString = mappedConnector.targetEnd.name;
			//if no rolename then try the association name
			if (string.IsNullOrEmpty(connectorString)) connectorString = mappedConnector.name;
			//if no associationName then we take the name of the target element
			if (string.IsNullOrEmpty(connectorString)) connectorString = mappedConnector.target.name;
			return connectorString;
		}

		/// <summary>
		/// create a mappingSet based on the data in the CSV file
		/// </summary>
		/// <param name="model">the model that contains the elements</param>
		/// <param name="filePath">the path to the CSV file</param>
		/// <returns>a mapping set representing the mapping in the file</returns>
		public static MappingSet createMappingSet(Model model, string filePath,MappingSettings settings, ElementWrapper sourceRootElement, ElementWrapper targetRootElement)
		{
			MappingSet newMappingSet = null;
            IEnumerable<CSVMappingRecord> mappingRecords;
            //read the csv file
            using (var textReader = new StreamReader(filePath))
            {
                var csv = new CSV.CsvReader(textReader, false);
                csv.Configuration.RegisterClassMap<CSVMappingRecordMap>();
                csv.Configuration.Delimiter = ";";
                mappingRecords = csv.GetRecords<CSVMappingRecord>();
            }
            //create the new mapping set
            newMappingSet = createMappingSet(sourceRootElement, targetRootElement, settings);
            //remove all existing mappings
            //TODO: warn if mappings exist?
            foreach(var mapping in newMappingSet.mappings)
            {
                mapping.delete();
            }
            //make sure the target node tree has been build
            ((MappingNode)newMappingSet.target).buildNodeTree();
            //now loop the records
            foreach (var csvRecord in mappingRecords)
            {
                EAOutputLogger.log(model, settings.outputName
                                    , $"Parsed source: '{csvRecord.sourcePath}' target: '{csvRecord.targetPath}' logic: '{csvRecord.mappingLogic}'"
                                    , 0, LogTypeEnum.log);
            }
            return newMappingSet;
            //         int i = 1;
            //Package rootPackage = null;
            //foreach (CSVMappingRecord mappingRecord in parsedFile) 
            //{

            //	//find source
            //	var source = findElement(model, mappingRecord.sourcePath, sourceRootElement);
            //	//find target
            //	var target = findElement(model, mappingRecord.targetPath,targetRootElement);
            //	if (source == null )
            //	{
            //		EAOutputLogger.log(model,settings.outputName
            //		                   ,string.Format("Could not find element that matches: '{0}'",mappingRecord.sourcePath)
            //		                   ,0,LogTypeEnum.error);
            //	}
            //	else if( target == null)
            //	{
            //		EAOutputLogger.log(model,settings.outputName
            //		                   ,string.Format("Could not find element that matches: '{0}'",mappingRecord.targetPath)
            //		                   ,0,LogTypeEnum.error);
            //	}
            //	else
            //	{
            //		//first check if the mappingSet is already created
            //		if (newMappingSet == null)
            //		{
            //			//determine if this should be a PackageMappingSet or an ElementMappingSet
            //			if (sourceRootElement is Package)
            //			{
            //				rootPackage = sourceRootElement as Package;
            //				//newMappingSet = new PackageMappingSet(sourceRootElement as Package);
            //			}
            //			else if (sourceRootElement is ElementWrapper)
            //			{
            //				rootPackage = sourceRootElement.owningPackage as Package;
            //				//newMappingSet = new ElementMappingSet(sourceRootElement as ElementWrapper);
            //			}
            //			else
            //			{
            //				rootPackage = source.owningPackage as Package;
            //				//newMappingSet = new PackageMappingSet((Package)source.owningPackage);
            //			}

            //		}
            //		MappingLogic newMappingLogic = null;
            //		//check if there is any mapping logic
            //		if (! string.IsNullOrEmpty(mappingRecord.mappingLogic))
            //		{
            //			if (settings.useInlineMappingLogic)
            //			{
            //				newMappingLogic = new MappingLogic(mappingRecord.mappingLogic);
            //			}
            //			else
            //			{
            //				 //Check fo an existing mapping logic
            //				 newMappingLogic = getExistingMappingLogic(model, settings, mappingRecord.mappingLogic, rootPackage);

            //				 if (newMappingLogic == null) 
            //				 {

            //					 var mappingElement = model.factory.createNewElement(rootPackage, "mapping logic " + i,settings.mappingLogicType) as ElementWrapper;
            //					 if (mappingElement != null)
            //					 {
            //					 	//increase counter for new mapping element name
            //				 	 	i++;
            //					 	mappingElement.notes = mappingRecord.mappingLogic;
            //					 	mappingElement.save();
            //					 	//create the mappingLogic
            //					 	newMappingLogic = new MappingLogic(mappingElement);
            //					 }
            //					 else
            //					 {
            //					 	//else we create an inline mapping logic anyway
            //					 	newMappingLogic = new MappingLogic(mappingRecord.mappingLogic);
            //					 } 
            //				 }
            //			}
            //		}
            //		Mapping newMapping = null;
            //		var sourceAssociationEnd = source as AssociationEnd;
            //		var targetAssociationEnd = target as AssociationEnd;
            //		//create the new mapping
            //		//we can't create connector mappings for mappings to or from associations so we have to use tagged value mappings for those.
            //		if (settings.useTaggedValues
            //		   || sourceAssociationEnd != null || targetAssociationEnd != null)
            //		{
            //			//if the source or target are associationEnds then we replace them by their association
            //			if (sourceAssociationEnd != null) source = sourceAssociationEnd.association as Element;
            //			if (targetAssociationEnd != null) target = targetAssociationEnd.association as Element;
            //			//newMapping = new TaggedValueMapping(source,target,mappingRecord.sourcePath,mappingRecord.targetPath,settings);
            //		}
            //		else
            //		{
            //			//newMapping = new ConnectorMapping(source,target,mappingRecord.sourcePath,mappingRecord.targetPath,settings);
            //		}
            //		if (newMappingLogic != null) newMapping.mappingLogic = newMappingLogic;
            //		newMapping.save();
            //		newMappingSet.addMapping(newMapping);

            //	}
            //}

        }
		public static void exportMappingSet(MappingSet mappingSet, string filePath)
		{
			if (mappingSet != null)
			{
                using (TextWriter writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    var csv = new CSV.CsvWriter(writer);
                    csv.Configuration.RegisterClassMap<CSVMappingRecordMap>();
                    csv.Configuration.Delimiter = ";";
                    var csvMappingRecords = new List<CSVMappingRecord>();
                    //create the CSV mapping records
                    foreach (var mapping in mappingSet.mappings)
                    {
                        //create the record
                        var mappingRecord = new CSVMappingRecord();
                        mappingRecord.sourcePath = ((MappingNode)mapping.source).getMappingPathExportString();
                        mappingRecord.targetPath = ((MappingNode)mapping.target).getMappingPathExportString();
                        mappingRecord.mappingLogic = mapping.mappingLogic != null ? mapping.mappingLogic.description : string.Empty;
                        //add the record to the list
                        csvMappingRecords.Add(mappingRecord);
                    }
                    //write the CSV mapping records to the filename
                    csv.WriteRecords(csvMappingRecords); 
                }
			}
		}
		/// <summary>
		/// get the existng mapping logic in the given package with the given logic description in order to re-use existing mapping logics
		/// </summary>
		/// <param name="model">the model to use</param>
		/// <param name="settings">maping settings to use</param>
		/// <param name="logicDescription">the description for the mapping logic</param>
		/// <param name="ownerPackage">the owner package to look in</param>
		/// <returns></returns>
		public static MappingLogic getExistingMappingLogic(Model model, MappingSettings settings, string logicDescription, Package ownerPackage)
		{
			string EAMappingType = ((Factory)model.factory).translateTypeName(settings.mappingLogicType);
			string sqlGetExistingMapping = "select * from t_object o " +
											" where o.Package_ID =" + ownerPackage.packageID +
											" and o.Note like'" + logicDescription + "' " +
											" and o.Object_Type = '" + EAMappingType + "' ";
			var mappingElement = model.getElementWrappersByQuery(sqlGetExistingMapping).FirstOrDefault();
			return mappingElement != null ? new MappingLogic(mappingElement) : null; //return null if no mapping element found
		}
		/// <summary>
		/// find an element based on a descriptor
		/// </summary>
		/// <param name="model">the model to searc in</param>
		/// <param name="elementDescriptor">the descriptor to search for</param>
		/// <param name="rootElement">the root element to start from</param>
		/// <returns>the element that corresponds to the descriptor</returns>
		public static Element findElement(Model model, string elementDescriptor, Element rootElement)
		{
			Element foundElement = null;

			if (rootElement == null)
			{
				//try with fully qualified name
				foundElement = model.getItemFromFQN(elementDescriptor) as Element;
			}
			else
			{
				//find as owned item 
				foundElement = rootElement.findOwnedItems(elementDescriptor).OfType<Element>().FirstOrDefault();
			}
			return foundElement;
				
		}
	}
}
