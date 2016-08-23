

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
		public DB2DatabaseTransformer(UTF_EA.Model model):this(getFactory(model),model)
		{
		}
		public DB2DatabaseTransformer(DatabaseFactory factory,UTF_EA.Model model):base(factory,model)
		{
			this._externalDatabase = factory.createDatabase("external");
		}
		private static DatabaseFactory getFactory(UTF_EA.Model model)
		{
			//TODO: use model to figure out the datatypes for DB2
			List<DB_EA.BaseDataType> baseDatatypes = new List<DB_EA.BaseDataType>();
			baseDatatypes.Add(new DB_EA.BaseDataType("CHAR",true, false));
			baseDatatypes.Add(new DB_EA.BaseDataType("TIMESTAMP",false, false));
			baseDatatypes.Add(new DB_EA.BaseDataType("DATE",false, false));
			baseDatatypes.Add(new DB_EA.BaseDataType("DECIMAL",true, true));
			DB_EA.DatabaseFactory.addFactory("DB2",baseDatatypes);
			return  DB_EA.DatabaseFactory.getFactory("DB2");
		}

		#region implemented abstract members of EADatabaseTransformer

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
					transformer = new DB2TableTransformer(this._newDatabase);
					this._tableTransformers.Add(transformer);
				}
			}
			else
			{
				if (!this.externalTableTransformers.Any(x => x.logicalClasses.Any(y => y.Equals(classElement))))
				{
					transformer = new DB2TableTransformer(this._externalDatabase);
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
				//add primary key
				transformer.addRemoteColumnsAndKeys();
				//copy the primary keys of the depending transformers as column in this table
				
				//add foreign keys
				transformer.transformForeignKeys();
			}
			return transformer;
		}


		#endregion
	}
}
