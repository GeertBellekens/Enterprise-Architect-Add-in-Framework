using System.Collections.Generic;
using System.Linq;
using System;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Databases.Strategy.DB2
{
	/// <summary>
	/// Description of DB2PrimaryKeyStrategy.
	/// </summary>
	public class DB2PrimaryKeyStrategy:DB2Strategy
	{
		PrimaryKey primaryKey {get {return (PrimaryKey) this.databaseItem; }}
		public DB2PrimaryKeyStrategy(StrategyFactory factory):base(factory) {}
		
		public override void beforeSave()
		{
			if (primaryKey.wrappedElement == null)
			{
				var pkIndex = new Index((Table)primaryKey.ownerTable,primaryKey.involvedColumns.Cast<Column>().ToList());
				pkIndex.name = getIndexName();
				pkIndex.isUnique = true;
				pkIndex.isClustered = true;
				pkIndex.save();
			}
			//check if any of the columns still have the DEFAULT initial value while beeing new
			foreach (var column in primaryKey.involvedColumns.OfType<Column>()
			         .Where(x => x.isNew 
			                && x.isNotNullable
			               && x.initialValue == "DEFAULT"))
			{
				column.initialValue = string.Empty;
				column.save();
			}

		}
		private string getIndexName()
		{
			return "GBX1" + primaryKey.ownerTable.name.Substring(3,5);
		}
	}
}
