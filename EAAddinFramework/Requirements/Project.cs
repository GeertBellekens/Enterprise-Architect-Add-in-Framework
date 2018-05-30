using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using TSF.UmlToolingFramework.Wrappers.EA;
using RQ = RequirementsFramework;

namespace EAAddinFramework.Requirements
{
    public class Project : RQ.Project
    {
        const string tagName = "RQProject";

        Package _wrappedPackage;
        internal Package wrappedPackage
        {
            get
            {
                return _wrappedPackage;
            }
            set
            {
                _wrappedPackage = value;
                this.resetProperties();
            }
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public Project() { }
        public Project(Package wrappedPackage)
        {
            this._wrappedPackage = wrappedPackage;
        }
        public IEnumerable<RQ.Requirement> requirements { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IEnumerable<RQ.Folder> rootFolders { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }



        protected static Package getCurrentProjectPackage(Element currentElement)
        {
            //if the current element is null then we can't find the project
            if (currentElement == null) return null;
            //get the owning package
            if (!(currentElement is Package)) currentElement = currentElement.owningPackage as Element;
            if (currentElement is RootPackage)
            {
                if (currentElement.notes.Contains(tagName)) return currentElement as Package;
            }
            //no rootPackage
            if (currentElement.taggedValues.Any(x => x.name == tagName && x.tagValue.ToString().Length > 0))
            {
                return currentElement as Package;
            }
            //no project found on this level, go one level up
            return getCurrentProjectPackage(currentElement.owningPackage as Element);
        }
        private void resetProperties()
        {
            if (this.wrappedPackage is RootPackage)
            {
                this.wrappedPackage.notes = tagName + "=" + _name;
            }
            else
            {
                this.wrappedPackage.addTaggedValue(tagName, _name);
            }
        }
        private string _name;
        public string name
        {
            get
            {
                if (this.wrappedPackage != null)
                {
                    if (wrappedPackage is RootPackage)
                    {
                        var keyValue = this.wrappedPackage.notes.Split('=');
                        if (keyValue.Count() == 2)
                        {
                            _name = keyValue[1];
                        }
                    }
                    else
                    {
                        var projectTag = wrappedPackage.getTaggedValue(tagName);
                        if (projectTag != null)
                        {
                            _name = projectTag.eaStringValue;
                        }
                    }
                }
                return _name;
            }
            set
            {
                if (this.wrappedPackage != null)
                {
                    if (wrappedPackage is RootPackage)
                    {
                        this.wrappedPackage.notes = tagName + "=" + value;
                    }
                    else
                    {
                        this.wrappedPackage.addTaggedValue(tagName, value);
                    }
                }
                _name = value;
            }
        }
    }
}
