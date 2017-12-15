using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Databases.Strategy.DB2
{
	/// <summary>
	/// Description of DB2PrimaryKeyStrategy.
	/// </summary>
	public class DB2ForeignKeyStrategy:DB2Strategy
	{
		ForeignKey foreignKey {get {return (ForeignKey) this.databaseItem; }}
		public DB2ForeignKeyStrategy(StrategyFactory factory):base(factory) {}
		
		public override void beforeSave()
		{
			if (foreignKey.wrappedElement == null)
			{
				var fkIndex = new Index((Table)foreignKey.ownerTable,foreignKey.involvedColumns.Cast<Column>().ToList());
				fkIndex.name = getIndexName();
				fkIndex.isUnique = false;
				fkIndex.isClustered = false;
				fkIndex.save();
			}
		}
		private string getIndexName()
		{
			int indexCounter = foreignKey.ownerTable.constraints.OfType<Index>().Count();
			return "GBX" + indexCounter + foreignKey.ownerTable.name.Substring(3,5);
		}
		
	}
}
