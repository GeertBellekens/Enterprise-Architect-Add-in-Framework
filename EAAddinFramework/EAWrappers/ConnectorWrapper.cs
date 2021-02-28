using EAAddinFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class ConnectorWrapper : Element, UML.Classes.Kernel.Relationship
    {
        internal global::EA.Connector wrappedConnector { get; set; }
        private UML.Classes.Kernel.Element _source;
        private UML.Classes.Kernel.Element _target;
        private AssociationEnd _sourceEnd;
        private AssociationEnd _targetEnd;
        public ConnectorWrapper(Model model, global::EA.Connector connector)
          : base(model)
        {
            this.wrappedConnector = connector;
            this.isDirty = true;
        }

        public string guardCondition
        {
            get => this.wrappedConnector.MiscData[1].ToString();

            set => this.EAModel.executeSQL(@"update t_connector
									set [PDATA2] = '" + value + @"'
									where connector_id = " + this.id);
        }


        internal Element sourceElement
        {
            get
            {
                //check if elementwrapper
                var elementSource = this.source as ElementWrapper;
                if (elementSource != null)
                {
                    return elementSource;
                }
                //check if attributeWrapper
                var attributeSource = this.source as AttributeWrapper;
                if (attributeSource != null)
                {
                    return attributeSource;
                }
                //check if operationWrapper
                var operationSource = this.source as Operation;
                if (operationSource != null)
                {
                    return operationSource;
                }
                //hceck if connectorWrapper
                var connectorSource = this.source as ConnectorWrapper;
                if (connectorSource != null)
                {
                    return connectorSource;
                }
                //nothing found
                return null;
            }
        }
        internal Element targetElement
        {
            get
            {
                //check if elementwrapper
                var elementSource = this.target as ElementWrapper;
                if (elementSource != null)
                {
                    return elementSource;
                }
                //check if attributeWrapper
                var attributeSource = this.target as AttributeWrapper;
                if (attributeSource != null)
                {
                    return attributeSource;
                }
                //check if operationWrapper
                var operationSource = this.target as Operation;
                if (operationSource != null)
                {
                    return operationSource;
                }
                //hceck if connectorWrapper
                var connectorSource = this.target as ConnectorWrapper;
                if (connectorSource != null)
                {
                    return connectorSource;
                }
                //nothing found
                return null;
            }
        }
        public override string orderingName => this.targetName;
        public string targetName => string.IsNullOrEmpty(this.targetEnd.name) ? this.target.name : this.targetEnd.name;
        public string sourceName => string.IsNullOrEmpty(this.sourceEnd.name) ? this.source.name : this.sourceEnd.name;
        public int id => this.wrappedConnector.ConnectorID;


        public global::EA.Connector WrappedConnector => this.wrappedConnector;
        public override void open()
        {
            var diagrams = this.getDependentDiagrams();
            if (diagrams.Count > 0)
            {
                diagrams[0].open();
                diagrams[0].selectItem(this);
                diagrams[0].selectItem(this); // do this a second time to make sure it works, see https://www.sparxsystems.com/forums/smf/index.php/topic,45515.0.html
            }
            else
            {
                if (this.source != null)
                {
                    this.source.open();
                }
            }
        }
        public override HashSet<UML.Classes.Kernel.Element> ownedElements
        {
            get => new HashSet<UML.Classes.Kernel.Element>();
            set => throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public override string uniqueID => this.wrappedConnector.ConnectorGUID;
        public string alias
        {
            get => this.wrappedConnector.Alias;
            set => this.wrappedConnector.Alias = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<UML.Diagrams.Diagram> getDependentDiagrams()
        {
            var dependentDiagrams = new List<UML.Diagrams.Diagram>();
            string sqlQuery = "select dl.DiagramID as Diagram_ID from t_diagramlinks dl where dl.ConnectorID = " + this.id;
            dependentDiagrams.AddRange(this.EAModel.getDiagramsByQuery(sqlQuery));
            return dependentDiagrams;
        }

        /// <summary>
        /// not fully correct, but we will return the element at the source of the relation
        /// TODO: fix this so it uses the actual ownership as prescribed by UML
        /// </summary>
        public override UML.Classes.Kernel.Element owner
        {
            get => this.source;
            set => throw new NotImplementedException();
        }
        /// the stereotypes defined on this Relationship
        public override HashSet<UML.Profiles.Stereotype> stereotypes
        {
            get => ((Factory)this.EAModel.factory).createStereotypes
                (this, this.wrappedConnector.StereotypeEx);
            set => this.WrappedConnector.StereotypeEx = Stereotype.getStereotypeEx(value);
        }
        /// returns the related elements.
        /// In EA the Connectoris a binary relationship. So only two Elements will 
        /// ever be returned.
        /// If there is a linked feature then the linked feature will be returned instead of the EA.Element.
        public List<UML.Classes.Kernel.Element> relatedElements
        {
            get
            {
                var returnedElements = new List<UML.Classes.Kernel.Element>();
                returnedElements.Add(this.source);
                returnedElements.Add(this.target);
                return returnedElements;
            }
            set => throw new NotImplementedException();
        }
        private UML.Extended.UMLItem getLinkedFeature(bool isSource)
        {
            UML.Extended.UMLItem linkedFeature = null;
            //determine start or end keyword
            string key = isSource ? "LFSP" : "LFEP";
            string featureGUID = KeyValuePairsHelper.getValueForKey(key, this.wrappedConnector.StyleEx);
            if (!string.IsNullOrEmpty(featureGUID))
            {
                if (featureGUID.EndsWith("}R")
                    || featureGUID.EndsWith("}L"))
                {
                    //remove the last "R" or "L"
                    featureGUID = featureGUID.Substring(0, featureGUID.Length - 1);
                }
                //get the linked feature
                linkedFeature = this.EAModel.getItemFromGUID(featureGUID);
            }
            return linkedFeature;
        }
        private void setLinkedFeature(UML.Classes.Kernel.Element linkedFeature, bool isSource)
        {
            //check if attribute
            Element actualEnd = linkedFeature as Attribute;
            //or maybe operation
            if (actualEnd == null)
            {
                actualEnd = linkedFeature as Operation;
            }

            if (actualEnd != null)
            {

                //set the client id to the id of the owner
                if (isSource)
                {
                    this._source = linkedFeature;
                    this.wrappedConnector.ClientID = ((ElementWrapper)actualEnd.owner).id;
                }
                else
                {
                    this._target = linkedFeature;
                    this.wrappedConnector.SupplierID = ((ElementWrapper)actualEnd.owner).id;
                }
                //set the linked feature
                string key = isSource ? "LFSP" : "LFEP";
                string suffix = isSource ? "R" : "L";
                string styleEx = KeyValuePairsHelper.setValueForKey(key, linkedFeature.uniqueID + suffix, this.wrappedConnector.StyleEx);
                this.wrappedConnector.StyleEx = styleEx;
            }

        }
        public UML.Classes.Kernel.Element target
        {
            get
            {
                if (this._target == null)
                {
                    UML.Classes.Kernel.Element linkedSupplierFeature = this.getLinkedFeature(false) as UML.Classes.Kernel.Element;
                    if (linkedSupplierFeature != null)
                    {
                        this._target = linkedSupplierFeature;
                    }
                    else
                    {
                        var targetElementWrapper = this.EAModel.getElementWrapperByID(this.wrappedConnector.SupplierID);
                        this._target = targetElementWrapper;
                        //in case the source is linked to a connector there's a dummy element with type ProxyConnector
                        if (targetElementWrapper != null && targetElementWrapper.EAElementType == "ProxyConnector")
                        {
                            //get the source connector
                            this._source = this.EAModel.getRelationByID(targetElementWrapper.wrappedElement.ClassifierID);
                        }
                    }
                }
                return this._target;
            }
            set
            {
                if (value is ElementWrapper)
                {
                    this._target = value;
                    this.WrappedConnector.SupplierID = ((ElementWrapper)value).id;
                    //make sure the target's relationships are reset as well
                    ((ElementWrapper)value).resetRelationships();
                }
                else
                {
                    this.setLinkedFeature(value, false);
                }
            }
        }
        public override List<UML.Classes.Kernel.Relationship> relationships
        {
            get
            {
                string sqlGetRelationships = @"select c.Connector_ID
											from (t_connector c
											inner join t_object o on (o.Object_ID in (c.Start_Object_ID, c.End_Object_ID)
											and o.Object_Type = 'ProxyConnector'))
											where o.Classifier_guid = '" + this.uniqueID + "'";
                return this.EAModel.getRelationsByQuery(sqlGetRelationships).Cast<UML.Classes.Kernel.Relationship>().ToList();
            }
            set
            {
                base.relationships = value; ;
            }
        }

        /// <summary>
        /// returns a list of elements that are linked to this relationship (but not the related elements)
        /// these are the notes or constrains linked to the relation, or the elments linked to this relation through another relation
        /// </summary>
        /// <returns>the list of elements that are somehow related tot this relation direction (without being the source or target)</returns>
        public List<UML.Classes.Kernel.Element> getLinkedElements()
        {
            List<UML.Classes.Kernel.Element> foundElements = new List<UML.Classes.Kernel.Element>();
            //add the elements linked to this relation via another connector
            foundElements.AddRange(this.relationships.OfType<ConnectorWrapper>().Where(x => x.target != this).Select(y => y.target));
            foundElements.AddRange(this.relationships.OfType<ConnectorWrapper>().Where(x => x.source != this).Select(y => y.source));
            //then add the notes/constrains that are linked to this relation via a notelink
            string selecNoteLinkElements = "select o.Object_ID from t_object o where o.pdata4 like '%idref_=" + this.id + ";%'";
            foundElements.AddRange(this.EAModel.getElementWrappersByQuery(selecNoteLinkElements));
            //return the found elements
            return foundElements;
        }
        public void addLinkedElement(ElementWrapper element)
        {
            if (element is NoteComment
                || element is UML.Classes.Kernel.Constraint)
            {
                //set idref to the id of this relation to create the notelink
                string pdata4 = element.wrappedElement.MiscData[3].ToString();
                string idRefString = this.getNextIdRefString(pdata4);
                pdata4 = KeyValuePairsHelper.setValueForKey(idRefString, this.id.ToString(), pdata4);
                string sqlUpdateNoteLink = "update t_object set PDATA4 = '" + pdata4 + "' where ea_guid =" + element.uniqueID;
                this.EAModel.executeSQL(sqlUpdateNoteLink);
            }
            else
            {
                //create an element of type ProxyConnector with
                //t_object.ClassifierGUID = sourceConnectorGUID
                //t_object.Classifier = sourcConnector.ConnectorID
                var proxyConnector = this.EAModel.factory.createNewElement<ProxyConnector>(this.owningPackage, string.Empty);
                proxyConnector.connector = this;
                proxyConnector.save();
                //then create a connector between the ProxyConnector Element and the target element
                var elementLink = this.EAModel.factory.createNewElement<Dependency>(proxyConnector, string.Empty);
                elementLink.target = element;
                elementLink.save();
            }
        }
        private string getNextIdRefString(string pdata)
        {
            int i = 0;
            string idRefValue = null;
            do
            {
                i++;
                idRefValue = KeyValuePairsHelper.getValueForKey("idref" + i, pdata);
            }
            while (!string.IsNullOrEmpty(idRefValue));
            return "idref" + i;
        }
        public UML.Classes.Kernel.Element source
        {
            get
            {
                if (this._source == null)
                {
                    //check first if there is a linked feature
                    UML.Classes.Kernel.Element linkedClientFeature = this.getLinkedFeature(true) as UML.Classes.Kernel.Element;
                    if (linkedClientFeature != null)
                    {
                        this._source = linkedClientFeature;
                    }
                    else
                    {
                        var sourceElementWrapper = this.EAModel.getElementWrapperByID(this.wrappedConnector.ClientID);
                        this._source = sourceElementWrapper;
                        //in case the source is linked to a connector there's a dummy element with type ProxyConnector
                        if (sourceElementWrapper.EAElementType == "ProxyConnector")
                        {
                            //get the source connector
                            this._source = this.EAModel.getRelationByID(sourceElementWrapper.wrappedElement.ClassifierID);
                        }
                    }
                }
                return this._source;
            }
            set
            {
                if (value is ElementWrapper)
                {
                    this._source = value;
                    this.WrappedConnector.ClientID = ((ElementWrapper)value).id;
                    //make sure we reset the cached relations of the source
                    ((ElementWrapper)value).resetRelationships();
                }
                else
                {
                    this.setLinkedFeature(value, true);
                }
            }
        }


        public bool isDerived
        {
            get
            {
                foreach (global::EA.CustomProperty property in this.wrappedConnector.CustomProperties)
                {
                    if (property.Name == "isDerived")
                    {
                        return property.Value != "0" 
                            && ! property.Value.Equals("false",StringComparison.InvariantCultureIgnoreCase) ;
                    }
                }
                //return false by default
                return false;
            }
            set
            {
                foreach (global::EA.CustomProperty property in this.wrappedConnector.CustomProperties)
                {
                    if (property.Name == "isDerived")
                    {
                        property.Value = value ? "-1" : "0";
                    }
                }
            }
        }

        public List<UML.Classes.Kernel.Property> navigableOwnedEnds
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public List<UML.Classes.Kernel.Property> ownedEnds
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public List<UML.Classes.Kernel.Type> endTypes
        {
            get
            {
                var returnTypes = new List<UML.Classes.Kernel.Type>();
                foreach (UML.Classes.Kernel.Property end in this.memberEnds)
                {
                    returnTypes.Add(end.type);
                }
                return returnTypes;
            }
            set => throw new NotImplementedException();
        }

        /// Each end represents participation of instances of the classifier 
        /// connected to the end in links of the association. This is
        /// an ordered association. Subsets Namespace::member.
        public List<UML.Classes.Kernel.Property> memberEnds
        {
            get
            {
                var returnedMembers = new List<UML.Classes.Kernel.Property>();
                returnedMembers.Add(this.sourceEnd as UML.Classes.Kernel.Property);
                returnedMembers.Add(this.targetEnd as UML.Classes.Kernel.Property);
                return returnedMembers;
            }
            set => throw new NotImplementedException();
        }
        public AssociationEnd sourceEnd
        {
            get
            {
                if (this._sourceEnd == null)
                {
                    this._sourceEnd = ((Factory)this.EAModel.factory).createAssociationEnd(this, this.wrappedConnector.ClientEnd, false);
                }
                return this._sourceEnd;
            }
        }
        public AssociationEnd targetEnd
        {
            get
            {
                if (this._targetEnd == null)
                {
                    this._targetEnd = ((Factory)this.EAModel.factory).createAssociationEnd(this, this.wrappedConnector.SupplierEnd, true);
                }
                return this._targetEnd;
            }
        }
        public bool isAbstract
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Kernel.Generalization> generalizations
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public HashSet<UML.Classes.Kernel.Property> attributes
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Kernel.Feature> features
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool isLeaf
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Kernel.RedefinableElement> redefinedElements
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Kernel.Classifier> redefinitionContexts
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override string name
        {
            get => this.wrappedConnector.Name;
            set => this.wrappedConnector.Name = value;
        }

        public UML.Classes.Kernel.VisibilityKind visibility
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public String qualifiedName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.Namespace owningNamespace
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public UML.Classes.Kernel.NamedElement client
        {
            get => this.source as UML.Classes.Kernel.NamedElement;
            set => this.source = value as Element;
        }

        public UML.Classes.Kernel.NamedElement supplier
        {
            get => this.target as UML.Classes.Kernel.NamedElement;
            set => this.target = value as Element;
        }

        public UML.Classes.Kernel.OpaqueExpression mapping
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public HashSet<UML.Classes.Dependencies.Substitution> substitutions
        {
            get => throw new NotImplementedException();
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
        /// <summary>
        /// convenience method to return the information flows that realize this Relationship
        /// </summary>
        /// <returns>the information flows that realize this Relationship</returns>
        public virtual HashSet<UML.InfomationFlows.InformationFlow> getInformationFlows()
        {
            HashSet<UML.InfomationFlows.InformationFlow> informationFlows = new HashSet<UML.InfomationFlows.InformationFlow>();
            string sqlGetInformationFlowIDs = @"select x.description
			from (t_connector c
			inner join t_xref x on (x.client = c.ea_guid and x.Name = 'MOFProps'))
			where c.ea_guid = '" + this.guid + "'";
            var queryResult = this.EAModel.SQLQuery(sqlGetInformationFlowIDs);
            var descriptionNode = queryResult.SelectSingleNode(this.EAModel.formatXPath("//description"));
            if (descriptionNode != null)
            {
                foreach (string ifGUID in descriptionNode.InnerText.Split(','))
                {
                    var informationFlow = this.EAModel.getRelationByGUID(ifGUID) as UML.InfomationFlows.InformationFlow;
                    if (informationFlow != null)
                    {
                        informationFlows.Add(informationFlow);
                    }
                }
            }
            return informationFlows;
        }

        /// <summary>
        /// Due to a bug in EA we first need to save the end with aggregation kind none, and then the other end.
        /// If not then the aggregationkind of the other set is reset to none.
        /// </summary>
        public override void save()
        {
            //set the direction
            this.setDirection();
            //save wrapped connector
            this.WrappedConnector.Update();

            if (this.sourceEnd.aggregation == UML.Classes.Kernel.AggregationKind.none)
            {
                this.sourceEnd.save();
                this.targetEnd.save();
            }
            else
            {
                this.targetEnd.save();
                this.sourceEnd.save();
            }
        }
        /// <summary>
        /// set the direction on the connector based on the navigability of the ends
        /// </summary>
        protected virtual void setDirection()
        {
            string direction = "Unspecified"; //default
            if (this.targetEnd != null && this.sourceEnd != null)
            {
                if (this.targetEnd.isNavigable)
                {
                    if (this.sourceEnd.isNavigable)
                    {
                        direction = "Bi-Directional";
                    }
                    else
                    {
                        direction = "Source -> Destination";
                    }
                }
                else
                {
                    if (this.sourceEnd.isNavigable)
                    {
                        direction = "Destination -> Source";
                    }
                    else
                    {
                        direction = "Unspecified";
                    }
                }
            }
            this.wrappedConnector.Direction = direction;
        }

        internal override void saveElement()
        {
            this.wrappedConnector.Update();
        }

        public override string notes
        {
            get => this.wrappedConnector.Notes;
            set => this.wrappedConnector.Notes = value;
        }


        public override TSF.UmlToolingFramework.UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
        {
            UML.Extended.UMLItem item = null;
            if (ElementWrapper.filterName(relativePath, this.name))
            {
                if (relativePath.Count == 1)
                {
                    item = this;
                }
            }
            return this;
        }


        #region Equals and GetHashCode implementation
        public override bool Equals(object obj)
        {
            ConnectorWrapper other = obj as ConnectorWrapper;
            if (other != null)
            {
                if (other.wrappedConnector.ConnectorGUID == this.wrappedConnector.ConnectorGUID)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return new Guid(this.wrappedConnector.ConnectorGUID).GetHashCode();
        }
        #endregion
        /// <summary>
        /// adding both the start and the end element to the diagram will automatically add the connector to the diagram
        /// </summary>
        public override void addToCurrentDiagram()
        {
            foreach (UML.Classes.Kernel.Element element in this.relatedElements)
            {
                element.addToCurrentDiagram();
            }
        }

        /// <summary>
        /// adds a related element to this connector.
        /// This operation checks if the source or target is empty and then adds the related element to the empty spot.
        /// If none of the two are empty then the target is being replaced
        /// </summary>
        /// <param name="relatedElement"></param>
        public void addRelatedElement(UML.Classes.Kernel.Element relatedElement)
        {
            if (this.WrappedConnector.ClientID <= 0)
            {
                this.source = relatedElement;
            }
            else
            {
                this.target = relatedElement;
            }

        }

        internal override global::EA.Collection eaTaggedValuesCollection => this.WrappedConnector.TaggedValues;

        public override string guid => this.WrappedConnector.ConnectorGUID;

        #region implemented abstract members of Element

        public override void deleteOwnedElement(Element ownedElement)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region implemented abstract members of Element

        public override bool makeWritable(bool overrideLocks)
        {
            return this.source.makeWritable(overrideLocks);
        }

        public override string getLockedUser()
        {
            if (this.sourceElement != null)
            {
                return this.sourceElement.getLockedUser();
            }

            return string.Empty;
        }

        public override string getLockedUserID()
        {
            if (this.sourceElement != null)
            {
                return this.sourceElement.getLockedUserID();
            }

            return string.Empty;
        }
        protected override string getTaggedValueQuery(string taggedValueName)
        {
            return @"select tv.ea_guid from t_connectortag tv
			where 
			tv.Property = '" + taggedValueName + "' and tv.ElementID = " + this.id;
        }

        #endregion
    }
}
