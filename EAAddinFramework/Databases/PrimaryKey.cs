
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


		public override void createAsNewItem(DB.Database existingDatabase)
		{
			//look for corresponding table in existingDatabase
			Table newTable = (Table)existingDatabase.tables.FirstOrDefault(x => x.name == this.owner.name);
			if (newTable != null )
			{
				var newPrimaryKey = new PrimaryKey(newTable,this._involvedColumns);
				newPrimaryKey.name = name;
				newPrimaryKey.save();
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
