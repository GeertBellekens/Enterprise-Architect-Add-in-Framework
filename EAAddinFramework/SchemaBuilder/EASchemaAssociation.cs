using System;
using System.Collections.Generic;
using TSF.UmlToolingFramework.UML;
using SBF = SchemaBuilderFramework;
using UML = TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.SchemaBuilder
{
	/// <summary>
	/// Description of EASchemaAssociation.
	/// </summary>
	public class EASchemaAssociation : EASchemaPropertyWrapper, SBF.SchemaAssociation
	{

		public EASchemaAssociation(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap):base(model,owner, objectToWrap)
		{
		}
		
		public UML.Classes.Kernel.Association sourceAssociation {
			get 
			{
				return this.model.getRelationByGUID(this.wrappedProperty.GUID) as UML.Classes.Kernel.Association;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public List<SBF.SchemaElement> relatedElements {
			get 
			{
				List<SBF.SchemaElement> elements = new List<SchemaBuilderFramework.SchemaElement>();
				foreach (UML.Classes.Kernel.Element element in this.sourceAssociation.relatedElements)
				{
					SBF.SchemaElement relatedSchemaElement = ((EASchema)this.owner.owner).getSchemaElementForUMLElement(element);
					if (relatedSchemaElement != null)
					{
						elements.Add(relatedSchemaElement);
					}
				}
				return elements;
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
	}
}
