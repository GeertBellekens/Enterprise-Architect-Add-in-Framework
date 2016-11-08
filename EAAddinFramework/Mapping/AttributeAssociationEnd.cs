using System;
using MP = MappingFramework;
using TSF_EA = TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;


namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of AttributeAssociationEnd.
	/// </summary>
	public class AttributeAssociationEnd:MappingEnd,MP.AttributeMappingEnd
	{
		internal TSF_EA.Attribute _mappedAttribute;
		public AttributeAssociationEnd(TSF_EA.Attribute mappedAttribute,string path):base(mappedAttribute,path)
		{
		}

		public UML.Classes.Kernel.Property mappedAttribute {
			get {
				return _mappedAttribute;
			}
			set {
				_mappedAttribute = (TSF_EA.Attribute)value;
			}
		}

		#region implemented abstract members of MappingEnd

		public override UML.Classes.Kernel.Element mappedEnd {
			get {
				return _mappedAttribute;
			}
			set {
				_mappedAttribute = (TSF_EA.Attribute)value;
			}
		}

		#endregion
	}
}
