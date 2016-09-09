
using System;
using DB=DatabaseFramework;

namespace EAAddinFramework.Databases.Transformation
{
	/// <summary>
	/// Description of EADatabaseItemTransformer.
	/// </summary>
	public abstract class EADatabaseItemTransformer:DB.Transformation.DatabaseItemTransformer
	{
		internal NameTranslator _nameTranslator;
		public EADatabaseItemTransformer(NameTranslator nameTranslator)
		{
			_nameTranslator = nameTranslator;
		}

		#region DatabaseItemTransformer implementation

		public abstract void rename(string newName);

		public abstract DB.Transformation.DatabaseItemTransformer getCorrespondingTransformer(DB.DatabaseItem item);
		
		public abstract DB.DatabaseItem databaseItem {get;}


		#endregion
	}
}
