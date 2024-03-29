﻿using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Usage : Abstraction, 
                             UML.Classes.Dependencies.Usage 
  {
    public Usage(Model model, EADBConnector usage)
      : base(model, usage)
    {}
  }
}
