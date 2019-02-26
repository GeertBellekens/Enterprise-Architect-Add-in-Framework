using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace EAAddinFramework.Databases.Strategy.MySQL
{
	/// <summary>
	/// Description of MySQLTableStrategy.
	/// </summary>
	public class MySQLTableStrategy:MySQLStrategy
	{
		View linkedView {get;set;}
		Table table 
		{
			get
			{
				return (Table)this.databaseItem;
			}
		}
		public MySQLTableStrategy(StrategyFactory factory):base(factory) {}

		public override void beforeSave()
		{
			//new table
			if (databaseItem.isNew)
			{
				//create new view
				linkedView = new View((Database)table.databaseOwner,this.getViewName() ,factory.getStrategy<View>());
				this.linkedView.definition = getViewDefinition();
			}
		}
		public override void afterSave()
		{
			if (databaseItem.isNew)
			{
				if (this.linkedView != null)
				{
					this.linkedView.save();
				}
			}
		}
		private string getViewName()
		{
			var nameStringBuilder = "V_" + this.table.name; // new StringBuilder(this.table.name);
            return nameStringBuilder;
			//nameStringBuilder[2] = 'V';
			//return nameStringBuilder.ToString();
		}
		private string getViewDefinition()
		{
			return "select * from " + table.tableOwner + "." + this.table.name;
		}

	}
}
