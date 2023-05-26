using System;
using System.Collections.Generic;
using UML = TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    public class Component : Class, UML.Components.BasicComponents.Component
    {
        public Component(Model model, EADBElementWrapper elementToWrap): base(model, elementToWrap)
        { }
        public bool isIndirectlyInstantiated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public HashSet<UML.Classes.Interfaces.Interface> required { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public HashSet<UML.Classes.Interfaces.Interface> provided { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
