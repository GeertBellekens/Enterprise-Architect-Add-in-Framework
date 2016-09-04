
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
	/// Description of EAForeignKeyTransformer.
	/// </summary>
	public abstract class EAForeignKeyTransformer:EADatabaseItemTransformer, DB.Transformation.ForeignKeyTransformer
	{
		internal ForeignKey _foreignKey;
		internal UTF_EA.Association _logicalAssociation;
		public EAForeignKeyTransformer(NameTranslator nameTranslator):base(nameTranslator)
		{
			_nameTranslator = nameTranslator;
		}
		public EAForeignKeyTransformer(ForeignKey foreignKey, UTF_EA.Association association,NameTranslator nameTranslator):this(nameTranslator)
		{
			_foreignKey = foreignKey;
			_logicalAssociation = association;
		}

		#region ForeignKeyTransformer implementation

		public DB.ForeignKey foreignKey 
		{
			get { return _foreignKey;}
			set { _foreignKey = (ForeignKey) value;}
		}

		public abstract void resetName();
		public UML.Classes.Kernel.Association logicalAssociation {
			get { return _logicalAssociation;}
			set { _logicalAssociation = (UTF_EA.Association) value;}
		}

		#endregion
	}
}
