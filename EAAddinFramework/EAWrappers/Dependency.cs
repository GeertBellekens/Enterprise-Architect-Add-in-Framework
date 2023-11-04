using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Dependency : ConnectorWrapper, 
                            UML.Classes.Dependencies.Dependency 
  {
    public Dependency(Model model, EADBConnector dependency)
      : base(model, dependency)
    {}
  }
}
