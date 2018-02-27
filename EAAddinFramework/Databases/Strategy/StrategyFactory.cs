using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Databases.Strategy
{
	/// <summary>
	/// Description of StrategyFactory.
	/// </summary>
	public class StrategyFactory
	{
		public virtual DatabaseItemStrategy getStrategy<T>() where T : DatabaseItem
        {
            //default implementation
            return new DatabaseItemStrategy(this);
        }
	}
}
