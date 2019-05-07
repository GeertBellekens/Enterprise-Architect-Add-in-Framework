using System.Linq;
using System;
using System.Collections.Generic;
using EAAddinFramework.Utilities;
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

		#region implemented abstract members of EASchemaPropertyWrapper


		protected override UTF_EA.Multiplicity sourceMultiplicity 
		{
			get 
			{
				return this.defaultMultiplicity;
			}
		}


		#endregion

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
		
		internal override UTF_EA.AttributeWrapper sourceAttributeWrapper 
		{ get { return sourceLiteral as UTF_EA.AttributeWrapper;}}
		internal override UTF_EA.AttributeWrapper subsetAttributeWrapper 
		{ get { return subSetLiteral as UTF_EA.AttributeWrapper;}}
		/// <summary>
		/// Checks if the attribute type is present as the source element of one of the schema elements
		/// If it finds a match the type is set to the subset elemnt of this schema element
		/// </summary>
		public void createSubsetLiteral()
		{
            //no need to do anything if the subset element does not exist
            if (this.owner.subsetElement == null || this.sourceLiteral == null)
            {
                return;
            }

            HashSet<SBF.SchemaElement> schemaElements = this.owner.owner.elements;
			if (this.subSetLiteral == null)
			{
				this.isNew = true;
				this.subSetLiteral = this.model.factory.createNewElement<UML.Classes.Kernel.EnumerationLiteral>(this.owner.subsetElement,this.sourceLiteral.name);
			}
			else
			{
				if (this.subSetLiteral.name != this.sourceLiteral.name)
				{
					EAOutputLogger.log(this.model,this.owner.owner.settings.outputName
                                              ,string.Format("Literal value '{0}' has been renamed from '{1}' since the last schema generation"
                                  					,this.sourceLiteral.owner.name + "." + this.sourceLiteral.name
                                  					,this.subSetLiteral.name)
                                              ,((UTF_EA.ElementWrapper)sourceLiteral.owner).id
                                              , LogTypeEnum.warning);
				}
			}
			this.subSetLiteral.name = this.sourceLiteral.name;
			this.subSetLiteral.stereotypes = this.sourceLiteral.stereotypes;
			//Set position for new items
			if (this.isNew) this.subSetLiteral.position = this.sourceLiteral.position;
			//alias (only if subset alias is empty)
			if (string.IsNullOrEmpty(((UTF_EA.EnumerationLiteral)subSetLiteral).alias))
				((UTF_EA.EnumerationLiteral)subSetLiteral).alias = ((UTF_EA.EnumerationLiteral)sourceLiteral).alias;
			//Check if the subset alias is different from the source alias and issue warning if that is the case
			if (!string.Equals(((UTF_EA.EnumerationLiteral)subSetLiteral).alias,((UTF_EA.EnumerationLiteral)sourceLiteral).alias))
			{
					EAOutputLogger.log(this.model,this.owner.owner.settings.outputName
                                              ,string.Format("Literal value '{0}' has alias '{1}' in the model and a different alias '{2}' in the subset"
                                  					,this.sourceLiteral.owner.name + "." + this.sourceLiteral.name
                                  					,((UTF_EA.EnumerationLiteral)subSetLiteral).alias
                                  					,((UTF_EA.EnumerationLiteral)sourceLiteral).alias)
                                              ,((UTF_EA.ElementWrapper)sourceLiteral.owner).id
                                              , LogTypeEnum.warning);				
			}
			//notes only update them if they are empty
			if (this.subSetLiteral.ownedComments.Count == 0 || ! this.subSetLiteral.ownedComments.Any(x => x.body.Length > 0)
                || this.owner.owner.settings.keepNotesInSync)
			{
				this.subSetLiteral.ownedComments = this.sourceLiteral.ownedComments;
				if (! this.owner.owner.settings.keepNotesInSync
                    && this.owner.owner.settings.prefixNotes
				    && this.owner.owner.settings.prefixNotesText.Length > 0
				    && this.subSetLiteral.ownedComments.Any(x => x.body.Length > 0))
				{
					foreach (var comment in subSetLiteral.ownedComments) 
					{
						comment.body = this.owner.owner.settings.prefixNotesText + Environment.NewLine + comment.body;
					}	
				}
			}
			((UTF_EA.Element)this.subSetLiteral).save();
            //copy tagged values
            ((EASchema)this.owner.owner).copyTaggedValues((UTF_EA.Element)this.sourceLiteral, (UTF_EA.Element)this.subSetLiteral);
            //add tagged value with reference to source literal value
            ((UTF_EA.Element)this.subSetLiteral).addTaggedValue(this.owner.owner.settings.sourceAttributeTagName,((UTF_EA.Element)this.sourceLiteral).guid);

		}
	}
}
