using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Databases.Strategy
{
	/// <summary>
	/// Description of StrategyFactory.
	/// </summary>
	public abstract class StrategyFactory
	{
		public abstract DatabaseItemStrategy getStrategy<T>() where T : DatabaseItem;
	}
}
