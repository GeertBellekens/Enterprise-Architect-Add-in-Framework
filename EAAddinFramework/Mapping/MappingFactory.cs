using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TSF.UmlToolingFramework.Wrappers.EA;
using CSV = CsvHelper;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;

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
        public const string isReverseEmptyName = "isReverseEmpty";
        public const string mappingSetName = "mappingSet";
        public const string mappingSetSourceName = "source";
        public const string mappingSetTargetName = "target";
        public static MappingSet createMappingSet(ElementWrapper sourceRoot, ElementWrapper targetRoot, MappingSettings settings)
        {
            //first create the root nodes
            var sourceRootNode = createNewRootNode(sourceRoot, settings, false);
            var targetRootNode = createNewRootNode(targetRoot, settings, true);
            //then create the new mappingSet
            var mappingSet = new MappingSet(sourceRootNode, targetRootNode, settings);
            //return
            return mappingSet;
        }
        public static ElementMappingNode createNewRootNode(ElementWrapper rootElement, MappingSettings settings, bool isTarget)
        {
            //depending on the type of root we create a dataModel structure (Package) or a Message structure (class).
            var newRootNode = rootElement is Package ?
                    new ElementMappingNode(rootElement, settings, MP.ModelStructure.DataModel, isTarget) :
                    new ElementMappingNode(rootElement, settings, MP.ModelStructure.Message, isTarget);
            newRootNode.isTarget = isTarget;
            return newRootNode;
        }

        private static List<string> getMappingPath(Element tagOwner, bool target)
        {
            var mappingPath = new List<String>();
            var taggedValue = target ? tagOwner.getTaggedValue(mappingTargetPathName) : tagOwner.getTaggedValue(mappingSourcePathName);
            if (taggedValue != null)
            {
                var tagValueString = taggedValue.eaStringValue;
                if (tagValueString == "<memo>")
                {
                    tagValueString = taggedValue.comment;
                }
                mappingPath = tagValueString.Split('.').ToList();
            }
            return mappingPath;
        }
        private static List<string> getMappingPath(TaggedValue mappingTag, bool target)
        {
            var mappingPath = new List<String>();
            try
            {
                //read from xml 
                var xdoc = XDocument.Load(new StringReader(mappingTag.comment));
                var pathString = target ?
                    xdoc.Descendants(mappingTargetPathName).FirstOrDefault()?.Value :
                    xdoc.Descendants(mappingSourcePathName).FirstOrDefault()?.Value;
                if (!string.IsNullOrEmpty(pathString))
                {
                    mappingPath = pathString.Split('.').ToList();
                }
            }
            catch (System.Xml.XmlException)
            {
                //parse error. empty mapping path is returned
            }
            return mappingPath;
        }

        private static List<string> getMappingPath(Element nodeSource, MappingNode targetRootNode)
        {
            if (nodeSource.owner == null || targetRootNode.source == null || nodeSource.uniqueID == targetRootNode.source.uniqueID)
            {
                return new List<string>() { nodeSource.uniqueID };
            }

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
            if (mappingTarget == null)
            {
                return null;
            }
            //check if the mappingPath of the source corresponds with the path of the node
            var startNodeMappingPath = startNode.mappingPath;
            //source is OK if mapping corresponds, or no mappingPath found and it is not virtual
            var sourceOK = sourceMappingPath.SequenceEqual(startNodeMappingPath) || (!sourceMappingPath.Any() && !startNode.isVirtual);
            // if no targetMapping found then we try to build a Mapping up to the target root node source element
            if (!targetMappingPath.Any())
            {
                targetMappingPath = getMappingPath(mappingTarget, targetRootNode);
            }
            //target is OK if the first item of the targetMappignPath corresponds to the targetRootNode
            var targetOK = targetMappingPath.FirstOrDefault() == targetRootNode.source?.uniqueID;
            //if target or source is not OK then we return null
            if (!sourceOK || !targetOK)
            {
                return null;
            }
            //first create the targetMappingNode
            var targetMappingNode = targetRootNode.createMappingNode(targetMappingPath);
            // if the targetmappingNode was not created there is probably something wrong with the targetMappingMath
            // we try again with a fresh mapping path
            if (targetMappingNode == null)
            {
                targetMappingPath = getMappingPath(mappingTarget, targetRootNode);
                targetMappingNode = targetRootNode.createMappingNode(targetMappingPath);
            }
            // if still not targetMappingNode then we return null
            if (targetMappingNode == null)
            {
                return null;
            }
            //return the actual mapping
            return createMapping(mappingItem, startNode, targetMappingNode);
        }
        public static Mapping createMapping(UML.Extended.UMLItem mappingItem, MappingNode startNode, MappingNode targetNode)
        {
            var connector = mappingItem as ConnectorWrapper;
            if (connector != null)
            {
                return new ConnectorMapping(connector, startNode, targetNode);
            }

            var taggedValue = mappingItem as TaggedValue;
            if (taggedValue != null)
            {
                return new TaggedValueMapping(taggedValue, startNode, targetNode);
            }

            throw new ArgumentException("MappingItem should be Connector or TaggedValue");
        }

        public static MappingNode getMappingNode(UML.Classes.Kernel.NamedElement source, MappingNode rootNode)
        {
           var mappingPath = getMappingPath(source as Element, rootNode);
           return rootNode.createMappingNode(mappingPath);
        }

        public static MappingNode createMappingNode(UML.Classes.Kernel.NamedElement source, MappingNode parent, MappingSettings settings)
        {
            //check if there is a virtual owner
            UML.Classes.Kernel.NamedElement virtualOwner = null;
            if (source.owner.uniqueID != parent.source.uniqueID)
            {
                virtualOwner = source.owner as UML.Classes.Kernel.NamedElement;
            }

            //AttributeMappingNode
            var attributeSource = source as AttributeWrapper;
            if (attributeSource != null)
            {
                return new AttributeMappingNode(attributeSource, parent as ElementMappingNode, settings, parent.structure, virtualOwner, parent.isTarget);

            }

            //AssociationMappingNode
            var associationSource = source as Association;
            if (associationSource != null)
            {
                return new AssociationMappingNode(associationSource, parent as ElementMappingNode, settings, parent.structure, virtualOwner, parent.isTarget);
            }

            //ClassifierMappingNode
            var classifierSource = source as ElementWrapper;
            if (classifierSource != null)
            {
                return new ElementMappingNode(classifierSource, parent, settings, parent.structure, parent.isTarget);
            }

            //not a valid source type, return null
            return null;
        }



        /// <summary>
        /// import the mapings specified in the file into the given mappingSet
        /// </summary>
        /// <param name="mappingSet">the mappingset to import the mappings into</param>
        /// <param name="filePath">the path to the file containing the mappings</param>
        public static void importMappings(MappingSet mappingSet, string filePath, Model model)
        {
            IEnumerable<CSVMappingRecord> mappingRecords;
            //remove all existing mappings
            foreach (var mapping in mappingSet.mappings)
            {
                mapping.delete();
            }
            //map source and target to be sure
            mappingSet.source.mapTo(mappingSet.target);
            //make sure the target node tree has been build
            ((MappingNode)mappingSet.target).buildNodeTree();
            //read the csv file
            using (var textReader = new StreamReader(filePath))
            {
                var csv = new CSV.CsvReader(textReader, false);
                csv.Configuration.RegisterClassMap<CSVMappingRecordMap>();
                csv.Configuration.Delimiter = ";";
                mappingRecords = csv.GetRecords<CSVMappingRecord>();
                var sourceNodes = new Dictionary<string, MP.MappingNode>();
                var targetNodes = new Dictionary<string, MP.MappingNode>();
                //now loop the records
                foreach (var csvRecord in mappingRecords)
                {
                    if (string.IsNullOrEmpty(csvRecord.sourcePath)
                        || (string.IsNullOrEmpty(csvRecord.targetPath)
                            && string.IsNullOrEmpty(csvRecord.mappingLogic)))
                    {
                        //don't even bother if not both fields are filled in
                        continue;
                    }
                    //convert any newLines (\n") coming from excel (Alt-Enter) to "real" newlines
                    csvRecord.mappingLogic = csvRecord.mappingLogic.Replace("\n", Environment.NewLine);
                    //find the source
                    //first check if we already known the node
                    MP.MappingNode sourceNode = null;
                    if (!string.IsNullOrEmpty(csvRecord.sourcePath)
                        && !sourceNodes.TryGetValue(csvRecord.sourcePath, out sourceNode))
                    {
                        //find out if we know a parent node of this node
                        var parentNode = findParentNode(sourceNodes, csvRecord.sourcePath);
                        if (parentNode == null)
                        {
                            //no parent found, start at the top
                            parentNode = mappingSet.source;
                        }
                        //find the node from the parent
                        sourceNode = parentNode.findNode(csvRecord.sourcePath.Split('.').ToList());
                    }
                    if (sourceNode == null)
                    {
                        EAOutputLogger.log($"Could not find source element corresponding to '{csvRecord.sourcePath}'", 0, LogTypeEnum.warning);
                        //don't bother going any further
                        continue;
                    }
                    //find the target
                    MP.MappingNode targetNode = null;
                    //first check if we already known the node
                    if (!string.IsNullOrEmpty(csvRecord.targetPath)
                        && !targetNodes.TryGetValue(csvRecord.targetPath, out targetNode))
                    {
                        //find out if we know a parent node of this node
                        var parentNode = findParentNode(targetNodes, csvRecord.targetPath);
                        if (parentNode == null)
                        {
                            //no parent found, start at the top
                            parentNode = mappingSet.target;
                        }
                        //find the node from the parent
                        targetNode = parentNode.findNode(csvRecord.targetPath.Split('.').ToList());
                        if (targetNode == null)
                        {
                            EAOutputLogger.log($"Could not find target element corresponding to '{csvRecord.targetPath}'", 0, LogTypeEnum.warning);
                        }
                    }

                    //if we found both then we map them
                    if (sourceNode != null)
                    {
                        if (targetNode != null)
                        {
                            var newMapping = sourceNode.mapTo(targetNode);
                            newMapping.mappingLogics = createMappingLogicsFromCSVString(csvRecord.mappingLogic, mappingSet.EAContexts, model);
                            newMapping.save();
                            EAOutputLogger.log($"Mapping created from '{csvRecord.sourcePath}' to '{csvRecord.targetPath}'", 0);
                        }
                        else
                        {
                            var newMapping = sourceNode.createEmptyMapping(false);
                            newMapping.mappingLogics = createMappingLogicsFromCSVString(csvRecord.mappingLogic, mappingSet.EAContexts, model);
                            newMapping.save();
                            EAOutputLogger.log($"Empty mapping created for '{csvRecord.sourcePath}' ", 0);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// the CSVString has mapping logic in the form of
        /// --context1 name--
        /// --other context name--
        /// mapping logic
        /// --context2--
        /// mapping logic 2
        /// 
        /// Or just plain mapping logic. In that case the mapping is valid for all contexts
        /// </summary>
        /// <param name="mappingLogic">the string containing the mapping logic</param>
        /// <param name="contexts">the possible contexts for the mapping set</param>
        /// <param name="model">the model</param>
        /// <returns>a list of new mapping logics based on the given string</returns>
        private static IEnumerable<MP.MappingLogic> createMappingLogicsFromCSVString(string mappingLogic, List<ElementWrapper> contexts, Model model)
        {
            var mappingLogics = new List<MappingLogic>();
            //read the lines
            using (var reader = new StringReader(mappingLogic))
            {
                string line;
                var currentContexts = new List<ElementWrapper>();
                string logic = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("--") && line.EndsWith("--") && line.Length > 4)
                    {
                        //we have a new context
                        //add the logics for the previous contexts
                        if (!string.IsNullOrEmpty(logic) && currentContexts.Any())
                        {
                            addMappingLogics(mappingLogics, currentContexts, logic);
                            //clear the current contexts
                            currentContexts.Clear();
                            //clear the logic
                            logic = string.Empty;
                        }
                        //get the name of the context
                        var contexTextName = line.Substring(2, line.Length - 4);
                        //get the context from the list
                        var newContext = contexts.FirstOrDefault(x => x.name.Equals(contexTextName, StringComparison.InvariantCultureIgnoreCase));
                        if (newContext == null && contexTextName.Equals("Default", StringComparison.CurrentCultureIgnoreCase))
                        {
                            EAOutputLogger.log($"Could not find context with name '{contexTextName}'", 0, LogTypeEnum.warning);
                        }
                        //add to the current contexts
                        currentContexts.Add(newContext);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(logic))
                        {
                            logic += Environment.NewLine;
                        }
                        //add the logic
                        logic += line;
                    }
                }
                //make sure to add the logic
                addMappingLogics(mappingLogics, currentContexts, logic);
            }
            return mappingLogics;
        }
        
        private static void addMappingLogics(List<MappingLogic> mappingLogics, List<ElementWrapper> currentContexts, string mappingLogicString)
        {
            //don't bother if the logic string is empty
            if (string.IsNullOrEmpty(mappingLogicString))
            {
                return;
            }
            if ( currentContexts.Any())
            {
                foreach(var context in currentContexts)
                {
                    mappingLogics.Add(new MappingLogic(mappingLogicString, context));
                }
            }
            else
            {
                //add logic without context
                mappingLogics.Add(new MappingLogic(mappingLogicString));
            }

        }

        private static MP.MappingNode findParentNode(Dictionary<string, MP.MappingNode> nodes, string nodePath)
        {
            //check if exact match
            if (nodes.ContainsKey(nodePath))
            {
                return nodes[nodePath];
            }
            //remove one level and search again
            var nodePathNames = nodePath.Split('.').ToList();
            if (nodePathNames.Count >= 2)
            {
                nodePathNames.Remove(nodePathNames.Last());
                return findParentNode(nodes, string.Join(".", nodePathNames));
            }
            //not found
            return null;
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
                        mappingRecord.mappingLogic = createMappingLogicString(mapping);
                        //add the record to the list
                        csvMappingRecords.Add(mappingRecord);
                    }
                    //write the CSV mapping records to the filename
                    csv.WriteRecords(csvMappingRecords);
                }
            }
        }
        private static string createMappingLogicString (MP.Mapping mapping)
        {
            string mappingLogicString = string.Empty;
            foreach (var mappingLogic in mapping.mappingLogics)
            {
                if (! string.IsNullOrEmpty(mappingLogicString))
                {
                    mappingLogicString += Environment.NewLine;
                }
                if (mapping.mappingLogics.Any(x => x.context != null))
                {
                    var contextName = mappingLogic.context?.name ?? "Default";
                    mappingLogicString += $"--{contextName}--{Environment.NewLine}";
                }
                mappingLogicString += mappingLogic.description;
            }
            return mappingLogicString;
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
            return mappingElement != null ? new MappingLogic(mappingElement, null) : null; //return null if no mapping element found
        }
    }
}
