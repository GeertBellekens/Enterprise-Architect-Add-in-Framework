using System;
using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.UML.Classes.Dependencies;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using UML = TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    internal class NaryAssociation : ElementWrapper, UML.Classes.Kernel.Association
    {
        public NaryAssociation(Model model, EADBElement wrappedElement)
        : base(model, wrappedElement)
        {}

        public bool isDerived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<UML.Classes.Kernel.Property> navigableOwnedEnds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<UML.Classes.Kernel.Property> ownedEnds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<UML.Classes.Kernel.Type> endTypes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<UML.Classes.Kernel.Property> memberEnds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public HashSet<Feature> features { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public HashSet<Substitution> substitutions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool isLeaf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public HashSet<RedefinableElement> redefinedElements { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public HashSet<Classifier> redefinitionContexts { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<UML.Classes.Kernel.Element> relatedElements { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void addRelatedElement(UML.Classes.Kernel.Element relatedElement)
        {
            throw new NotImplementedException();
        }

        public HashSet<UML.InfomationFlows.InformationFlow> getInformationFlows()
        {
            throw new NotImplementedException();
        }

        public List<UML.Classes.Kernel.Element> getLinkedElements()
        {
            throw new NotImplementedException();
        }
    }
}
