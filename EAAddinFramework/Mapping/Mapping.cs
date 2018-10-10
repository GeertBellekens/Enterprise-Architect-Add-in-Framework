using System;
using TSF.UmlToolingFramework.Wrappers.EA;
using MP = MappingFramework;

namespace EAAddinFramework.Mapping
{
    /// <summary>
    /// Description of Mapping.
    /// </summary>
    public abstract class Mapping : MP.Mapping
    {
        internal MappingNode _source;
        internal MappingNode _target;
        internal MappingLogic _mappingLogic;
        public Mapping(MappingNode sourceEnd, MappingNode targetEnd)
        {
            this._source = sourceEnd;
            this._source.addMapping(this);
            this._target = targetEnd;
            this._target.addMapping(this);
        }
        public Mapping(MappingNode sourceEnd, MappingNode targetEnd, MappingLogic logic) : this(sourceEnd, targetEnd)
        {
            this._mappingLogic = logic;
        }
        #region Mapping implementation

        public MP.MappingNode source
        {
            get => this._source;
            set => this._source = (MappingNode)value;
        }

        public MP.MappingNode target
        {
            get => this._target;
            set => this._target = (MappingNode)value;
        }

        public abstract void save();
        public abstract MP.MappingLogic mappingLogic { get; set; }
        public string mappingLogicDescription
        {
            get => this.mappingLogic != null ? this.mappingLogic.description : string.Empty;
            set
            {
                if (this.mappingLogic == null)
                {
                    this.mappingLogic = new MappingLogic(value);
                }
                else
                {
                    this.mappingLogic.description = value;
                }
            }
        }
        #endregion
    }
}
