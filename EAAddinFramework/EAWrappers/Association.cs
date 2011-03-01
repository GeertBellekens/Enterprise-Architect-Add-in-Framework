using System;
using System.Collections.Generic;
using System.Linq;  

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Association : ConnectorWrapper, UML.Classes.Kernel.Association{
    public Association(Model model, global::EA.Connector association)
      : base(model, association)
    {}
    
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get { 
        return new HashSet<UML.Classes.Kernel.Element>
          ( this.memberEnds.Cast<UML.Classes.Kernel.Element>() );
      }
      set { 
        this.memberEnds = value.Cast<UML.Classes.Kernel.Property>().ToList();   
      }
    }
    
	public HashSet<T> getDependentTypedElements<T>() where T:UML.Classes.Kernel.TypedElement
    {
		throw new NotImplementedException();
    }
  }
}
