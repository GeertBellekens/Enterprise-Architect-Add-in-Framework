using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Generalization : ConnectorWrapper, 
                                UML.Classes.Kernel.Generalization 
  {
    public Generalization(Model model, global::EA.Connector connector) 
      : base(model, connector)
    {}
    
    public bool isSubstitutable {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.Classifier general {
      get { return this.target as UML.Classes.Kernel.Classifier; }
      set { throw new NotImplementedException(); }
    }

    public UML.Classes.Kernel.Classifier specific {
      get { return this.source as UML.Classes.Kernel.Classifier; }
      set { throw new NotImplementedException(); }
    }
  }
}
