using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RQ = RequirementsFramework;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Requirements
{
    public class Requirement : RQ.Requirement
    {
        internal ElementWrapper wrappedElement { get; set; }
        public Requirement(ElementWrapper wrappedElement)
        {
            this.wrappedElement = wrappedElement;
        }
        public string title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string creator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string contributor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime created { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime modified { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string priority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RQ.Folder folder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RQ.Project project { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual bool synchronizeToEA()
        {
            throw new NotImplementedException();
        }
    }
}
