
using System;
using System.Collections.Generic;
using System.Linq;
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
		internal List<DB2TableTransformer> _externalTransformers = new List<DB2TableTransformer>();
		public DB2TableTransformer(Database database):base(database){}
		
		#region implemented abstract members of EATableTransformer
		protected override void createTable(System.Collections.Generic.List<UML.Classes.Kernel.Class> logicalClasses)
		{
			throw new NotImplementedException();
		}
		protected override void createTable(UTF_EA.Class classElement)
		{
			this._logicalClasses.Add(classElement);
			if (classElement.alias == string.Empty) classElement.alias = "unknown table name";
			this.table = new Table(_database, classElement.alias);
		}

		protected override Column transformLogicalAttribute(UTF_EA.Attribute attribute)
		{
			var columnTransformer = new DB2ColumnTransformer(this._table);
			this._columnTransformers.Add(columnTransformer);
			return (Column) columnTransformer.transformLogicalProperty(attribute);
		}

		public override List<DB.Transformation.ColumnTransformer> columnTransformers {
			get { return _columnTransformers.Cast<DB.Transformation.ColumnTransformer>().ToList();}
			set { _columnTransformers = value.Cast<DB2ColumnTransformer>().ToList();}
		}

		#endregion
		/// <summary>
		/// gets the external Class Elements that are needed for this logical element.
		/// This means the classes that are in another package as the logical class, but to which this element has an association to with
		/// multiplicity of 1..1 or 0..1. We will need these external classes because they will create one or more columns in the associated table.
		/// </summary>
		/// <returns>the externalClasses for this logical element</returns>
		public List<UTF_EA.Class> getExternalClassElements()
		{
			return getDependingClassElements().Where( x => ! this.logicalClasses.Any(y => y.owningPackage.Equals(x.owningPackage))).ToList();
		}
		/// <summary>
		/// gets the Class Elements that are needed for this logical element.
		/// This means the classes to which this element has an association to with
		/// multiplicity of 1..1 or 0..1. We will need these classes because they will create one or more columns in the associated table.
		/// </summary>
		/// <returns>the classes on which this logical element depends for this logical element</returns>
		public List<UTF_EA.Class> getDependingClassElements()
		{
			List<UTF_EA.Class> dependingClassElements = new List<UTF_EA.Class>();
			foreach (var logicalClass in this.logicalClasses) 
			{
				foreach (var association in logicalClass.relationships.OfType<UTF_EA.Association>())
				{
					foreach (var end in association.memberEnds) 
					{
						if (!logicalClass.Equals(end.type) 
					          && end.type is UTF_EA.Class
					          && (end.upper.integerValue.HasValue && end.upper.integerValue == 1))
						{
							dependingClassElements.Add(( UTF_EA.Class) end.type);
							break;
						}
					}
				}
			}
			return dependingClassElements;
			
		}
	}
}
