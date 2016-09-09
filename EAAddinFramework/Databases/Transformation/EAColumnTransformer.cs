

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
	/// Description of EAColumnTransformer.
	/// </summary>
	public abstract class EAColumnTransformer:EADatabaseItemTransformer, DB.Transformation.ColumnTransformer
	{
		internal Table _table;
		internal Column _column;
		internal UTF_EA.Attribute _attribute;
		internal UTF_EA.AssociationEnd _associationEnd;
		
		public EAColumnTransformer(Table table, NameTranslator nameTranslator):base(nameTranslator)
		{
			this._table =  table;
		}

		#region ColumnTransformer implementation

		public virtual DB.Column transformLogicalProperty(UML.Classes.Kernel.Property attribute)
		{
			createColumn((UTF_EA.Attribute)attribute);
			if (this._column != null) this._column.logicalAttribute = (UTF_EA.Attribute)attribute;
			return this._column;
		}
		protected abstract void createColumn(UTF_EA.Attribute attribute);
				
		public UML.Classes.Kernel.Property logicalProperty {get;set;}

		public DB.Table table 
		{
			get {return _table;}
			set {_table = (Table)value;}
		}

		public DB.Column column 
		{
			get {return _column;}
			set {_column = (Column)value;}
		}

		#region implemented abstract members of EADatabaseItemTransformer
				
		public override DB.Transformation.DatabaseItemTransformer getCorrespondingTransformer(DB.DatabaseItem item)
		{
			if (item == this.column) return this;
			return null;
		}
		#endregion				
		public override DB.DatabaseItem databaseItem
		{
			get 
			{
				return this.column;
			}
		}
		
		#endregion
	}
}
