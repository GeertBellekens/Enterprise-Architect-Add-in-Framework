using System.Collections.Generic;
using System.Linq;
using System;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of ProxyConnector.
	/// </summary>
	public class ProxyConnector:ElementWrapper
	{
		public ProxyConnector(Model model, global::EA.Element wrappedElement):base(model,wrappedElement)
		{	
		}
		public ConnectorWrapper connector 
		{
		    get 
	    	{
		    	return this.EAModel.getRelationByID((int)this.getProperty(getPropertyNameName(),this.wrappedElement.ClassifierID));
	    	}
			set 
			{
				this.setProperty(getPropertyNameName(),value.id,this.wrappedElement.ClassifierID);
			}
		}
		internal override void saveElement()
		{
			if (this.getProperty("connector") != null) this.wrappedElement.ClassifierID = (int)this.getProperty("connector");
			base.saveElement();
		}
	}
}
