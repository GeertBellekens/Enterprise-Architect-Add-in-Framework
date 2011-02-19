using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class ParameterDirectionKind {
    internal static UML.Classes.Kernel.ParameterDirectionKind 
      getUMLParameterDirectionkind( string EAdirection )
    {
      switch (EAdirection) {
      case "in":     return UML.Classes.Kernel.ParameterDirectionKind._in;
      case "out":    return UML.Classes.Kernel.ParameterDirectionKind._out;
      case "inout":  return UML.Classes.Kernel.ParameterDirectionKind._inout;
      case "return": return UML.Classes.Kernel.ParameterDirectionKind._return;
      default:       return UML.Classes.Kernel.ParameterDirectionKind._in;
      }
    }
    
    internal static string getEAParameterDirectionKind
      ( UML.Classes.Kernel.ParameterDirectionKind UMLdirection )
    {
      switch (UMLdirection) {
      case UML.Classes.Kernel.ParameterDirectionKind._in:     return "in";
      case UML.Classes.Kernel.ParameterDirectionKind._out:    return "out";
      case UML.Classes.Kernel.ParameterDirectionKind._inout:  return "inout";
      case UML.Classes.Kernel.ParameterDirectionKind._return: return "return";
      default:                                                return "in";
      }
    }
  }
}
