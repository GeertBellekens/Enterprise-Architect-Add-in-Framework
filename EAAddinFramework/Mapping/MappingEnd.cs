using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingEnd.
	/// </summary>
	public  class MappingEnd:MP.MappingEnd
	{
		
		public MappingEnd(Element mappedElement,string path)
		{
			this.mappedEnd = mappedElement;
			this.mappingPath = path;
		}
		public MappingEnd(Element mappedElement)
		{
			this.mappedEnd = mappedElement;
			this.mappingPath = strippedFQN();
		}
		public MappingEnd(Element mappedElement,ElementWrapper rootElement)
		{
			this.mappedEnd = mappedElement;
			this.mappingPath = strippedFQN();
			if (rootElement != null && mappingPath.StartsWith(rootElement.fqn))
			{
				this.mappingPath = mappingPath.Remove(0,rootElement.fqn.Length - rootElement.name.Length);
			}
		}
		#region MappingEnd implementation
		public TSF.UmlToolingFramework.UML.Classes.Kernel.Element mappedEnd  {get;set;}
		public virtual string mappingPath { get;set;}
		public virtual string fullMappingPath 
		{
			get
			{
				return this.mappingPath + "." + this.mappedEnd.name;
			}
		}
		private string strippedFQN()
		{
			if (this.mappedEnd != null)
			{
				var fullFQN = mappedEnd.fqn;
				if (fullFQN.Length > mappedEnd.name.Length +1)
				{
					return fullFQN.Substring(0,fullFQN.Length - mappedEnd.name.Length -1);
				}
				return fullFQN;
			}
			return string.Empty;
		}
	
		#endregion
		
	}
}
