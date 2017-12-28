using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Description of AssociationClass.
	/// </summary>
	public class AssociationClass:Class, UML.Classes.AssociationClasses.AssociationClass
	{
		private Association _relatedAssociation;
		public AssociationClass(Model model, global::EA.Element elementToWrap)
      : base(model, elementToWrap){}
		
		public Association relatedAssociation
		{
			get
			{
				if (_relatedAssociation ==null)
				{
					_relatedAssociation = this.EAModel.getRelationByID(this.WrappedElement.AssociationClassConnectorID) as Association;
				}
				return _relatedAssociation;
			}
			set
			{
				this._relatedAssociation = value;
				if (this.WrappedElement.AssociationClassConnectorID != value.id)
				{
					this.wrappedElement.CreateAssociationClass(value.id);
				}
			}
		}
		#region Properties from Association
		public void addRelatedElement(UML.Classes.Kernel.Element relatedElement)
		{
			this.relatedAssociation.addRelatedElement(relatedElement);
		}
		public List<UML.Classes.Kernel.Element> relatedElements {
			get 
			{
				return this.relatedAssociation.relatedElements;
			}
			set 
			{
				this.relatedAssociation.relatedElements = value;
			}
		}
		public bool isDerived {
			get 
			{
				return this.relatedAssociation.isDerived;
			}
			set 
			{
				this.relatedAssociation.isDerived = value;
			}
		}
		public List<UML.Classes.Kernel.Property> navigableOwnedEnds 
		{
			get 
			{
				return this.relatedAssociation.navigableOwnedEnds;
			}
			set 
			{
				this.relatedAssociation.navigableOwnedEnds = value;
			}
		}
		public List<UML.Classes.Kernel.Property> ownedEnds {
			get 
			{
				return this.relatedAssociation.ownedEnds;
			}
			set 
			{
				this.relatedAssociation.ownedEnds = value;
			}
		}
		public List<UML.Classes.Kernel.Type> endTypes 
		{
			get 
			{
				return this.relatedAssociation.endTypes;
			}
			set 
			{
				this.relatedAssociation.endTypes = value;
			}
		}
		public List<UML.Classes.Kernel.Property> memberEnds {
			get 
			{
				return this.relatedAssociation.memberEnds;
			}
			set 
			{
				this.relatedAssociation.memberEnds = value;
			}
		}
		public override List<UML.Diagrams.Diagram> getDependentDiagrams()
		{
			return this.relatedAssociation.getDependentDiagrams();
		}
		#endregion
		
		public HashSet<UML.InfomationFlows.InformationFlow> getInformationFlows()
		{
			return this.relatedAssociation.getInformationFlows();
		}

		public List<UML.Classes.Kernel.Element> getLinkedElements()
		{
			return this.relatedAssociation.getLinkedElements();
		}

	}
}
