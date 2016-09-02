
using System;

namespace EAAddinFramework.Databases.Transformation
{
	/// <summary>
	/// Description of EADatabaseItemTransformer.
	/// </summary>
	public class EADatabaseItemTransformer
	{
		internal NameTranslator _nameTranslator;
		public EADatabaseItemTransformer(NameTranslator nameTranslator)
		{
			_nameTranslator = nameTranslator;
		}
	}
}
