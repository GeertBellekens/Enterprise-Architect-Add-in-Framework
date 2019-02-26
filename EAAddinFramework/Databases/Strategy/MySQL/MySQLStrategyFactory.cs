using System.Collections.Generic;
using System.Linq;
using System;
using EAAddinFramework.Databases.Strategy;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Databases.Strategy.MySQL
{
	/// <summary>
	/// Description of MySQLStrategyFactory.
	/// </summary>
	public class MySQLStrategyFactory:StrategyFactory
	{
		private static MySQLStrategyFactory instance;
		private MySQLStrategyFactory(){}
		public static MySQLStrategyFactory getInstance()
		{
			if (instance == null)
				instance = new MySQLStrategyFactory();
			return instance;
		}
		#region implemented abstract members of StrategyFactory
		public override DatabaseItemStrategy getStrategy<T>()
		{
			Type TType = typeof(T);
			if( typeof(Table).IsAssignableFrom(TType))
			{
				return new MySQLTableStrategy(this);
			}
			if( typeof(PrimaryKey).IsAssignableFrom(TType))
			{
				return new MySQLPrimaryKeyStrategy (this);
			}
			if( typeof(ForeignKey).IsAssignableFrom(TType))
			{
				return new MySQLForeignKeyStrategy(this);
			}
			if( typeof(Column).IsAssignableFrom(TType))
			{
				return new MySQLColumnStrategy(this);
			}
			else
			{
				return new MySQLStrategy(this);
			}
		}
		#endregion
	}
}
