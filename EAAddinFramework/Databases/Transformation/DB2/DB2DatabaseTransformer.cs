

using System;
using System.Collections.Generic;
using System.Linq;
using EAAddinFramework.Databases.Strategy;
using UML=TSF.UmlToolingFramework.UML;
using DB=DatabaseFramework;
using DB_EA = EAAddinFramework.Databases;
using EAAddinFramework.Utilities;
using DDL_Parser;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.Databases.Transformation.DB2
{
	/// <summary>
	/// Description of DB2DatabaseTransformer.
	/// </summary>
	public class DB2DatabaseTransformer:EADatabaseTransformer
	{
		private string outputName = "DB2DatabaseTransformer.complete";
		internal List<DB2TableTransformer> externalTableTransformers = new List<DB2TableTransformer>();
		internal Database _externalDatabase;
		public DB2DatabaseTransformer(Package logicalPackage,NameTranslator nameTranslator, StrategyFactory strategyFactory, bool compareOnly = false):this(logicalPackage.EAModel, nameTranslator,strategyFactory,compareOnly)
		{
			this._logicalPackage = logicalPackage;
		}
		public DB2DatabaseTransformer(Model model,NameTranslator nameTranslator,StrategyFactory strategyFactory,bool compareOnly = false):this(getFactory(model,strategyFactory),model,nameTranslator,compareOnly)
		{
		}
		public DB2DatabaseTransformer(DatabaseFactory factory,Model model,NameTranslator nameTranslator,bool compareOnly = false):base(factory,model,nameTranslator,compareOnly)
		{
			this._externalDatabase = factory.createDatabase("external");
		}
		public static DatabaseFactory getFactory(Model model, StrategyFactory strategyFactory)
		{
			return DB_EA.DatabaseFactory.getFactory("DB2",model, strategyFactory);
		}

		#region implemented abstract members of EADatabaseItemTransformer
		public override void rename(string newName)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region implemented abstract members of EADatabaseTransformer

		/// <summary>
		/// create the initial database from the logical package
		/// </summary>
		protected override void createNewDatabase()
		{
			if (this.logicalPackage != null)
			{
				this._newDatabase = factory.createDatabase(this._logicalPackage.alias);
			}
		}

		protected override void addTable(Class classElement)
		{
				addDB2Table(classElement);
		}
		protected DB2TableTransformer addDB2Table(AssociationEnd associationEnd)
		{
			
			DB2TableTransformer transformer = addDB2Table(associationEnd.type as Class);
			if (transformer == null)
			{
				transformer = this.tableTransformers.OfType<DB2TableTransformer>().FirstOrDefault( x => x.logicalClass.Equals(associationEnd.type));
			}
			if (transformer == null)
			{
				transformer = this.externalTableTransformers.FirstOrDefault( x => x.logicalClass.Equals(associationEnd.type));
			}
			return transformer;
		}
		protected DB2TableTransformer addDB2Table(Class classElement)
		{
			DB2TableTransformer transformer = null;
			if (classElement.owningPackage.Equals(this.logicalPackage))
			{
				if ( ! this._tableTransformers.Any(x => x.logicalClasses.Any(y => y.Equals(classElement))))
				{
					transformer = new DB2TableTransformer(this._newDatabase,_nameTranslator);
					this._tableTransformers.Add(transformer);
				}
			}
			else
			{
				if (!this.externalTableTransformers.Any(x => x.logicalClasses.Any(y => y.Equals(classElement))))
				{
					transformer = new DB2TableTransformer(this._externalDatabase,_nameTranslator);
					this.externalTableTransformers.Add(transformer);
				}
			}
			if (transformer != null)
			{
				//transform to table
				transformer.transformLogicalClass(classElement);
				//now do the external tables linked to this classElement
				foreach (var dependingAssociationEnd in transformer.getDependingAssociationEnds()) 
				{
					if (! transformer.dependingTransformers.ContainsKey(dependingAssociationEnd))
					{
						var dependingEndTransformer = addDB2Table(dependingAssociationEnd);
						if (dependingEndTransformer != null)transformer.dependingTransformers.Add(dependingAssociationEnd,dependingEndTransformer);
					}
				}
				//add the remote columns and primary and foreign keys
				transformer.addRemoteColumnsAndKeys();
			}
			return transformer;
		}
		protected override void nameUnnamedTables()
		{
			int nameCounter = getTableNameCounter();
			string fixedTableString = this.getFixedTableString();
			foreach (var tableTransformer in this._tableTransformers.Where(x => string.IsNullOrEmpty(x._table.name)
			                                                               && !x.table.isAbstract) )
			{
				nameCounter++;
				tableTransformer.setTableName(fixedTableString,nameCounter);
			}
			//rename foreign keys to match new table names
			foreach (var foreignKeyTransformers in this._tableTransformers.Select(x => x.foreignKeyTransformers)) 
			{
				foreach (DB2ForeignKeyTransformer foreignKeyTransformer in foreignKeyTransformers) 
				{
					foreignKeyTransformer.resetName();
				}
				
			}
		}
		private int getTableNameCounter()
		{
			var nameTables = this._tableTransformers.Where(x => !(string.IsNullOrEmpty(x._table.name))).Select(x => x._table.name).OrderBy(x => x);
			string lastName = nameTables.LastOrDefault();
			if(! string.IsNullOrEmpty(lastName))
			{
				//we have the last named table, now get the counter from the name
				return  getCounterFromString(lastName, lastName.Length);
			}
			//return default
			return 0;
		}
		private string getFixedTableString()
		{
			if (this._newDatabase.name.Length > 5)
			{
				return this._newDatabase.name.Substring(0,2) + "T" + this._newDatabase.name.Substring(3,this.newDatabase.name.Length -5);
			}
			else return "GBTXXX";
		}
		private int getCounterFromString(string name, int lenght)
		{
			//if we reach 0 lenght then we return 1 as default
			if (lenght == 0) return 1;
			int counter;
			if (int.TryParse(name.Substring(name.Length - lenght),out counter))
		    {
				return counter;
		    }
			return getCounterFromString(name, lenght -1);
		}
				/// <summary>
		/// abstract tables need to be removed. All foreign keys to these tables have to be replaced by foreign key's to the tables for the 
		/// subclasses of the logical class
		/// </summary>
		protected override void removeAbstractTables()
		{
			foreach (var abstractTableTransformer in this.tableTransformers.Where(x => x.table.isAbstract))
			{
				//check if there are any foreign keys pointing to this table
				foreach (var foreignTabletransformers in this._tableTransformers
				         .Where(x => x.foreignKeyTransformers
				          .Any(y => y.foreignKey.foreignTable == abstractTableTransformer.table)))
				{
					//if there are any subclasses then there should be a foreign key transformer to each of the subclasses be added
					//then the original foreignKeyTransformer should be removed
					foreach (var foreignKeyTransformer in foreignTabletransformers.foreignKeyTransformers.Where(x => x.foreignKey.foreignTable == abstractTableTransformer.table)) 
					{
						foreach (var subclasses in abstractTableTransformer.logicalClasses.Select(x => x.subClasses))
						{
							foreach (var subclass in subclasses) 
							{
								var subTabletransformer = this.tableTransformers.FirstOrDefault(x => x.table.logicalElements.Any(y => subclass.Equals(y)));
								//get corresponding columns from subtable
								var correspondingsubColumns = getCorrespondingColumn(foreignKeyTransformer.foreignKey.involvedColumns,subTabletransformer.table);
								var newForeignKeyTransfomer = new DB2ForeignKeyTransformer((Table)foreignKeyTransformer.foreignKey.ownerTable ,correspondingsubColumns,(DB2TableTransformer)subTabletransformer,(AssociationEnd)foreignKeyTransformer.logicalAssociationEnd,this._nameTranslator);
							}
						}
						//delete the original foereign key
						foreignKeyTransformer.foreignKey.delete();
						foreignTabletransformers.foreignKeyTransformers.Remove(foreignKeyTransformer);
					}
				}
				//delete abstract table and its transformer
	
				abstractTableTransformer.table.delete();
				this.tableTransformers.Remove(abstractTableTransformer);
				
			}
		}
		private List<Column> getCorrespondingColumn(List<DB.Column> originalColumns, DB.Table newTable)
		{
			var newColumns = new List<Column>();
			foreach (Column originalcolumn in originalColumns) 
			{
				Column newColumn = newTable.columns.FirstOrDefault(x => x.name == originalcolumn.name) as Column;
				if (newColumn != null) newColumns.Add(newColumn);
			}
			return newColumns;
		}


    public void complete(Database database, DDL withDDL) 
    {
    	EAOutputLogger.clearLog(this._model,this.outputName);
      this.log( string.Format(
        "completing {0} statements (parsed with {1} errors)",
        withDDL.statements.Count, withDDL.errors.Count
      ));
      
      // perform several fixes to complete the schema according to the DDL
      this.fixUniqueIndexes               (database, withDDL);
      this.fixClusteredIndexes            (database, withDDL);
      this.fixOnDeleteRestrictForeignKeys (database, withDDL);
      this.fixWithDefaultFields           (database, withDDL);
      this.fixIncludedFieldsInIndex       (database, withDDL);
      this.fixCheckConstraints			  (database, withDDL);

    }
	
	void fixCheckConstraints(Database database, DDL withDDL)
	{
      var fieldsWithConstraints =  from table in withDDL.statements.OfType<CreateTableStatement>()
            from field in table.Fields
      		from constraint in field.Constraints.OfType<DDL_Parser.CheckConstraint>()
          select new {
            Table      = table.SimpleName,
            FieldName       = field.SimpleName,
            Constraintname = constraint.Field.Name,
            ConstraintRules = constraint.Rules
          };
      int fixes = 0;
      foreach(var field in fieldsWithConstraints) {
        // find table
        var table = (Table)database.getTable(field.Table);
  			if( table != null ) {
          // find field
          var column = (Column)table.getColumn(field.FieldName);
          if( column != null ) 
          {
          	var checkConstraint = table.constraints.OfType<CheckConstraint>()
          		.FirstOrDefault( x => x.name.Equals(field.Constraintname,StringComparison.InvariantCultureIgnoreCase));
            if(checkConstraint == null) 
            {
            	checkConstraint = new CheckConstraint(field.Constraintname, column,field.ConstraintRules);
            	checkConstraint.save();
                fixes++;
            }
            else
            {
            	if (checkConstraint.rule != field.ConstraintRules)
            	{
            		checkConstraint.rule = field.ConstraintRules;
            		checkConstraint.save();
            		fixes++;
            	}
            }
          } else {
            this.log( "WARNING: field " + table.name + "."  + field.FieldName + " not found" );
          }
        } else {
          this.log( "WARNING: table " + field.Table + " not found" );
        }
      }
      this.log(string.Format(
        "RESULT: Fix check constraints: fixed {0}/{1}",
        fixes, fieldsWithConstraints.Count()
      ));
	}
    private void fixUniqueIndexes(Database database, DDL withDDL) {
      // find all unique indexes
      var indexes =  from index in withDDL.statements.OfType<CreateIndexStatement>()
                    where index.Parameters.Keys.Contains("UNIQUE")
                       && index.Parameters["UNIQUE"] == "True"
                   select index;

      int fixes = 0;
      foreach(var index in indexes) {
        // find table
        var table = (Table)database.getTable(index.Table.Name);
  			if( table != null ) {
  			  // find constraint
          var constraint = (Index)table.getConstraint(index.SimpleName);
          if( constraint != null ) {
            if( ! constraint.isUnique ) {
              constraint.isUnique = true;
              fixes++;
              this.log( "FIXED " + index.Name + "'s missing UNIQUE constraint");
            }
          } else {
            this.log( "WARNING: index " + table.name+ "." + index.Name + " not found" );
          }
        } else {
          this.log( "WARNING: table " + index.Table + " not found" );
        }
      }
      this.log(string.Format(
        "RESULT: Fix unique index: fixed {0}/{1}",
        fixes, indexes.Count()
      ));
    }

    private void fixClusteredIndexes(Database database, DDL withDDL) {
      // find all Clustered indexes
      var indexes =  from index in withDDL.statements.OfType<CreateIndexStatement>()
                    where index.Parameters.Keys.Contains("CLUSTER")
                       && index.Parameters["CLUSTER"] == "True"
                   select index;

      int fixes = 0;
      foreach(var index in indexes) {
        // find table
        var table = (Table)database.getTable(index.Table.Name);
  			if( table != null ) {
  			  // find constraint
          var constraint = (Index)table.getConstraint(index.SimpleName);
          if( constraint != null ) {
            if( ! constraint.isClustered ) {
              constraint.isClustered = true;
              fixes++;
              this.log( "FIXED " + index.Name + "'s missing CLUSTERED constraint");
            }
          } else {
            this.log( "WARNING: index "+ table.name+ "." + index.Name + " not found" );
          }
        } else {
          this.log( "WARNING: table " + index.Table + " not found" );
        }
      }
      this.log(string.Format(
        "RESULT: Fix Clustered index: fixed {0}/{1}",
        fixes, indexes.Count()
      ));
    }

    private void fixOnDeleteRestrictForeignKeys(Database database, DDL withDDL) {
      var keys = from alter in withDDL.statements.OfType<AlterTableAddConstraintStatement>()
                where alter.Constraint is ForeignKeyConstraint
                   && alter.Constraint.Parameters.Keys.Contains("ON_DELETE")
                   && alter.Constraint.Parameters["ON_DELETE"] == "RESTRICT"
               select new {
                 Table      = alter.Table,
                 Name       = alter.Constraint.Name,
                 SimpleName = alter.Constraint.SimpleName
               };

      int fixes = 0;
      foreach(var key in keys) {
        // find table
        var table = (Table)database.getTable(key.Table.Name);
        if( table != null ) {
          // find constraint
          var constraint = (ForeignKey)table.getConstraint(key.SimpleName);
          if( constraint != null ) {
            if( ! constraint.onDelete.Equals("Restrict") ) {
              constraint.onDelete = "Restrict";
              fixes++;
              this.log( "FIXED " + key.Name + "'s missing On Delete Restrict constraint");
            }
          } else {
            this.log( "WARNING: foreign key " + table.name + "." + key.Name + " not found" );
          }
        } else {
          this.log( "WARNING: table " + key.Table + " not found" );
        }
      }
      this.log(string.Format(
        "RESULT: Fix On Delete Restrict foreign keys: fixed {0}/{1}",
        fixes, keys.Count()
      ));
    }

    private void fixWithDefaultFields(Database database, DDL withDDL) {
      var fields =  from table in withDDL.statements.OfType<CreateTableStatement>()
                    from field in table.Fields
                   where field.Parameters.Keys.Contains("DEFAULT")
                      && field.Parameters["DEFAULT"] == "True"
                  select new {
                    Table      = table.Name,
                    Name       = field.Name,
                    SimpleName = field.SimpleName
                  };

      int fixes = 0;
      foreach(var field in fields) {
        // find table
        var table = (Table)database.getTable(field.Table.Name);
  			if( table != null ) {
          // find field
          var column = (Column)table.getColumn(field.SimpleName);
          if( column != null ) {
            if( ! column.initialValue.Equals("DEFAULT") ) {
              column.initialValue = "DEFAULT";
              column.save();
              fixes++;
            }
          } else {
            this.log( "WARNING: field " + table.name + "."  + field.Name + " not found" );
          }
        } else {
          this.log( "WARNING: table " + field.Table + " not found" );
        }
      }
      this.log(string.Format(
        "RESULT: Fix With Default field: fixed {0}/{1}",
        fixes, fields.Count()
      ));
    }

    private void fixIncludedFieldsInIndex(Database database, DDL withDDL) {
      // find all Clustered indexes
      var indexes =  from index in withDDL.statements.OfType<CreateIndexStatement>()
                    where index.Parameters.Keys.Contains("INCLUDE")
                   select index;

      int fixes = 0;
      foreach(var index in indexes) {
        // find table
        var table = (Table)database.getTable(index.Table.Name);
  			if( table != null ) {
  			  // find constraint
          var constraint = (Index)table.getConstraint(index.SimpleName);
          if( constraint != null ) {
            string columnName = index.Parameters["INCLUDE"];
            // step 1: remove involved column
            var column = (Column)constraint.getInvolvedColumn(columnName);
            if( column != null ) {
              // mark the column as included in the constraint
              constraint.markAsIncluded(column);
            } else {
              this.log( "WARNING: column " + table.name + "."  + columnName + " not found" );
            }
            fixes++;
            this.log( "MARKED " + index.Name + "'s INCLUDE column");
          } else {
            this.log( "WARNING: index " + table.name + "."  + index.Name + " not found" );
          }
        } else {
          this.log( "WARNING: table " + index.Table + " not found" );
        }
      }
      this.log(string.Format(
        "RESULT: Fix Included Column in Index: TODOs {0}/{1}",
        fixes, indexes.Count()
      ));
    }

    private void log(string msg) {
      EAOutputLogger.log( this._model, this.outputName, msg );
    }

		#endregion
	}
}
