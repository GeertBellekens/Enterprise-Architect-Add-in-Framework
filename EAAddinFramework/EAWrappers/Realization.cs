using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Realization : Abstraction, 
                             UML.Classes.Dependencies.Realization 
  {
    public Realization(Model model, global::EA.Connector realization)
      : base(model, realization)
    {}
  }
}
