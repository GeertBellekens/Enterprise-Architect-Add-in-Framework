using System;
using System.Collections.Generic;

using UML = TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
    class PrimitiveType : DataType, UML.Classes.Kernel.PrimitiveType
    {
        private string _name;
        public PrimitiveType(Model model, String typeName) : base(model, null)
        {
            this._name = typeName;
        }
        public PrimitiveType(Model model, global::EA.Element element) : base(model, element)
        {

        }
        public override String name
        {
            get
            {
                if (this.wrappedElement == null)
                {
                    return this._name;
                }
                else
                {
                    return base.name;
                }
            }
            set
            {
                if (this.wrappedElement == null)
                {
                    this._name = value;
                }
                else
                {
                    base.name = value;
                }
            }

        }
    }
}
