using System;
using System.Collections.Generic;
using System.Linq;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    /// <summary>
    /// Description of Constraint.
    /// </summary>
    public class AttributeConstraint : Element, UML.Classes.Kernel.Constraint
    {
        internal global::EA.AttributeConstraint wrappedConstraint { get; set; }
       // private Element _owner;
        private OpaqueExpression _opaqueExpression;
        private AttributeWrapper attributeWrapper => this.owner as AttributeWrapper;
        //private ElementWrapper elementWrapperOwner => this.owner as ElementWrapper;
        public AttributeConstraint(Model model, global::EA.AttributeConstraint wrappedConstraint) : base(model)
        {
            this.wrappedConstraint = wrappedConstraint;
            this.isDirty = true;
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
        internal override global::EA.Collection eaTaggedValuesCollection => throw new NotImplementedException();
        public override string notes
        {
            get => this.wrappedConstraint != null ?
                        this.wrappedConstraint.Notes :
                        string.Empty;
            set
            {
                if (this.wrappedConstraint != null)
                {
                    this.wrappedConstraint.Notes = value;
                }
            }
        }
        internal override string convertFromEANotes(string newFormat)
        {
            return this.EAModel.convertFromEANotes(this.notes, newFormat);
        }
        internal override void convertToEANotes(string externalNotes, string externalFormat)
        {
            this.notes = this.EAModel.convertToEANotes(externalNotes, externalFormat);
        }
        public override string guid => this.uniqueID;
        #endregion
        #region Element implementation

        public override void addStereotype(UML.Profiles.Stereotype stereotype)
        {
            throw new NotImplementedException();
        }

        public override List<T> getRelationships<T>(bool outgoing = true, bool incoming = true)
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

        public override HashSet<UML.Classes.Kernel.Element> ownedElements
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override HashSet<UML.Classes.Kernel.Comment> ownedComments
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override UML.Classes.Kernel.Element owner
        {
            get
            {
                if (this._owner == null)
                {
                    this._owner = this.EAModel.getAttributeWrapperByID(this.wrappedConstraint.AttributeID);
                }
                return this._owner;
            }
            set => this._owner = value as Element;
        }

        public override HashSet<UML.Profiles.Stereotype> stereotypes
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override List<UML.Classes.Kernel.Relationship> relationships
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override HashSet<UML.Profiles.TaggedValue> taggedValues
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override UML.Diagrams.Diagram compositeDiagram
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override int position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
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
            for (int i = this.attributeWrapper.wrappedAttribute.Constraints.Count - 1; i >= 0; i--)
            {
                var currentConstraint = (global::EA.AttributeConstraint)this.attributeWrapper.wrappedAttribute.Constraints.GetAt(short.Parse(i.ToString()));
                if (this.name == currentConstraint.Name)
                {
                    this.attributeWrapper.wrappedAttribute.Constraints.DeleteAt(short.Parse(i.ToString()), false);
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
            get => this.wrappedConstraint.Name;
            set => this.wrappedConstraint.Name = value;
        }



        public override string fqn => this.owner.fqn + "." + this.name;

        public override string uniqueID => this.owner.uniqueID + this.name;

        protected override string getTaggedValueQuery(string taggedValueName)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Constraint implementation

        public HashSet<UML.Classes.Kernel.Element> constrainedElement
        {
            get => new HashSet<UML.Classes.Kernel.Element> { this.owner };
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.ValueSpecification specification
        {
            get
            {
                if (this._opaqueExpression == null)
                {
                    this._opaqueExpression = new OpaqueExpression(this.wrappedConstraint.Notes, this.wrappedConstraint.Type);
                }
                return this._opaqueExpression;
            }
            set
            {
                if (value == null)
                {
                    this._opaqueExpression = null;
                }
                else
                {
                    OpaqueExpression opaqueExpression = value as OpaqueExpression;
                    if (opaqueExpression != null)
                    {
                        this._opaqueExpression = opaqueExpression;
                        this.wrappedConstraint.Notes = this._opaqueExpression.bodies.FirstOrDefault();
                        this.wrappedConstraint.Type = opaqueExpression.languages.FirstOrDefault();
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
            get => this.owner as UML.Classes.Kernel.Namespace;
            set => throw new NotImplementedException();
        }
        public string constraintType
        {
            get => this.wrappedConstraint.Type;
            set => this.wrappedConstraint.Type = value;
        }
        #endregion

        #region PackageableElement implementation

        public override UML.Classes.Kernel.Package owningPackage
        {
            get => this.attributeWrapper.owningPackage;
            set => throw new NotImplementedException();
        }

        #endregion

        #region NamedElement implementation



        public UML.Classes.Kernel.VisibilityKind visibility
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string qualifiedName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.Namespace owningNamespace
        {
            get => this.context;
            set => throw new NotImplementedException();
        }

        public List<UML.Classes.Dependencies.Dependency> clientDependencies
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public List<UML.Classes.Dependencies.Dependency> supplierDependencies
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }



        #endregion
    }
}
