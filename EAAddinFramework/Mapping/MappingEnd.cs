using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of MappingEnd.
	/// </summary>
	public abstract class MappingEnd:MP.MappingEnd
	{
		
		protected MappingEnd(Element mappedElement,string path)
		{
			this.mappedEnd = mappedElement;
			this.ownerPath = path;
		}
		#region MappingEnd implementation
		public abstract TSF.UmlToolingFramework.UML.Classes.Kernel.Element mappedEnd  {get;set;}
		public virtual string ownerPath { get;set;}
	
		#endregion
		
	}
}
