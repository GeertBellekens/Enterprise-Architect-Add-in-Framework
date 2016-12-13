using System.Collections.Generic;
using System.Linq;
using System;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingFactory.
	/// </summary>
	public static class MappingFactory
	{
		public static List<Mapping> createNewMappings(TSF.UmlToolingFramework.Wrappers.EA.Attribute attribute,string basepath)
		{
			List<Mapping> returnedMappings = new List<Mapping>();
			string attributeBasePath = basepath + "." + attribute.name;
			//connectors from owned attributes
			foreach (ConnectorWrapper mappedConnector in attribute.relationships.OfType<ConnectorWrapper>())
			{
				if (! mappedConnector.taggedValues.Any( x => x.name == "mappingPath" && x.tagValue.ToString() != attributeBasePath))
				{
					var connectorMapping = new ConnectorMapping(mappedConnector,attributeBasePath);
					returnedMappings.Add(connectorMapping);
				}
			}
			//tagged value references from owned attributes	
			foreach (TaggedValue mappedTaggedValue in attribute.taggedValues.Where(x => x.tagValue is Element) )
			{
				string mappingPath = string.Empty;
				var tvKeyValues = mappedTaggedValue.comment.Split('=');
				if (tvKeyValues.Count() >= 2 && tvKeyValues[0] == "mappingPath")
				{
					mappingPath = tvKeyValues[1];

				}
				//if not filled in or corrsponds to the attributeBasePath
				if (string.IsNullOrEmpty(mappingPath) || mappingPath == attributeBasePath)
				{
					var tagMapping = new TaggedValueMapping(mappedTaggedValue,attributeBasePath);
					returnedMappings.Add(tagMapping);
				}
			}
			//add the mappings for the type of the attribute
			var attributeType = attribute.type as ElementWrapper;
			if (attributeType != null) returnedMappings.AddRange(createOwnedMappings(attributeType,attributeBasePath));
			return returnedMappings;
		}
		public static List<Mapping> createOwnedMappings(ElementWrapper ownerElement,string basepath,ElementWrapper targetRootElement)
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
				returnedMappings.AddRange(createNewMappings(ownedAttribute,basepath));
			}
			return returnedMappings;
		}
		public static List<Mapping> createOwnedMappings(ElementWrapper ownerElement,string basepath)
		{
			return createOwnedMappings( ownerElement, basepath,null);
		}
		public static List<Mapping> createRootMappings(ElementWrapper root,string basePath)
		{
			//get the owned mappings
			var returnedMappings =  MappingFactory.createOwnedMappings(root,basePath);
			//get the mappings of all associated elements
			foreach (var assocation in root.relationships
			         					.OfType<Association>()
			         					.Where (x => x.source.Equals(root))) 
			{
				var targetElement = assocation.target as ElementWrapper;
				if (targetElement != null) returnedMappings.AddRange(createRootMappings(targetElement,basePath + "." + getConnectorString(assocation)));
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
	}
}
