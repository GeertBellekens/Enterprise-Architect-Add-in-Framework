
using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA=TSF.UmlToolingFramework.Wrappers.EA;
using DB=DatabaseFramework;
using DB_EA = EAAddinFramework.Databases;

namespace EAAddinFramework.Databases.Transformation.DB2
{
	/// <summary>
	/// Description of DB2TableTransformer.
	/// </summary>
	public class DB2TableTransformer:EATableTransformer
	{
		protected List<DB2ColumnTransformer> _columnTransformers = new List<DB2ColumnTransformer>();
		public DB2TableTransformer(Database database):base(database){}
		
		#region implemented abstract members of EATableTransformer
		protected override void createTable(System.Collections.Generic.List<UML.Classes.Kernel.Class> logicalClasses)
		{
			throw new NotImplementedException();
		}
		protected override void createTable(UTF_EA.Class classElement)
		{
			if (classElement.alias == string.Empty) classElement.alias = "unknown table name";
			this.table = new Table(_database, classElement.alias);
		}

		protected override Column transformLogicalAttribute(UTF_EA.Attribute attribute)
		{
			var columnTransformer = new DB2ColumnTransformer(this._table);
			this._columnTransformers.Add(columnTransformer);
			return (Column) columnTransformer.transformLogicalProperty(attribute);
		}

		public override List<DB.Transformation.ColumnTransformer> columnTransformers {
			get { return _columnTransformers.Cast<DB.Transformation.ColumnTransformer>().ToList();}
			set { _columnTransformers = value.Cast<DB2ColumnTransformer>().ToList();}
		}

		#endregion
	}
}
