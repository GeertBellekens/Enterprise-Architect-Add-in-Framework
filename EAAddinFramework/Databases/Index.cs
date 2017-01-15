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
      base(owner, operation) {}

		public Index(Table owner, List<Column> involvedColumns) : 
      base(owner, involvedColumns) {}

		public override string itemType {
			get { return "Index"; }
		}

    // TODO: lazy load value ?!
    // TODO: persist setting ?!
    public bool isUnique    { get; set; }
    public bool isClustered { get; set; }

		#region implemented abstract members of Constraint
		protected override string getStereotype()
		{
			return "index";
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
		}

		public override DB.DatabaseItem createAsNewItem(DB.DatabaseItem owner, bool save = true)
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
				index.name = name;
				if (save) index.save();
				return index;
			}
			return null;
		}

		public override void save()
		{
			base.save();
      // TODO ?
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
