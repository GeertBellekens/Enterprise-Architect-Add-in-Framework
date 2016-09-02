

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
	/// Description of DB2DatabaseTransformer.
	/// </summary>
	public class DB2DatabaseTransformer:EADatabaseTransformer
	{
		internal List<DB2TableTransformer> externalTableTransformers = new List<DB2TableTransformer>();
		internal Database _externalDatabase;
		public DB2DatabaseTransformer(UTF_EA.Package logicalPackage,NameTranslator nameTranslator):this(logicalPackage.model, nameTranslator)
		{
			this._logicalPackage = logicalPackage;
		}
		public DB2DatabaseTransformer(UTF_EA.Model model,NameTranslator nameTranslator):this(getFactory(model),model,nameTranslator)
		{
		}
		public DB2DatabaseTransformer(DatabaseFactory factory,UTF_EA.Model model,NameTranslator nameTranslator):base(factory,model,nameTranslator)
		{
			this._externalDatabase = factory.createDatabase("external");
		}
		private static DatabaseFactory getFactory(UTF_EA.Model model)
		{
			DB_EA.DatabaseFactory.addFactory("DB2",model);
			return  DB_EA.DatabaseFactory.getFactory("DB2");
		}

		#region implemented abstract members of EADatabaseTransformer
		public override DB.Database saveDatabase()
		{
			//TODO: save changes from new database to existing database
			return this.existingDatabase;
		}
		/// <summary>
		/// create the initial database from the logical package
		/// </summary>
		protected override void createNewDatabase()
		{
			this._newDatabase = factory.createDatabase(this._logicalPackage.alias);
		}

		protected override void addTable(UTF_EA.Class classElement)
		{
			addDB2Table(classElement);
		}
		protected DB2TableTransformer addDB2Table(UTF_EA.AssociationEnd associationEnd)
		{
			DB2TableTransformer transformer = addDB2Table(associationEnd.type as UTF_EA.Class);
			if (transformer == null)
			{
				transformer = this.tableTransformers.OfType<DB2TableTransformer>().FirstOrDefault( x => x.logicalClass.Equals(associationEnd.type));
			}
			if (transformer == null)
			{
				transformer = this.externalTableTransformers.FirstOrDefault( x => x.logicalClass.Equals(associationEnd.type));
			}
			if (transformer != null)
			{
				transformer.associationEnd = associationEnd;
			}
			return transformer;
		}
		protected DB2TableTransformer addDB2Table(UTF_EA.Class classElement)
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
					transformer.dependingTransformers.Add(addDB2Table(dependingAssociationEnd));
				}
				//add the remote columns and primary and foreign keys
				transformer.addRemoteColumnsAndKeys();
			}
			return transformer;
		}


		#endregion
	}
}
