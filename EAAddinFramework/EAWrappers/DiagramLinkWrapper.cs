using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  /// A diagramElement that wraps the EA.DiagramLink class.
  /// This is the representation of a Relationship on a diagram.
  /// For some strange reason EA doesn't create a DiagramLink for each 
  /// relation shown on the diagram.
  /// Only the relations that have their visualisation altered on the diagram 
  /// have a DiagramLink object.
  /// (this was in version 7.5.850, it might change in version 8.0)
  public class DiagramLinkWrapper : UML.Diagrams.DiagramElement {
    internal global::EA.DiagramLink wrappedDiagramLink { get; set; }
    internal ConnectorWrapper relation { get; set; }
    internal Model model { get; set; }
    internal Diagram diagram { get; set; }
    
    public DiagramLinkWrapper(Model model,global::EA.DiagramLink diagramlink){
      this.model = model;
      this.wrappedDiagramLink = diagramlink;
      this.relation = this.model.getRelationByID(this.wrappedDiagramLink.ConnectorID);
      this.diagram = this.getDiagramForDiagramLink(diagramlink);
    }
    
    public DiagramLinkWrapper(Model model, ConnectorWrapper relation,
                              Diagram diagram){
      this.model = model;
      this.relation = relation;
      this.diagram = diagram;
      this.wrappedDiagramLink = diagram.getDiagramLinkForRelation(relation);
    }
    
    private Diagram getDiagramForDiagramLink
      (global::EA.DiagramLink diagramLink)
    {
      return this.model.getDiagramByID(diagramLink.DiagramID);
    }
    
    /// In EA relations can be hidden from a diagram. They still have a 
    /// DiagramLink object, but it is set to hidden.
    internal bool isHidden {
      get { 
        return this.wrappedDiagramLink == null ? 
          false : this.wrappedDiagramLink.IsHidden;
      }
    }

    public UML.Classes.Kernel.Element element {
      get { return this.relation; }
      set { this.relation = value as ConnectorWrapper; }
    }
    
    public int xPosition {
      get { return 0; }
      set { throw new NotImplementedException(); }
    }
    public int yPosition {
      get { return 0; }
      set { throw new NotImplementedException(); }
    }
    public void select()
    {
    	this.diagram.open();
    	this.diagram.wrappedDiagram.SelectedConnector = this.relation.wrappedConnector;
    }
  }
}
