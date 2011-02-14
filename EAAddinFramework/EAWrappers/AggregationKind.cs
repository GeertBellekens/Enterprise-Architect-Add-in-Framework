using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class AggregationKind {
    public static UML.Classes.Kernel.AggregationKind getUMLAggregationKind
      (int aggregation) 
    {
      switch(aggregation) {
        case 0:  return UML.Classes.Kernel.AggregationKind.none;
        case 1:  return UML.Classes.Kernel.AggregationKind.shared;
        case 2:  return UML.Classes.Kernel.AggregationKind.composite;
        default: return UML.Classes.Kernel.AggregationKind.none;
      }
    }
    public static int getEAAggregationKind
      (UML.Classes.Kernel.AggregationKind aggregation)
    {
      switch(aggregation) {
        case UML.Classes.Kernel.AggregationKind.none:      return 0;
        case UML.Classes.Kernel.AggregationKind.shared:    return 1;
        case UML.Classes.Kernel.AggregationKind.composite: return 2;
        default:                                           return 0;
      }
    }
  }
}
