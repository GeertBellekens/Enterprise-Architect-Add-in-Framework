using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Databases.Strategy.MySQL
{
	/// <summary>
	/// Description of MySQLPrimaryKeyStrategy.
	/// </summary>
	public class MySQLForeignKeyStrategy:MySQLStrategy
	{
		ForeignKey foreignKey {get {return (ForeignKey) this.databaseItem; }}
		public MySQLForeignKeyStrategy(StrategyFactory factory):base(factory) {}
		
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
			return "FK_" + indexCounter + "_" + foreignKey.ownerTable.name + "_IX";
		}
		
	}
}
