
using System;
using System.Collections.Generic;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using TSF.UmlToolingFramework.UML.Classes.Kernel;

namespace EAAddinFramework.Mapping
{
    /// <summary>
    /// Description of MappingSet.
    /// </summary>
    public class MappingSet : MP.MappingSet
    {
        internal List<Mapping> _mappings = new List<Mapping>();
        protected MappingNode _source;
        protected MappingNode _target;
        public MappingSettings settings { get; set; }
        public MappingSet(MP.MappingNode source, MP.MappingNode target, MappingSettings settings)
        {
            this.source = source;
            this.source.mappingSet = this;
            this.target = target;
            this.target.mappingSet = this;
            this.settings = settings;
            //get the mapings of the source object
            this.source.getMyMappings();
        }
        private List<ElementWrapper> _contexts;
        public List<ElementWrapper> EAContexts
        {
            get
            {
                if (_contexts == null)
                {
                    _contexts = this._source.model?.getElementWrappersByQuery(
                        this.settings.contextQuery.Replace("#ea_guid#",this.source.source?.uniqueID));
                }
                return _contexts;
            }
        }


        #region MappingSet implementation

        public IEnumerable<MP.Mapping> mappings
        {
            get
            {
                return _mappings.Cast<MP.Mapping>().ToList();
            }
            set
            {
                _mappings = value.Cast<Mapping>().ToList();
            }
        }
        
        public MP.MappingNode source
        {
            get { return this._source; }
            set { this._source = (MappingNode)value; }
        }
        public MP.MappingNode target
        {
            get { return this._target; }
            set { this._target = (MappingNode)value; }
        }

        public IEnumerable<NamedElement> contexts => this.EAContexts;

        public void addMapping(MP.Mapping mapping)
        {
            //set backlink
            mapping.mappingSet = this;
            //add to list
            this._mappings.Add((Mapping)mapping);
        }
        /// <summary>
        /// Load all mappings for this mappingSet
        /// </summary>
        public void loadAllMappings()
        {
            //preload source and target package
            //var sourceEAElement = this.source.source as ElementWrapper;
            //if (sourceEAElement != null)
            //{
            //    //preload all owned elements of the source  package
            //    var sourcePackage = sourceEAElement as TSF.UmlToolingFramework.Wrappers.EA.Package;
            //    if (sourcePackage == null)
            //    {
            //        sourcePackage = sourceEAElement.owningPackage as TSF.UmlToolingFramework.Wrappers.EA.Package;
            //    }
            //    sourcePackage?.getAllOwnedElements(new List<string>() { "Class", "DataType", "Enumeration", "PrimitiveType" });
            //}
            //var targetEAElement = this.target.source as ElementWrapper;
            //if (targetEAElement != null)
            //{
            //    //preload all owned elements of the target  package
            //    var targetPackage = targetEAElement as TSF.UmlToolingFramework.Wrappers.EA.Package;
            //    if (targetPackage == null)
            //    {
            //        targetPackage = targetEAElement.owningPackage as TSF.UmlToolingFramework.Wrappers.EA.Package;
            //    }
            //    targetPackage?.getAllOwnedElements(new List<string>() { "Class", "DataType", "Enumeration", "PrimitiveType" });
            //}
            this.target.getMappings();
        }
        /// <summary>
        /// load only the mappings for the given source element
        /// </summary>
        /// <param name="sourceElement">the element to start from</param>
        public void loadMappings(NamedElement sourceElement)
        {
            var mappingNode = MappingFactory.getMappingNode(sourceElement, (MappingNode)this.source);
            mappingNode.getMappings();
            //if we are showing a partial mapping then we show all elements
            if (!mappingNode.getOwnedMappings().Any())
            {
                this.showAll(this.source);
            }
        }
        private void showAll(MP.MappingNode node)
        {
            node.showAll = true;
            foreach (var subnode in node.childNodes)
            {
                this.showAll(subnode);
            }
        }

        #endregion
    }
}
