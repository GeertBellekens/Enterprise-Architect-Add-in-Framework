using System;
using System.Collections.Generic;
using System.Linq;
using EAAddinFramework.Utilities;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using UML = TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    /// A diagramElement that wraps the EA.DiagramLink class.
    /// This is the representation of a Relationship on a diagram.
    /// For some strange reason EA doesn't create a DiagramLink for each 
    /// relation shown on the diagram.
    /// Only the relations that have their visualisation altered on the diagram 
    /// have a DiagramLink object.
    /// (this was in version 7.5.850, it might change in version 8.0)
    public class DiagramLinkWrapper : UML.Diagrams.DiagramElement
    {
        internal global::EA.DiagramLink wrappedDiagramLink { get; set; }
        public ConnectorWrapper relation { get; set; }
        internal Model model { get; set; }
        private Diagram _diagram;

        public DiagramLinkWrapper(Model model, global::EA.DiagramLink diagramlink)
        {
            this.model = model;
            this.wrappedDiagramLink = diagramlink;
            this.relation = this.model.getRelationByID(this.wrappedDiagramLink.ConnectorID);
        }

        public DiagramLinkWrapper(Model model, ConnectorWrapper relation,
                                  Diagram diagram)
        {
            this.model = model;
            this.relation = relation;
            this._diagram = diagram;
            this.wrappedDiagramLink = diagram.getDiagramLinkForRelation(relation);
        }
        private DiagramObjectWrapper _sourceDiagramObject;
        public DiagramObjectWrapper sourceDiagramObject 
        { 
            get
            {
                if (this._sourceDiagramObject == null)
                {
                    this._sourceDiagramObject = ((Diagram)this.diagram).diagramObjectWrappers.OfType<DiagramObjectWrapper>()
                                                .FirstOrDefault(x => x.element.uniqueID == this.relation.sourceElement.uniqueID);
                }
                return this._sourceDiagramObject;
            }
        }
        private DiagramObjectWrapper _targetDiagramObject;
        public DiagramObjectWrapper targetDiagramObject
        {
            get
            {
                if (this._targetDiagramObject == null)
                {
                    this._targetDiagramObject = ((Diagram)this.diagram).diagramObjectWrappers.OfType<DiagramObjectWrapper>()
                                                .FirstOrDefault(x => x.element.uniqueID == this.relation.targetElement.uniqueID);
                }
                return this._targetDiagramObject;
            }
        }
        /// <summary>
        /// return the unique ID of this element
        /// </summary>
        public string uniqueID
        {
            get
            {
                return null;
            }
        }
        private Diagram getDiagramForDiagramLink
          (global::EA.DiagramLink diagramLink)
        {
            return this.model.getDiagramByID(diagramLink.DiagramID);
        }

        /// In EA relations can be hidden from a diagram. They still have a 
        /// DiagramLink object, but it is set to hidden.
        internal bool isHidden
        {
            get
            {
                return this.wrappedDiagramLink == null ?
                  false : this.wrappedDiagramLink.IsHidden;
            }
        }


        public void setStyle(LinkStyle linkStyle)
        {
            string lineStyleString = this.wrappedDiagramLink.Style;
            string mode = getMode(linkStyle);
            string tree = getTree(linkStyle);
            //set the Mode
            lineStyleString = KeyValuePairsHelper.setValueForKey("Mode", mode, lineStyleString);
            //set the TREE
            if (string.IsNullOrEmpty(tree))
            {
                lineStyleString = KeyValuePairsHelper.RemoveKey("TREE", lineStyleString);
            }
            else
            {
                lineStyleString = KeyValuePairsHelper.setValueForKey("TREE", tree, lineStyleString);
            }
            this.wrappedDiagramLink.Style = lineStyleString;
        }
        private string getMode(LinkStyle linkStyle)
        {
            switch (linkStyle)
            {
                case LinkStyle.lsDirectMode:
                    return "1";
                case LinkStyle.lsAutoRouteMode:
                    return "2";
                case LinkStyle.lsCustomMode:
                case LinkStyle.lsTreeVerticalTree:
                case LinkStyle.lsTreeHorizontalTree:
                case LinkStyle.lsLateralHorizontalTree:
                case LinkStyle.lsLateralVerticalTree:
                case LinkStyle.lsOrthogonalSquareTree:
                case LinkStyle.lsOrthogonalRoundedTree:
                    return "3";
                default:
                    return "3";
            }

        }
        private string getTree(LinkStyle linkStyle)
        {
            switch (linkStyle)
            {
                case LinkStyle.lsDirectMode:
                case LinkStyle.lsAutoRouteMode:
                case LinkStyle.lsCustomMode:
                    return string.Empty;
                case LinkStyle.lsTreeVerticalTree:
                    return "V";
                case LinkStyle.lsTreeHorizontalTree:
                    return "H";
                case LinkStyle.lsLateralHorizontalTree:
                    return "LH";
                case LinkStyle.lsLateralVerticalTree:
                    return "LC";
                case LinkStyle.lsOrthogonalSquareTree:
                    return "OS";
                case LinkStyle.lsOrthogonalRoundedTree:
                    return "OR";
                default:
                    return "OR";
            }

        }
        public string typeString
        {
            get
            {
                //get the relation
                var relation = this.model.getRelationByID(this.wrappedDiagramLink.ConnectorID);
                //return it's type
                return relation != null ? relation.WrappedConnector.Type : string.Empty;
            }
        }
        public int startXPosition
        {
            get => int.TryParse(KeyValuePairsHelper.getValueForKey("SX", this.wrappedDiagramLink.Geometry), out var SX)
                ? SX 
                : 0;
            set => this.wrappedDiagramLink.Geometry = KeyValuePairsHelper.setValueForKey("SX", value.ToString(), this.wrappedDiagramLink.Geometry);
        }
        public int startYPosition
        {
            get => int.TryParse(KeyValuePairsHelper.getValueForKey("SY", this.wrappedDiagramLink.Geometry), out var SX)
                ? SX
                : 0;
            set => this.wrappedDiagramLink.Geometry = KeyValuePairsHelper.setValueForKey("SY", value.ToString(), this.wrappedDiagramLink.Geometry);
        }
        public int endXPosition
        {
            get => int.TryParse(KeyValuePairsHelper.getValueForKey("EX", this.wrappedDiagramLink.Geometry), out var SX)
                ? SX
                : 0;
            set => this.wrappedDiagramLink.Geometry = KeyValuePairsHelper.setValueForKey("EX", value.ToString(), this.wrappedDiagramLink.Geometry);
        }
        public int endYPosition
        {
            get => int.TryParse(KeyValuePairsHelper.getValueForKey("EY", this.wrappedDiagramLink.Geometry), out var SX)
                ? SX
                : 0;
            set => this.wrappedDiagramLink.Geometry = KeyValuePairsHelper.setValueForKey("EY", value.ToString(), this.wrappedDiagramLink.Geometry);
        }
        internal void setBend(int bendX, int bendY)
        {
            this.wrappedDiagramLink.Path = $"{bendX}:{bendY * -1};";
        }
        public UML.Classes.Kernel.Element element
        {
            get { return this.relation; }
            set { this.relation = value as ConnectorWrapper; }
        }

        public int xPosition
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }
        public int yPosition
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }
        /// <summary>
        /// returns the diagram where this diagramLink is shown
        /// </summary>
        public UML.Diagrams.Diagram diagram
        {
            get
            {
                if (_diagram == null)
                {
                    _diagram = this.model.getDiagramByID(this.wrappedDiagramLink.DiagramID);
                }
                return _diagram;
            }
        }
        /// <summary>
        /// returns a list of diagrams that show this item.
        /// DiagramObjects are specific for a single diagram, so the list will contain only one diagram
        /// </summary>
        /// <returns>all diagrams that show this item</returns>
        public virtual List<UML.Diagrams.Diagram> getDependentDiagrams()
        {
            var dependentDiagrams = new List<UML.Diagrams.Diagram>();
            dependentDiagrams.Add(this.diagram);
            return dependentDiagrams;
        }
        public void select()
        {
            this.diagram.open();
            ((EA.Diagram)this.diagram).wrappedDiagram.SelectedConnector = this.relation.wrappedConnector.eaConnector;
        }

        public void open()
        {
            throw new NotImplementedException();
        }

        public void openProperties()
        {
            throw new NotImplementedException();
        }

        public void addToCurrentDiagram()
        {
            throw new NotImplementedException();
        }

        public void selectInCurrentDiagram()
        {
            throw new NotImplementedException();
        }

        public void delete()
        {
            throw new NotImplementedException();
        }

        public string name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public UML.Classes.Kernel.Element owner
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

        public string fqn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public HashSet<UML.Profiles.Stereotype> stereotypes
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

        public void save()
        {
            this.wrappedDiagramLink.Update();
        }

        public bool makeWritable(bool overrideLocks)
        {
            return this.diagram.makeWritable(overrideLocks);
        }

        public bool isReadOnly
        {
            get
            {
                return this.diagram.isReadOnly;
            }
        }

        public void setOrientation(bool vertical)
        {
            //do nothing
        }

        public List<UML.Classes.Kernel.Element> getAllOwners()
        {
            throw new NotImplementedException();
        }


    }
}
