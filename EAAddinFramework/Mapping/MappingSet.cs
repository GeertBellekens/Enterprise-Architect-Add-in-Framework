
using MappingFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TSF.UmlToolingFramework.UML.Classes.Kernel;
using TSF.UmlToolingFramework.Wrappers.EA;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;

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
            //get the mappings of the source object
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
            set { this._source = (MappingNode)value;
                this.sourceNodeDictionary = null;
            }
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
            this.source.getMappings();
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
        private Dictionary<string, MP.MappingNode> _sourceNodeDictionary;
        private Dictionary<string, MP.MappingNode> sourceNodeDictionary
        {
            get
            {
                if (this._sourceNodeDictionary == null)
                {
                    this._sourceNodeDictionary = new Dictionary<string, MP.MappingNode>();
                    ((MappingNode)this.source).addAllChildNodesToNodesDictionary(this._sourceNodeDictionary);
                }
                return this._sourceNodeDictionary;
            }
            set => this._sourceNodeDictionary = value;
        }
        private Dictionary<string, MP.MappingNode> _targetNodeDictionary;
        private Dictionary<string, MP.MappingNode> targetNodeDictionary
        {
            get
            {
                if (this._targetNodeDictionary == null)
                {
                    this._targetNodeDictionary = new Dictionary<string, MP.MappingNode>();
                    ((MappingNode)this.target).addAllChildNodesToNodesDictionary(this._targetNodeDictionary);
                }
                return this._targetNodeDictionary;
            }
            set => this._targetNodeDictionary = value;
        }
        public IEnumerable<MP.MappingNode> getFilteredNodes(string filter, bool isSource = true)
        {
            var filteredNodes = new List<MP.MappingNode>();
            var dictionaryToFilter = isSource ? this.sourceNodeDictionary : this.targetNodeDictionary;      
            //filter the keys of the sourcenodedictionary based on the given filter, with "*" as wildcard
            foreach (string key in dictionaryToFilter.Keys)
            {
                if (Regex.IsMatch(key, $"^{filter}$"))
                {
                    filteredNodes.Add(dictionaryToFilter[key]);
                }
            }
            return filteredNodes;
        }

        #endregion
    }
}
