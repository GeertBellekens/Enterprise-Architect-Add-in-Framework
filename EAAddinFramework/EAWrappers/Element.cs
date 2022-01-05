using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public abstract class Element : UML.Classes.Kernel.Element
    {
        private Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
        internal global::EA.Element wrappedElement { get; set; }
        protected bool saveOwnedElements { get; set; } = true;
        /// <summary>
        /// resets teh property info making sure all properties are reset
        /// </summary>
        public virtual void reload()
        {
            this.properties.Clear();
        }
        protected object getProperty(string propertyName, object initialValue)
        {
            if (!this.properties.ContainsKey(propertyName))
            {
                this.properties.Add(propertyName, new PropertyInfo(initialValue, initialValue));
            }
            return this.properties[propertyName].propertyValue;
        }
        protected object getProperty(string propertyName)
        {
            return this.properties.ContainsKey(propertyName) ? this.properties[propertyName].propertyValue : null;
        }
        protected PropertyInfo getPropertyInfo(string propertyName)
        {
            return this.properties.ContainsKey(propertyName) ? this.properties[propertyName] : null;
        }
        protected void setProperty(string propertyName, object propertyValue, object initialValue)
        {
            if (this.properties.ContainsKey(propertyName))
            {
                this.properties[propertyName].propertyValue = propertyValue;
            }
            else
            {
                this.properties.Add(propertyName, new PropertyInfo(propertyValue, initialValue));
            }
        }
        public static string getPropertyNameName([CallerMemberName] string name = null)
        {
            return name;
        }

        public Model EAModel { get; internal set; }
        public UML.Extended.UMLModel model => this.EAModel;

        internal Element(Model model)
        {
            this.EAModel = model;
        }
        internal virtual string convertFromEANotes(string newFormat)
        {
            return this.EAModel.convertFromEANotes(this.notes, newFormat);
        }
        internal virtual void convertToEANotes(string externalNotes, string externalFormat)
        {
            this.notes = this.EAModel.convertToEANotes(externalNotes, externalFormat);
        }
        internal abstract global::EA.Collection eaTaggedValuesCollection { get; }
        internal void refreshTaggedValues()
        {
            this.eaTaggedValuesCollection.Refresh();
        }
        public abstract String notes { get; set; }
        public abstract HashSet<UML.Classes.Kernel.Element> ownedElements
        { get; set; }
        /// <summary>
        /// default implementation
        /// </summary>
        public virtual int position
        {
            get => 0;
            set
            {
                //do nothing
            }
        }
        /// <summary>
        /// the name to be used when ordering elemnts of this type alphabetically
        /// </summary>
        public virtual string orderingName => this.name;
        //indicates whether or not this element is new
        internal bool isNew { get; set; }
        private bool? _isDirty = true;
        public virtual bool isDirty
        {
            get
            {
                if (this.isNew)
                {
                    this._isDirty = true;
                }
                else
                {
                    var dirtyProperties = this.properties.Any() && this.properties.Values.Any(x => x.isDirty);
                    if (dirtyProperties)
                    {
                        this._isDirty = true;
                    }
                    else if (!this._isDirty.HasValue)
                    {
                        return false;
                    }

                }
                return this._isDirty.HasValue ? this._isDirty.Value : false;
            }
            set => this._isDirty = value;
        }
        public virtual bool isPropertyDirty(string propertyName)
        {
            return this.properties.ContainsKey(propertyName)
                && this.properties[propertyName].isDirty;
        }
        public virtual HashSet<UML.Classes.Kernel.Comment> ownedComments
        {
            get
            {
                HashSet<UML.Classes.Kernel.Comment> comments = new HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Comment>();
                comments.Add(((Factory)this.EAModel.factory).createDescriptionComment(this));
                return comments;
            }
            set
            {
                //first clear the notes
                string newNotes = string.Empty;
                foreach (DescriptionComment comment in value)
                {
                    newNotes += comment.body;
                }
                this.notes = newNotes;
            }
        }
        protected Element _owner;
        public abstract UML.Classes.Kernel.Element owner
        { get; set; }

        internal void removeTaggedValue(TaggedValue taggedValue)
        {
            //make sure the collection is current
            this.eaTaggedValuesCollection.Refresh();
            //delete in EA
            for (short i = 0; i < this.eaTaggedValuesCollection.Count; i++)
            {
                var eaTag = this.eaTaggedValuesCollection.GetAt(i);
                //if the eaTag equals the wrapped tag then we delete it
                if (taggedValue.equalsTagObject(eaTag))
                {
                    this.eaTaggedValuesCollection.DeleteAt(i, false);
                    break;
                }
            }
            // remove from list
            this._taggedValues?.Remove(taggedValue);
        }



        /// <summary>
        /// returns a list of all owners starting with the owner of this element up to the root package of the model
        /// </summary>
        /// <returns>a list of all owners starting with the over of this element up to the root package of the model</returns>
        public List<UML.Classes.Kernel.Element> getAllOwners()
        {
            var owners = new List<UML.Classes.Kernel.Element>();
            if (this.owner != null)
            {
                owners.Add(this.owner);
                owners.AddRange(this.owner.getAllOwners());
            }
            return owners;
        }
        public abstract HashSet<UML.Profiles.Stereotype> stereotypes
        { get; set; }
        public virtual void addStereotype(UML.Profiles.Stereotype stereotype)
        {
            HashSet<UML.Profiles.Stereotype> newStereotypes =
              new HashSet<UML.Profiles.Stereotype>(this.stereotypes);
            if (!newStereotypes.Contains(stereotype))
            {
                newStereotypes.Add(stereotype);
                this.stereotypes = newStereotypes;
            }
        }

        public virtual string uniqueID => null;

        public Stereotype addStereotype(string text)
        {
            Stereotype stereotype = (Stereotype)this.EAModel.factory.createStereotype(this, text);
            this.addStereotype(stereotype);
            return stereotype;
        }

        public bool hasStereotype(string stereotype)
        {
            return this.stereotypes.Any(
          x => x.name.Equals(
            stereotype,
            StringComparison.CurrentCultureIgnoreCase
          )
        );
        }

        public List<string> stereotypeNames
        {
            get
            {
                var returnedNames = new List<string>();
                foreach (UML.Profiles.Stereotype stereotype in this.stereotypes)
                {
                    returnedNames.Add(stereotype.name);
                }
                return returnedNames;
            }
        }
        public abstract string guid { get; }

        /// <summary>
        /// returns a list of diagrams that show this item.
        /// Default implementation on this level is an empty list.
        /// To be overridden by concrete subclasses
        /// </summary>
        /// <returns>all diagrams that show this item</returns>
        public virtual List<UML.Diagrams.Diagram> getDependentDiagrams()
        {
            return new List<UML.Diagrams.Diagram>();
        }

        /// returns the owner of the given type
        /// This operation will keep on looking upwards through the owners until 
        /// it finds one with the given type.
        /// NON UML
        public T getOwner<T>() where T : UML.Classes.Kernel.Element
        {
            if (this.owner is T || this.owner == null)
            {
                return (T)this.owner;
            }
            else
            {
                return ((Element)this.owner).getOwner<T>();
            }
        }

        /// default implementation returns an empty list because there is only one
        /// subclass that can actually implement this operation: ElementWrapper.
        public virtual List<UML.Classes.Kernel.Relationship> relationships
        {
            get => new List<UML.Classes.Kernel.Relationship>();
            set { /* do nothing */ }
        }

        /// default implementation returns an empty list because there is only one
        /// subclass that can actually implement this operation: EAElementWrapper.
        public virtual List<T> getRelationships<T>(bool outgoing = true, bool incoming = true)
          where T : UML.Classes.Kernel.Relationship
        {
            return this.relationships.OfType<T>().ToList();
        }


        internal abstract void saveElement();

        public virtual void save()
        {
            if (this.isDirty)
            {
                this.saveElement();
                //after saving the element is not new anymore
                this.isNew = false;
                // after saving the initialValue of the Properties has to be updated
                // or we keep comparing with the original value in subsequent calls
                foreach (var property in this.properties.Values)
                {
                    property.initialValue = property.propertyValue;
                }
            }
            if (this.saveOwnedElements)
            {
                foreach (UML.Classes.Kernel.Element element in this.ownedElements)
                {
                    ((Element)element).save();
                }
            }
        }
        //default empty implemenation
        public virtual UML.Diagrams.Diagram compositeDiagram
        {
            get => null;
            set { }//do absolutely nothing
        }

        //default not implemented
        public virtual HashSet<T> getUsingDiagrams<T>() where T : class, UML.Diagrams.Diagram
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// selects the element. 
        /// </summary>
        public virtual void select()
        {
            this.EAModel.selectedElement = this;
        }
        /// <summary>
        /// opens the element. 
        /// </summary>
        public virtual void open()
        {
            this.EAModel.selectedElement = this;
        }


        public abstract TSF.UmlToolingFramework.UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath);

        public virtual string name
        {
            get => string.Empty;
            set { }
        }
        protected HashSet<UML.Profiles.TaggedValue> _taggedValues;
        /// <summary>
        /// default empty implementation
        /// </summary>
        public virtual HashSet<UML.Profiles.TaggedValue> taggedValues
        {
            get
            {
                if (this._taggedValues == null)
                {
                    if (this.eaTaggedValuesCollection == null)
                    {
                        this._taggedValues = new HashSet<UML.Profiles.TaggedValue>();
                    }
                    else
                    {
                        //make sure we have the latest set of tagged values
                        this.eaTaggedValuesCollection?.Refresh();
                        //create the tagged values from the EA collection
                        this._taggedValues = new HashSet<UML.Profiles.TaggedValue>(this.EAModel.factory.createTaggedValues(this, this.eaTaggedValuesCollection));
                    }
                    
                }
                return this._taggedValues;
            }
            set => throw new NotImplementedException();
        }
        protected abstract string getTaggedValueQuery(string taggedValueName);
        /// <summary>
        /// returns the tagged value with the given name
        /// if a tagged valeu with the given name doesn't exist null is returned
        /// </summary>
        /// <param name="taggedValueName">the name of the tagged value to return</param>
        /// <returns>the tagged value with the given name</returns>
        public TaggedValue getTaggedValue(string taggedValueName)
        {
            //bool getTag = true;
            bool getTag = this._taggedValues != null;
            if (this._taggedValues == null)
            {
                //check if tagged value exists
                var xDoc = this.EAModel.SQLQuery(this.getTaggedValueQuery(taggedValueName));
                //get the tagged value if there is one
                if (xDoc.SelectNodes(this.EAModel.formatXPath("//Row")).Count > 0)
                {
                    getTag = true;
                }
            }
            return getTag ?
                this.taggedValues.FirstOrDefault(x => taggedValueName.Equals(x.name, StringComparison.InvariantCultureIgnoreCase)) as TaggedValue
                : null;
        }

        /// <summary>
        /// default empty implementation
        /// </summary>
        /// <returns>empty set</returns>
        public virtual HashSet<UML.Profiles.TaggedValue> getReferencingTaggedValues()
        {
            if (string.IsNullOrEmpty(this.uniqueID)) return new HashSet<UML.Profiles.TaggedValue>(); // in case we don't have a GUID
            return this.EAModel.getTaggedValuesWithValue(this.uniqueID, true);
        }

        public virtual string fqn
        {
            get
            {
                string nodepath = string.Empty;
                if (this.owner != null)
                {
                    nodepath = this.owner.fqn;
                }
                if (this.name.Length > 0)
                {
                    if (nodepath.Length > 0)
                    {
                        nodepath = nodepath + ".";
                    }
                    nodepath = nodepath + this.name;
                }
                return nodepath;
            }
        }
        /// <summary>
        /// opens the (standard) properties dialog in EA
        /// </summary>
        public virtual void openProperties()
        {
            this.EAModel.openProperties(this);
        }
        /// <summary>
        /// adds this element to the currently opened diagram
        /// </summary>
        public virtual void addToCurrentDiagram()
        {
            UML.Diagrams.Diagram currentDiagram = this.EAModel.currentDiagram;
            if (currentDiagram != null)
            {
                currentDiagram.addToDiagram(this);
            }
        }
        /// <summary>
        /// selects this element in the current diagram
        /// </summary>
        public virtual void selectInCurrentDiagram()
        {
            UML.Diagrams.Diagram currentDiagram = this.EAModel.currentDiagram;
            if (currentDiagram != null)
            {
                currentDiagram.selectItem(this);
            }
        }
        /// <summary>
        /// returns the name as ToString
        /// </summary>
        /// <returns>the name as ToString</returns>
        public override string ToString()
        {
            return this.name;
        }
        /// <summary>
        /// adds a new tagged value to the element with the given name and value
        /// if a tagged value with that name already exists the value of the existing tagged value is updated./
        /// </summary>
        /// <param name="name">the name of the tagged value to add</param>
        /// <param name="tagValue">the value of the tagged value</param>
        /// <param name = "comment">the comment to add to the tagged value</param>
        /// <param name = "addDuplicate"></param>
        /// <returns>the added (or updated) tagged value</returns>

        public virtual TaggedValue addTaggedValue(string name, string tagValue, string comment = null, bool addDuplicate = false)
        {
            TaggedValue newTaggedValue = null;
            if (!addDuplicate)
            {
                //we don't wan't any duplicates so we get the existing one
                newTaggedValue = this.getTaggedValue(name);
            }
            else
            {
                //we allow "duplicate" tagged values, but only if the value or the comment is different
                newTaggedValue = this.taggedValues.FirstOrDefault(x => x.name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                                                            && ((TaggedValue)x).eaStringValue.Equals(tagValue, StringComparison.InvariantCultureIgnoreCase)
                                                            && ((TaggedValue)x).comment.Equals(comment, StringComparison.InvariantCultureIgnoreCase)) as TaggedValue;
                if (newTaggedValue == null)
                {
                    //if an Empty tagged value exists, we use that one
                    newTaggedValue = this.taggedValues.FirstOrDefault(x => x.name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                                                                && string.IsNullOrEmpty(((TaggedValue)x).eaStringValue)) as TaggedValue;
                }
            }
            if (newTaggedValue == null)
            {
                //no existing tagged value found, or we need to create duplicates
                newTaggedValue = (TaggedValue)this.EAModel.factory.createNewTaggedValue(this, name);
                //add it to the local list of tagged values
                if (this._taggedValues != null)
                {
                    this._taggedValues.Add(newTaggedValue);
                }
            }
            newTaggedValue.tagValue = tagValue;
            if (comment != null)
            {
                newTaggedValue.comment = comment;
            }

            newTaggedValue.save();
            return newTaggedValue;
        }
        /// <summary>
        /// adds a new tagged value to the element with the given name and value
        /// if a tagged value with that name already exists the value of the existing tagged value is updated./
        /// </summary>
        /// <param name="name">the name of the tagged value to add</param>
        /// <param name="linkedElement">the element to be linked via this tagged value</param>
        /// <param name = "comment">the comment to add to the tagged value</param>
        /// <param name = "addDuplicate"></param>
        /// <returns>the added (or updated) tagged value</returns>
        public virtual TaggedValue addTaggedValue(string name, UML.Extended.UMLItem linkedElement, string comment = null, bool addDuplicate = false)
        {
            return this.addTaggedValue(name, linkedElement.uniqueID, comment, addDuplicate);
        }
        /// <summary>
        /// copies the tagged values from the source as tagged values of this element
        /// if a tagged value already exists the value is copied. Else a new tagged value is created
        /// </summary>
        /// <param name="sourceElement">the element to copy the tagged values from</param>
        public virtual void copyTaggedValues(Element sourceElement)
        {
            foreach (TaggedValue taggedValue in sourceElement.taggedValues)
            {
                this.addTaggedValue(taggedValue.name, taggedValue.eaStringValue, taggedValue.comment);
            }
        }

        /// <summary>
        /// deletes this element from the model
        /// In Enterprise Architect we can only delete an element by removing it from its parents collection.
        /// </summary>
        public virtual void delete()
        {
            if (this.owner != null)
            {
                ((Element)this.owner).deleteOwnedElement(this);
            }
        }
        public abstract void deleteOwnedElement(Element ownedElement);
        /// <summary>
        /// the package owning this element (if any)
        /// this will return null in case of a root package
        /// </summary>
        public virtual UML.Classes.Kernel.Package owningPackage
        {
            get
            {
                var ownerPackage = this.owner as UML.Classes.Kernel.Package;
                //if the owner is a package then return the owner
                if (ownerPackage == null)
                {
                    ownerPackage = ((Element)this.owner).owningPackage;
                }

                return (UML.Classes.Kernel.Package)this.getProperty(getPropertyNameName(), ownerPackage);
            }
            set
            {
                //do nothing, implemented at the subclasses		
            }
        }

        public abstract bool makeWritable(bool overrideLocks);

        public bool isReadOnly
        {
            get
            {
                //first check if locking is enabled
                if (!this.EAModel.isSecurityEnabled)
                {
                    return false;
                }

                if (this.isLocked)
                {
                    return this.getLockedUserID() != this.EAModel.currentUserID;
                }
                //not locked so readonly (only works when "require user lock to edit is on")
                return true;
            }
        }
        /// <summary>
        /// returns the name of the user currently locking this element
        /// </summary>
        /// <returns>the name of the user currently locking this element</returns>
        public abstract string getLockedUser();

        public abstract string getLockedUserID();
        /// <summary>
        /// returns true if currently locked
        /// </summary>
        /// <returns>true if currently locked</returns>
        public bool isLocked => (this.getLockedUser() != string.Empty);

        public virtual HashSet<UML.Classes.Kernel.Constraint> constraints
        {
            get => new HashSet<UML.Classes.Kernel.Constraint>();//default implementation empty list
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// finds the element based on the given descriptor
        /// </summary>
        /// <param name="itemDescriptor">the itemdescriptor</param>
        /// <returns></returns>
        public virtual List<UML.Extended.UMLItem> findOwnedItems(string itemDescriptor)
        {
            //default implementation, search based on the fully qualified name
            var ownedItems = new List<UML.Extended.UMLItem>();
            // split descriptor and search fo the parts
            var descriptorParts = itemDescriptor.Split('.').ToList();
            //normally the first part is the name of this item, so we remove that
            if (descriptorParts.Any() && descriptorParts[0] == this.name)
            {
                descriptorParts.RemoveAt(0);
            }
            ownedItems.AddRange(this.findOwnedItems(descriptorParts));
            //if not found anything directly then try with FQN
            if (!ownedItems.Any())
            {
                var item = this.EAModel.getItemFromFQN(this.fqn + "." + itemDescriptor);
                if (item != null)
                {
                    ownedItems.Add(item);
                }
            }
            return ownedItems;
        }
        public virtual List<UML.Extended.UMLItem> findOwnedItems(List<String> descriptionParts)
        {
            //default implementation returns empty collection
            return new List<UML.Extended.UMLItem>();
        }
        protected class PropertyInfo
        {
            public PropertyInfo(object propertyValue, object initialValue)
            {
                this.propertyValue = propertyValue;
                this.initialValue = initialValue;
            }
            public object propertyValue { get; set; }
            public object initialValue { get; set; }
            public bool isDirty
            {
                get
                {
                    if (this.initialValue == null)
                    {
                        return (this.propertyValue != null);
                    }
                    else
                    {
                        return !this.initialValue.Equals(this.propertyValue);
                    }
                }
            }
        }
    }
}
