
using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA=TSF.UmlToolingFramework.Wrappers.EA;
using DB=DatabaseFramework;
using DB_EA = EAAddinFramework.Databases;
using EAAddinFramework.Utilities;

namespace EAAddinFramework.Databases.Transformation
{
	/// <summary>
	/// Description of EAPrimaryKeyTransformer.
	/// </summary>
	public abstract class EAPrimaryKeyTransformer:EADatabaseItemTransformer, DB.Transformation.PrimaryKeyTransformer
	{
		internal PrimaryKey _primarykey;
		internal Table _table;
		public EAPrimaryKeyTransformer(Table table, NameTranslator nameTranslator):base(nameTranslator)
		{	
			_table = table;
		}


		public DB.PrimaryKey primaryKey {
			get {
				return _primarykey;
			}
			set {
				this._primarykey = (PrimaryKey)value;
			}
		}

		public abstract void resetName();
	
	}
}
