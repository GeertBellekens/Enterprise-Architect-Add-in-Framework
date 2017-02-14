
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		internal List<DB2TableTransformer> dependingTransformers = new List<DB2TableTransformer>();
		internal List<DB2ForeignKeyTransformer> _foreignKeyTransformers = new List<DB2ForeignKeyTransformer>();
		internal DB2PrimaryKeyTransformer _primaryKeyTransformer = null;
		internal UTF_EA.AssociationEnd associationEnd;
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="database">the database the table should belong to</param>
		/// <param name="nameTranslator">the nametranslator</param>
		public DB2TableTransformer(Database database,NameTranslator nameTranslator):base(database,nameTranslator){}
		
		internal UTF_EA.Class logicalClass
		{
			get{ return logicalClasses.FirstOrDefault() as UTF_EA.Class;}
		}
		
		#region implemented abstract members of EATableTransformer
		protected override void createTable(System.Collections.Generic.List<UML.Classes.Kernel.Class> logicalClasses)
		{
			throw new NotImplementedException();
		}
		protected override void createTable(UTF_EA.Class classElement)
		{
			this._logicalClasses.Add(classElement);
			this.table = new Table(_database, classElement.alias);
			setTableSpaceName();
		}
		
		protected override Column transformLogicalAttribute(UTF_EA.Attribute attribute)
		{
			var columnTransformer = new DB2ColumnTransformer(this._table,this._nameTranslator);
			this._columnTransformers.Add(columnTransformer);
			return (Column) columnTransformer.transformLogicalProperty(attribute);
		}

		public override List<DB.Transformation.ColumnTransformer> columnTransformers {
			get { return _columnTransformers.Cast<DB.Transformation.ColumnTransformer>().ToList();}
			set { _columnTransformers = value.Cast<DB2ColumnTransformer>().ToList();}
		}

		#region implemented abstract members of EADatabaseItemTransformer
		public override void rename(string newName)
		{
			this.table.name = newName;
			this.logicalClass.alias = newName;
			setTableSpaceName();
		}
		//TableSpaceName is exactly the same as the tabelName except for the fact that the third character is an "S" iso a "T"
		private void setTableSpaceName()
		{
			if (this.table.name.Length > 3)
			{
				StringBuilder nameStringBuilder = new StringBuilder(this.table.name);
				nameStringBuilder[2] = 'S';
				this.table.tableSpace = nameStringBuilder.ToString();
			}
		}
		#endregion
		#region implemented abstract members of EATableTransformer
		public override DB.Transformation.PrimaryKeyTransformer primaryKeyTransformer {
			get {
				return _primaryKeyTransformer;
			}
			set {
				this._primaryKeyTransformer = (DB2PrimaryKeyTransformer)value;
			}
		}
		#endregion
		#region implemented abstract members of EATableTransformer


		public override List<DB.Transformation.ForeignKeyTransformer> foreignKeyTransformers {
			get { return _foreignKeyTransformers.Cast<DB.Transformation.ForeignKeyTransformer>().ToList();}
			set { _foreignKeyTransformers = value.Cast<DB2ForeignKeyTransformer>().ToList();}
		}
		public override void setTableName(string fixedTableString, int nameCounter)
		{
			string counterPart = nameCounter.ToString();
			if (counterPart.Length < 2) counterPart = "0" + counterPart;
			string tableName = fixedTableString + counterPart;
			this.logicalClass.alias = tableName;
			this._table.name = tableName;
			foreach (var fkTransformer in this.foreignKeyTransformers) {
				fkTransformer.resetName();
			}
			if (this._primaryKeyTransformer != null) this._primaryKeyTransformer.resetName();
			
		}


		#endregion

		public void addRemoteColumnsAndKeys()
		{
			List<DB_EA.Column> PKInvolvedColumns = new List<DB_EA.Column>();
			//check attributes
			foreach (var attributes in allLogicalClasses.Select(z => z.attributes.Where(x => x.isID)))
			{
				foreach (var attribute in attributes.OrderBy(x => x.position))
				{
					//get the corresponding transformer
					var columnTransformer = this.columnTransformers.FirstOrDefault( x => attribute.Equals(x.logicalProperty));
					if (columnTransformer != null)
					{
						PKInvolvedColumns.Add((DB_EA.Column) columnTransformer.column);
					}
				}
			}
			//add the columns for the primary key of the dependent table
			foreach (var dependingTransformer in this.dependingTransformers) 
			{
				if (dependingTransformer.table.primaryKey != null)
				{
					var dependingColumnTransfomers = new List<DB2ColumnTransformer>();
					foreach (Column column in dependingTransformer.table.primaryKey.involvedColumns.OrderBy(x => x.position))
					{
						dependingColumnTransfomers.Add( new DB2ColumnTransformer(this._table, column, dependingTransformer,_nameTranslator));
					}
					this._columnTransformers.AddRange(dependingColumnTransfomers);
					List<Column> FKInvolvedColumns = new List<Column>();
					foreach (var columnTransformer in dependingColumnTransfomers) 
					{
						//get the FK column
						var FKInvolvedColumn = columnTransformer.getFKInvolvedColumn();
						if (FKInvolvedColumn != null) FKInvolvedColumns.Add(FKInvolvedColumn);
						//get the PK column
						var PKInvolvedColumn = columnTransformer.getPKInvolvedColumn();
						if (PKInvolvedColumn != null) PKInvolvedColumns.Add(PKInvolvedColumn);
					}
					if (FKInvolvedColumns.Count > 0 )
					{
						this._foreignKeyTransformers.Add(new DB2ForeignKeyTransformer(this._table,FKInvolvedColumns,dependingTransformer,_nameTranslator));

					}
				}
			}
			//create primaryKey
			if (PKInvolvedColumns.Count > 0)
			{
				this._primaryKeyTransformer = new DB2PrimaryKeyTransformer(_table, PKInvolvedColumns,_nameTranslator);
			}
			
		}
			

		#endregion

		/// <summary>
		/// gets the Class Elements that are needed for this logical element.
		/// This means the classes to which this element has an association to with
		/// multiplicity of 1..1 or 0..1. We will need these classes because they will create one or more columns in the associated table.
		/// </summary>
		/// <returns>the classes on which this logical element depends for this logical element</returns>
		public List<UTF_EA.AssociationEnd> getDependingAssociationEnds()
		{
			var dependingAssociationEnds = new List<UTF_EA.AssociationEnd>();
			foreach (var currentClass in this.allLogicalClasses) 
			{
				foreach (var association in currentClass.relationships.OfType<UTF_EA.Association>())
				{
					foreach (UTF_EA.AssociationEnd end in association.memberEnds) 
					{
						if (!currentClass.Equals(end.type) 
					          && end.type is UTF_EA.Class
					          && (end.upper.integerValue.HasValue && end.upper.integerValue == 1))
						{
							//if both end have an upper value of 1 then we take only the one with {id} or else use the association direction
							var otherEnd = association.memberEnds.FirstOrDefault(x => x != end) as UTF_EA.AssociationEnd;
							//both have an upper value of 1
							if (otherEnd.upper.integerValue.HasValue && otherEnd.upper.integerValue == 1)
							{
								//this one is the one with ID
								if (end.isID)
								{
									dependingAssociationEnds.Add(end);
									break;
								}
								//the other end is not an ID either, 
								// then we use the association direction to determine wher the FK should be placed
								if (! otherEnd.isID
								         && association.targetEnd == end)
								{
									dependingAssociationEnds.Add(end);
									break;
								}
							}
							else
							{
								dependingAssociationEnds.Add(end);
								break;
							}
						}
					}
				}
			}
			return dependingAssociationEnds;
		}
	}
}
