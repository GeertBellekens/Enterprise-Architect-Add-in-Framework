using System;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;

namespace EAAddinFramework.Mapping
{
    public class MappingNode : MP.MappingNode
    {
        protected Element _source;
        public MappingNode(UML.Classes.Kernel.NamedElement source, MappingNode parent)
        {
            this.source = source;
            this.parent = parent;
        }
        public string name
        {
            get { return this._source.name; }
        }

        public UML.Classes.Kernel.NamedElement source
        {
            get
            {
                return this._source as UML.Classes.Kernel.NamedElement;
            }
            set
            {
                this._source = (Element)value;
            }
        }
        public IEnumerable<MP.MappingNode> childNodes { get; set; }
        public MP.MappingNode parent { get; set; }
    }
}
