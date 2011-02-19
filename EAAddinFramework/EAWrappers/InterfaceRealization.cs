using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class InterfaceRealization : Realization,
                                  UML.Classes.Interfaces.InterfaceRealization 
  {
    public InterfaceRealization(Model model, global::EA.Connector connector)
      : base(model, connector)
    {}
    
    public UML.Classes.Interfaces.Interface contract {
      get { return this.supplier as UML.Classes.Interfaces.Interface; }
      set { throw new NotImplementedException(); }
    }
    public UML.Classes.Interfaces.BehavioredClassifier implementingClassifier{
      get {return this.client as UML.Classes.Interfaces.BehavioredClassifier;}
      set { throw new NotImplementedException(); }
    }
  }
}
