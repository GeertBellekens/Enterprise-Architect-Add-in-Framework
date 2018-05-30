using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSF.UmlToolingFramework.Wrappers.EA;
using EARQ = EAAddinFramework.Requirements;

namespace EAAddinFramework.Requirements.DoorsNG
{
    public class DoorsNGRequirement : EARQ.Requirement
    {
        public DoorsNGRequirement(ElementWrapper wrappedelement) : base(wrappedelement) { }
        public string url
        {
            //TODO: implement
            get;
            set;
        }
        public bool synchronizeToDoorsNG()
        {
            throw new NotImplementedException();
        }
    }
}
