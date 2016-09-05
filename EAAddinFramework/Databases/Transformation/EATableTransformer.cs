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
	/// Description of EATableTransformer.
	/// </summary>
	public abstract class EATableTransformer:EADatabaseItemTransformer, DB.Transformation.TableTransformer
	{

		#region TableTransformer implementation
		internal Table _table;
		internal List<UTF_EA.Class> _logicalClasses = new List<UTF_EA.Class>();
		internal Database _database;
		public EATableTransformer(Database database, NameTranslator nametranslator):base(nametranslator)
		{
			_database = database;
		}

		public DB.Database database 
		{
			get {return _database;}
			set {_database = (Database) value;}
		}

		public abstract List<DB.Transformation.ForeignKeyTransformer> foreignKeyTransformers {get;set;}

		public abstract List<DB.Transformation.ColumnTransformer> columnTransformers {get;set;}

		public DB.Table table 
		{
			get{ return _table;}
			set{_table = (Table)value;}
		}
		
		public List<UML.Classes.Kernel.Class> logicalClasses 
		{
			get 
			{
				return _logicalClasses.Cast<UML.Classes.Kernel.Class>().ToList();
			}
			set 
			{
				_logicalClasses = value.Cast<UTF_EA.Class>().ToList();
			}
		}
		
		public virtual DB.Table  transformLogicalClasses(List<UML.Classes.Kernel.Class> logicalClasses)
		{
			throw new NotImplementedException();
		}
		protected abstract void createTable(List<UML.Classes.Kernel.Class> logicalClasses);

		public virtual DB.Table transformLogicalClass(UML.Classes.Kernel.Class logicalClass)
		{
			//create the table
			createTable((UTF_EA.Class) logicalClass);
			//add the class as logical class
			_table.logicalClasses.Add((UTF_EA.Class)logicalClass);
			//create the columns from the attributes
			createColumnsFromAttributes();
			return this.table;
		}
		protected virtual void createColumnsFromAttributes()
		{
			foreach (var classElement in _logicalClasses) 
			{				
				foreach (var attribute in classElement.attributes)					
				{
					var column = transformLogicalAttribute((UTF_EA.Attribute)attribute);
				}
			}
		}

		public virtual void save()
		{
			//update name if needed
			//TODO: check if needed
			
		}
		public abstract void setTableName(string fixedTableString, int nameCounter);

		protected abstract Column transformLogicalAttribute(UTF_EA.Attribute attribute);

		protected abstract void createTable(UTF_EA.Class classElement);


		#endregion
	}
}
