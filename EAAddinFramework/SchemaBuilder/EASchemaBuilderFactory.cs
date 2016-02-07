using System;
using System.Collections.Generic;
using SBF=SchemaBuilderFramework;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.SchemaBuilder
{
	/// <summary>
	/// Description of EASchemaBuilderFactory.
	/// </summary>
	public class EASchemaBuilderFactory:SBF.SchemaBuilderFactory
	{
		public const string sourceAttributeTagName = "sourceAttribute";
        public const string sourceAssociationTagName = "sourceAssociation";
        
		private UTF_EA.Model EAModel {get {return (UTF_EA.Model)this.model;}}
		private EASchema currentSchema;
		
		/// returns the singleton instance for the given model.
	    public static new EASchemaBuilderFactory getInstance(UML.UMLModel model){
	      EASchemaBuilderFactory factory = SBF.SchemaBuilderFactory.getInstance(model) as EASchemaBuilderFactory;
	      if( factory == null ) {
	      	factory = new EASchemaBuilderFactory((UTF_EA.Model)model);
	      }
	      return factory;
	    }
		
		/// <summary>
		/// in order for the relations to work we need tagged value types "sourceAttribute and "sourceAssociation".
		/// If they don't exists we add them
		/// </summary>
		/// <param name="model">the model</param>
		public void checkTaggedValueTypes()
		{
			if (this.EAModel != null)
			{
				if (!this.EAModel.taggedValueTypeExists(sourceAttributeTagName))
				{
					string sourceAttributeTagDetail = 
	@"Type=RefGUID;
	Values=Attribute;
	AppliesTo=Attribute;";
						this.EAModel.addTaggedValueType(sourceAttributeTagName,"is derived from this Attribute",sourceAttributeTagDetail);
				}
				if (!this.EAModel.taggedValueTypeExists(sourceAssociationTagName))
				{
					string sourceAssociationTagDetail = 
	@"Type=String;
	AppliesTo=Association,Aggregation;";
						this.EAModel.addTaggedValueType(sourceAssociationTagName,"is derived from this Association",sourceAssociationTagDetail);
				}
			}
		}
		
	    /// returns the singleton instance for a new model
	    public static new EASchemaBuilderFactory getInstance()
	    {
	    	return SBF.SchemaBuilderFactory.getInstance() as EASchemaBuilderFactory;
	    }
	    
		protected EASchemaBuilderFactory (UTF_EA.Model model):base(model)
		{
			
		}
		
		public override SBF.Schema createSchema(object objectToWrap)
		{
			this.currentSchema = new EASchema( this.EAModel,(EA.SchemaComposer) objectToWrap);
			return this.currentSchema;
		}
		
		public EASchemaPropertyWrapper createSchemaPropertyWrapper(SBF.SchemaElement owner, EA.SchemaProperty objectToWrap)
		{
			if (objectToWrap.UMLType == "Attribute")
			{
				var sourceObject = this.EAModel.getElementByGUID(objectToWrap.GUID);
				if (sourceObject is UTF_EA.EnumerationLiteral)
				{
					return (EASchemaLiteral) this.createSchemaLiteral(owner, objectToWrap);
				}
				else
				{
					return (EASchemaProperty) this.createSchemaProperty(owner, objectToWrap);
				}
					
			}
			else
			{
				return (EASchemaAssociation) this.createSchemaAssociation(owner, objectToWrap);
			}
		}
		public override SBF.SchemaProperty createSchemaProperty(SBF.SchemaElement owner, object objectToWrap)
		{
			return new EASchemaProperty(this.EAModel, (EASchemaElement)owner, (EA.SchemaProperty)objectToWrap);
		}
		
		public override SBF.SchemaElement createSchemaElement(SBF.Schema owner, object objectToWrap)
		{
			return new EASchemaElement(this.EAModel,(EASchema) owner, (EA.SchemaType)objectToWrap);
		}
		
		public override SBF.SchemaAssociation createSchemaAssociation(SBF.SchemaElement owner, object objectToWrap)
		{
			return new EASchemaAssociation(this.EAModel,(EASchemaElement) owner, (EA.SchemaProperty) objectToWrap);
		}
		
		public override SBF.SchemaLiteral createSchemaLiteral(SBF.SchemaElement owner, object objectToWrap)
		{
			return new EASchemaLiteral(this.EAModel,(EASchemaElement) owner, (EA.SchemaProperty) objectToWrap);
		}
		
	}
}
