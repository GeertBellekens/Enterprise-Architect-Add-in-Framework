
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
	/// Description of DB2ForeignKeyTransformer.
	/// </summary>
	public class DB2ForeignKeyTransformer:EAForeignKeyTransformer
	{
		public DB2ForeignKeyTransformer(ForeignKey foreignKey, UTF_EA.Association association,NameTranslator nameTranslator):base(foreignKey,association,nameTranslator)
		{
		}
		public DB2ForeignKeyTransformer(Table table,List<Column> FKInvolvedColumns,DB2TableTransformer dependingTransformer,NameTranslator nameTranslator):base(nameTranslator)
		{
			this._foreignKey = new ForeignKey((Table) table, FKInvolvedColumns);
			this._foreignKey.foreignTable = dependingTransformer.table;
			resetName();
			this._foreignKey.logicalAssociation = (UTF_EA.Association)dependingTransformer.associationEnd.association;
			table.constraints.Add(this._foreignKey);
		}
		#region implemented abstract members of EAForeignKeyTransformer

		public override void resetName()
		{
			this._foreignKey.name = "FK_" + this.foreignKey.ownerTable.name + "_" + this.foreignKey.foreignTable.name + "_1" ; //TODO: sequence number for multple foreign keys
		}

		#endregion

		#region implemented abstract members of EADatabaseItemTransformer

		public override void rename(string newName)
		{
			this.foreignKey.name = newName;
		}

		#endregion
	}
}
