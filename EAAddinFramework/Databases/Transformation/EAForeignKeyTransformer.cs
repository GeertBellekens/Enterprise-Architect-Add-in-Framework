
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
		internal UTF_EA.Association _logicalAssociation
		{
			get
			{
				return _logicalAssociationEnd != null ? (UTF_EA.Association) _logicalAssociationEnd.association : null;
			}
		}

		public UML.Classes.Kernel.Property logicalAssociationEnd 
		{
			get 
			{
				return this._logicalAssociationEnd;
			}
			set 
			{
				this._logicalAssociationEnd = value as UTF_EA.AssociationEnd;
			}
		}

		internal UTF_EA.AssociationEnd _logicalAssociationEnd;
		public EAForeignKeyTransformer(NameTranslator nameTranslator):base(nameTranslator)
		{
			_nameTranslator = nameTranslator;
		}
		public EAForeignKeyTransformer(ForeignKey foreignKey, UTF_EA.AssociationEnd associationEnd,NameTranslator nameTranslator):this(nameTranslator)
		{
			_foreignKey = foreignKey;
			_logicalAssociationEnd = associationEnd;
		}

		#region ForeignKeyTransformer implementation

		public DB.ForeignKey foreignKey 
		{
			get { return _foreignKey;}
			set { _foreignKey = (ForeignKey) value;}
		}

		public abstract void resetName();

		#region implemented abstract members of EADatabaseItemTransformer



		public override DB.Transformation.DatabaseItemTransformer getCorrespondingTransformer(DB.DatabaseItem item)
		{
			if (item == this.foreignKey) return this;
			return null;
		}


		public override DB.DatabaseItem databaseItem {
			get {
				return this.foreignKey;
			}
		}


		#endregion

		public UML.Classes.Kernel.Association logicalAssociation {
			get { return _logicalAssociation;}
		}

		#endregion

		public virtual void save()
		{
			throw new NotImplementedException();
		}
	}
}
