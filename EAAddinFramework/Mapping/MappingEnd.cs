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
			this.mappingPath = mappedElement.fqn;
		}
		#region MappingEnd implementation
		public TSF.UmlToolingFramework.UML.Classes.Kernel.Element mappedEnd  {get;set;}
		public virtual string mappingPath { get;set;}
	
		#endregion
		
	}
}
