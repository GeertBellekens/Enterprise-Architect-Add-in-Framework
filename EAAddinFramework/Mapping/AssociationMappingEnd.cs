using System;
using MP = MappingFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of AssociationMappingEnd.
	/// </summary>
	public class AssociationMappingEnd:MappingEnd, MP.AssociationMappingEnd
	{
		internal Association _mappedAssociation;
		public AssociationMappingEnd(Association association, string path):base(association,path)
		{}

		#region AssociationMappingEnd implementation

		public UML.Classes.Kernel.Association mappedAssociation {
			get 
			{
				return _mappedAssociation;
			}
			set 
			{
				_mappedAssociation = (Association)value;
			}
		}

		#endregion

		#region MappingEnd implementation

		public override UML.Classes.Kernel.Element mappedEnd {
			get 
			{
				return _mappedAssociation;
			}
			set 
			{
				_mappedAssociation = (Association)value;
			}
		}

		#endregion
	}
}
