using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace EAAddinFramework.Databases.Strategy.DB2
{
	/// <summary>
	/// Description of DB2TableStrategy.
	/// </summary>
	public class DB2TableStrategy:DB2Strategy
	{
		View linkedView {get;set;}
		public DB2TableStrategy(StrategyFactory factory):base(factory) {}

		public override void beforeSave()
		{
			//new table
			if (databaseItem.wrappedElement == null)
			{
				//create new view
				linkedView = new View((Database)this.databaseItem.owner,this.getViewName() ,factory.getStrategy<View>());
				this.linkedView.definition = getViewDefinition();
			}
		}
		public override void afterSave()
		{
			if (this.linkedView != null)
			{
				this.linkedView.save();
			}
		}
		private string getViewName()
		{
			var nameStringBuilder = new StringBuilder(this.databaseItem.name);
			nameStringBuilder[2] = 'V';
			return nameStringBuilder.ToString();
		}
		private string getViewDefinition()
		{
			return "select * from " + ((Table)this.databaseItem).tableOwner + "." + this.databaseItem.name;
		}
	}
}
