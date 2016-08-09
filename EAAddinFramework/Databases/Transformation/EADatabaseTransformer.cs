
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
	public abstract class EADatabaseTransformer:DB.Transformation.DatabaseTransformer
	{
		#region DatabaseTransformer implementation

		internal Database _database;
		internal UTF_EA.Package _logicalPackage;
		public DB.Database database 
		{
			get {return this._database;}
			set {this._database = (Database)value;}
		}
		public UML.Classes.Kernel.Package logicalPackage 
		{
			get {return this._logicalPackage;}
			set {this._logicalPackage = (UTF_EA.Package)value;}
		}
		public List<DB.Transformation.TableTransformer> tableTransformers {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public DatabaseFramework.Database transformLogicalPackage(TSF.UmlToolingFramework.UML.Classes.Kernel.Package logicalPackage)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
