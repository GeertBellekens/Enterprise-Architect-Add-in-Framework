using System.Collections.Generic;
using System.Linq;
using System;
using EAAddinFramework.Databases.Strategy;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Databases.Strategy.DB2
{
	/// <summary>
	/// Description of DB2StrategyFactory.
	/// </summary>
	public class DB2StrategyFactory:StrategyFactory
	{
		private static DB2StrategyFactory instance;
		private DB2StrategyFactory(){}
		public static DB2StrategyFactory getInstance()
		{
			if (instance == null)
				instance = new DB2StrategyFactory();
			return instance;
		}
		#region implemented abstract members of StrategyFactory
		public override DatabaseItemStrategy getStrategy<T>()
		{
			Type TType = typeof(T);
			if( typeof(Table).IsAssignableFrom(TType))
			{
				return new DB2TableStrategy(this);
			}
			if( typeof(PrimaryKey).IsAssignableFrom(TType))
			{
				return new DB2PrimaryKeyStrategy (this);
			}
			if( typeof(ForeignKey).IsAssignableFrom(TType))
			{
				return new DB2ForeignKeyStrategy(this);
			}
			if( typeof(Column).IsAssignableFrom(TType))
			{
				return new DB2ColumnStrategy(this);
			}
			else
			{
				return new DB2Strategy(this);
			}
		}
		#endregion
	}
}
