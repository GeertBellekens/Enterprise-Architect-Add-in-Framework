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



        /// <summary>
        /// import the mapings specified in the file into the given mappingSet
        /// </summary>
        /// <param name="mappingSet">the mappingset to import the mappings into</param>
        /// <param name="filePath">the path to the file containing the mappings</param>
        public static void importMappings(MappingSet mappingSet, string filePath)
		{
            IEnumerable<CSVMappingRecord> mappingRecords;
            //remove all existing mappings
            foreach (var mapping in mappingSet.mappings)
            {
                mapping.delete();
            }
            //make sure the target node tree has been build
            ((MappingNode)mappingSet.target).buildNodeTree();
            //read the csv file
            using (var textReader = new StreamReader(filePath))
            {
                var csv = new CSV.CsvReader(textReader, false);
                csv.Configuration.RegisterClassMap<CSVMappingRecordMap>();
                csv.Configuration.Delimiter = ";";
                mappingRecords = csv.GetRecords<CSVMappingRecord>();
                //now loop the records
                foreach (var csvRecord in mappingRecords)
                {
                    //find the source
                    var sourceNode = mappingSet.source.findNode(csvRecord.sourcePath.Split('.').ToList());
                    if (sourceNode == null)
                    {
                        EAOutputLogger.log($"Could not find source element corresponding to '{csvRecord.sourcePath}'", 0,LogTypeEnum.warning);
                    }
                    //find the target
                    var targetNode = mappingSet.target.findNode(csvRecord.targetPath.Split('.').ToList());
                    if (targetNode == null)
                    {
                        EAOutputLogger.log($"Could not find target element corresponding to '{csvRecord.targetPath}'", 0, LogTypeEnum.warning);
                    }
                    //if we found both then we map them
                    if(sourceNode != null && targetNode != null )
                    {
                        var newMapping = sourceNode.mapTo(targetNode);
                        newMapping.mappingLogicDescription = csvRecord.mappingLogic;
                        newMapping.save();
                        EAOutputLogger.log($"Mapping created from '{csvRecord.sourcePath}' to '{csvRecord.targetPath}'", 0);
                    }
                }
            }
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
	}
}
