using System.Linq;
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
	public class EASchemaLiteral: EASchemaPropertyWrapper, SBF.SchemaLiteral
	{
		private UTF_EA.EnumerationLiteral _sourceLiteral;
		/// <summary>
		/// constructor. Nothing specific, just calling base constructor
		/// </summary>
		/// <param name="model">the model</param>
		/// <param name="owner">the owner Schema Element</param>
		/// <param name="objectToWrap">the EA.SchemaProperty object to wrap</param>
		public EASchemaLiteral(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap):base(model,owner, objectToWrap){}

		#region implemented abstract members of EASchemaPropertyWrapper
		protected override UTF_EA.Multiplicity defaultMultiplicity 
		{
			get 
			{
				return new UTF_EA.Multiplicity("1..1");
			}
		}
		#endregion		
		/// <summary>
		/// The property int he model where this Schema property was derived from
		/// </summary>
		public UML.Classes.Kernel.EnumerationLiteral sourceLiteral 
		{
			get 
			{
				if (this._sourceLiteral == null)
				{
					this._sourceLiteral = this.model.getAttributeWrapperByGUID(this.wrappedProperty.GUID) as UTF_EA.EnumerationLiteral;
				}
				return this._sourceLiteral;
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		/// <summary>
		/// the property in the subset model generated from this property
		/// </summary>
		public UML.Classes.Kernel.EnumerationLiteral subSetLiteral {get;set;}

		/// <summary>
		/// Checks if the attribute type is present as the source element of one of the schema elements
		/// If it finds a match the type is set to the subset elemnt of this schema element
		/// </summary>
		public void createSubsetLiteral()
		{
			HashSet<SBF.SchemaElement> schemaElements = this.owner.owner.elements;
			if (this.subSetLiteral == null)
			{
				this.subSetLiteral = this.model.factory.createNewElement<UML.Classes.Kernel.EnumerationLiteral>(this.owner.subsetElement,this.sourceLiteral.name);
			}
			this.subSetLiteral.stereotypes = this.sourceLiteral.stereotypes;
			this.subSetLiteral.ownedComments = this.sourceLiteral.ownedComments;

			((UTF_EA.Element)this.subSetLiteral).save();
			//copy tagged values
			((UTF_EA.Element)this.subSetLiteral).copyTaggedValues((UTF_EA.Element)this.sourceLiteral);
			//add tagged value with reference to source literal value
			((UTF_EA.Element)this.subSetLiteral).addTaggedValue(EASchemaBuilderFactory.sourceAttributeTagName,((UTF_EA.Element)this.sourceLiteral).guid);

		}
	}
}
