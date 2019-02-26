using System.Collections.Generic;
using System.Linq;
using System;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Databases.Strategy.MySQL
{
	/// <summary>
	/// Description of MySQLPrimaryKeyStrategy.
	/// </summary>
	public class MySQLPrimaryKeyStrategy:MySQLStrategy
	{
		PrimaryKey primaryKey {get {return (PrimaryKey) this.databaseItem; }}
		public MySQLPrimaryKeyStrategy(StrategyFactory factory):base(factory) {}
		
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
			return "PK_" + primaryKey.ownerTable.name + "_IX";
		}
	}
}
