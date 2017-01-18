
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
	/// Description of EADatabaseTransformer.
	/// </summary>
	public abstract class EADatabaseTransformer:EADatabaseItemTransformer, DB.Transformation.DatabaseTransformer
	{
		private DatabaseFactory _factory;
		internal UTF_EA.Model _model;		
		internal Database _newDatabase;
		internal Database _existingDatabase;
		internal UTF_EA.Package _logicalPackage;
		internal bool compareOnly {get;set;}
		protected List<EATableTransformer> _tableTransformers = new List<EATableTransformer>();

		public EADatabaseTransformer(DatabaseFactory factory, UTF_EA.Model model,NameTranslator nameTranslator,bool compareOnly = false):base(nameTranslator)
		{
			this._factory = factory;
			this._model = model;
			this.compareOnly = compareOnly;
		}

		public DatabaseFactory factory {
			get {return _factory;}
		}

		#region DatabaseTransformer implementation


		public UML.Extended.UMLModel model 
		{
			get {return _model;}
			set {_model = (UTF_EA.Model) value;}
		}

		public void renameItem(DB.DatabaseItem item, string newName)
		{
			//get corresponding transformer
			var transformer = getCorrespondingTransformer(item);
			if (transformer != null) transformer.rename(newName);
		}
		
		public override DB.Transformation.DatabaseItemTransformer getCorrespondingTransformer(DB.DatabaseItem item)
		{
			//check if the item is our new database
			if (item == this.newDatabase) return this;
			//go deeper
			DB.Transformation.DatabaseItemTransformer correspondingTransformer = null;
			foreach (var tableTransformer in this.tableTransformers) 
			{
				correspondingTransformer = tableTransformer.getCorrespondingTransformer(item);
				if (correspondingTransformer != null) break;
			}
			return correspondingTransformer;
		}
		
		public override DB.DatabaseItem databaseItem {
			get {
				return this.existingDatabase;
			}
		}
		
		public void refresh()
		{
			if (this.logicalPackage != null)
			{
				this._newDatabase = null;
				this._existingDatabase = null;
				this._tableTransformers.Clear();
				this.transformLogicalPackage(this.logicalPackage);
				
			}
		}

		public DB.Database newDatabase 
		{
			get 
			{
				return this._newDatabase;
			}
			set {this._newDatabase = (Database)value;}
		}

		public DB.Database existingDatabase 
		{
			get 
			{
				if (this._existingDatabase == null)
				{
				   if( logicalPackage != null)
					{
						var traces = logicalPackage.relationships.Where(x => x.stereotypes.Any(y => y.name.Equals("trace",StringComparison.InvariantCultureIgnoreCase))).Cast<UTF_EA.ConnectorWrapper>();
						foreach (var trace in traces) {
							if (trace.target.Equals(_logicalPackage)
						    && trace.source is UTF_EA.Package
						    && trace.source.stereotypes.Any(x => x.name.Equals("Database",StringComparison.InvariantCultureIgnoreCase)))
							{
								_existingDatabase = factory.createDataBase(trace.source as UTF_EA.Package,this.compareOnly);
								break;//return only one
							}
						}
					}
				}
				return this._existingDatabase;
			}
			set {this._existingDatabase = (Database)value;}
		}

		public UML.Classes.Kernel.Package logicalPackage 
		{
			get 
			{
				if (_logicalPackage == null
				    && _existingDatabase != null
				   && _existingDatabase._wrappedPackage != null)
				{
					//search for the first logical package we find
					_logicalPackage = _existingDatabase._wrappedPackage.relationships.OfType<UTF_EA.Abstraction>()
									.Where(x => x.stereotypes.Any(y => y.name.Equals("trace",StringComparison.InvariantCultureIgnoreCase)
											&& x.target is UTF_EA.Package))
											.Select(z => z.target as UTF_EA.Package).FirstOrDefault();
				}
				return this._logicalPackage;
			}
			set {this._logicalPackage = (UTF_EA.Package)value;}
		}
		public List<DB.Transformation.TableTransformer> tableTransformers {
			get 
			{
				return _tableTransformers.Cast<DB.Transformation.TableTransformer>().ToList();
			}
			set 
			{
				this._tableTransformers = value.Cast<EATableTransformer>().ToList();
			}
		}
		public virtual DatabaseFramework.Database transformLogicalPackage(UML.Classes.Kernel.Package logicalPackage)
		{
			this.logicalPackage = logicalPackage;
			
			createNewDatabase();
			//first create all tables
			this.createTables();
			//return the database
			return newDatabase;
		}
		/// <summary>
		/// create the initial database from the logical package
		/// </summary>
		/// <param name="ldmPackage">the logical pacakge</param>
		/// <returns>the new database</returns>
		protected abstract void createNewDatabase();

		protected virtual void createTables()
		{
			if (logicalPackage != null)
			{
				//not for abstract classes
				foreach (UTF_EA.Class classElement in logicalPackage.ownedElements.OfType<UTF_EA.Class>())
				{
					addTable(classElement);
				}
				//set table names for tables that don't have a name yet;
				nameUnnamedTables();
				removeAbstractTables();
			}
		}
		protected abstract void addTable(UTF_EA.Class classElement);
		
		protected abstract void nameUnnamedTables();
		/// <summary>
		/// abstract tables need to be removed. All foreign keys to these tables have to be replaced by foreign key's to the tables for the 
		/// subclasses of the logical class
		/// </summary>
		protected abstract void removeAbstractTables();
			
		
		#endregion
	}
}
