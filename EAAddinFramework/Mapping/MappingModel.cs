using System;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;

namespace EAAddinFramework.Mapping
{
    public class MappingModel : MappingNode, MP.MappingModel
    {
        public MappingModel(UML.Classes.Kernel.NamedElement source) : base(source, null) { }

    }
}
