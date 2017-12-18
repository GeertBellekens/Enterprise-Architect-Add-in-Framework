using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of Index.
	/// </summary>
	public class Index : Constraint, DB.Index
	{
		public Index(Table owner,Operation operation) :
      base(owner, operation,owner.strategy.getStrategy<Index>()) {}

		public Index(Table owner, List<Column> involvedColumns) : 
      base(owner, involvedColumns,owner.strategy.getStrategy<Index>()) {}

		public override string itemType {
			get { return "Index"; }
		}

		internal bool? _isUnique;
    public virtual bool isUnique {
      get {
        if( ! this._isUnique.HasValue ) {
          this._isUnique = false;
          // lazy load
          foreach(var item in this._wrappedOperation.taggedValues) {
            if( item.name.Equals("Unique") ) {
              this._isUnique = item.tagValue.ToString().Equals("1");
            }
          }
        }
        return (bool)this._isUnique;
      }
      set {
        this._isUnique = value;
        string strValue = (bool)this._isUnique ? "1" : "0";
        // create tagged value if needed
        // if not overridden then we don't need the tagged value;
        if( this.wrappedElement != null ) {
          this.wrappedElement.addTaggedValue("Unique", strValue);
        } else if( this.logicalElement != null ) {
            ((Element)this.logicalElement).addTaggedValue( "Unique", strValue );
        }
      }
    }

		internal bool? _isClustered;
    public virtual bool isClustered {
      get {
        if( ! this._isClustered.HasValue ) {
          this._isClustered = false;
          // lazy load
          foreach(var item in this._wrappedOperation.taggedValues) {
            if( item.name.Equals("Clustered") ) {
              this._isClustered = item.tagValue.ToString().Equals("1");
            }
          }
        }
        return (bool)this._isClustered;
      }
      set {
        this._isClustered = value;
        string strValue = (bool)this._isClustered ? "1" : "0";
        // create tagged value if needed
        // if not overridden then we don't need the tagged value;
        if( this.wrappedElement != null ) {
          this.wrappedElement.addTaggedValue("Clustered", strValue);
        } else if( this.logicalElement != null ) {
            ((Element)this.logicalElement).addTaggedValue( "Clustered", strValue );
        }
      }
    }

		#region implemented abstract members of Constraint
		protected override string getStereotype()
		{
			return "index";
		}
		#endregion

		public override List<UML.Classes.Kernel.Element> logicalElements{
			get 
			{
				//there is no logical element for an index
				return new List<UML.Classes.Kernel.Element>();
			}
			set
			{
				//do nothing as there is not logical element for an index
			}
		}

		protected override DatabaseItem createAsNew(DatabaseItem owner, bool save = true)
		{
			Table newTable = owner as Table;
			Database existingDatabase = owner as Database;
			if (newTable == null)
			{
				//look for corresponding table in existingDatabase
				newTable = (Table)existingDatabase.tables.FirstOrDefault(x => x.name == this.ownerTable.name);
			}

			if (newTable != null )
			{
				var index = new Index(newTable, this._involvedColumns);
				index.isNew = true;
				index.name = name;
				if (save) index.save();
				return index;
			}
			return null;
		}

		protected override void saveMe() 
		{
			base.saveMe();
		    this.isUnique    = this.isUnique;
		    this.isClustered = this.isClustered;
		}

		internal override void createTraceTaggedValue()
		{
			throw new NotImplementedException();
		}

    // TODO ?
		internal override TaggedValue traceTaggedValue 
		{
			get 
			{
				if (_wrappedOperation != null) return _wrappedOperation.taggedValues.OfType<TaggedValue>()
					.FirstOrDefault(x => x.name.Equals("pkInfo",StringComparison.InvariantCultureIgnoreCase));
				//if no wrappped operation then no tagged value
				return null;
			}
			set 
			{
				if (_wrappedOperation != null
				   && value != null)
				{
					var tag = _wrappedOperation.addTaggedValue(value.name, value.eaStringValue);
					tag.comment = value.comment;
				}
			}
		}

	}
}
