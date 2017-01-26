using System.Collections.Generic;
using System.Linq;
using System;
using EAAddinFramework.Utilities;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using FileHelpers;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingFactory.
	/// </summary>
	public static class MappingFactory
	{
		const string mappingSourcePathName = "mappingSourcePath";
		const string mappingTargetPathName = "mappingTargetPath";
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
						connectorMapping = new ConnectorMapping(mappedConnector,basepath,targetBasePath);
					}
					else
					{
						connectorMapping = new ConnectorMapping(mappedConnector,basepath,targetRootElement);
					}
					returnedMappings.Add(connectorMapping);
				}
			}
			//tagged value references from owned attributes	
			foreach (TaggedValue mappedTaggedValue in attribute.taggedValues.Where(x => x.tagValue is Element) )
			{
				string mappingSourcePath = getValueForKey(mappingSourcePathName,mappedTaggedValue.comment);
				string targetBasePath = getValueForKey(mappingTargetPathName,mappedTaggedValue.comment);

				//if not filled in or corresponds to the attributeBasePath
				if (string.IsNullOrEmpty(mappingSourcePath) || mappingSourcePath == basepath)
				{
					TaggedValueMapping tagMapping;
					if (! string.IsNullOrEmpty(targetBasePath))
					{
						tagMapping = new TaggedValueMapping(mappedTaggedValue,basepath,targetBasePath);
					}
					else
					{
						tagMapping = new TaggedValueMapping(mappedTaggedValue,basepath,targetRootElement);
					}	
					returnedMappings.Add(tagMapping);
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
				var connectorMapping = new ConnectorMapping(mappedConnector,connectorPath,targetRootElement);
				returnedMappings.Add(connectorMapping);
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
		/// parses the the given source string for any keyvalue pairs and returns the value corresponding to the given key.
		/// The format of the soure string is assumed as follows: key1=value1;key2=value2,...
		/// </summary>
		/// <param name="key">the key to searche for</param>
		/// <param name = "source">the source string to search in</param>
		/// <returns>the value correspondign to the given key</returns>
		public static string getValueForKey(string key,string source)
		{
			//first split the string into keyvalue pairs
			foreach (var keyValuePair in source.Split(';')) 
			{
				//then split the key and value
				var keyValues = keyValuePair.Split('=');
				if (keyValues.Count() >= 2 && keyValues[0] == key)
				{
					return keyValues[1];
				}
			}
			//if nothing found then return null;
			return null;
		}
		/// <summary>
		/// create a mappingSet based on the data in the CSV file
		/// </summary>
		/// <param name="model">the model that contains the elements</param>
		/// <param name="filePath">the path to the CSV file</param>
		/// <returns>a mapping set representing the mapping in the file</returns>
		public static MappingSet createMappingSet(Model model, string filePath,MappingSettings settings, Element sourceRootElement = null, Element targetRootElement = null)
		{
			MappingSet newMappingSet = null;
			var engine = new FileHelperEngine<CSVMappingRecord>();
			var parsedFile = engine.ReadFile(filePath);
			int i = 0;
			foreach (CSVMappingRecord mappingRecord in parsedFile) 
			{
				i++;
				//get the proper descriptors for source and target
				string sourceDescriptor = mappingRecord.source;
				if (! string.IsNullOrEmpty(mappingRecord.sourcePath)) sourceDescriptor = mappingRecord.sourcePath + "." + mappingRecord.source;
				string targetDescriptor = mappingRecord.target;
				if (! string.IsNullOrEmpty(mappingRecord.targetPath)) targetDescriptor = mappingRecord.targetPath + "." + mappingRecord.target;
				
				//find source
				var source = findElement(model, sourceDescriptor, sourceRootElement);
				//find target
				var target = findElement(model, targetDescriptor,targetRootElement);
				if (source == null )
				{
					EAOutputLogger.log(model,settings.outputName
					                   ,string.Format("Could not find element that matches: '{0}'",sourceDescriptor)
					                   ,0,LogTypeEnum.error);
				}
				else if( target == null)
				{
					EAOutputLogger.log(model,settings.outputName
					                   ,string.Format("Could not find element that matches: '{0}'",targetDescriptor)
					                   ,0,LogTypeEnum.error);
				}
				else
				{
					Package rootPackage = null;
					//first check if the mappingSet is already created
					if (newMappingSet == null)
					{
						//determine if this should be a PackageMappingSet or an ElementMappingSet
						if (sourceRootElement is Package)
						{
							rootPackage = sourceRootElement as Package;
							newMappingSet = new PackageMappingSet(sourceRootElement as Package,true);
						}
						else if (sourceRootElement is ElementWrapper)
						{
							rootPackage = sourceRootElement.owningPackage as Package;
							newMappingSet = new ElementMappingSet(sourceRootElement as ElementWrapper,true);
						}
						else
						{
							rootPackage = source.owningPackage as Package;
							newMappingSet = new PackageMappingSet((Package)source.owningPackage,true);
						}
						
					}
					MappingLogic newMappingLogic = null;
					//check if there is any mapping logic
					if (! string.IsNullOrEmpty(mappingRecord.mappingLogic))
					{
						if (settings.useInlineMappingLogic)
						{
							newMappingLogic = new MappingLogic(mappingRecord.mappingLogic);
						}
						else
						{
							//determine where the mapping logic element should be stored
							//TODO check for existing mapping logic elements
							//TODO dynamically create based on the type string in the settings
							 var mappingElement = model.factory.createNewElement<Activity>(rootPackage, "mapping logic " + i);
							 mappingElement.notes = mappingRecord.mappingLogic;
							 mappingElement.save();
							 //create the mappingLogic
							 newMappingLogic = new MappingLogic(mappingElement);
							 
						}
					}
					Mapping newMapping = null;
					//create the new mapping
					if (settings.useTaggedValues)
					{
						//TODO newMapping = new TaggedValueMapping():
					}
					else
					{
						//TODO newMapping = new ConnectorMapping();
					}
					if (newMappingLogic != null) newMapping.mappingLogic = newMappingLogic;
					newMapping.save();
					//TODO newMappingSet.addMapping();
					
				}
			}
			return newMappingSet;
		}
		public static Element findElement(Model model, string elementDescriptor, Element rootElement)
		{
			Element foundElement = null;
			if (rootElement == null)
			{
				//the elementDescriptor must be a fully qualified name
				foundElement = model.getItemFromFQN(elementDescriptor) as Element;
			}
			else
			{
				foundElement = rootElement.findOwnedItems(elementDescriptor).OfType<Element>().FirstOrDefault();
			}
			return foundElement;
				
		}
	}
}
