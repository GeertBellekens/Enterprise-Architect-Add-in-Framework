
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
            //get the list of mappings
            this.source.getMyMappings(target);
            //this._mappings = this.source.getMappings(target).Cast<Mapping>().ToList();
            //map source to target
            //this.source.mapTo(target);
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

        #endregion
    }
}
