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
	/// Description of DB2ColumnTransformer.
	/// </summary>
	public class DB2ColumnTransformer:EAColumnTransformer
	{
		
		private DB2TableTransformer _dependingTransformer = null;
		Column _involvedColumn = null;
		public Column getPKInvolvedColumn()
		{
			if (this._associationEnd.isID) return _column;
			return null;
		}
		public Column getFKInvolvedColumn()
		{
			//only add FK's for classes in the same pakage;
			if (_dependingTransformer._database.Equals(this.table.databaseOwner))
			{
				return _column;
			}
			return null;
		}
		
		public DB2ColumnTransformer(Table table,NameTranslator nameTranslator):base(table,nameTranslator){}
		public DB2ColumnTransformer(Table table, Column column, UTF_EA.Attribute attribute,NameTranslator nameTranslator):this(table,nameTranslator)
		{
			this.logicalProperty = attribute;
			this.column = column;
		}
		public DB2ColumnTransformer(Table table, Column involvedColumn,DB2TableTransformer dependingTransformer,UTF_EA.AssociationEnd associationEnd,NameTranslator nameTranslator):this(table,nameTranslator)
		{
			this._involvedColumn = involvedColumn;
			this._dependingTransformer = dependingTransformer;
			this._associationEnd = associationEnd;
			_column = new Column((DB_EA.Table)table, involvedColumn.name);
			_column.type = involvedColumn.type;
			_column.logicalAttribute = ((DB_EA.Column)involvedColumn).logicalAttribute;
			if (this._associationEnd != null)
			{
				if (_associationEnd.lower > 0 )
				{
					_column.isNotNullable = true;
				}
			}
		}

		#region implemented abstract members of EAColumnTransformer


		protected override void createColumn(UTF_EA.Attribute attribute)
		{
			this.logicalProperty = attribute;
			this.column = transformLogicalAttribute(attribute);
		}

		#region implemented abstract members of EADatabaseItemTransformer
		public override void rename(string newName)
		{
			this.column.name = newName;
			//we can only change the alias if this is a direct transformation
			if (this._dependingTransformer == null)
			{
				if (this.logicalProperty != null) ((UTF_EA.Attribute)this.logicalProperty).alias = newName;
			}
			else
			{
				//otherwise we override on database level to allow for a different name
				this.column.isRenamed = true;
			}
		}

		#endregion

		#endregion

		private Column transformLogicalAttribute(UTF_EA.Attribute attribute)
		{
			bool isEqualDirty = false;
			//first translate the columname if needed
			if (string.IsNullOrEmpty(attribute.alias))
			{
				attribute.alias = this._nameTranslator.translate(attribute.name, attribute.owner.name);
				isEqualDirty = true;
				//should we save here?
			}
			this.column = new Column(this._table, attribute.alias);
			this.column.isEqualDirty = isEqualDirty;
			//get base type
			var attributeType = attribute.type as UTF_EA.ElementWrapper;
			if (attributeType == null) Logger.logError (string.Format("Attribute {0}.{1} does not have a element as datatype"
			                                                    ,attribute.owner.name, attribute.name));
			else
			{
				DataType datatype = _table._owner._factory.createDataType(attributeType.alias);
				if (datatype == null) Logger.logError (string.Format("Could not find {0} as Datatype for attribute {1}.{2}"
				                                                    ,attributeType.alias, attribute.owner.name, attribute.name));
				else
				{
					column.type = datatype;
				}
			}
			//set not null property
			if (attribute.lower == 0)
			{
				column.isNotNullable = false;
			}
			else
			{
				column.isNotNullable = true;
			}
			//set inital value
			if (attribute.defaultValue != null
				&& ! string.IsNullOrEmpty( attribute.defaultValue.ToString()))
			{
				this.column.initialValue = attribute.defaultValue.ToString();
			}

			//set position
			this.column.position = attribute.position;
			return this._column;
		}
		private Column transformLogicalAssociationEnd(UTF_EA.AssociationEnd associationEnd)
		{
			throw new NotImplementedException();
		}

	}
}
