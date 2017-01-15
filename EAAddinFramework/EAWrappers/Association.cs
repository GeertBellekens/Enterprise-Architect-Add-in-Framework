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
    
	public virtual HashSet<T> getDependentTypedElements<T>() where T:UML.Classes.Kernel.TypedElement
    {
		throw new NotImplementedException();
    }
  	
	public HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> ownedDiagrams {
		get {
			throw new NotImplementedException();
		}
		set {
			throw new NotImplementedException();
		}
	}
	private AssociationClass _associationClass = null;
	public AssociationClass associationClass 
	{
		get
		{
			//TODO use the EA.Connector.AssociationClass property once fixed by Sparx
      // if (_associationClass == null)
      // {
      //   int associationClassID;
      //   if (int.TryParse(this.WrappedConnector.MiscData,out associationClassID))
      //   {
      //     if (associationClassID > 0)
      //     {
      //       this._associationClass = this.model.getElementWrapperByID(associationClassID) as AssociationClass;
      //     }
      //   }
      // }
			return _associationClass;
		}
	}

	public HashSet<UML.InfomationFlows.InformationFlow> getConveyingFlows()
	{
		//associations cannot be conveyed in EA.
		return new HashSet<UML.InfomationFlows.InformationFlow>();
	}
  }
}
