using System;
using System.Collections.Generic;
using System.Linq;
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
        public Mapping(MappingNode sourceEnd, MappingNode targetEnd)
        {
            this._source = sourceEnd;
            this._source.addMapping(this);
            this._target = targetEnd;
            this._target.addMapping(this);
        }
        public Mapping(MappingNode sourceEnd, MappingNode targetEnd, IEnumerable<MappingLogic> logics) : this(sourceEnd, targetEnd)
        {
            this.mappingLogics = logics;
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

        public void addMappingLogic(MP.MappingLogic mappingLogic)
        {
            this.EAMappingLogics.Add((MappingLogic)mappingLogic);
        }

        public void removeMappingLogic(MP.MappingLogic mappingLogic)
        {
            this.EAMappingLogics.Remove((MappingLogic)mappingLogic);
        }

        private List<MappingLogic> _EAMappingLogics;

        protected List<MappingLogic> EAMappingLogics
        {
            get
            {
                if (this._EAMappingLogics == null)
                {
                    this._EAMappingLogics = this.loadMappingLogics();
                }
                //make sure all the contexts are valid
                this.validateContexts();
                return this._EAMappingLogics;
            }
            set
            {
                this._EAMappingLogics = value;
            }
        }
        private void validateContexts()
        {
            var updated = false;
            foreach (var mappingLogic in this._EAMappingLogics)
            {
                //if the contex it not allowed in the mappingset then remove it
                if (mappingLogic.context != null && 
                    ! this.source.mappingSet.contexts.Any(x => x.uniqueID == mappingLogic.context.uniqueID))
                {
                    mappingLogic.context = null;
                    updated = true;
                }
            }
            if (updated)
            {
                //save if any of the contexts have been cleared.
                this.save();
            }
        }

        protected abstract List<MappingLogic> loadMappingLogics();

        public IEnumerable<MP.MappingLogic> mappingLogics
        {
            get => this.EAMappingLogics;
            set => this.EAMappingLogics = value.Cast<MappingLogic>().ToList();
        }


        public abstract bool isEmpty { get; set; }
        public abstract bool isReverseEmpty { get; set; }

        public bool isReadOnly => this.source.isReadOnly;

        public abstract MP.MappingSet mappingSet { get; set; }

        #endregion
    }
}
