
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
        internal List<Mapping> _mappings = new List<Mapping>();
        protected MappingModel _source;
        protected MappingModel _target;
        public MappingSet(MP.MappingModel source, MP.MappingModel target)
        {
            this.source = source;
            this.target = target;
        }

        #region MappingSet implementation

        public List<MP.Mapping> mappings
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
        
        public MP.MappingModel source
        {
            get { return this._source; }
            set { this._source = (MappingModel)value; }
        }
        public MP.MappingModel target
        {
            get { return this._target; }
            set { this._target = (MappingModel)value; }
        }

        public void addMapping(MP.Mapping mapping)
        {
            this._mappings.Add((Mapping)mapping);
        }
        #endregion
    }
}
