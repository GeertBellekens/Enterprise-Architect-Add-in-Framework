using System;
using System.Linq;
using TSF.UmlToolingFramework.Wrappers.EA;
using MP = MappingFramework;

namespace EAAddinFramework.Mapping
{
    /// <summary>
    /// Description of ConnectorMapping.
    /// </summary>
    public class ConnectorMapping : Mapping
    {
        internal ConnectorWrapper wrappedConnector { get; private set; }
        public ConnectorMapping(ConnectorWrapper wrappedConnector, MappingNode sourceEnd, MappingNode targetEnd) : base(sourceEnd, targetEnd)
        {
            this.wrappedConnector = wrappedConnector;
        }

        #region implemented abstract members of Mapping

        public override MP.MappingLogic mappingLogic
        {
            get
            {
                //check via linked element
                //TODO: implement in a faster way if mapping via elements is supported again
                //if (this._mappingLogic == null)
                //{
                //    //check links to notes or constraints
                //    //TODO: make a difference between regular notes and mapping logic?
                //    var connectedElement = this.wrappedConnector.getLinkedElements().OfType<ElementWrapper>().FirstOrDefault(x => x.notes.Length > 0);
                //    if (connectedElement != null)
                //    {
                //        this._mappingLogic = new MappingLogic(connectedElement);
                //    }
                //}
                // check via tagged value
                if (this._mappingLogic == null)
                {
                    var logicString = getTaggedValueSafe(MappingFactory.mappingLogicName);
                    if (!string.IsNullOrEmpty(logicString))
                    {
                        this._mappingLogic = new MappingLogic(logicString);
                    }
                }
                return this._mappingLogic;
            }
            set
            {
                var mappingElementWrapper = value?.mappingElement as ElementWrapper;
                if (mappingElementWrapper != null)
                {
                    this.wrappedConnector.addTaggedValue(MappingFactory.mappingLogicName, mappingElementWrapper.uniqueID);
                    //TODO get this working for at least notes and constraints, for now we go with a tagged value
                    // this.wrappedConnector.addLinkedElement(mappingElementWrapper);
                }
                else
                {
                    addTaggedValueSafe(MappingFactory.mappingLogicName, value.description);   
                }

            }
        }
        private bool? _isEmpty = null;
        public override bool isEmpty
        {
            get
            {
                if (!this._isEmpty.HasValue)
                {
                    var emptyTag = this.wrappedConnector.taggedValues
                        .FirstOrDefault(x => x.name.Equals(MappingFactory.isEmptyMappingName, StringComparison.InvariantCultureIgnoreCase));
                    return "True".Equals(emptyTag?.tagValue.ToString(), StringComparison.InvariantCultureIgnoreCase); 
                }
                return this._isEmpty.Value;
            }
            set
            {
                this.wrappedConnector.addTaggedValue(MappingFactory.isEmptyMappingName, value.ToString());
            }
        }

        #endregion

        #region implemented abstract members of Mapping

        protected override void saveMe()
        {
            if (this._mappingLogic != null)
            {
                this.mappingLogic = this._mappingLogic; //make sure to set the mapping logic value correctly
            }
            //set mapping path
            if (this.source.structure == MP.ModelStructure.Message || this.source.isVirtual)
            {
                addTaggedValueSafe(MappingFactory.mappingSourcePathName, string.Join(".", ((MappingNode)this.source).getMappingPath()));
            }
            if (this.target.structure == MP.ModelStructure.Message || this.target.isVirtual)
            {
                addTaggedValueSafe(MappingFactory.mappingTargetPathName, string.Join(".", ((MappingNode)this.target).getMappingPath()));
            }
            this.wrappedConnector.save();
        }

        public override void deleteWrappedItem()
        {
            this.wrappedConnector?.delete();
        }
        private void addTaggedValueSafe(string tagName, string value)
        {
            if (value.Length < 255)
            {
                this.wrappedConnector.addTaggedValue(tagName, value);
            }
            else
            {
                this.wrappedConnector.addTaggedValue(tagName, "<memo>", value);
            }
        }
        private string getTaggedValueSafe(string tagName)
        {
            //get the tag
            var tag = this.wrappedConnector.taggedValues
                        .FirstOrDefault(x => x.name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase));
            //return string value
            return tag?.tagValue?.ToString() == "<memo>" ? tag?.comment : tag?.tagValue?.ToString();
        }

        #endregion
    }
}
