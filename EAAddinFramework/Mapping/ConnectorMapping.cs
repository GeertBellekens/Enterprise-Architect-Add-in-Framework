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
		public ConnectorMapping(ConnectorWrapper wrappedConnector,string basePath,ElementWrapper targetRootElement):base(wrappedConnector.sourceElement,wrappedConnector.targetElement,basePath,targetRootElement)
		{
			this.wrappedConnector = wrappedConnector;
		}
		public ConnectorMapping(ConnectorWrapper wrappedConnector,string basePath,string targetBasePath):base(wrappedConnector.sourceElement,wrappedConnector.targetElement,basePath,targetBasePath)
		{
			this.wrappedConnector = wrappedConnector;
		}


		#region implemented abstract members of Mapping

		public override MP.MappingLogic mappingLogic {
			get 
			{
				if (_mappingLogic == null)
				{
					//check links to notes or constraints
					//TODO: make a difference between regular notes and mapping logic?
					var connectedElement = this.wrappedConnector.getLinkedElements().OfType<ElementWrapper>().FirstOrDefault(x => x.notes.Length > 0);
					if (connectedElement != null) _mappingLogic = new MappingLogic(connectedElement);
				}
				return _mappingLogic;
			}
			set 
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region implemented abstract members of Mapping

		public override void save()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
