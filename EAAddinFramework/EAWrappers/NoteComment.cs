using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  /// A comment is a textual annotation that can be attached to a set of 
  /// elements. This class wraps notes that can be made visible on a diagram.
  public class NoteComment : ElementWrapper, UML.Classes.Kernel.Comment {
    public NoteComment(Model model, global::EA.Element wrappedNote) 
      : base(model, wrappedNote)
    {}

    /// Specifies a string that is the comment.
    public String body {
      get { return this.wrappedElement.Notes.TrimEnd(); }
      set { this.wrappedElement.Notes = value; }
    }

    /// References the Element(s) being commented.
    public HashSet<UML.Classes.Kernel.Element> annotatedElements {
      get {
        HashSet<UML.Classes.Kernel.Element> returnedElements = 
          new HashSet<UML.Classes.Kernel.Element>();
        // loop all connectors to find the ones with type "NoteLink"
        // then get the other end element
        foreach( global::EA.Connector connector 
                 in this.wrappedElement.Connectors ) 
        {
          if( connector.Type == "NoteLink" ) {
            // found a good link.
            // now figure out which side is the note and which side the 
            // annotated element
            if( connector.ClientID == this.wrappedElement.ElementID ) {
              returnedElements.Add( this.EAModel.getElementWrapperByID
                                      (connector.SupplierID) );
            } else {
              returnedElements.Add( this.EAModel.getElementWrapperByID
                                      (connector.ClientID) );
            }
          }
        }
        return returnedElements;
      }
      set { throw new NotImplementedException(); }
    }
  }
}
