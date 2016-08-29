
using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA=TSF.UmlToolingFramework.Wrappers.EA;
using DB=DatabaseFramework;
using DB_EA = EAAddinFramework.Databases;
using EAAddinFramework.Utilities;

namespace EAAddinFramework.Databases.Transformation.DB2
{
	/// <summary>
	/// Description of DB2PrimaryKeyTransformer.
	/// </summary>
	public class DB2PrimaryKeyTransformer:EAPrimaryKeyTransformer
	{
		public DB2PrimaryKeyTransformer(Table table, List<Column> involvedColumns):base(table)
		{
			this._table.primaryKey = new DB_EA.PrimaryKey((DB_EA.Table)table, involvedColumns);
			this._table.primaryKey.name = "PK_" + table.name;
		}
	}
}
