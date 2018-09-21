
using System;
using System.Collections.Generic;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;

namespace EAAddinFramework.Mapping
{
    /// <summary>
    /// Description of MappingSet.
    /// </summary>
    public class MappingSet : MP.MappingSet
    {
        internal List<Mapping> _mappings;
        protected MappingNode _source;
        protected MappingNode _target;
        public MappingSet(MP.MappingNode source, MP.MappingNode target)
        {
            this.source = source;
            this.target = target;
            //get the list of mappings
            this._mappings = this.source.getMappings(null).Cast<Mapping>().ToList();
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

        public void addMapping(MP.Mapping mapping)
        {
            this._mappings.Add((Mapping)mapping);
        }
        #endregion
    }
}
