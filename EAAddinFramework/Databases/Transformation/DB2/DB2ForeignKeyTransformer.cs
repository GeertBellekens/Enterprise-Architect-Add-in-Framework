
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
		
		public DB2ForeignKeyTransformer(ForeignKey foreignKey, UTF_EA.AssociationEnd associationEnd,NameTranslator nameTranslator):base(foreignKey,associationEnd,nameTranslator)
		{
		}
		public DB2ForeignKeyTransformer(Table table,List<Column> FKInvolvedColumns,DB2TableTransformer dependingTransformer,UTF_EA.AssociationEnd associationEnd, NameTranslator nameTranslator):base(nameTranslator)
		{
			this._foreignKey = new ForeignKey((Table) table, FKInvolvedColumns);
			this._foreignKey.foreignTable = dependingTransformer.table;
			this._logicalAssociationEnd = associationEnd;
			//get the association on which this foreign key is based
			if (associationEnd != null) this._foreignKey.logicalAssociation = (UTF_EA.Association)associationEnd.association;
			table.constraints.Add(this._foreignKey);
			//reset the name of the foreign key
			resetName();
			//make sure we get the correct override settings for the FK and the involved columns
			var dummy = this._foreignKey.isOverridden;
		}
		#region implemented abstract members of EAForeignKeyTransformer

		public override void resetName()
		{
			if (this.logicalAssociation != null
			    && ! string.IsNullOrEmpty(((UTF_EA.Association)this.logicalAssociation).alias))
			{
				this._foreignKey.name =  ((UTF_EA.Association)this.logicalAssociation).alias;
			}
			else
			{
				this._foreignKey.name = "FK_" + this.foreignKey.ownerTable.name + "_" + this.foreignKey.foreignTable.name + "_1" ; //TODO: sequence number for multple foreign keys
			}				
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
