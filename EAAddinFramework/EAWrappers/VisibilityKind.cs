using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  internal class VisibilityKind {
    /// translates the UML VisibilityKind to the string used in EA
    internal static String getEAVisibility
      (UML.Classes.Kernel.VisibilityKind visibility)
    {
      switch (visibility) {
        case UML.Classes.Kernel.VisibilityKind._private:   return "Private";
        case UML.Classes.Kernel.VisibilityKind._public:    return "Public";
        case UML.Classes.Kernel.VisibilityKind._protected: return "Protected";
        case UML.Classes.Kernel.VisibilityKind._package:   return "Package";
        default:                                           return "";
      }
    }

    /// translates the EA visibilityString to the UML VisibilityKind.
    /// If the proper translation cannot be found the defaultVisibility will 
    /// be returned
    internal static UML.Classes.Kernel.VisibilityKind getUMLVisibilityKind
      (String visibility, UML.Classes.Kernel.VisibilityKind defaultVisibility)
    {
      switch (visibility) {
        case "Private":   return UML.Classes.Kernel.VisibilityKind._private;
        case "Public":    return UML.Classes.Kernel.VisibilityKind._public;
        case "Protected": return UML.Classes.Kernel.VisibilityKind._protected;
        case "Package":   return UML.Classes.Kernel.VisibilityKind._package;
        default:          return defaultVisibility;
      }
    }
  }
}
