
using System;
using System.Collections.Generic;
using DB = DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
namespace EAAddinFramework.Databases
{
    /// <summary>
    /// Description of DataType.
    /// </summary>
    public class DataType : DB.DataType
    {
        public DataType(BaseDataType baseType, int length, int precision)
        {
            this.type = baseType;
            this.length = length;
            this.precision = precision;
        }

        #region DataType implementation

        public DB.BaseDataType type { get; set; }

        public List<DB.DatabaseItem> mergedEquivalents
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public bool isNotRealized
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public void save()
        {
            throw new NotImplementedException();
        }
        public void delete()
        {
            throw new NotImplementedException();
        }

        public int position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DB.DatabaseItem derivedFromItem
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool isValid
        {
            get
            {
                //datatype is valid if it has a name
                return (!string.IsNullOrEmpty(this.name));

            }
        }
        public void Select()
        {
            //you cannot select a Datatype
        }
        public List<UML.Classes.Kernel.Element> logicalElements
        {
            get
            {
                // datatypes don't have logical elements
                return new List<UML.Classes.Kernel.Element>();
            }
        }
        public DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner, bool save = true)
        {
            throw new NotImplementedException();
        }

        public DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner, string newName, bool save = true)
        {
            throw new NotImplementedException();
        }

        public void update(DB.DatabaseItem newDatabaseItem, bool save = true)
        {
            //don't think we need it here
            throw new NotImplementedException();
        }
        public DB.DatabaseItem owner
        {
            get
            {
                //don't think we need it here
                throw new NotImplementedException();
            }
        }

        public DB.DataBaseFactory factory
        {
            get
            {
                //don't think we need that here
                throw new NotImplementedException();
            }
        }

        public string itemType
        {
            get { return "DataType"; }
        }
        public string properties
        {
            get
            {
                string _properties = this.type?.properties;
                if (this.type != null && this.type.hasLength)
                {
                    _properties += " (" + this.length;
                    if (this.type.hasPrecision)
                    {
                        _properties += "," + this.precision;
                    }
                    _properties += ")";
                }
                return _properties;
            }
        }
        public string name
        {
            get { return this.type?.name; }
            set { throw new NotImplementedException(); }
        }
        public int length { get; set; }

        public int precision { get; set; }



        #endregion
        //datatype can't be overriden
        public bool isOverridden
        {
            get
            {
                return false;
            }
            set
            {
                //do nothing
            }
        }

        public bool isRenamed
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string renamedName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool isEqualDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string logicalName
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
