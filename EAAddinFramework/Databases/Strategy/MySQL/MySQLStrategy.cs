using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Databases.Strategy.MySQL
{
	/// <summary>
	/// Description of MySQLStrategy.
	/// </summary>
	public class MySQLStrategy:DatabaseItemStrategy
	{
		public MySQLStrategy(StrategyFactory factory):base(factory) {}
	}
}
