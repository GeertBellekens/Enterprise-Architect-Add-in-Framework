using System;
using System.Collections.Generic;
using EAAddinFramework.Databases.Strategy;
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
		public DatabaseItemStrategy strategy {get;private set;}
		protected DatabaseItem(DatabaseItemStrategy strategy)
		{
			this.strategy = strategy;
			this.strategy.databaseItem = this;
			this.strategy.onNew();
		}
		internal abstract Element wrappedElement {get;set;}
		internal abstract TaggedValue traceTaggedValue {get;set;}
		internal abstract void createTraceTaggedValue();
		internal bool? _isOverridden;
		internal bool? _isRenamed;
		string _renamedName = string.Empty;
		public bool isEqualDirty {get;set;}
		public bool isNew{get;protected set;}
		
		public string _logicalName;
		public string logicalName 
		{
			get
			{
				if (string.IsNullOrEmpty(_logicalName))
				{
					_logicalName = logicalElement != null ? 
										logicalElement.name : 
										string.Empty;
				}
				return _logicalName;
			}
		}

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
		List<DB.DatabaseItem> _mergedEquivalents;
		public List<DB.DatabaseItem> mergedEquivalents
		{
			get
			{
				if (_mergedEquivalents == null)
				{
					_mergedEquivalents = new List<DB.DatabaseItem>();
				}
				return _mergedEquivalents;
			}
			set
			{
				_mergedEquivalents = value;
			}
		}
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

		public abstract bool isValid {get;}
		

		public virtual int position 
		{
			get 
			{
				//default implementation
				return 0;
			}
			set 
			{
				//default implementation: do nothing
			}
		}
		public DB.DatabaseItem derivedFromItem {get;set;}
		public DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner, bool save = true)
		{
			//create as new
			var newItem = createAsNew((DatabaseItem)owner, save);
			//return
			return newItem;
		}
		protected abstract DatabaseItem createAsNew(DatabaseItem owner, bool save = true);

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
		/// <summary>
		/// indicates wether this item is not to be realized in the physical database.
		/// Will be true when a logical item with no physical counterpart is being overridden.
		/// Thas means that this item will not be realized as a physical item in the database
		/// </summary>
		bool _isNotRealized;
		public bool isNotRealized
		{
			get
			{
				foreach (var logicalItem in logicalElements)
				{
					var isNotRealizedTaggedValue = logicalItem.taggedValues.FirstOrDefault(x => x.name == "isNotRealized");
					if (isNotRealizedTaggedValue != null)
					{
						string taggedValueValue = isNotRealizedTaggedValue.tagValue.ToString();
						foreach (string taggedValuePart in taggedValueValue.Split(';')) 
						{
							if (taggedValuePart.Equals(this.owner.name,StringComparison.InvariantCulture))
							{
								this._isNotRealized = true;
								return _isNotRealized;
							}
						}
					}
				}
				return _isNotRealized;
			}
			set
			{
				this._isNotRealized = value;
				foreach (var logicalItem in logicalElements) 
				{
					
					//check if the tagged value exists
					var isNotRealizedTaggedValue = logicalItem.taggedValues.FirstOrDefault(x => x.name == "isNotRealized");
					string taggedValueValue = isNotRealizedTaggedValue != null ? isNotRealizedTaggedValue.tagValue.ToString(): string.Empty;
					if (!taggedValueValue.Contains(this.owner.name))
					{
						//add the owner in which this item is not realized
						if (value) taggedValueValue += this.owner.name + ";";
					}
					else
					{
						if (!value)
						{
							var taggedValueParts = taggedValueValue.Split(';').ToList();
							foreach (string taggedValuePart in taggedValueParts)
							{
								if (taggedValuePart.Equals(this.owner.name,StringComparison.InvariantCulture))
							    {
									//if it equals the name of the owner then remove that part
							    	taggedValueParts.Remove(taggedValuePart);
							    	taggedValueValue = string.Join(";",taggedValueParts.ToArray());
							    	break;
							    }
							}
						}
					}
					//add the tagged value
					((Element)logicalItem).addTaggedValue("isNotRealized",taggedValueValue);
				}
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
		
		private DatabaseItem _owner
		{
			get
			{
				return owner as DatabaseItem;
			}
		}
		
		public void save()
		{
			this.strategy.beforeSave();
			saveMe();
			this.strategy.afterSave();
		}
		protected abstract void saveMe();

		public void delete()
		{
			this.strategy.beforeDelete();
			deleteMe();
			this.strategy.afterDelete();
		}
		protected abstract void deleteMe();
		

		public abstract string name {get;set;}

		public abstract string itemType {get;}

		public abstract string properties {get;}
		public override bool Equals(object obj)
		{
			DatabaseItem other = obj as DatabaseItem;
			if (other == null) return false;
			bool nameEquals = this.name == other.name;
			if (! nameEquals) return false;
			//check itemType
			bool itemTypeEquals = this.itemType == other.itemType;
			if (! itemTypeEquals) return false;
			//check wrapped element
			bool wrappedElementEqual = false;
			wrappedElementEqual = this.wrappedElement != null && this.wrappedElement.Equals(other.wrappedElement)
				|| this.wrappedElement == null && other.wrappedElement == null;
			if (!wrappedElementEqual) return false;
			//check owner
			bool ownerEqual = false;
			ownerEqual = this.owner != null && this.owner.Equals(other.owner) || this.owner == null && other.owner == null;
			if (! ownerEqual) return false;
			//check logical elements
			bool logicalElementsEqual = false;
			logicalElementsEqual = this.logicalElements.All(x => other.logicalElements.Any(y => y.Equals(x)))
				|| this.logicalElements.Count == 0 && other.logicalElements.Count == 0;
			return logicalElementsEqual;
		}


		public static bool operator ==(DatabaseItem lhs, DatabaseItem rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DatabaseItem lhs, DatabaseItem rhs) {
			return !(lhs == rhs);
		}
	}
}
