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
			this.name = newDatabaseItem.name;
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
		public abstract UML.Classes.Kernel.Element logicalElement {get;}

		public abstract void createAsNewItem(DB.Database existingDatabase);

		public abstract bool isValid{get;}
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
		
	}
}
