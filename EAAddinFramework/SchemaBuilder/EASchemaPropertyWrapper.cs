
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
	public abstract class EASchemaPropertyWrapper
	{
		protected UTF_EA.Model model;
		protected EA.SchemaProperty wrappedProperty;
		protected EASchemaElement _owner;
		
		public EASchemaPropertyWrapper(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap)
		{
			this._owner = owner;
			this.model = model;
			this.wrappedProperty = objectToWrap;
		}
		
		public SBF.SchemaElement owner 
		{
			get 
			{
				return this._owner;
			}
			set 
			{
				this._owner = (EASchemaElement) value;
			}
		}

	}
}
