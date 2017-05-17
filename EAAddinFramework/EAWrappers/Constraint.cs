using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using EAAddinFramework.Utilities;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of Constraint.
	/// </summary>
	public class Constraint:Element, UML.Classes.Kernel.Constraint
	{
		internal global::EA.Constraint wrappedConstraint {get;set;}
		private Element _owner;
		private OpaqueExpression _opaqueExpression;
		private ElementWrapper elementWrapperOwner 
		{
			get
			{
				return this.owner as ElementWrapper;
			}
		}
		public Constraint(Model model, global::EA.Constraint wrappedConstraint):base(model)
		{
			this.wrappedConstraint = wrappedConstraint;
		}

		#region implemented abstract members of Element
		internal override void saveElement()
		{
			this.save();
		}
		public override void deleteOwnedElement(Element ownedElement)
		{
			throw new NotImplementedException();
		}
		public override string getLockedUser()
		{
			return ((Element)this.owner).getLockedUser();
		}
		public override string getLockedUserID()
		{
			return ((Element)this.owner).getLockedUserID();
		}
		internal override global::EA.Collection eaTaggedValuesCollection 
		{
			get 
			{
				throw new NotImplementedException();
			}
		}
		public override string notes 
		{
			get 
			{
				return this._opaqueExpression.bodies.FirstOrDefault();
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		public override string guid 
		{
			get 
			{
				return this.uniqueID;
			}
		}
		#endregion
		#region Element implementation

		public override void addStereotype(UML.Profiles.Stereotype stereotype)
		{
			throw new NotImplementedException();
		}

		public override List<T> getRelationships<T>() 
		{
			throw new NotImplementedException();
		}

		public override HashSet<T> getUsingDiagrams<T>()
		{
			throw new NotImplementedException();
		}

		public override UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
		{
			throw new NotImplementedException();
		}

		public override HashSet<UML.Profiles.TaggedValue> getReferencingTaggedValues()
		{
			throw new NotImplementedException();
		}

		public override HashSet<UML.Classes.Kernel.Element> ownedElements {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public override HashSet<UML.Classes.Kernel.Comment> ownedComments 
		{
			get 
			{
				throw new NotImplementedException();
			}
			set 
			{
				throw new NotImplementedException();
			}
		}

		public override UML.Classes.Kernel.Element owner {
			get 
			{
				if (_owner == null)
				{
					_owner = this.model.getElementWrapperByID(this.wrappedConstraint.ParentID);
				}
				return _owner;
			}
			set 
			{
				_owner = value as Element;
			}
		}

		public override HashSet<UML.Profiles.Stereotype> stereotypes {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public override List<UML.Classes.Kernel.Relationship> relationships {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public override HashSet<UML.Profiles.TaggedValue> taggedValues {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public override UML.Diagrams.Diagram compositeDiagram {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public override int position {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		#endregion

		#region UMLItem implementation

		public override void select()
		{
			this.owner.select();
		}

		public override void open()
		{
			this.owner.open();
		}

		public override void openProperties()
		{
			this.owner.openProperties();
		}

		public override void addToCurrentDiagram()
		{
			//do nothing, cannot be added to a diagram
		}

		public override void selectInCurrentDiagram()
		{
			//do nothing, cannot be added to a diagram
		}

		public override void delete()
		{
			for (int i = this.elementWrapperOwner.wrappedElement.Constraints.Count -1; i >= 0; i--)
			{
				var currentConstraint = (global::EA.Constraint)elementWrapperOwner.wrappedElement.Constraints.GetAt(short.Parse(i.ToString()));
				if (this.name == currentConstraint.Name)
				{
					elementWrapperOwner.wrappedElement.Constraints.DeleteAt(short.Parse(i.ToString()),false);
				}
			}
		}

		public override void save()
		{
			this.wrappedConstraint.Update();
		}

		public override List<UML.Diagrams.Diagram> getDependentDiagrams()
		{
			return new List<UML.Diagrams.Diagram>();
		}

		public override bool makeWritable(bool overrideLocks)
		{
			return this.owner.makeWritable(overrideLocks);
		}

		public override string name 
		{
			get 
			{
				return this.wrappedConstraint.Name;
			}
			set
			{
				this.wrappedConstraint.Name = value;
			}
		}



		public override string fqn 
		{
			get 
			{
				return this.owner.fqn + "." + this.name;
			}
		}

		public override string uniqueID 
		{
			get 
			{
				return this.owner.uniqueID + this.name;
			}
		}


		#endregion

		#region Constraint implementation

		public HashSet<UML.Classes.Kernel.Element> constrainedElement {
			get 
			{
				return new HashSet<UML.Classes.Kernel.Element>{this.owner};
			}
			set {
				throw new NotImplementedException();
			}
		}

		public UML.Classes.Kernel.ValueSpecification specification {
			get 
			{
				if (_opaqueExpression == null)
				{
					_opaqueExpression = new OpaqueExpression(this.wrappedConstraint.Notes,this.wrappedConstraint.Type);
				}
				return _opaqueExpression;
			}
			set 
			{
				if (value == null)
				{
					_opaqueExpression = null;
				}
				else
				{
					OpaqueExpression opaqueExpression = value as OpaqueExpression;
					if (opaqueExpression != null)
					{
						_opaqueExpression = opaqueExpression;
					}
					else
					{
						throw new NotImplementedException();
					}
				}
				
			}
		}

		public UML.Classes.Kernel.Namespace context 
		{
			get 
			{
				return this.owner as  UML.Classes.Kernel.Namespace;
			}
			set 
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region PackageableElement implementation

		public UML.Classes.Kernel.Package owningPackage 
		{
			get 
			{
				return ((ElementWrapper)this.owner).owningPackage;
			}
			set 
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region NamedElement implementation



		public UML.Classes.Kernel.VisibilityKind visibility {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public string qualifiedName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public UML.Classes.Kernel.Namespace owningNamespace 
		{
			get 
			{
				return this.context;
			}
			set {
				throw new NotImplementedException();
			}
		}

		public List<UML.Classes.Dependencies.Dependency> clientDependencies {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public List<UML.Classes.Dependencies.Dependency> supplierDependencies {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
