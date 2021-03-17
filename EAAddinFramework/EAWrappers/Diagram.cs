using System;
using System.Collections.Generic;
using System.Linq;

using System.Xml;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using UML = TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class Diagram : UML.Diagrams.Diagram
    {
        public global::EA.Diagram wrappedDiagram { get; set; }

        public Model model;

        public Diagram(Model model, global::EA.Diagram wrappedDiagram)
        {
            this.model = model;
            this.wrappedDiagram = wrappedDiagram;
        }
        /// <summary>
        /// return the unique ID of this element
        /// </summary>
        public string uniqueID
        {
            get
            {
                return this.wrappedDiagram.DiagramGUID;
            }
        }
        /// all elements shown on this diagram.
        /// Currently only diagramObjectWrappers and relations
        public HashSet<UML.Diagrams.DiagramElement> diagramElements
        {
            get
            {
                List<UML.Diagrams.DiagramElement> returnedDiagramElements =
                  new List<UML.Diagrams.DiagramElement>
                    (this.diagramObjectWrappers);
                returnedDiagramElements.AddRange
                  (this.diagramLinkWrappers.Cast<UML.Diagrams.DiagramElement>());
                return new HashSet<UML.Diagrams.DiagramElement>
                  (returnedDiagramElements);
            }
            set { throw new NotImplementedException(); }
        }
        public List<UML.Classes.Kernel.Element> elements
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        /// <summary>
        /// returns a list of diagrams that show this item.
        /// </summary>
        /// <returns>all diagrams that show this item</returns>
        public virtual List<UML.Diagrams.Diagram> getDependentDiagrams()
        {
            //TODO: implement operation
            return new List<UML.Diagrams.Diagram>();
        }
        public HashSet<UML.Diagrams.DiagramElement> getDiagramElements<T>()
          where T : UML.Classes.Kernel.Element
        {
            HashSet<UML.Diagrams.DiagramElement> returnedDiagramElements =
              new HashSet<UML.Diagrams.DiagramElement>();
            foreach (UML.Diagrams.DiagramElement diagramElement
                    in this.diagramElements)
            {
                if (diagramElement.element is T)
                {
                    returnedDiagramElements.Add(diagramElement);
                }
            }
            return returnedDiagramElements;
        }

        public String name
        {
            get { return this.wrappedDiagram.Name; }
            set { this.wrappedDiagram.Name = value; }
        }
        public string diagramType
        {
            get => this.wrappedDiagram.Type;
        }

        /// find the diagramlink object (if any) that represents the given 
        /// relation
        public global::EA.DiagramLink getDiagramLinkForRelation
          (ConnectorWrapper relation)
        {
            foreach (global::EA.DiagramLink diagramLink
                    in this.wrappedDiagram.DiagramLinks)
            {
                if (diagramLink.ConnectorID == relation.wrappedConnector.ConnectorID)
                {
                    return diagramLink;
                }
            }
            return null;
        }

        public HashSet<DiagramLinkWrapper> diagramLinkWrappers
        {
            get
            {
                List<ConnectorWrapper> relations = this.getRelations();
                return new HashSet<DiagramLinkWrapper>
                  (((Factory)this.model.factory).createDiagramElements
                    (relations.Cast<UML.Classes.Kernel.Element>().ToList(),
                     this).Cast<DiagramLinkWrapper>());
            }
        }
        private HashSet<UML.Diagrams.DiagramElement> _diagramObjectWrappers;
        public HashSet<UML.Diagrams.DiagramElement> diagramObjectWrappers
        {
            get
            {
                if (_diagramObjectWrappers == null)
                {
                    _diagramObjectWrappers = ((Factory)this.model.factory).createDiagramElements
                (this.wrappedDiagram.DiagramObjects);
                }
                return _diagramObjectWrappers;
            }
        }
        public string diagramGUID
        {
            get
            {
                return this.wrappedDiagram.DiagramGUID;
            }
        }


        /// <summary>
        /// the relations on a diagram in EA are sometimes expressed as 
        /// DiagramLink but not always.
        /// We are looking for all relations that have both their ends displayed 
        /// on the diagram.
        /// To make this a bit faster the list of id's is retrieved using an sql 
        /// query
        /// </summary>
        /// <returns>all reations ont he diagram</returns>
        public virtual List<ConnectorWrapper> getRelations()
        {
            string SQLQuery = @"
      SELECT c.Connector_ID
        FROM (((( t_Connector c 
      INNER JOIN t_object source ON c.start_object_id = source.object_id )
      INNER JOIN t_object target ON c.end_object_id = target.object_id )
      INNER JOIN t_diagramObjects s ON source.object_id = s.object_id )
      INNER JOIN t_diagramObjects t ON target.object_id = t.object_id )
      WHERE s.Diagram_ID = " + this.wrappedDiagram.DiagramID +
           "  AND t.Diagram_ID = " + this.wrappedDiagram.DiagramID + ";";
            return this.model.getRelationsByQuery(SQLQuery);
        }

        public global::EA.DiagramObject getdiagramObjectForElement
          (ElementWrapper element)
        {
            foreach (global::EA.DiagramObject diagramObject
                in this.wrappedDiagram.DiagramObjects)
            {
                if (diagramObject.ElementID == element.wrappedElement.ElementID)
                {
                    return diagramObject;
                }
            }
            return null;
        }

        public int height
        {
            get { return this.wrappedDiagram.cy; }
            set { this.wrappedDiagram.cy = value; }
        }

        public int width
        {
            get { return this.wrappedDiagram.cx; }
            set { this.wrappedDiagram.cx = value; }
        }
        public UML.Classes.Kernel.Element owner
        {
            get
            {
                if (this.wrappedDiagram.ParentID != 0)
                {
                    return this.model.getElementWrapperByID
                      (this.wrappedDiagram.ParentID);
                }
                else
                {
                    return this.model.getElementWrapperByPackageID
                      (this.wrappedDiagram.PackageID);
                }
            }
            set
            {
                throw new NotImplementedException();

                //this.wrappedDiagram.PackageID = ((Element)value).
            }
        }

        public virtual UML.Classes.Kernel.Package owningPackage
        {
            get
            {
                var ownerPackage = this.owner as Package;
                //if the owner is a package then return the owner
                if (ownerPackage != null) return ownerPackage;
                //if the owner is not a package and it exists then return the owningPackage of the owner
                if (this.owner != null) return ((Element)this.owner).owningPackage;
                //the owner does not exis, so return null as the owningPackage
                return null;
            }
            set
            {
                //throw new NotImplementedException();
                this.wrappedDiagram.PackageID = ((Package)value).packageID;

            }
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

        public void save()
        {
            this.wrappedDiagram.Update();
        }

        public int DiagramID
        {
            get { return this.wrappedDiagram.DiagramID; }
        }

        public void open()
        {
            this.model.currentDiagram = this;
        }

        public String comment
        {
            get { return this.wrappedDiagram.Notes; }
            set { this.wrappedDiagram.Notes = value; }
        }
        public override int GetHashCode()
        {
            return this.DiagramID;
        }
        public override bool Equals(object obj)
        {
            return obj is Diagram && ((Diagram)obj).GetHashCode() == this.GetHashCode();
        }

        public void select()
        {
            this.model.selectDiagram(this);
        }
        /// <summary>
        /// searches downward for the item with the given relative path
        /// This relative path includes the own name
        /// </summary>
        /// <param name="relativePath">list of names inlcuding the own name</param>
        /// <returns>the item matching the path</returns>
        public TSF.UmlToolingFramework.UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
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
        public string fqn
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
        /// returns all operations called in this sequence diagram
        /// </summary>
        /// <returns>all operations called in this sequence diagram</returns>
        public List<UML.Classes.Kernel.Operation> getCalledOperations()
        {
            List<UML.Classes.Kernel.Operation> calledOperations = new List<UML.Classes.Kernel.Operation>();
            foreach (DiagramLinkWrapper linkwrapper in this.diagramLinkWrappers)
            {
                Message message = linkwrapper.relation as Message;
                if (message != null)
                {
                    UML.Classes.Kernel.Operation operation = message.calledOperation;
                    if (operation != null)
                    {
                        calledOperations.Add(operation);
                    }
                }
            }
            return calledOperations;
        }
        /// <summary>
        /// opens the (standard) properties dialog in EA
        /// </summary>
        public void openProperties()
        {
            this.model.openProperties(this);
        }

        public void clearSelection()
        {
            //clear selected objects
            short i;
            for (i = Convert.ToInt16(this.wrappedDiagram.SelectedObjects.Count -1); i >= 0; i--)
            {
                this.wrappedDiagram.SelectedObjects.DeleteAt(i, false);
            }
            //clear selected connector
            this.wrappedDiagram.SelectedConnector = null;
        }
        public void selectItem(UML.Extended.UMLItem itemToSelect)
        {
            //clear selection
            this.clearSelection();
            //then select the item to select
            if (itemToSelect is Operation)
            {
                bool found = false;
                //if the item is a relation or an operation then search through the links first
                foreach (DiagramLinkWrapper diagramLinkWrapper in this.diagramLinkWrappers)
                {
                    if (itemToSelect is Operation
                       && diagramLinkWrapper.relation is Message)
                    {
                        Message message = (Message)diagramLinkWrapper.relation;
                        if (itemToSelect.Equals(message.calledOperation))
                        {
                            this.wrappedDiagram.SelectedConnector = message.wrappedConnector;
                            found = true;
                            //done, no need to loop further
                            break;
                        }
                    }
                }
                //The operation could also be called in an Action.
                if (!found)
                {
                    List<UML.Actions.BasicActions.CallOperationAction> actions = ((Operation)itemToSelect).getDependentCallOperationActions().ToList();
                    List<UML.Diagrams.DiagramElement> diagramObjects = this.diagramObjectWrappers.ToList();

                    foreach (Action action in actions)
                    {
                        //try to find an diagramObjectwrapper that refrences the action
                        UML.Diagrams.DiagramElement diagramObject = diagramObjects.Find(
                            x => x.element.Equals(action));
                        if (diagramObject != null)
                        {
                            //found it, select the action and break out of for loop
                            this.selectItem(action);
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    //can't find a message on this diagram that calls the operation.
                    //then we try it with the operations parent
                    this.selectItem(((Operation)itemToSelect).owner);

                }
            }
            else if (itemToSelect is ConnectorWrapper)
            {
                this.wrappedDiagram.SelectedConnector = ((ConnectorWrapper)itemToSelect).wrappedConnector;
                //check if it worked
                if (wrappedDiagram.SelectedConnector == null
                   && itemToSelect is Message)
                {
                    this.selectItem(((Message)itemToSelect).calledOperation);
                }
            }
            else if (itemToSelect is ElementWrapper)
            {
                ElementWrapper elementToSelect = (ElementWrapper)itemToSelect;
                this.wrappedDiagram.SelectedObjects.AddNew(elementToSelect.wrappedElement.ElementID.ToString(),
                                                           elementToSelect.wrappedElement.Type);
            }
        }

        public HashSet<TSF.UmlToolingFramework.UML.Profiles.Stereotype> stereotypes
        {
            get
            {
                return ((Factory)this.model.factory).createStereotypes(this, this.wrappedDiagram.Stereotype);
            }
            set
            {
                this.wrappedDiagram.StereotypeEx = Stereotype.getStereotypeEx(value);
            }
        }



        /// <summary>
        /// adds an element to the diagram to the given coördinates
        /// </summary>
        /// <param name="element">the element to add</param>
        /// <param name="x">the x position</param>
        /// <param name="y">th y position</param>
        /// <param name="newHeight">the height of the new diagramElement</param>
        /// <param name="newWidth">the width of the new diagramElement</param>
        /// <returns>the new diagramElement</returns>
        public UML.Diagrams.DiagramElement addToDiagram(UML.Classes.Kernel.Element element, int x = 0, int y = 0, int newHeight = 0, int newWidth = 0)
        {
            UML.Diagrams.DiagramElement diagramElement = null;
            if (element != null)
            {
                //first check whether this element is not already added to this diagram
                diagramElement = this.getDiagramElement(element);
                if (diagramElement == null)
                {
                    if (element is EA.ElementWrapper)
                    {
                        string addNewString = createAddNewString(x, y, newHeight, newWidth);
                        //first save the diagram to make sure we don't lose any unsaved changes
                        this.model.saveOpenedDiagram(this);
                        global::EA.DiagramObject newDiagramObject = this.wrappedDiagram.DiagramObjects.AddNew(addNewString, "") as global::EA.DiagramObject;
                        diagramElement = ((Factory)this.model.factory).createDiagramElement(newDiagramObject);
                        diagramElement.element = element;
                        //save the element diagramObject
                        ((DiagramObjectWrapper)diagramElement).save();
                        //add the diagramObject to the list
                        this.diagramObjectWrappers.Add(diagramElement);
                        // now refresh to make sure we see the new element on the diagram
                        this.reFresh();
                    }

                    else if (!(element.owner is UML.Classes.Kernel.Package))
                    {
                        diagramElement = this.addToDiagram(element.owner);
                    }
                }
            }
            return diagramElement;
        }

        /// <summary>
        /// add the given diagram to this diagram
        /// </summary>
        /// <param name="diagram">the diagram to add</param>
        /// <param name="x">the x position</param>
        /// <param name="y">th y position</param>
        /// <param name="newHeight">the height of the new diagramElement</param>
        /// <param name="newWidth">the width of the new diagramElement</param>
        /// <returns>the diagramElement representing the diagram</returns>
        public UML.Diagrams.DiagramElement addToDiagram(UML.Diagrams.Diagram diagram, int x = 0, int y = 0, int newHeight = 0, int newWidth = 0)
        {
            if (this.owner is ElementWrapper)
            {
                ElementWrapper elementDiagram = ((ElementWrapper)this.owner).addOwnedElement<ElementWrapper>(diagram.name, "UMLDiagram");
                elementDiagram.save();
                //to make the elementDiagram actuall link to the diagram we need to set PDATA1 to the diagramID
                // and NType = 0 for Frame or 1 for Reference
                this.model.executeSQL("update t_object set Ntype = 0, PDATA1 = " + ((Diagram)diagram).DiagramID.ToString() + " where ea_guid = '" + elementDiagram.WrappedElement.ElementGUID + "'");
                return this.addToDiagram(elementDiagram, x, y, newHeight, newWidth);
            }
            else return null;
        }
        /// <summary>
        /// creates a string to be used when adding diagramObjects to an EA diagram
        /// </summary>
        /// <param name="x">the x coördinate</param>
        /// <param name="y">the y coördinate</param>
        /// <param name="newHeight">the height of the new diagramElement</param>
        /// <param name="newWidth">the width of the new diagramElement</param>
        /// <returns></returns>
        private string createAddNewString(int x = 0, int y = 0, int newHeight = 0, int newWidth = 0)
        {
            //if all all zero then return empty string
            if (x == 0 && y == 0 && newHeight == 0 && newWidth == 0) return string.Empty;
            //else create the string to add the new element
            int r = x + newWidth;
            int b = y + newHeight;
            string addNewString = "l=" + x + ";r=" + r + ";t=" + y + ";b=" + b;
            return addNewString;
        }
        /// <summary>
        /// reloads the diagram. Looses any unsaved changes.
        /// </summary>
        public void reFresh()
        {
            this.model.refreshDiagram(this);
        }

        public void addToCurrentDiagram()
        {
            //to add this to a diagram we first need to make a new element of type UMLDiagram
            //then we have to add that element tot he diagram.
            UML.Diagrams.Diagram currentDiagram = this.model.currentDiagram;
            if (currentDiagram != null)
            {
                currentDiagram.addToDiagram(this);
            }
        }
        /// <summary>
        /// select this diagram in the current diagram
        /// </summary>
        public void selectInCurrentDiagram()
        {
            //TODO implement
            // get the diagramObject for an element with type "UMLDiagram" and where PDATA1 contain this diagramID
            string SQLSelect = @"select o.Object_ID from (t_object o
			inner join t_diagramobjects do on o.Object_ID = do.Object_ID)
			where o.[Object_Type] = 'UMLDiagram'
			and do.[Diagram_ID] = " + ((Diagram)this.model.currentDiagram).DiagramID.ToString() + @" 
			and o.PDATA1 = '" + this.DiagramID.ToString() + "'";
            ElementWrapper elementDiagram = this.model.getElementWrappersByQuery(SQLSelect).FirstOrDefault();
            if (elementDiagram != null)
            {
                elementDiagram.selectInCurrentDiagram();
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
        /// The elements that have this diagram as composite diagram
        /// </summary>
        public HashSet<UML.Classes.Kernel.Element> compositeElements
        {
            get
            {
                HashSet<UML.Classes.Kernel.Element> results = new HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Element>();
                string sqlGet = " select o.Object_ID from t_object o                          " +
                                " where o.PDATA1 = '" + this.DiagramID.ToString() + "'           " +
                                " and o.Object_Type <> 'Package'                              " +
                                " union                                                       " +
                                " select o.Object_ID from t_object o                          " +
                                " inner join t_xref x on o.ea_guid = x.Client                 " +
                                " where x.Supplier = '" + this.diagramGUID + "'                ";
                foreach (ElementWrapper element in this.model.getElementWrappersByQuery(sqlGet))
                {
                    results.Add(element);
                }
                return results;
            }
        }

        public void autoLayout()
        {
            //get the xml variant of the diagrams GUID
            string XMLdiagramID = this.model.wrappedModel.GetProjectInterface().GUIDtoXML(this.wrappedDiagram.DiagramGUID);
            //layout the diagram
            this.model.wrappedModel.GetProjectInterface().LayoutDiagramEx(XMLdiagramID, global::EA.ConstLayoutStyles.lsDiagramDefault, 4, 20, 20, false);
            //save the diagram
            this.reFresh();
        }

        public void delete()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// indicates if the given element is shown on this diagram
        /// </summary>
        /// <param name="element">the element to look for</param>
        /// <returns>true if the given element is shown on this diagram, false otherwise</returns>
        public bool contains(UML.Classes.Kernel.Element element)
        {
            var elementWrapper = element as ElementWrapper;
            //check very quickly if this element is on the diagram
            if (elementWrapper != null)
            {
                string sqlContainsElement = "select 'true' AS ElementExists from t_diagramobjects do " +
                                         " where do.Diagram_ID = " + this.DiagramID +
                                           " and do.Object_ID = " + elementWrapper.id;
                var containsElementXml = this.model.SQLQuery(sqlContainsElement);
                var containsNode = containsElementXml.SelectSingleNode(this.model.formatXPath("//ElementExists"));
                return containsNode != null && containsNode.InnerText == "true";
            }
            //use the standard technique
            return this.getDiagramElement(element) != null;
        }
        /// <summary>
        /// returns the DiagramElement for the given element.
        /// Returns null if the element is not shown on this diagram
        /// </summary>
        /// <param name="element">the element that is used on this diagram</param>
        /// <returns>the diagramElement that represents the element shown on this diagram</returns>
        public UML.Diagrams.DiagramElement getDiagramElement(UML.Classes.Kernel.Element element)
        {
            if (element is ElementWrapper)
            {
                return this.diagramObjectWrappers.FirstOrDefault(x => x.element != null && x.element.Equals(element));
            }
            else if (element is ConnectorWrapper)
            {
                return this.diagramLinkWrappers.FirstOrDefault(x => x.element != null && x.element.Equals(element));
            }
            else
            {
                return null;
            }
        }

        public bool makeWritable(bool overrideLocks)
        {
            //if security is not enabled then it is always writeable
            if (!this.model.isSecurityEnabled) return true;
            //if already writable return true
            if (!this.isReadOnly) return true;
            //TODO: implement overrideLocks
            if (!this.isLocked)
            {
                //no lock found, go ahead and try to lock the element
                try
                {
                    bool lockingSucceeded = this.wrappedDiagram.ApplyUserLock();
                    if (lockingSucceeded)
                    {
                        // if succeeded we also lock all "owned" elements of the diagram
                        foreach (var element in this.getOwnedElements())
                        {
                            element.makeWritable(overrideLocks);
                        }
                    }
                    return lockingSucceeded;
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

        public bool isReadOnly
        {
            get
            {
                //first check if locking is enabled
                if (!this.model.isSecurityEnabled) return false;

                if (this.isLocked)
                {
                    return this.getLockedUserID() != this.model.currentUserID;
                }
                //not locked so readonly (only works when "require user lock to edit is on")
                return true;
            }
        }
        /// <summary>
        /// returns the name of the user currently locking this element
        /// </summary>
        /// <returns>the name of the user currently locking this element</returns>
        public string getLockedUser()
        {
            string lockedUser = string.Empty;

            string SQLQuery = @"select u.FirstName, u.Surname from t_seclocks s
                                inner join t_secuser u on s.userID = u.userID 
                                where s.entityID = '" + this.diagramGUID + "'";
            XmlDocument result = model.SQLQuery(SQLQuery);
            XmlNode firstNameNode = result.SelectSingleNode("//FirstName");
            XmlNode lastNameNode = result.SelectSingleNode("//Surname");
            if (firstNameNode != null && lastNameNode != null)
            {
                lockedUser = firstNameNode.InnerText + " " + lastNameNode.InnerText;
            }

            return lockedUser;
        }

        public string getLockedUserID()
        {
            string SQLQuery = @"select s.UserID from t_seclocks s
                        			where s.entityID = '" + this.diagramGUID + "'";
            XmlDocument result = this.model.SQLQuery(SQLQuery);
            XmlNode userIDNode = result.SelectSingleNode("//UserID");
            return userIDNode.InnerText;
        }
        /// <summary>
        /// returns true if currently locked
        /// </summary>
        /// <returns>true if currently locked</returns>
        public bool isLocked
        {
            get
            {
                return (this.getLockedUser() != string.Empty);
            }
        }
        /// <summary>
        /// returns the owned elements of this diagram.
        /// That are all elements that are shown on the diagram and that have the same parent as the diagram
        /// Plus all notes shown on this diagram
        /// </summary>
        /// <returns>list of "owned" elements</returns>
        public List<Element> getOwnedElements()
        {
            var ownedElements = new List<Element>();

            string SQLQuery = @"select o.Object_ID from ((t_diagram d
                                inner join t_diagramobjects do on d.Diagram_ID = do.Diagram_ID)
                                inner join t_object o on o.Object_ID = do.Object_ID)
                                where d.ea_guid = '" + this.diagramGUID + @"'
                                 and (d.ParentID = o.ParentID 
	                                or 
	                                (o.ParentID = 0 
	                                and o.Object_Type = 'Note'))";
            return this.model.getElementWrappersByQuery(SQLQuery).Cast<Element>().ToList();
        }
        /// <summary>
        /// saves the image of this diagram to the given folder as png
        /// </summary>
        /// <param name="imagePath">path where to save the file</param>
		public void saveImageToFile(string imagePath)
        {
            //lets try the cached image first GetDiagramImageAndMap only works in version 13 (1308?)
            if (!this.model.wrappedModel.GetProjectInterface().GetDiagramImageAndMap(this.uniqueID, imagePath)) //for some bizar reason this operation takes the regular GUID.
            {
                //only do this if the cache version doesn't work
                this.model.wrappedModel.GetProjectInterface().PutDiagramImageToFile(this.model.wrappedModel.GetProjectInterface().GUIDtoXML(this.uniqueID)
                                                                                , imagePath + "\\" + this.uniqueID + ".png", 1);
            }
        }
    }
}
