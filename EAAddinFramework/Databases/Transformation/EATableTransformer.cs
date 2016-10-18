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

		public abstract DB.Transformation.PrimaryKeyTransformer primaryKeyTransformer {get;set;}

		#region implemented abstract members of EADatabaseItemTransformer
		public override DB.Transformation.DatabaseItemTransformer getCorrespondingTransformer(DB.DatabaseItem item)
		{
			//check if the item is our table
			if (item == this.table) return this;
			//check columntransformers
			DB.Transformation.DatabaseItemTransformer correspondingTransformer = null;
			foreach (var columnTransformer in this.columnTransformers) 
			{
				correspondingTransformer = columnTransformer.getCorrespondingTransformer(item);
				if (correspondingTransformer != null) return correspondingTransformer;
			}
			//check foreignKey transformers
			foreach (var foreignKeyTransformer in this.foreignKeyTransformers) 
			{
				correspondingTransformer = foreignKeyTransformer.getCorrespondingTransformer(item);
				if (correspondingTransformer != null) return correspondingTransformer;
			}
			//check primary key transformer
			if (this.primaryKeyTransformer != null) return primaryKeyTransformer.getCorrespondingTransformer(item);
			//not found			
			return correspondingTransformer;
		}
		public override DB.DatabaseItem databaseItem {
			get {
				return this.table;
			}
		}
		#endregion
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
		/// <summary>
		/// creates columns from the attributes of the logical classes.
		/// In case the logical class has superclasses then the attributes of the superclasses are transformed as well.
		/// </summary>
		protected virtual void createColumnsFromAttributes()
		{
			foreach (var classElement in this.allLogicalClasses) 
			{
				foreach (var attribute in classElement.attributes.OrderBy(x => x.position))
				{
					var column = transformLogicalAttribute((UTF_EA.Attribute)attribute);
				}
			}		
		}
		private Dictionary<UTF_EA.Class, List<UTF_EA.Class>> _superClasses = new Dictionary<UTF_EA.Class, List<UTF_EA.Class>>();
		
		private List<UTF_EA.Class> _allLogicalClasses;
		/// <summary>
		/// a list of all logical classes inlcuding their superclasses
		/// </summary>
		public List<UTF_EA.Class> allLogicalClasses
		{
			get
			{
				if (_allLogicalClasses == null)
				{
					foreach (var logicalClass in _logicalClasses) 
					{
						//get the superclasses
						_allLogicalClasses = this.getsuperClasses(logicalClass);
						//get the current classElement
						_allLogicalClasses.Add(logicalClass);
					}
				}
				return _allLogicalClasses;
			}
		}
		/// <summary>
		/// gets the superclasses of an element
		/// </summary>
		/// <param name="classElement"></param>
		/// <returns></returns>
		protected virtual List<UTF_EA.Class> getsuperClasses(UTF_EA.Class classElement)
		{
			if (!_superClasses.ContainsKey(classElement))
			{
				if (classElement.superClasses.Count == 0)
				{
					//no superclasses, return empty list
					_superClasses.Add(classElement, new List<UTF_EA.Class>());
				}
				else
				{
					var superClasses = new List<UTF_EA.Class>();
					//first add the superclasses of the superclasses
					foreach (UTF_EA.Class superClass in classElement.superClasses) 
					{
						superClasses.AddRange(getsuperClasses(superClass));
					}
					//the add the superclasses themselves
					foreach (UTF_EA.Class superClass in classElement.superClasses) 
					{
						superClasses.Add(superClass);
					}
					_superClasses.Add(classElement, superClasses);
				}
			}
			return _superClasses[classElement];
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
