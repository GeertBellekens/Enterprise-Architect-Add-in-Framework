using System.Collections.Generic;
using System.Linq;
using System;

namespace EAAddinFramework.Databases.Strategy
{
	/// <summary>
	/// Description of DatabaseItemStrategy.
	/// </summary>
	public class DatabaseItemStrategy
	{
		protected StrategyFactory factory {get;set;}
		public DatabaseItemStrategy(StrategyFactory factory)
		{
			this.factory = factory;
		}
		public DatabaseItemStrategy getStrategy<T>() where T : DatabaseItem
		{
			return this.factory.getStrategy<T>();
		}
		
		public DatabaseItem databaseItem {get;set;}
		public virtual void onNew()
		{
			//default empty implementation
		}
		public virtual void beforeSave()
		{
			//default empty implementation
		}
		public virtual void afterSave()
		{
			//default empty implementation
		}
		public virtual void beforeDelete()
		{
			//default empty implementation
		}
		public virtual void afterDelete()
		{
			//default empty implementation
		}
	}
}
