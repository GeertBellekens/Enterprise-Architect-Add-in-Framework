using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using UML = TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class DiagramObjectWrapper : UML.Diagrams.DiagramElement, UML.Extended.UMLItem
    {
        public global::EA.DiagramObject wrappedDiagramObject { get; set; }
        public Model model { get; set; }

        public DiagramObjectWrapper(Model model,
                                    global::EA.DiagramObject diagramObject)
        {
            this.wrappedDiagramObject = diagramObject;
            this.model = model;
        }
        /// <summary>
        /// return the unique ID of this element
        /// </summary>
        public string uniqueID
        {
            get
            {
                return this.wrappedDiagramObject.InstanceGUID.ToString();
            }
        }
        public DiagramObjectWrapper(Model model, ElementWrapper element,
                                    Diagram diagram)
          : this(model, diagram.getdiagramObjectForElement(element))
        { }

        public UML.Diagrams.Diagram diagram
        {
            get
            {
                return this.model.getDiagramByID(this.wrappedDiagramObject.DiagramID);
            }
        }
        public int xPosition
        {
            get { return this.wrappedDiagramObject.left; }
            set
            {
                if (wrappedDiagramObject.right == 0)
                {
                    wrappedDiagramObject.right = wrappedDiagramObject.left + 20;
                }
                //move the right the same amount as the left
                this.wrappedDiagramObject.right += value - this.wrappedDiagramObject.left;
                this.wrappedDiagramObject.left = value;
            }
        }

        public int yPosition
        {
            get { return this.wrappedDiagramObject.top * -1; }
            set
            {
                if (wrappedDiagramObject.bottom == 0)
                {
                    wrappedDiagramObject.bottom = wrappedDiagramObject.top + -20;
                }
                //move the top the same amount
                this.wrappedDiagramObject.bottom += this.wrappedDiagramObject.top + (value * -1);
                this.wrappedDiagramObject.top = value * -1;
            }
        }

        public UML.Classes.Kernel.Element element
        {
            get
            {
                return this.model.getElementWrapperByID
                 (this.wrappedDiagramObject.ElementID);
            }
            set
            {
                ElementWrapper elementWrapper = value as ElementWrapper;
                //TODO add support for other type of elements
                if (elementWrapper != null)
                {
                    this.wrappedDiagramObject.ElementID = elementWrapper.WrappedElement.ElementID;
                }
            }
        }
        public void save()
        {
            this.wrappedDiagramObject.Update();
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
            this.open();
            this.element.select();
            
        }

        public void open()
        {
            this.diagram.selectItem(this.element);
            this.diagram.open();
        }

        public void openProperties()
        {
            this.element.openProperties();
        }

        public void addToCurrentDiagram() { }//do nothing


        public void selectInCurrentDiagram()
        {
            this.model.currentDiagram.selectItem(this.element);
        }

        public void delete()
        {
            throw new NotImplementedException();
        }

        public string name { get => this.element.name; }

        public UML.Classes.Kernel.Element owner
        {
            get => this.diagram.owner;
            set => this.diagram.owner = value;
        }

        public string fqn { get => this.diagram.fqn + "." + this.element.name; }


        public HashSet<UML.Profiles.Stereotype> stereotypes
        {
            get => new HashSet<UML.Profiles.Stereotype>();
            set { }  //do nothing;
        }

        public bool makeWritable(bool overrideLocks)
        {
            return this.diagram.makeWritable(overrideLocks);
        }

        public bool isReadOnly { get => this.diagram.isReadOnly; }

        public void setOrientation(bool vertical)
        {
            string currentStyle = this.wrappedDiagramObject.Style.ToString();
            string vPartitionValue;
            vPartitionValue = vertical ? "1" : "0";
            this.wrappedDiagramObject.SetStyleEx("VPartition", vPartitionValue);
            string styleAfter = this.wrappedDiagramObject.Style.ToString();
        }

        public List<UML.Classes.Kernel.Element> getAllOwners()
        {
            return this.diagram.getAllOwners();
        }
    }
}
