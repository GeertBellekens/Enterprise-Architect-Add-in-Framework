
using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of PrimaryKey.
	/// </summary>
	public class PrimaryKey:Constraint, DB.PrimaryKey
	{

		
		public PrimaryKey(Table owner,Operation operation) : base(owner, operation)
		{
			
		}
		public PrimaryKey(Table owner, List<Column> involvedColumns):base(owner, involvedColumns)
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

		public override TSF.UmlToolingFramework.UML.Classes.Kernel.Element logicalElement {
			get {
				var firstColumn = this.involvedColumns.FirstOrDefault();
				if (firstColumn != null) return firstColumn.logicalElement;
				return null;
			}
		}
		public override DB.DatabaseItem createAsNewItem(DB.Database existingDatabase, bool save = true)
		{
			//look for corresponding table in existingDatabase
			Table newTable = (Table)existingDatabase.tables.FirstOrDefault(x => x.name == this.owner.name);
			if (newTable != null && newTable.primaryKey == null ) //only create it if htere is not already one
			{
				var newPrimaryKey = new PrimaryKey(newTable,this._involvedColumns);
				newPrimaryKey.name = name;
				//newPrimaryKey.isOverridden = this.isOverridden;
				newPrimaryKey.derivedFromItem = this;
				if (save) newPrimaryKey.save();
				return newPrimaryKey;
			}
			return null;
		}
		public override void save()
		{
			base.save();
			//loop all columns and add or remove their primary key indicator
			foreach (Column column in this.ownerTable.columns) 
			{
				if (this.involvedColumns.Any(x => x.name == column.name))
			    {
					column._wrappedattribute.wrappedAttribute.IsOrdered = true;
			    }
				else
				{
					column._wrappedattribute.wrappedAttribute.IsOrdered = false;
				}
				column._wrappedattribute.save();
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
