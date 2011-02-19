using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public class Interface : ElementWrapper, UML.Classes.Interfaces.Interface {
    public Interface(Model model, global::EA.Element element) : 
      base(model, element)
    {}

    public HashSet<UML.Classes.Kernel.Classifier> nestedClassifiers {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Interfaces.InterfaceRealization> 
      InterfaceRealizations 
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Interfaces.Interface> redefinedInterface {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Dependencies.Substitution> substitutions {
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
    
    public HashSet<UML.Classes.Kernel.Feature> features {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public bool isLeaf {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
      get {
        // remove the stereotype with name "interface" because that is not 
        // really a stereotype but nearly an indication that this is an 
        // interface
        List<UML.Profiles.Stereotype> myStereotypes = 
          new List<UML.Profiles.Stereotype>(base.stereotypes);
        for( int i = myStereotypes.Count -1; i>=0; i-- ) {
          if( myStereotypes[i].name == "interface" ) {
            myStereotypes.RemoveAt(i);
          }
        }
        return new HashSet<UML.Profiles.Stereotype>(myStereotypes);
      }
      set { base.stereotypes = value; }
    }
  }
}
