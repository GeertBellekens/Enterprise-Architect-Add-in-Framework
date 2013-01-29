/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 04.04.2012
 * Time: 20:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using EA;

namespace EAAddinFramework.Utilities
{
	/// <summary>
	/// Description of EventPropertiesHelper.
	/// </summary>
	public class EventPropertiesHelper
	{
		EA.Repository repository;
		EA.EventProperties eventProperties;
		
		public const String PackageID = "PackageID";
		public const String ElementID = "ElementID";
		public const String AttributeID = "AttributeID";
		public const String MethodID = "MethodID";
		public const String ConnectorID = "ConnectorID";
				
		public EventPropertiesHelper(EA.Repository repository, EA.EventProperties eventProperties)
		{
			this.repository = repository;
			this.eventProperties = eventProperties;
		}
		
		public bool GetPackage(out EA.Package package)
		{
			package = null;
			int packageId;
			if(GetEAObjectId(PackageID,out packageId))
			{
				package = repository.GetPackageByID(packageId);
				return package != null;
			}
			return false;
		}

		public bool GetElement(out EA.Element element)
		{
			element = null;
			int elementId;
			if(GetEAObjectId(ElementID,out elementId))
			{
				element = repository.GetElementByID(elementId);
				return element != null;
			}
			return false;
		}
		
		public bool GetAttribute(out EA.Attribute attribute)
		{
			attribute = null;
			int attributeId;
			if(GetEAObjectId(AttributeID,out attributeId))
			{
				attribute = repository.GetAttributeByID(attributeId);
				return attribute != null;
			}
			return false;
		}
		
		public bool GetMethod(out EA.Method method)
		{
			method = null;
			int methodId;
			if(GetEAObjectId(MethodID,out methodId))
			{
				method = repository.GetMethodByID(methodId);
				return method != null;
			}
			return false;
		}
		
		public bool GetConnector(out EA.Connector connector)
		{
			connector = null;
			int connectorId;
			if(GetEAObjectId(ConnectorID,out connectorId))
			{
				connector = repository.GetConnectorByID(connectorId);
				return connector != null;
			}
			return false;
		}
		
		public bool GetEAObjectId(String idKey, out int id)
		{
			id = 0;
			return int.TryParse(eventProperties.Get(idKey).Value.ToString(), out id);
		}
	}
}
