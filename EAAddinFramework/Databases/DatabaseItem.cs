using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of DatabaseItem.
	/// </summary>
	public abstract class DatabaseItem:DB.DatabaseItem
	{
		internal abstract Element wrappedElement {get;set;}
		internal abstract TaggedValue traceTaggedValue {get;set;}
		internal abstract void createTraceTaggedValue();
		internal bool? _isOverridden;
		string _renamedName = string.Empty;
		public string renamedName 
		{
			get 
			{
				if (string.IsNullOrEmpty(_renamedName) && isRenamed)
				{
					_renamedName = name;
				}
				return _renamedName;
			}
			set
			{
				_renamedName = value;
			}
		}
		bool? _isRenamed;
		public bool isRenamed 
		{
			get 
			{
				if (! _isRenamed.HasValue)
				{
					//get the tagged value
					if (this.wrappedElement != null)
					{
						_isRenamed = this.wrappedElement.taggedValues
							.Any( x => x.name.Equals("dbrename",StringComparison.InvariantCultureIgnoreCase)
							     && x.tagValue.ToString().Equals("true",StringComparison.InvariantCultureIgnoreCase));
					}
					else
					{
						return false;
					}
				}
				return _isRenamed.Value;
			}
			set
			{
				this._isRenamed = value;
				//create tagged value if needed
				if (this.wrappedElement != null)
				{
					if (value)
					{
						wrappedElement.addTaggedValue("dbrename",value.ToString().ToLower());
					}
					else
					{
						//if the tagged value exists then set it to false
						if (wrappedElement.taggedValues
							.Any(x => x.name.Equals("dbrename",StringComparison.InvariantCultureIgnoreCase)))
						{
							wrappedElement.addTaggedValue("dbrename",value.ToString().ToLower());
						}
					}
				}
			}
		}
		public virtual DB.DataBaseFactory factory 
		{
			get 
			{
				if (owner != null)
				return  owner.factory;
				return null;
			}
		}

		public void update(DB.DatabaseItem newDatabaseItem, bool save = true)
		{
			//check if types are the same
			if (newDatabaseItem.itemType != this.itemType) throw new Exception(string.Format("Cannot update object of type {0} with object of type {1}",this.itemType, newDatabaseItem.itemType));
			//only take over name if the new item is not renamed
			if (!((DatabaseItem)newDatabaseItem).isRenamed) this.name = newDatabaseItem.name;
			this.isOverridden = newDatabaseItem.isOverridden;
			this.updateDetails(newDatabaseItem);
			if (save)
			{
				this.save();
			}
		}

		public void Select()
		{
			if (this.wrappedElement != null) this.wrappedElement.select();
		}
		public abstract List<UML.Classes.Kernel.Element> logicalElements {get;set;}
		public UML.Classes.Kernel.Element logicalElement
		{
			get{ return this.logicalElements.FirstOrDefault();}
		}

		public abstract bool isValid{get;}

		public virtual int position 
		{
			get 
			{
				//default implmementation
				return 0;
			}
			set 
			{
				//default implementation: do nothing
			}
		}
		public DB.DatabaseItem derivedFromItem {get;set;}
		public abstract DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner, bool save = true);

		public DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner, string newName, bool save = true)
		{
			//temporarily rename this item to the new name
			string originalName = this.name;
			this.name = newName;
			//create the new items
			var newItem = this.createAsNewItem(owner, save);
			//rename back to original
			this.name = originalName;
			//return the new item
			return newItem;
		}
		protected abstract void updateDetails(DB.DatabaseItem newDatabaseItem);
		internal DatabaseFactory _factory
		{
			get
			{
				return factory as DatabaseFactory;
			}

		}
		public virtual bool isOverridden 
		{
			get 
			{
				if (! _isOverridden.HasValue)
				{
					//get the tagged value
					if (this.wrappedElement != null)
					{
						_isOverridden = this.wrappedElement.taggedValues
							.Any( x => x.name.Equals("dboverride",StringComparison.InvariantCultureIgnoreCase)
							     && x.tagValue.ToString().Equals("true",StringComparison.InvariantCultureIgnoreCase));
					}
					else if (this.logicalElement != null)
					{
						_isOverridden = this.logicalElement.taggedValues
							.Any( x => x.name.Equals("dboverride",StringComparison.InvariantCultureIgnoreCase)
							     && x.tagValue.ToString().Equals("true",StringComparison.InvariantCultureIgnoreCase));
					}
					else
					{
						return false;
					}
				}
				return _isOverridden.Value;
			}
			set
			{
				this._isOverridden = value;
				//create tagged value if needed if not overridden then we don't need the tagged value;
				if (this.wrappedElement != null)
				{
					wrappedElement.addTaggedValue("dboverride",value.ToString().ToLower());
				}
				else if (this.logicalElement != null)
				{
					((Element)this.logicalElement).addTaggedValue("dboverride",value.ToString().ToLower());
				}
			}
		}

		public abstract DB.DatabaseItem owner {get;}
		
		internal DatabaseItem _owner
		{
			get
			{
				return owner as DatabaseItem;
			}
		}
		
		public abstract void save();

		public abstract void delete();

		public abstract string name {get;set;}

		public abstract string itemType {get;}

		public abstract string properties {get;}
		public override bool Equals(object obj)
		{
			DatabaseItem other = obj as DatabaseItem;
			if (other == null) return false;
			bool ownerEqual = false;
			ownerEqual = this.owner != null && this.owner.Equals(other.owner) || this.owner == null && other.owner == null;
			bool wrappedElementEqual = false;
			wrappedElementEqual = this.wrappedElement != null && this.wrappedElement.Equals(other.wrappedElement)
				|| this.wrappedElement == null && other.wrappedElement == null;
			bool logicalElementsEqual = false;
			logicalElementsEqual = this.logicalElements.All(x => other.logicalElements.Any(y => y.Equals(x)))
				|| this.logicalElements.Count == 0 && other.logicalElements.Count == 0;
			bool itemTypeEquals = this.itemType == other.itemType;
			bool nameEquals = this.name == other.name;
			return ownerEqual && wrappedElementEqual && logicalElementsEqual && itemTypeEquals && nameEquals;
		}
	}
}
