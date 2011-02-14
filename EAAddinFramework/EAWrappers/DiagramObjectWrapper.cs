using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class DiagramObjectWrapper : UML.Diagrams.DiagramElement {
    internal global::EA.DiagramObject wrappedDiagramObject { get; set; }
    internal Model model { get; set; }
    
    public DiagramObjectWrapper(Model model,
                                global::EA.DiagramObject diagramObject) 
    {
      this.wrappedDiagramObject = diagramObject;
      this.model = model;
    }
    
    public DiagramObjectWrapper(Model model, ElementWrapper element,
                                Diagram diagram) 
      : this(model, diagram.getdiagramObjectForElement(element)) 
    {}
    
    public int xPosition {
      get { return this.wrappedDiagramObject.left;  }
      set { this.wrappedDiagramObject.left = value; }
    }
    
    public int yPosition {
      get { return this.wrappedDiagramObject.top * -1;  }
      set { this.wrappedDiagramObject.top = value * -1; }
    }
    
    public UML.Classes.Kernel.Element element {
      get {
         return this.model.getElementWrapperByID
          ( this.wrappedDiagramObject.ElementID );
      }
      set { throw new NotImplementedException(); }
    }
  }
}
