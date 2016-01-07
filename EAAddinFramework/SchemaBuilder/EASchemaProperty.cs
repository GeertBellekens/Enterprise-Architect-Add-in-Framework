
using System;
using System.Collections.Generic;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.SchemaBuilder
{
	/// <summary>
	/// Description of EASchemaProperty.
	/// </summary>
	public class EASchemaProperty: EASchemaPropertyWrapper, SBF.SchemaProperty
	{
		public EASchemaProperty(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap):base(model,owner, objectToWrap)
		{
		}
		public TSF.UmlToolingFramework.UML.Classes.Kernel.Property sourceProperty 
		{
			get 
			{
				return this.model.getAttributeByGUID(this.wrappedProperty.GUID);
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		

	}
}
