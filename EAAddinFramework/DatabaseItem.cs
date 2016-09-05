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
		public virtual DB.DataBaseFactory factory 
		{
			get 
			{
				if (owner != null)
				return  owner.factory;
				return null;
			}
		}

		public void Update(DB.DatabaseItem newDatabaseItem)
		{
			//check if types are the same
			if (newDatabaseItem.itemType != this.itemType) throw new Exception(string.Format("Cannot update objec of type {0} with object of type {1}",this.itemType, newDatabaseItem.itemType));
			this.name = newDatabaseItem.name;
			this.udateDetails(newDatabaseItem);
			this.save();
		}

		public abstract void createAsNewItem(DB.Database existingDatabase);

		protected abstract void udateDetails(DB.DatabaseItem newDatabaseItem);
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
				return (this.traceTaggedValue != null 
				        && this.traceTaggedValue.comment.ToLower().Contains("dboverride=true"));
			}
			set
			{
				//create tagged value if needed if not overridden then we don't need the tagged value;
				if (value)
				{
					if (this.traceTaggedValue == null) 
					{
						createTraceTaggedValue();
					}
				}
				if (this.traceTaggedValue != null)
				{
					//set the value
					string goodString = "dboverride=" + value.ToString().ToLower();
					string replaceString = "dboverride=" + (!value).ToString().ToLower();
	
					if(this.traceTaggedValue.comment.ToLower().Contains(replaceString))
					{
						this.traceTaggedValue.comment = this.traceTaggedValue.comment.ToLower().Replace(replaceString,goodString);
					}
					else
					{
						this.traceTaggedValue.comment += goodString;
					}
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
