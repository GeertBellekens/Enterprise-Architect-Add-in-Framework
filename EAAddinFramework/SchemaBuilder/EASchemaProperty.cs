
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
		/// <summary>
		/// constructor. Nothing specific, just calling base constructor
		/// </summary>
		/// <param name="model">the model</param>
		/// <param name="owner">the owner Schema Element</param>
		/// <param name="objectToWrap">the EA.SchemaProperty object to wrap</param>
		public EASchemaProperty(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap):base(model,owner, objectToWrap){}
		
		/// <summary>
		/// The property int he model where this Schema property was derived from
		/// </summary>
		public UML.Classes.Kernel.Property sourceProperty 
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
		/// <summary>
		/// the property in the subset model generated from this property
		/// </summary>
		public UML.Classes.Kernel.Property subSetProperty {get;set;}

		/// <summary>
		/// Creates the subset property in the subsetElement of the owning SchemaElement
		/// </summary>
		/// <returns>the new property</returns>
		public UML.Classes.Kernel.Property createSubsetProperty()
		{
			this.subSetProperty = this.model.factory.createNewElement<UML.Classes.Kernel.Property>(this.owner.subsetElement,this.sourceProperty.name);
			this.subSetProperty.type = this.sourceProperty.type;
			this.subSetProperty.stereotypes = this.sourceProperty.stereotypes;			
			((UTF_EA.Element) this.subSetProperty).save();
			//copy tagged values
			((UTF_EA.Element) this.subSetProperty).copyTaggedValues((UTF_EA.Element)this.sourceProperty);
			return this.subSetProperty;
		}
	}
}
