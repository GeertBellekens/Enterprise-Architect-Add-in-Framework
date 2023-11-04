using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Abstraction : Dependency, 
                             UML.Classes.Dependencies.Abstraction 
  {
    public Abstraction(Model model, EADBConnector wrappedConnector)
      : base(model, wrappedConnector)
    {}
  }
}
