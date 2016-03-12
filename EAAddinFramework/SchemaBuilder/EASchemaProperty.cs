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
	public class EASchemaProperty: EASchemaPropertyWrapper, SBF.SchemaProperty
	{
		private UTF_EA.Attribute _sourceProperty;
		/// <summary>
		/// constructor. Nothing specific, just calling base constructor
		/// </summary>
		/// <param name="model">the model</param>
		/// <param name="owner">the owner Schema Element</param>
		/// <param name="objectToWrap">the EA.SchemaProperty object to wrap</param>
		public EASchemaProperty(UTF_EA.Model model,EASchemaElement owner, EA.SchemaProperty objectToWrap):base(model,owner, objectToWrap){}

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
		public UML.Classes.Kernel.Property sourceProperty 
		{
			get 
			{
				if (this._sourceProperty == null)
				{
					this._sourceProperty = this.model.getAttributeWrapperByGUID(this.wrappedProperty.GUID) as UTF_EA.Attribute;
				}
				return this._sourceProperty;
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
		/// Checks if the attribute type is present as the source element of one of the schema elements
		/// If it finds a match the type is set to the subset elemnt of this schema element
		/// </summary>
		public void createSubsetProperty()
		{
			HashSet<SBF.SchemaElement> schemaElements = this.owner.owner.elements;
			if (this.subSetProperty == null)
			{
				this.subSetProperty = this.model.factory.createNewElement<UML.Classes.Kernel.Property>(this.owner.subsetElement,this.sourceProperty.name);
			}
			this.subSetProperty.type = this.sourceProperty.type;
			this.subSetProperty.stereotypes = this.sourceProperty.stereotypes;
			((UTF_EA.Attribute)this.subSetProperty).multiplicity = this.multiplicity;
			//notes only update them if they are empty
			if (this.subSetProperty.ownedComments.Count == 0 || ! this.subSetProperty.ownedComments.Any(x => x.body.Length > 0))
			{
				this.subSetProperty.ownedComments = this.sourceProperty.ownedComments;
			}
			//resolve the type
			foreach (EASchemaElement element in schemaElements)
			{
				if (element.sourceElement != null && 
				    element.subsetElement != null &&
				    this.subSetProperty != null &&
				    element.sourceElement.Equals(this.subSetProperty.type))
				{
					//replace the type if it matches the source element
					this.subSetProperty.type = element.subsetElement;
				}
			}
			((UTF_EA.Element) this.subSetProperty).save();
			//copy tagged values
			((UTF_EA.Element) this.subSetProperty).copyTaggedValues((UTF_EA.Element)this.sourceProperty);
			//add tagged value with reference to source association
			((UTF_EA.Element)this.subSetProperty).addTaggedValue(EASchemaBuilderFactory.sourceAttributeTagName,((UTF_EA.Element)this.sourceProperty).guid);

		}
		/// <summary>
		/// adds a dependency from the attributes owner to the type of the attributes
		/// </summary>
		public void addAttributeTypeDependency()
		{
			if (this.owner.subsetElement != null 
			    && this.subSetProperty != null 
			    && this.subSetProperty.type != null 
			    && !(this.subSetProperty.type is UML.Classes.Kernel.PrimitiveType))
			{
				//add a trace relation from the subset element to the source element
				//check if trace already exists?
				var dependency = this.owner.subsetElement.getRelationships<UTF_EA.Dependency>()
							.FirstOrDefault(x => this.subSetProperty.type.Equals(x.supplier) && x.name == this.subSetProperty.name) ;
				if (dependency == null)
				{
					dependency = this.model.factory.createNewElement<UTF_EA.Dependency>(this.owner.subsetElement,this.subSetProperty.name);
					dependency.addRelatedElement(this.subSetProperty.type);
					dependency.save();
				}
			}
		}
	}
}
