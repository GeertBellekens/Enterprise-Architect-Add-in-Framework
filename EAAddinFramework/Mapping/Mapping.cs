using System;
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

        public void save()
        {
            if (this.isReadOnly)
            {
                throw new InvalidOperationException($"Element {this.source.name} is read-only");
            }
            this.saveMe();
        }
        protected abstract void saveMe();

        public abstract void deleteWrappedItem();
        public void delete()
        {
            if (this.isReadOnly)
            {
                throw new InvalidOperationException($"Element {this.source.name} is read-only");
            }
            //remove from source and target
            this.source.removeMapping(this);
            this.target.removeMapping(this);
            this.deleteWrappedItem();
        }

        public abstract MP.MappingLogic mappingLogic { get; set; }
        public string mappingLogicDescription
        {
            get => this.mappingLogic != null ? this.mappingLogic.description : string.Empty;
            set
            {
                if (this.mappingLogic == null )
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        this.mappingLogic = new MappingLogic(value);
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(value))
                    {
                        //set the value
                        this.mappingLogic.description = value;
                    }
                    else
                    {
                        //delete the mapping
                        this.mappingLogic.delete();
                        this.mappingLogic = null;
                    }
                }
            }
        }

        public abstract bool isEmpty { get; set; }

        public bool isReadOnly => this.source.isReadOnly;
        #endregion
    }
}
