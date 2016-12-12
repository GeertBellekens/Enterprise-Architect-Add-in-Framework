using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of ConnectorMapping.
	/// </summary>
	public class ConnectorMapping:Mapping
	{
		internal ConnectorWrapper wrappedConnector{get;private set;}
		public ConnectorMapping(ConnectorWrapper wrappedConnector,string basePath):base(wrappedConnector.sourceElement,wrappedConnector.targetElement,basePath)
		{
			this.wrappedConnector = wrappedConnector;
		}
	}
}
