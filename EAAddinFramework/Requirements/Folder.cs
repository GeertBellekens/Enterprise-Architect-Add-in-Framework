using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RQ = RequirementsFramework;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Requirements
{
    public abstract class Folder : RQ.Folder
    {
        public Project project { get; set; }
        public Package wrappedPackage { get; set; }
        protected Folder(Package wrappedPackage, Project project, Folder parentFolder)
        {
            this.wrappedPackage = wrappedPackage;
            this.project = project;
            this.parentFolder = parentFolder;
        }
        public abstract Folder createNewFolder(Package wrappedPackage, Project project, Folder parentFolder);

        public string name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RQ.Folder parentFolder { get; set; }
        private List<Folder> _subFolders;
        /// <summary>
        /// returns the folders owned by this Folder
        /// </summary>
        public IEnumerable<RQ.Folder> subFolders
        {
            get
            {
                //TODO: investigate: what in case a subPackage was added to the wrapped package
                if (_subFolders == null)
                {
                    _subFolders = new List<Folder>();
                    foreach(var subPackage in wrappedPackage.getOwnedElementWrappers<Package>(string.Empty, false))
                    {
                        _subFolders.Add(this.createNewFolder(subPackage, this.project, this));
                    }
                }
                return _subFolders;
            }
            set
            {
                _subFolders = value.Cast<Folder>().ToList(); ;
            }
        }

        public abstract IEnumerable<RQ.Requirement> requirements { get; set; }

        public IEnumerable<RQ.Requirement> getAllOwnedRequirements()
        {
            var allRequirements = new List<RQ.Requirement>(this.requirements);
            foreach(var subFolder in this.subFolders)
            {
                allRequirements.AddRange(subFolder.getAllOwnedRequirements());
            }
            return allRequirements;
        }
    }
}
