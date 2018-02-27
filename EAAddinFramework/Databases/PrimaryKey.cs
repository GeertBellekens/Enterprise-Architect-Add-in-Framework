
using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of PrimaryKey.
	/// </summary>
	public class PrimaryKey:Constraint, DB.PrimaryKey
	{

		
		public PrimaryKey(Table owner,Operation operation) : base(owner, operation,owner.strategy.getStrategy<PrimaryKey>())
		{
			
		}
		public PrimaryKey(Table owner, List<Column> involvedColumns):base(owner, involvedColumns, owner.strategy.getStrategy<PrimaryKey>())
		{
			
		}
		public override string itemType {
			get {return "Primary Key";}
		}

		#region implemented abstract members of Constraint
		protected override string getStereotype()
		{
			return "PK";
		}
		#endregion

		public override List<UML.Classes.Kernel.Element> logicalElements{
			get {
				var _logicalElements = new List<UML.Classes.Kernel.Element>();
				foreach (var involvedColumn in involvedColumns) 
				{
					_logicalElements.AddRange(involvedColumn.logicalElements);
				}
				return _logicalElements;
			}
			set
			{
				//no nothing, you cannot set the logical element directly but you have to go through the involved columns.
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
			if (newTable != null && newTable.primaryKey == null ) //only create it if htere is not already one
			{
				var newPrimaryKey = new PrimaryKey(newTable,this._involvedColumns);
				newPrimaryKey.name = name;
				newPrimaryKey.isNew = true;
				//newPrimaryKey.isOverridden = this.isOverridden;
				newPrimaryKey.derivedFromItem = this;
				if (save) newPrimaryKey.save();
				return newPrimaryKey;
			}
			return null;
		}
		protected override void saveMe()
		{
			base.saveMe();
			//loop all columns and add or remove their primary key indicator
			foreach (Column column in this.ownerTable.columns) 
			{
				if (this.involvedColumns.Any(x => x.name == column.name))
			    {
					column.wrappedattribute.wrappedAttribute.IsOrdered = true;
			    }
				else
				{
					column.wrappedattribute.wrappedAttribute.IsOrdered = false;
				}
				column.wrappedattribute.save();
			}
		}

		internal override void createTraceTaggedValue()
		{
			throw new NotImplementedException();
		}
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
