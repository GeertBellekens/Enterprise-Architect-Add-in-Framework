using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingEnd.
	/// </summary>
	public  class MappingEnd:MappingNode, MP.MappingEnd
	{
		
		public MappingEnd(Element mappedElement,string path):base((UML.Classes.Kernel.NamedElement)mappedElement, null)
		{
            //TODO: find the parent based on the path. We will need the the top mapping model element somehow
			this.mappingPath = path;
		}
		public MappingEnd(Element mappedElement) : base((UML.Classes.Kernel.NamedElement)mappedElement, null)
        {
            //TODO
			this.mappingPath = strippedFQN();
		}
		public MappingEnd(Element mappedElement,ElementWrapper rootElement): base((UML.Classes.Kernel.NamedElement) mappedElement, null)
		{
            //TODO
			this.mappingPath = strippedFQN();
			if (rootElement != null && mappingPath.StartsWith(rootElement.fqn))
			{
				this.mappingPath = mappingPath.Remove(0,rootElement.fqn.Length - rootElement.name.Length);
			}
		}
		#region MappingEnd implementation
		public TSF.UmlToolingFramework.UML.Classes.Kernel.Element source  {get;set;}
		public virtual string mappingPath { get;set;}
		public virtual string fullMappingPath 
		{
			get
			{
				return this.mappingPath + "." + this.source.name;
			}
		}
		private string strippedFQN()
		{
			if (this.source != null)
			{
				var fullFQN = source.fqn;
				if (fullFQN.Length > source.name.Length +1)
				{
					return fullFQN.Substring(0,fullFQN.Length - source.name.Length -1);
				}
				return fullFQN;
			}
			return string.Empty;
		}
	
		#endregion
		
	}
}
