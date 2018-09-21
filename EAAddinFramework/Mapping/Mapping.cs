using System;
using MP = MappingFramework;
using UML = TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;

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
            _source = sourceEnd;
            _target = targetEnd;
        }
        public Mapping(MappingNode sourceEnd, MappingNode targetEnd, MappingLogic logic) : this(sourceEnd, targetEnd)
        {
            _mappingLogic = logic;
        }
        //public Mapping(Element sourceElement, Element targetElement, string basepath)
        //    : this(new MappingNode(sourceElement, basepath), new MappingNode(targetElement)) { }
        //public Mapping(Element sourceElement, Element targetElement, string basepath, ElementWrapper targetRoot)
        //    : this(new MappingNode(sourceElement, basepath), new MappingNode(targetElement, targetRoot)) { }
        //public Mapping(Element sourceElement, Element targetElement, string basepath, string targetBasePath)
        //    : this(new MappingNode(sourceElement, basepath), new MappingNode(targetElement, targetBasePath)) { }

        /// <summary>
        /// strips the given path from the element name at the end.
        /// So if the path ends with .ElementName then that part is stripped off
        /// </summary>
        /// <param name="endElement">the element at the end of the mapping</param>
        /// <param name="path">the path string</param>
        /// <returns>the stripped path</returns>
        protected string stripPathFromElementName(Element endElement, string path)
        {
            if (path.EndsWith("." + endElement.name, StringComparison.InvariantCulture))
            {
                return path.Substring(0, path.Length - (endElement.name.Length + 1));
            }
            //else return unchanged
            return path;
        }
        #region Mapping implementation

        public MP.MappingNode source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = (MappingNode)value;
            }
        }

        public MP.MappingNode target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = (MappingNode)value;
            }
        }

        public abstract void save();
        public abstract MP.MappingLogic mappingLogic { get; set; }

        //public override string ToString()
        //{
        //    return this.source.fullMappingPath + " - " + this.target.fullMappingPath;
        //}

        #endregion
    }
}
