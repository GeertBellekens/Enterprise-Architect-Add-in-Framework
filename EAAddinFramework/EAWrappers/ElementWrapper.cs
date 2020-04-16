using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace TSF.UmlToolingFramework.Wrappers.EA
{

    public class ElementWrapper : Element, UML.Classes.Kernel.PackageableElement
    {
        internal global::EA.Element wrappedElement { get; set; }
        private HashSet<UML.Classes.Kernel.Property> _attributes;
        private List<UML.Classes.Kernel.Relationship> _allRelationships;
        private HashSet<AttributeWrapper> _attributeWrappers;
        private HashSet<UML.Classes.Kernel.EnumerationLiteral> _ownedLiterals;
        private HashSet<UML.Classes.Kernel.Constraint> _constraints;
        private string _linkedDocument;
        protected string _uniqueID;

        public ElementWrapper(Model model, global::EA.Element wrappedElement)
          : base(model)
        {
            this.initialize(wrappedElement);
        }

        protected virtual void initialize(global::EA.Element wrappedElement)
        {
            this.wrappedElement = wrappedElement;
            this.isDirty = false;
            if (wrappedElement != null)
            {
                this._uniqueID = wrappedElement.ElementGUID;
            }
        }
        protected UML.Classes.Kernel.Package _owningPackage;
        public override UML.Classes.Kernel.Package owningPackage
        {
            get
            {
                if (this._owningPackage == null)
                {
                    this._owningPackage = base.owningPackage;
                }

                return base.owningPackage;
            }
            set
            {
                if (this._owningPackage == null)
                {
                    this._owningPackage = base.owningPackage;
                }

                this.setProperty(getPropertyNameName(), value, this._owningPackage);
            }
        }

        /// <summary>
        /// resets the cached relations
        /// </summary>
        internal void resetRelationships()
        {
            this._allRelationships = null;
        }
        /// <summary>
        /// resets the cached attributes
        /// </summary>
        internal virtual void resetAttributes()
        {
            this._attributes = null;
            this._attributeWrappers = null;
            this._ownedLiterals = null;
        }
        public string linkedDocument
        {
            get
            {
                if (this._linkedDocument != null)
                {
                    this._linkedDocument = this.wrappedElement.GetLinkedDocument();
                }
                return this._linkedDocument;
            }
            set
            {
                if (value != this._linkedDocument)
                {
                    //write the value to a temp file
                    string tempFile = Path.GetTempFileName();
                    File.WriteAllText(tempFile, value);
                    //load the new contents in the linked document
                    this.wrappedElement.LoadLinkedDocument(tempFile);
                    //delete the temp file
                    File.Delete(tempFile);
                }

            }
        }

        void reloadWrappedElement()
        {
            this.wrappedElement = this.EAModel.wrappedModel.GetElementByGuid(this._uniqueID);
        }
        public ElementWrapper classifier
        {
            get => this.EAModel.getElementWrapperByPackageID((int)this.getProperty(getPropertyNameName(), this.wrappedElement.ClassifierID));
            set => this.setProperty(getPropertyNameName(), value.id, this.wrappedElement.ClassifierID);
        }
        public override string name
        {
            get
            {
                try
                {
                    //check first if already exists
                    string prop = (string)this.getProperty(getPropertyNameName());
                    return prop ?? (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Name);
                }
                catch (System.InvalidCastException)
                {
                    this.reloadWrappedElement();
                    //check first if already exists
                    string prop = (string)this.getProperty(getPropertyNameName());
                    return prop ?? (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Name);
                }
            }
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Name);
        }
        public string alias
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Alias);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Alias);
        }
        public string status
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Status);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Status);
        }
        public string header1
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedElement.RunState);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.RunState);
        }
        public string genLinks
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Genlinks);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Genlinks);
        }
        public string phase
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Phase);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Phase);
        }
        public override String notes
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Notes);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Notes);
        }
        /// indicates whether this element is abstract.
        /// In the EA API this is stored as a string with the values "0" or "1"
        public bool isAbstract
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Abstract) == "1";
            set => this.setProperty(getPropertyNameName(), value ? "1" : "0", this.wrappedElement.Abstract);

        }
        public override int position
        {
            get => (int)this.getProperty(getPropertyNameName(), this.wrappedElement.TreePos);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.TreePos);
        }
        /// the visibility indicates the scope of the element
        public UML.Classes.Kernel.VisibilityKind visibility
        {
            get => VisibilityKind.getUMLVisibilityKind((string)this.getProperty(getPropertyNameName(), this.wrappedElement.Visibility),
                UML.Classes.Kernel.VisibilityKind._public);
            set => this.setProperty(getPropertyNameName(), VisibilityKind.getEAVisibility(value), this.wrappedElement.Visibility);
        }
        public void setStereotype(string stereotype)
        {
            this.setProperty("stereotypes", stereotype, this.wrappedElement.StereotypeEx);
        }
        public override HashSet<UML.Profiles.Stereotype> stereotypes
        {
            get => ((Factory)this.EAModel.factory).createStereotypes(this, (string)this.getProperty(getPropertyNameName(), this.wrappedElement.StereotypeEx));
            set => this.setProperty(getPropertyNameName(), Stereotype.getStereotypeEx(value), this.wrappedElement.StereotypeEx);
        }

        public string author
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Author);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Author);
        }

        public string version
        {
            get => (string)this.getProperty(getPropertyNameName(), this.wrappedElement.Version);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Version);
        }

        public List<string> keywords
        {
            get => ((string)this.getProperty(getPropertyNameName(), this.wrappedElement.Tag)).Split(',').ToList();
            set
            {
                string string_value = value == null ? "" : string.Join(",", value);
                this.setProperty(getPropertyNameName(), string_value, this.wrappedElement.Tag);
            }
        }

        public DateTime created
        {
            get => (DateTime)this.getProperty(getPropertyNameName(), this.wrappedElement.Created);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Created);
        }

        public DateTime modified
        {
            get => (DateTime)this.getProperty(getPropertyNameName(), this.wrappedElement.Modified);
            set => this.setProperty(getPropertyNameName(), value, this.wrappedElement.Modified);
        }



        internal override void saveElement()
        {
            if (this.getProperty("name") != null)
            {
                this.wrappedElement.Name = (string)this.getProperty("name");
            }

            if (this.getProperty("alias") != null)
            {
                this.wrappedElement.Alias = (string)this.getProperty("alias");
            }

            if (this.getProperty("genLinks") != null)
            {
                this.wrappedElement.Genlinks = (string)this.getProperty("genLinks");
            }

            if (this.getProperty("subType") != null)
            {
                this.wrappedElement.Subtype = (int)this.getProperty("subType");
            }

            if (this.getProperty("notes") != null)
            {
                this.wrappedElement.Notes = (string)this.getProperty("notes");
            }

            if (this.getProperty("isAbstract") != null)
            {
                this.wrappedElement.Abstract = (string)this.getProperty("isAbstract");
            }

            if (this.getProperty("position") != null)
            {
                this.wrappedElement.TreePos = (int)this.getProperty("position");
            }

            if (this.getProperty("visibility") != null)
            {
                this.wrappedElement.Visibility = (string)this.getProperty("visibility");
            }

            if (this.getProperty("stereotypes") != null)
            {
                this.wrappedElement.StereotypeEx = (string)this.getProperty("stereotypes");
            }

            if (this.getProperty("classifier") != null)
            {
                this.wrappedElement.ClassifierID = (int)this.getProperty("classifier");
            }

            if (this.getProperty("owner") != null)
            {
                if (this.getProperty("owner") is Package)
                {
                    this.wrappedElement.PackageID = ((Package)this.getProperty("owner")).packageID;
                }
                else
                {
                    this.wrappedElement.ParentID = ((ElementWrapper)this.getProperty("owner")).id;
                }
            }
            if (this.getProperty("owningPackage") != null)
            {
                this.wrappedElement.PackageID = ((Package)this.getProperty("owningPackage")).packageID;
            }

            if (this.getProperty("author") != null)
            {
                this.wrappedElement.Author = (string)this.getProperty("author");
            }

            if (this.getProperty("version") != null)
            {
                this.wrappedElement.Version = (string)this.getProperty("version");
            }

            if (this.getProperty("status") != null)
            {
                this.wrappedElement.Status = (string)this.getProperty("status");
            }

            if (this.getProperty("keywords") != null)
            {
                this.wrappedElement.Tag = (string)this.getProperty("keywords");
            }

            if (this.getProperty("created") != null)
            {
                this.wrappedElement.Created = (DateTime)this.getProperty("created");
            }

            if (this.getProperty("modified") != null)
            {
                this.wrappedElement.Modified = (DateTime)this.getProperty("modified");
            }

            if (this.getProperty("modifier") != null)
            {
                this.addTaggedValue("modifier", (string)this.getProperty("modifier"));
            }

            if (this.getProperty("notes") != null)
            {
                this.wrappedElement.Notes = (string)this.getProperty("notes");
            }

            if (this.getProperty("domain") != null)
            {
                this.addTaggedValue("domain", (string)this.getProperty("domain"));
            }

            if (this.getProperty("label") != null)
            {
                this.addTaggedValue("label", (string)this.getProperty("label"));
            }

            if (this.getProperty("type") != null)
            {
                this.addTaggedValue("type", (string)this.getProperty("type"));
            }

            if (this.getProperty("size") != null)
            {
                this.addTaggedValue("size", (string)this.getProperty("size"));
            }

            if (this.getProperty("format") != null)
            {
                this.addTaggedValue("format", (string)this.getProperty("format"));
            }

            if (this.getProperty("initialValue") != null)
            {
                this.addTaggedValue("initialValue", (string)this.getProperty("initialValue"));
            }

            if (this.getProperty("header1") != null)
            {
                this.wrappedElement.Header1 = (string)this.getProperty("header1");
            }

            this.wrappedElement.Update();
        }
        public List<string> primitiveParentNames
        {
            get
            {
                var parentNames = new List<string>();
                if (this.genLinks.Contains("Parent="))
                {
                    foreach (string genLink in this.wrappedElement.Genlinks.Split(';'))
                    {
                        var parentTuple = genLink.Split('=');
                        if (parentTuple.Count() == 2
                            && parentTuple[0] == "Parent"
                            && !string.IsNullOrEmpty(parentTuple[1]))
                        {
                            parentNames.Add(parentTuple[1]);
                        }
                    }
                }
                return parentNames;
            }
        }

        public virtual string EAElementType => this.wrappedElement.Type;
        /// <summary>
        /// return the unique ID of this element
        /// </summary>
        public override string uniqueID => this._uniqueID;

        public string subType
        {
            get
            {
                int subType = (int)this.getProperty(getPropertyNameName(), this.wrappedElement.Subtype);
                string wrappedType = this.wrappedElement.Type;
                //PackagingComponents have Package as Type and 20 as subtype
                if (wrappedType == "Package" && this.wrappedElement.Subtype == 20)
                {
                    wrappedType = "PackagingComponent";
                }
                else if (wrappedType == "StateNode")
                {
                    switch (subType)
                    {
                        case 100:
                            wrappedType = "ActivityInitial";
                            break;
                        case 101:
                            wrappedType = "ActivityFinal";
                            break;
                        case 102:
                            wrappedType = "FlowFinal";
                            break;
                        case 6:
                            wrappedType = "SynchronisationNode";
                            break;
                        case 3:
                            wrappedType = "StateInitial";
                            break;
                        case 4:
                            wrappedType = "StateFinal";
                            break;
                        case 5:
                            wrappedType = "StateHistory";
                            break;
                        case 11:
                            wrappedType = "StateChoice";
                            break;
                        case 10:
                            wrappedType = "StateJunction";
                            break;
                        case 13:
                            wrappedType = "StateEntryPoint";
                            break;
                        case 14:
                            wrappedType = "StateExitPoint";
                            break;
                        case 12:
                            wrappedType = "StateTerminate";
                            break;
                    }
                }
                else if (wrappedType == "Text")
                {
                    switch (subType)
                    {
                        case 76:
                            wrappedType = "Legend";
                            break;
                        case 18:
                            wrappedType = "DiagramNotes";
                            break;
                        case 19:
                            wrappedType = "Hyperlink";
                            break;
                    }
                }
                return wrappedType;
            }
            //TODO: properly translate subtypes to their integer value. This now only works if the integer value is used directly.
            set => this.setProperty(getPropertyNameName(), int.Parse(value), this.wrappedElement.Subtype);
        }

        public virtual global::EA.Element WrappedElement
        {
            get => this.wrappedElement;
            set => this.wrappedElement = value;
        }

        public global::EA._CustomProperty getCustomProperty(string propertyName)
        {
            foreach (global::EA._CustomProperty property in this.wrappedElement.CustomProperties)
            {
                if (property.Name == propertyName)
                {
                    return property;
                }
            }
            return null;
        }




        /// returns the attributes owned by this class
        /// This seems to be a subset of the classifiers attributes property, but 
        /// is is unclear what exactly the difference is.
        public HashSet<UML.Classes.Kernel.Property> ownedAttributes
        {
            get => this.attributes;
            set => throw new NotImplementedException();
        }

        public override HashSet<UML.Classes.Kernel.Constraint> constraints
        {
            get
            {
                if (this._constraints == null)
                {
                    //refresh attributes to make sure we have an up-to-date list
                    this._constraints = new HashSet<UML.Classes.Kernel.Constraint>();
                    foreach (var constraint in this.EAModel.factory.createElements(this.wrappedElement.Constraints).Cast<Constraint>())
                    {
                        this._constraints.Add(constraint);
                    }
                }
                return this._constraints;
            }
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Kernel.Property> attributes
        {
            get
            {
                if (this._attributes == null)
                {
                    //get the attributes
                    this._attributes = new HashSet<UML.Classes.Kernel.Property>(this.attributeWrappers.OfType<UML.Classes.Kernel.Property>());
                }
                return this._attributes;
            }
            set => throw new NotImplementedException();
        }

        public HashSet<AttributeWrapper> attributeWrappers
        {
            get
            {
                if (this._attributeWrappers == null)
                {
                    //refresh attributes to make sure we have an up-to-date list
                    this.wrappedElement.Attributes.Refresh();
                    //get the attribute wrappers
                    this._attributeWrappers = new HashSet<AttributeWrapper>(Factory.getInstance(this.EAModel)
                                     .createElements(this.wrappedElement.Attributes, this).Cast<AttributeWrapper>());
                }
                return this._attributeWrappers;
            }
        }



        /// represents the internal EA's elementID
        public int id => this.wrappedElement.ElementID;

        public override HashSet<UML.Classes.Kernel.Element> ownedElements
        {
            get
            {
                var elements = new List<UML.Classes.Kernel.Element>();
                //get nested elements
                elements.AddRange(this.ownedElementWrappers);
                //add attributes
                elements.AddRange(this.ownedAttributes.Cast<UML.Classes.Kernel.Element>());
                //add operations
                elements.AddRange(this.ownedOperations.Cast<UML.Classes.Kernel.Element>());
                return new HashSet<UML.Classes.Kernel.Element>(elements);
            }
            set => throw new NotImplementedException();
        }
        protected List<ElementWrapper> _ownedElementWrappers;
        virtual public List<ElementWrapper> ownedElementWrappers
        {
            get
            {
                this.wrappedElement.Elements.Refresh();
                if (this._ownedElementWrappers == null)
                {
                    this._ownedElementWrappers = this.EAModel.factory.createElements(this.wrappedElement.Elements).OfType<ElementWrapper>().ToList();
                }
                return this._ownedElementWrappers;
            }
        }

        public override UML.Classes.Kernel.Element owner
        {
            get
            {
                //fill in the owner if still empty
                if (this._owner == null)
                {
                    if (this.wrappedElement.ParentID > 0)
                    {
                        this._owner = this.EAModel.getElementWrapperByID(this.wrappedElement.ParentID);
                    }
                    else
                    {
                        this._owner = this.EAModel.getElementWrapperByPackageID(this.wrappedElement.PackageID);
                    }
                }

                return (Element)this.getProperty(getPropertyNameName(), this._owner);
            }
            set
            {
                this._owner = (Element)value;
                this.setProperty(getPropertyNameName(), value, this._owner);
            }
        }

        private HashSet<UML.Classes.Kernel.Classifier> _superClasses;
        /// EA provides a shortcut tot the superclasses through its 
        /// element.BaseClasses
        /// Normally we would get those via the element.generalizations.general
        public HashSet<UML.Classes.Kernel.Classifier> superClasses
        {
            get
            {
                if (_superClasses == null)
                {
                   _superClasses = new HashSet<UML.Classes.Kernel.Classifier>
                                    (this.EAModel.factory.createElements(this.wrappedElement.BaseClasses)
                                    .OfType<UML.Classes.Kernel.Classifier>());
                }
                return _superClasses;
            }
            
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Kernel.Classifier> subClasses
        {
            get
            {
                var returnedSubclasses = new HashSet<UML.Classes.Kernel.Classifier>();
                foreach (var subclass in this.generalizations.Where(x => this.Equals(x.target)).Select(y => y.source as UML.Classes.Kernel.Classifier))
                {
                    if (subclass != null)
                    {
                        returnedSubclasses.Add(subclass);
                    }
                }
                return returnedSubclasses;
            }
        }
        /// <summary>
        /// all the subclasses, including the subclasses of the subclasses recursively
        /// </summary>
        public HashSet<UML.Classes.Kernel.Classifier> allSubClasses
        {
            get
            {
                var _allSubclasses = this.subClasses.Cast<ElementWrapper>().ToList();
                var directSubClasses = new List<ElementWrapper>(_allSubclasses);
                foreach(var directSubClass in directSubClasses)
                {
                    _allSubclasses.AddRange(directSubClass.allSubClasses.Cast<ElementWrapper>());
                }
                return new HashSet<UML.Classes.Kernel.Classifier>(_allSubclasses.Cast<UML.Classes.Kernel.Classifier>());
            }
        }
        /// the generalisations (inheritance relations) of this element
        public HashSet<UML.Classes.Kernel.Generalization> generalizations
        {
            get => new HashSet<UML.Classes.Kernel.Generalization>
                  (this.getRelationships<UML.Classes.Kernel.Generalization>());
            set => throw new NotImplementedException();
        }

        private HashSet<UML.Classes.Kernel.Operation> _ownedOperations;
        /// the operations owned by this element
        public HashSet<UML.Classes.Kernel.Operation> ownedOperations
        {
            get
            {
                if (this._ownedOperations == null)
                {
                    this._ownedOperations = new HashSet<UML.Classes.Kernel.Operation>
                    (this.EAModel.factory.createElements(this.wrappedElement.Methods)
                    .Cast<UML.Classes.Kernel.Operation>());
                }
                return this._ownedOperations;
            }
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// returns the Relationships with the given type T
        /// </summary>
        /// <returns>the relations of type T</returns>
        public override List<T> getRelationships<T>(bool outgoing = true, bool incoming = true)
        {
            if (this._allRelationships == null)
            {
                //check if we can find them quickly via a query
                var typedRelations = getRelationsByQuery<T>(outgoing, incoming);
                if (typedRelations != null) return typedRelations;
                //get them the regular way
                //to make sure the connectors collection is still accurate we do a refresh first
                this.WrappedElement.Connectors.Refresh();
                this._allRelationships = this.EAModel.factory.createElements(this.wrappedElement.Connectors).Cast<UML.Classes.Kernel.Relationship>().ToList();
            }
            List<T> returnedRelationships = new List<T>();
            // we still need to filter out those relationships that are there because of linked features
            foreach (UML.Classes.Kernel.Relationship relationship in this._allRelationships)
            {
                if (relationship is T)
                {
                    foreach (UML.Classes.Kernel.Element relatedElement in relationship.relatedElements)
                    {
                        if (this.Equals(relatedElement))
                        {
                            if (incoming && outgoing)
                                returnedRelationships.Add((T)relationship);
                            else if (incoming && this.Equals(((ConnectorWrapper) relationship).sourceElement))
                                returnedRelationships.Add((T)relationship);
                            else if (outgoing && this.Equals(((ConnectorWrapper)relationship).targetElement))
                                returnedRelationships.Add((T)relationship);
                        }
                    }
                }
            }
            return returnedRelationships;
        }
        /// <summary>
        /// gets the relations of the given type using a query (a lot faster)
        /// will return an empty list if no relations found. 
        /// will return null if the type is not supported
        /// </summary>
        /// <typeparam name="T">the type of relation</typeparam>
        /// <returns>the relations of the given type connected to this element</returns>
        private List<T> getRelationsByQuery<T>(bool outgoing, bool incoming)
        {
            var foundRelations = new List<T>();
            var relationTypeName = typeof(T).Name;
            var eaRelationType = relationTypeName;
            switch (relationTypeName)
            {
                //these are the supported types. Add translation if needed
                case "Association":
                case "Dependency":
                case "Generalization":
                case "Abstraction":
                    break;
                default:
                    //not one of the supported types
                    return null;
            }
            //create the queries
            if (outgoing)
            {
                var outgoingQuery = " select c.Connector_ID                                     " +
                                " from t_connector c                                        " +
                                " inner join t_object o on (o.Object_ID = c.Start_Object_ID " +
                                $" 					and o.ea_guid = '{this.uniqueID}')      " +
                                " where                                                     " +
                                $" c.Connector_Type = '{eaRelationType}'                    ";

                foundRelations.AddRange(this.EAModel.getRelationsByQuery(outgoingQuery).OfType<T>().ToList());
            }
            if (incoming)
            {
                var incomingquery = " select c.Connector_ID                                     " +
                                " from t_connector c                                        " +
                                " inner join t_object o on (o.Object_ID = c.End_Object_ID   " +
                                $" 					and o.ea_guid = '{this.uniqueID}')      " +
                                " where                                                     " +
                                $" c.Connector_Type = '{eaRelationType}'                    ";

                foundRelations.AddRange(this.EAModel.getRelationsByQuery(incomingquery).OfType<T>().ToList());
            }
            //return the found connectors
            return foundRelations;
        }

        public override List<UML.Classes.Kernel.Relationship> relationships
        {
            get => this.getRelationships<UML.Classes.Kernel.Relationship>();
            set => throw new NotImplementedException();
        }

        /// from Dependencies.Named Element.
        /// contains the dependencies that have this element as client
        public List<UML.Classes.Dependencies.Dependency> clientDependencies
        {
            get
            {
                List<UML.Classes.Dependencies.Dependency> returnedDependencies =
                  new List<UML.Classes.Dependencies.Dependency>();
                foreach (UML.Classes.Dependencies.Dependency dependency
                        in this.getRelationships<Dependency>())
                {
                    if (dependency.client.Equals(this))
                    {
                        returnedDependencies.Add(dependency);
                    }
                }
                return returnedDependencies;
            }
            set => throw new NotImplementedException();
        }
        /// from Dependencies.Named Element.
        /// contains the dependencies that have this element as supplier
        public List<UML.Classes.Dependencies.Dependency> supplierDependencies
        {
            get
            {
                List<UML.Classes.Dependencies.Dependency> returnedDependencies =
                  new List<UML.Classes.Dependencies.Dependency>();
                foreach (UML.Classes.Dependencies.Dependency dependency
                         in this.getRelationships<Dependency>())
                {
                    if (dependency.supplier.Equals(this))
                    {
                        returnedDependencies.Add(dependency);
                    }
                }
                return returnedDependencies;
            }
            set => throw new NotImplementedException();
        }

        public String qualifiedName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.Namespace owningNamespace
        {
            get => this.EAModel.getElementWrapperByPackageID(this.wrappedElement.PackageID) as Package;
            set => throw new NotImplementedException();
        }

        /// two elementwrappers represent the same object 
        /// if their ea.guid are the same
        public override bool Equals(object obj)
        {
            ElementWrapper otherElement = obj as ElementWrapper;
            return otherElement != null
                && otherElement.wrappedElement != null
                && this.wrappedElement != null
              && this.wrappedElement.ElementGUID == otherElement.wrappedElement.ElementGUID;
        }

        /// return the hashcode based on the elements guid
        public override int GetHashCode()
        {
            return new Guid(this.wrappedElement.ElementGUID).GetHashCode();
        }
        /// <summary>
        /// creates a new diagram under this element
        /// </summary>
        /// <param name="name">the name of the new diagram</param>
        /// <returns>the new diagram</returns>
        public virtual T addOwnedDiagram<T>(String name) where T : class, UML.Diagrams.Diagram
        {
            return ((Factory)this.EAModel.factory).addNewDiagramToEACollection<T>(this.wrappedElement.Diagrams, name);
        }
        /// creates a new element of the given type as an owned element of this 
        /// element
        public T addOwnedElement<T>(String name)
          where T : class, UML.Classes.Kernel.Element
        {
            return this.addOwnedElement<T>(name, typeof(T).Name);
        }
        /// creates a new element of the given type as an owned element of this 
        /// element
        public virtual T addOwnedElement<T>(String name, string EAType)
          where T : class, UML.Classes.Kernel.Element
        {
            System.Type type = typeof(T);
            T newElement;

            if (((Factory)this.EAModel.factory).isEAAtttribute(type))
            {
                newElement = ((Factory)this.EAModel.factory).addElementToEACollection<T>(this.wrappedElement.Attributes, name, string.Empty);
                //reset the cached attributes
                this.resetAttributes();
            }
            else if (((Factory)this.EAModel.factory).isEAOperation(type))
            {
                newElement = ((Factory)this.EAModel.factory).addElementToEACollection<T>(this.wrappedElement.Methods, name, string.Empty);
            }
            else if (((Factory)this.EAModel.factory).isEAConnector(type))
            {
                newElement = ((Factory)this.EAModel.factory).addElementToEACollection<T>(this.wrappedElement.Connectors, name, EAType);
                //if we add a connector to the element wrapper we have to make sure to reset the connectors
                this.resetRelationships();
            }
            else if (((Factory)this.EAModel.factory).isEAParameter(type))
            {
                newElement = ((Factory)this.EAModel.factory).addElementToEACollection<T>(this.wrappedElement.Connectors, name, EAType);
            }
            else if (type.Name == "Constraint")
            {
                newElement = ((Factory)this.EAModel.factory).addElementToEACollection<T>(this.wrappedElement.Constraints, name, EAType);
            }
            else
            {
                newElement = ((Factory)this.EAModel.factory).addElementToEACollection<T>(this.wrappedElement.Elements, name, EAType);
            }
            return newElement;
        }




        public virtual HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> ownedDiagrams
        {
            get
            {
                HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> diagrams = new HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram>();
                foreach (global::EA.Diagram eaDiagram in this.wrappedElement.Diagrams)
                {
                    diagrams.Add(((Factory)this.EAModel.factory).createDiagram(eaDiagram));
                }
                return diagrams;
            }
            set => throw new NotImplementedException();
        }
        //returns a list of diagrams that somehow use this element.
        public override HashSet<T> getUsingDiagrams<T>()
        {
            string sqlGetDiagrams = @"select distinct d.Diagram_ID from t_DiagramObjects d
                                  where d.Object_ID = " + this.wrappedElement.ElementID;
            List<UML.Diagrams.Diagram> allDiagrams = this.EAModel.getDiagramsByQuery(sqlGetDiagrams).Cast<UML.Diagrams.Diagram>().ToList(); ; ;
            HashSet<T> returnedDiagrams = new HashSet<T>();
            foreach (UML.Diagrams.Diagram diagram in allDiagrams)
            {
                if (diagram is T)
                {
                    T typedDiagram = (T)diagram;
                    if (!returnedDiagrams.Contains(typedDiagram))
                    {
                        returnedDiagrams.Add(typedDiagram);
                    }
                }
            }
            return returnedDiagrams;
        }
        /// <summary>
        /// returns the attributes that use this Element as type
        /// </summary>
        /// <returns>the attributes that use this Element as type</returns>
        public HashSet<UML.Classes.Kernel.Property> getUsingAttributes()
        {
            string sqlGetAttributes = @"select a.ea_guid from t_attribute a
    								where a.Classifier = '" + this.wrappedElement.ElementID.ToString() + "'";
            return new HashSet<UML.Classes.Kernel.Property>(this.EAModel.getAttributesByQuery(sqlGetAttributes));

        }
        /// <summary>
        /// gets the TypedElements of the given type that use this element as type
        /// </summary>
        /// <returns>the TypedElements that use this element as type</returns>
        public HashSet<T> getDependentTypedElements<T>() where T : UML.Classes.Kernel.TypedElement
        {
            HashSet<T> dependentTypedElements = new HashSet<T>();
            // get the attributes
            foreach (UML.Classes.Kernel.Property attribute in this.getUsingAttributes())
            {
                if (attribute is T)
                {
                    dependentTypedElements.Add((T)attribute);
                }
            }
            // get the parameters
            foreach (UML.Classes.Kernel.Parameter parameter in this.getUsingParameters())
            {
                if (parameter is T)
                {
                    dependentTypedElements.Add((T)parameter);
                }
            }
            return dependentTypedElements;
        }
        /// <summary>
        /// convenience method to return the Information flows that convey this classifier
        /// </summary>
        /// <returns>all InformationFlows that convey this classifier</returns>
        public HashSet<UML.InfomationFlows.InformationFlow> getConveyingFlows()
        {
            var conveyingFlows = new HashSet<UML.InfomationFlows.InformationFlow>();
            string sqlGetInformationFlows = @"select c.Connector_ID
									from ( t_xref x 
									inner join t_connector c on c.ea_guid like x.client)
									where x.Name = 'MOFProps' 
									and x.Behavior = 'conveyed'
									and x.Description like '%" + this.guid + "%'";
            foreach (var connector in this.EAModel.getRelationsByQuery(sqlGetInformationFlows))
            {
                InformationFlow informationFlow = connector as InformationFlow;
                if (informationFlow != null)
                {
                    conveyingFlows.Add(informationFlow);
                }
            }
            return conveyingFlows;
        }



        /// <summary>
        /// returns the parameters having use this Element as type
        /// </summary>
        /// <returns>the parameters that use this element as type</returns>
        public HashSet<UML.Classes.Kernel.Parameter> getUsingParameters()
        {
            // get the "regular" parameters
            string sqlGetParameters = @"select p.ea_guid from t_operationparams p
    								where p.Classifier = '" + this.wrappedElement.ElementID.ToString() + "'";
            HashSet<UML.Classes.Kernel.Parameter> parameters = new HashSet<UML.Classes.Kernel.Parameter>(this.EAModel.getParametersByQuery(sqlGetParameters));
            // get the return parameters
            foreach (Operation operation in this.getOperationsWithMeAsReturntype())
            {
                parameters.Add(((Factory)this.EAModel.factory).createEAParameterReturnType(operation));
            }
            return parameters;
        }
        /// <summary>
        /// returns the operations that have this element as return type
        /// </summary>
        /// <returns>the operations that have this element as return type</returns>
        public HashSet<UML.Classes.Kernel.Operation> getOperationsWithMeAsReturntype()
        {
            // get the return-parameters
            string sqlGetReturnParameters = @"select o.OperationID from t_operation o
    								where o.Classifier = '" + this.wrappedElement.ElementID.ToString() + "'";
            return new HashSet<UML.Classes.Kernel.Operation>(this.EAModel.getOperationsByQuery(sqlGetReturnParameters));
        }
        public override void open()
        {
            this.EAModel.selectedElement = this;
        }
        public override void select()
        {
            this.EAModel.selectedElement = this;
        }
        /// <summary>
        /// gets the item from the given relative path.
        /// </summary>
        /// <param name="relativePath">the "." separated path</param>
        /// <returns>the item with the given path</returns>
        public override UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
        {
            UML.Extended.UMLItem item = null;
            List<string> filteredPath = new List<string>(relativePath);
            if (filterName(filteredPath, this.name))
            {
                if (filteredPath.Count > 1)
                {
                    //remove first item from filteredPath
                    filteredPath.RemoveAt(0);
                    //search deeper
                    foreach (UML.Classes.Kernel.Element element in this.ownedElements)
                    {

                        item = element.getItemFromRelativePath(filteredPath);
                        if (item != null)
                        {
                            return item;
                        }
                    }
                    //still not found, now search diagram
                    foreach (UML.Diagrams.Diagram diagram in this.ownedDiagrams)
                    {
                        item = diagram.getItemFromRelativePath(filteredPath);
                        if (item != null)
                        {
                            return item;
                        }
                    }

                }
                else
                {
                    item = this;
                }
            }

            return item;
        }
        internal static bool filterName(List<string> path, string name)
        {
            List<string> nameparts = name.Split('.').ToList<string>();
            List<string> newPath = new List<string>();
            bool found = false;
            if (path.Count > 0
                && path.Count >= nameparts.Count)
            {
                foreach (string namePart in nameparts)
                {
                    // if "(" is present in the path, and not in the name
                    // then only check the part before the "("
                    if (path[0].Contains("(")
                        && !name.Contains("("))
                    {
                        path[0] = path[0].Substring(0, path[0].IndexOf("("));
                    }
                    if (namePart == path[0])
                    {
                        //pop the first one of the path
                        path.RemoveAt(0);
                    }
                    else
                    {
                        //not correct, don't bother searching further
                        return false;
                    }
                }
                //all strings matched, add name as first item;
                path.Insert(0, name);
                found = true;
            }
            return found;
        }
        public TSF.UmlToolingFramework.UML.CommonBehaviors.BasicBehaviors.BehavioralFeature specification
        {
            get
            {
                string sqlQuery = "select OperationID from t_operation where Behaviour like '" + this.wrappedElement.ElementGUID + "'";
                List<Operation> operations = this.EAModel.getOperationsByQuery(sqlQuery);
                if (operations.Count > 0)
                {
                    return operations[0];
                }
                else
                {
                    return null;
                }
            }
            set => throw new NotImplementedException();
        }

        public override TSF.UmlToolingFramework.UML.Diagrams.Diagram compositeDiagram
        {
            get => this.EAModel.factory.createDiagram(this.wrappedElement.CompositeDiagram);
            set => this.wrappedElement.SetCompositeDiagram(((Diagram)value).diagramGUID);
        }




        internal override global::EA.Collection eaTaggedValuesCollection => this.wrappedElement.TaggedValues;

        public override string guid => this.WrappedElement.ElementGUID;
        protected override string getTaggedValueQuery(string taggedValueName)
        {
            return @"select tv.ea_guid from t_objectproperties tv
			where 
			tv.Property = '" + taggedValueName + "' and tv.Object_ID = " + this.id;
        }

        /// <summary>
        /// deletes an element owned by this element
        /// </summary>
        /// <param name="ownedElement">the owned element to delete</param>
        public override void deleteOwnedElement(Element ownedElement)
        {
            if (ownedElement is ElementWrapper)
            {
                for (short i = 0; i < this.WrappedElement.Elements.Count; i++)
                {
                    var eaElement = this.WrappedElement.Elements.GetAt(i) as global::EA.Element;
                    if (eaElement?.ElementGUID == ownedElement.guid)
                    {
                        this.WrappedElement.Elements.Delete(i);
                        this.WrappedElement.Elements.Refresh();
                        break;
                    }
                }
            }
            else if (ownedElement is AttributeWrapper)
            {
                for (short i = 0; i < this.WrappedElement.Attributes.Count; i++)
                {
                    var eaAttribute = this.WrappedElement.Attributes.GetAt(i) as global::EA.Attribute;
                    if (eaAttribute?.AttributeGUID == ownedElement.guid)
                    {
                        this.WrappedElement.Attributes.Delete(i);
                        this.WrappedElement.Attributes.Refresh();
                        this.resetAttributes();
                        break;
                    }
                }
            }
            else if (ownedElement is ConnectorWrapper)
            {
                for (short i = 0; i < this.WrappedElement.Connectors.Count; i++)
                {
                    var eaConnector = this.WrappedElement.Connectors.GetAt(i) as global::EA.Connector;
                    if (eaConnector?.ConnectorGUID == ownedElement.guid)
                    {
                        this.WrappedElement.Connectors.Delete(i);
                        this.WrappedElement.Connectors.Refresh();
                        this.resetRelationships();
                        break;
                    }
                }
            }
            else if (ownedElement is Operation)
            {
                for (short i = 0; i < this.WrappedElement.Methods.Count; i++)
                {
                    var eaMethod = this.WrappedElement.Methods.GetAt(i) as global::EA.Method;
                    if (eaMethod?.MethodGUID == ownedElement.guid)
                    {
                        this.WrappedElement.Methods.Delete(i);
                        this.WrappedElement.Methods.Refresh();
                        break;
                    }
                }
            }
            else
            {
                //type not supported (yet)
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// returns the name of the user currently locking this element
        /// </summary>
        /// <returns>the name of the user currently locking this element</returns>
        public override string getLockedUser()
        {
            string lockedUser = string.Empty;
            //if (this.wrappedElement.Locked)
            //{
            string SQLQuery = @"select u.FirstName, u.Surname from t_seclocks s
                                    inner join t_secuser u on s.userID = u.userID 
                                    where s.entityID = '" + this.guid + "'";
            var result = this.EAModel.SQLQuery(SQLQuery);
            XmlNode firstNameNode = result.SelectSingleNode("//FirstName");
            XmlNode lastNameNode = result.SelectSingleNode("//Surname");
            if (firstNameNode != null && lastNameNode != null)
            {
                lockedUser = firstNameNode.InnerText + " " + lastNameNode.InnerText;
            }
            //}
            return lockedUser;
        }
        public override string getLockedUserID()
        {
            string SQLQuery = @"select s.UserID from t_seclocks s
                                    where s.entityID = '" + this.guid + "'";
            XmlDocument result = this.EAModel.SQLQuery(SQLQuery);
            XmlNode userIDNode = result.SelectSingleNode("//UserID");
            return userIDNode.InnerText;
        }
        /// <summary>
        /// Makes the element writeable
        /// </summary>
        /// <param name="overrideLocks">remove any existing locks</param>
        /// <returns>true if successful</returns>
		public override bool makeWritable(bool overrideLocks)
        {
            //if security is not enabled then it is always writeable
            if (!this.EAModel.isSecurityEnabled)
            {
                return true;
            }
            //if already writable return true
            if (!this.isReadOnly)
            {
                return true;
            }
            //TODO: override locks
            if (!this.isLocked)
            {
                //no lock found, go ahead and try to lock the element
                try
                {
                    return this.wrappedElement.ApplyUserLock();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                //lock found, don't even try it.
                return false;
            }
        }
        public HashSet<UML.Classes.Kernel.EnumerationLiteral> ownedLiterals
        {
            get
            {
                if (this._ownedLiterals == null)
                {
                    this._ownedLiterals = new HashSet<UML.Classes.Kernel.EnumerationLiteral>(this.attributeWrappers.OfType<UML.Classes.Kernel.EnumerationLiteral>());
                }
                return this._ownedLiterals;
            }
            set => throw new NotImplementedException();
        }

        List<Attribute> getOwnedAttributes(string attributeName)
        {
            //owner.Attribute
            string sqlGetAttributes = "select a.ea_guid from t_attribute a " +
                                    " where a.Object_ID = " + this.id +
                                    " and a.name = '" + attributeName + "'";
            return this.EAModel.getAttributesByQuery(sqlGetAttributes);
        }

        public List<ElementWrapper> getOwnedElements(string elementName)
        {
            string sqlGetOwnedElement = "select o.Object_ID from t_object o " +
                                        " where " +
                                        " o.Name = '" + elementName + "' " +
                                        " and o.ParentID = " + this.id;
            return this.EAModel.getElementWrappersByQuery(sqlGetOwnedElement);
        }
        public List<Diagram> getOwnedDiagrams(string diagramName)
        {
            string sqlGetOwnedDiagram = "select d.Diagram_ID from t_diagram d " +
                                        " where " +
                                        " d.Name = '" + diagramName + "' " +
                                        " and d.ParentID = " + this.id;
            return this.EAModel.getDiagramsByQuery(sqlGetOwnedDiagram);
        }


        /// <summary>
        /// get the associationEnds on the far end that correspond with the given rolename
        /// This is either when the rolename matches, or when the rolename is empty and the name of the opposite class matches
        /// </summary>
        /// <param name="rolename">the name we are looking for</param>
        /// <returns>a list of matching association ends</returns>
        public List<AssociationEnd> getRelatedAssociationEnds(string rolename)
        {
            var assocationEnds = new List<AssociationEnd>();
            //first get the assocations, then get the end that corresponds with the rolename
            //this is either when the rolename matches, or when the rolename is empty and the name of the opposite class matches
            string sqlGetConnectorWrappers =
                                        //association with the given rolename
                                        "select c.Connector_ID from t_connector c " +
                                        " where c.Start_Object_ID = " + this.id +
                                        " and c.DestRole = '" + rolename + "' " +
                                        " union " +
                                        " select c.Connector_ID from t_connector c " +
                                        " where c.End_Object_ID = " + this.id +
                                        " and c.SourceRole = '" + rolename + "' " +
                                        " union " +
                                        //associations without rolename where the other end has the given name
                                        " select c.Connector_ID from (t_connector c " +
                                        " inner join t_object o on c.End_Object_ID = o.Object_ID) " +
                                        " where c.Start_Object_ID = " + this.id +
                                        " and c.DestRole is null " +
                                        " and o.Name = '" + rolename + "' " +
                                        " union " +
                                        " select c.Connector_ID from (t_connector c " +
                                        " inner join t_object o on c.Start_Object_ID = o.Object_ID) " +
                                        " where c.End_Object_ID = " + this.id +
                                        " and c.SourceRole is null " +
                                        " and o.name = '" + rolename + "' " +
                                        //associations with the given name
                                        " union " +
                                        " select c.Connector_ID from t_connector c " +
                                        " where c.Start_Object_ID = " + this.id +
                                        " and c.Name = '" + rolename + "' " +
                                        " union " +
                                        " select c.Connector_ID from t_connector c " +
                                        " where c.End_Object_ID = " + this.id +
                                        " and c.Name = '" + rolename + "' ";
            var connectorWrappers = this.EAModel.getRelationsByQuery(sqlGetConnectorWrappers);
            //loop the associations to get the ends at the other side
            foreach (var connectorWrapper in connectorWrappers)
            {
                if (this.Equals(connectorWrapper.source))
                {
                    assocationEnds.Add(connectorWrapper.targetEnd);
                }
                else
                {
                    assocationEnds.Add(connectorWrapper.sourceEnd);
                }
            }
            return assocationEnds;
        }


        public override List<UML.Extended.UMLItem> findOwnedItems(List<string> descriptionParts)
        {
            List<UML.Extended.UMLItem> ownedItems = new List<UML.Extended.UMLItem>();
            if (descriptionParts.Count > 0)
            {
                string firstpart = descriptionParts[0];
                //start by finding an element with the given name
                var candidateItems = new List<Element>();
                candidateItems.AddRange(this.getOwnedElements(firstpart));
                candidateItems.AddRange(this.getOwnedAttributes(firstpart));
                candidateItems.AddRange(this.getRelatedAssociationEnds(firstpart));
                if (descriptionParts.Count > 1)
                {
                    //remove the first part
                    descriptionParts.RemoveAt(0);
                    //loop the candidates to get their owned items
                    foreach (var element in candidateItems)
                    {
                        //go one level down
                        ownedItems.AddRange(element.findOwnedItems(descriptionParts));
                    }
                }
                else
                {
                    //only one item so add the direct owned elements
                    ownedItems.AddRange(candidateItems);
                    //Add also the diagrams owned by this package
                    ownedItems.AddRange(this.getOwnedDiagrams(firstpart));
                }
            }
            return ownedItems;
        }
        /// <summary>
        /// exports all diagrams to the given path
        /// </summary>
        /// <param name="imagePath">the path to the folder where to store all images</param>
        public void exportAllDiagrams(string imagePath)
        {

            //the standard export feature has the unwanted side effect that it opens all the diagrams in the package tree
            //exporting as HTML does not have this side effect, but it requires to go into the html tree, and get each and every single image, 
            // and then look into the html to figure out to which diagram the image belongs to.
            //			//get the package guid, either of this element if it is a package or of it's owning package
            //			string packageGUID = this is UML.Classes.Kernel.Package ? this.uniqueID : this.owningPackage.uniqueID;
            //			// as an alternative we can generate an HTML export and get the diagrams from there
            //			this.model.wrappedModel.GetProjectInterface().RunHTMLReport(this.model.wrappedModel.GetProjectInterface().GUIDtoXML(packageGUID),
            //			                                                            imagePath,".png","<default>",".html");


            //first export all owned diagrams
            foreach (var diagram in this.ownedDiagrams)
            {
                diagram.saveImageToFile(imagePath);
            }
            //then loop owned elements
            foreach (var ownedElement in this.ownedElements.OfType<UML.Classes.Kernel.Namespace>())
            {
                ownedElement.exportAllDiagrams(imagePath);
            }
        }
    }
}
