using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Class : ElementWrapper, UML.Classes.Kernel.Class, 
                       UML.Classes.Interfaces.BehavioredClassifier 
  {

    public Class(Model model, global::EA.Element elementToWrap)
      : base(model, elementToWrap)
    {}
    
    public HashSet<UML.Classes.Kernel.Classifier> nestedClassifiers {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Feature> features {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public bool isLeaf {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.RedefinableElement> redefinedElements {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public HashSet<UML.Classes.Kernel.Classifier> redefinitionContexts {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Interfaces.InterfaceRealization> 
      interfaceRealizations 
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Dependencies.Substitution> substitutions {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
  	

  }
}
