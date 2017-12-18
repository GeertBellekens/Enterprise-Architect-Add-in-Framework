using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Databases.Strategy.DB2
{
	/// <summary>
	/// Description of DB2PrimaryKeyStrategy.
	/// </summary>
	public class DB2ColumnStrategy:DB2Strategy
	{
		Column column {get {return (Column) this.databaseItem; }}
		public DB2ColumnStrategy(StrategyFactory factory):base(factory) {}
		
		public override void beforeSave()
		{
			if (column.isNew 
				&& column.isNotNullable
			   	&& string.IsNullOrEmpty(column.initialValue))
			{
				//not nullable columns that are not part of a Primary key get "DEFAULT" as default value, which results in "WITH DEFAULT" in the DDL
				//if the columns turns out to be part of a Primary key then the DEFAULT is taken back off afterwards in the PrimaryKeyStrategy
				column.initialValue = "DEFAULT"; 
			}
		}
	}
}
