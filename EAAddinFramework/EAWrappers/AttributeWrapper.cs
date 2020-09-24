using System;
using System.Collections.Generic;
using System.Linq;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    /// <summary>
    /// Description of AttributeWrapper.
    /// </summary>
    public class AttributeWrapper : Element
    {
        private UML.Classes.Kernel.Type _type;
        private string _uniqueID = null;

        public AttributeWrapper(Model model, global::EA.Attribute wrappedAttribute)
          : base(model)
        {
            this.wrappedAttribute = wrappedAttribute;
            var dummy = this.type; //make sure we get the type here to avoid multithreading errors
            this._uniqueID = wrappedAttribute.AttributeGUID;
            this.isDirty = false;
        }
        public override void reload()
        {
            base.reload();
            this._type = null;
        }
        internal global::EA.Attribute wrappedAttribute { get; set; }
        public int id => this.wrappedAttribute.AttributeID;
        public override UML.Classes.Kernel.Package owningPackage
        {
            get => base.owningPackage;
            set => ((ElementWrapper)this.owner).owningPackage = value;
        }


        public global::EA.Attribute WrappedAttribute => this.wrappedAttribute;
        public override string name
        {
            get
            {
                //check first if already exists
                string prop = (string)this.getProperty(getPropertyNameName());
                return prop ?? (string)this.getProperty(getPropertyNameName(), this.wrappedAttribute.Name);
            }
            set => this.setProperty(getPropertyNameName(), value, this.wrappedAttribute.Name);
        }
        public string alias
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedAttribute.Alias);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedAttribute.Alias);
        }
        public override int position
        {
            get => (int)this.getProperty(getPropertyNameName(), this.wrappedAttribute.Pos);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedAttribute.Pos);
        }

        public UML.Classes.Kernel.Type type
        {
            get
            {
                if (this._type == null)
                {
                    this._type = this.EAModel.getElementWrapperByID((int)this.getProperty("ClassifierID", this.wrappedAttribute.ClassifierID)) as UML.Classes.Kernel.Type;
                    // check if the type is defined as an element in the model.
                    if (this._type == null)
                    {
                        // no element, create primitive type based on the name of the type
                        this._type = this.EAModel.factory.createPrimitiveType(this.getProperty("Type", this.wrappedAttribute.Type));
                    }
                }
                return this._type;
            }
            set
            {
                this._type = value;
                if (value != null)
                {
                    //set classifier if needed
                    ElementWrapper elementWrapper = value as ElementWrapper;
                    if (elementWrapper != null)
                    {
                        this.setProperty("ClassifierID", ((ElementWrapper)value).id, this.wrappedAttribute.ClassifierID);
                    }
                    //always set type field
                    this.setProperty("Type", value.name, this.wrappedAttribute.Type);
                }

            }
        }
        public override HashSet<UML.Profiles.Stereotype> stereotypes
        {
            get => ((Factory)this.EAModel.factory).createStereotypes(this, (string)this.getProperty(getPropertyNameName(), this.getWrappedStereotypeString()));
            set => this.setProperty(getPropertyNameName(), Stereotype.getStereotypeEx(value), this.getWrappedStereotypeString());
        }
        private HashSet<UML.Classes.Kernel.Constraint> _constraints;
        public override HashSet<UML.Classes.Kernel.Constraint> constraints 
        { 
            get
            {
                if (this._constraints == null)
                {
                    this._constraints = new HashSet<UML.Classes.Kernel.Constraint>();
                    foreach (var constraint in this.EAModel.factory.createElements(this.wrappedAttribute.Constraints).Cast<AttributeConstraint>())
                    {
                        this._constraints.Add(constraint);
                    }
                }
                return this._constraints;
            }
            set => throw new NotImplementedException();
        }
        private string getWrappedStereotypeString()
        {
            //in some cases the StereotypeEx is empty while Stereotyp is not. (smells like a bug in the EA API)
            // below the workaround to make sure we have the stereotype
            var stereotypeString = this.wrappedAttribute.StereotypeEx;
            if (string.IsNullOrEmpty(stereotypeString))
            {
                stereotypeString = this.wrappedAttribute.Stereotype;
            }

            return stereotypeString;
        }
        public void setStereotype(string stereotype)
        {
            var newStereotypes = new HashSet<UML.Profiles.Stereotype>();
            newStereotypes.Add(new Stereotype(this.EAModel, this, stereotype));
            this.stereotypes = newStereotypes;
        }

        public override String notes
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedAttribute.Notes);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedAttribute.Notes);
        }



        internal override void saveElement()
        {
            if (this.getProperty("name") != null)
            {
                this.wrappedAttribute.Name = (string)this.getProperty("name");
            }

            if (this.getProperty("alias") != null)
            {
                this.wrappedAttribute.Alias = (string)this.getProperty("alias");
            }

            if (this.getProperty("ClassifierID") != null)
            {
                this.wrappedAttribute.ClassifierID = (int)this.getProperty("ClassifierID");
            }

            if (this.getProperty("Type") != null)
            {
                this.wrappedAttribute.Type = (string)this.getProperty("Type");
            }

            if (this.getProperty("notes") != null)
            {
                this.wrappedAttribute.Notes = (string)this.getProperty("notes");
            }

            if (this.getProperty("position") != null)
            {
                this.wrappedAttribute.Pos = (int)this.getProperty("position");
            }

            if (this.getProperty("stereotypes") != null)
            {
                this.wrappedAttribute.StereotypeEx = (string)this.getProperty("stereotypes");
            }

            this.wrappedAttribute.Update();
        }
        /// <summary>
        /// return the unique ID of this element
        /// </summary>
        public override string uniqueID
        {
            get
            {
                if (string.IsNullOrEmpty(this._uniqueID))
                {
                    this._uniqueID = this.guid;
                }
                return this._uniqueID;
            }
        }
        



        public override HashSet<UML.Classes.Kernel.Element> ownedElements
        {
            get => new HashSet<UML.Classes.Kernel.Element>();
            set => throw new NotImplementedException();
        }


        public override UML.Classes.Kernel.Element owner
        {
            get
            {
                if (this._owner == null)
                {
                    this._owner = this.EAModel.getElementWrapperByID(this.wrappedAttribute.ParentID);
                }
                return this._owner;
            }
            set
            {
                //only used to set the owner object of the attribute in case that was already initialized, not to change the owner of the attribute.
                //in that case create a new attribute with the same properties as this one on the new owner.
                if (this.wrappedAttribute.ParentID != ((ElementWrapper)value).id)
                {
                    throw new NotImplementedException("You cannot change the owner of an Attribute");
                }

                this._owner = value as ElementWrapper;
            }

        }





        public override TSF.UmlToolingFramework.UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
        {
            UML.Extended.UMLItem item = null;
            List<string> filteredPath = new List<string>(relativePath);
            if (ElementWrapper.filterName(filteredPath, this.name))
            {
                if (filteredPath.Count == 1)
                {
                    item = this;
                }
            }
            return item;
        }


        #region Equals and GetHashCode implementation
        public override bool Equals(object obj)
        {
            var other = obj as AttributeWrapper;
            if (other != null)
            {
                if (other.wrappedAttribute.AttributeGUID == this.wrappedAttribute.AttributeGUID)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return new Guid(this.wrappedAttribute.AttributeGUID).GetHashCode();
        }
        #endregion


        internal override global::EA.Collection eaTaggedValuesCollection => this.WrappedAttribute.TaggedValues;
        public override string guid => this.WrappedAttribute.AttributeGUID;

        #region implemented abstract members of Element

        public override void deleteOwnedElement(Element ownedElement)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region implemented abstract members of Element
        public override bool makeWritable(bool overrideLocks)
        {
            return this.owner.makeWritable(overrideLocks);
        }
        public override string getLockedUser()
        {
            return ((Element)this.owner).getLockedUser();
        }
        public override string getLockedUserID()
        {
            return ((Element)this.owner).getLockedUserID();
        }

        protected override string getTaggedValueQuery(string taggedValueName)
        {
            return @"select tv.ea_guid from t_attributetag tv
				where 
				tv.Property = '" + taggedValueName + "' and tv.ElementID = " + this.id;
        }

        #endregion
        public override List<UML.Extended.UMLItem> findOwnedItems(List<string> descriptionParts)
        {
            //return the owned items of the type of the attribute
            var ownedItems = new List<UML.Extended.UMLItem>();
            var elementType = this.type as Element;
            if (elementType != null)
            {
                ownedItems.AddRange(((Element)this.type).findOwnedItems(descriptionParts));
            }
            return ownedItems;
        }
        public AttributeConstraint addAttributeConstraint(string name)
        {
           return ((Factory)this.EAModel.factory).addElementToEACollection<AttributeConstraint>(this.wrappedAttribute.Constraints, name, "AttributeConstraint");
        }
    }
}
