using System.Collections.Generic;
using System.Linq;
using System;

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
		}
		private string getIndexName()
		{
			return "GBX1" + primaryKey.ownerTable.name.Substring(3,5);
		}
	}
}
