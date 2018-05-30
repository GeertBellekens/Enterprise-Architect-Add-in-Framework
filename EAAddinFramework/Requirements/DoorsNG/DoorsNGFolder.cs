using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EARQ = EAAddinFramework.Requirements;
using TSF.UmlToolingFramework.Wrappers.EA;
using RequirementsFramework;
using System.Net.Http;
using Nito.AsyncEx;

namespace EAAddinFramework.Requirements.DoorsNG
{
    public class DoorsNGFolder : EARQ.Folder
    {
        private DoorsNGProject doorsNGProject { get => (DoorsNGProject)this.project; }
        public DoorsNGFolder(Package wrappedPackage, DoorsNGProject project, DoorsNGFolder parentFolder):base(wrappedPackage, project, parentFolder)
        {
            this.wrappedPackage = wrappedPackage;
            this.project = project;
            this.parentFolder = parentFolder;
        }
        public string url
        {
            //TODO: implement
            get;
            set;
        }
        private List<Requirement> _requirements;
        public override IEnumerable<RequirementsFramework.Requirement> requirements
        {
            get
            {
                if (_requirements == null)
                {
                    this._requirements = this.getRequirementsDoorsNG();
                }
                return _requirements;
            }
            set
            {
                this._requirements = value.Cast<Requirement>().ToList();
            }
        }
        private List<Requirement> getRequirementsDoorsNG()
        {
            var serverRequirements = new List<Requirement>();
            //first make sure we are authenticated
            HttpClient client =  this.doorsNGProject.Authenticate();

            return null;
        }

        public override Folder createNewFolder(Package wrappedPackage, Project project, Folder parentFolder)
        {
            throw new NotImplementedException();
        }
    }
}
