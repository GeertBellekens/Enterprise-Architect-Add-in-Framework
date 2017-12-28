using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
  public class InformationFlow : ConnectorWrapper, 
                             UML.InfomationFlows.InformationFlow 
  {
	public InformationFlow(Model model, global::EA.Connector connector)
	  : base(model, connector){}
	
	private HashSet<UML.Classes.Kernel.Classifier> _conveyed = null;
	private ConnectorWrapper _realization = null;
	public HashSet<UML.Classes.Kernel.Relationship> _realizations  = new HashSet<UML.Classes.Kernel.Relationship>();
	#region InformationFlow implementation
	
	public HashSet<UML.Classes.Kernel.Classifier> conveyed 
	{
		get 
		{
			if (_conveyed == null)
			{
				string getXrefDescription = @"select x.Description from t_xref x 
											where x.Name = 'MOFProps'
											and x.Behavior = 'conveyed'
											and x.client = '" + this.guid + "'";
				//xrefdescription contains the GUID's of the conveyed elements comma separated
				var xrefDescription = this.EAModel.SQLQuery(getXrefDescription).SelectSingleNode(this.EAModel.formatXPath("//Description"));
				if (xrefDescription != null)
				{
					foreach (string conveyedGUID in xrefDescription.InnerText.Split(','))
					{
						var conveyedElement = this.EAModel.getElementWrapperByGUID(conveyedGUID) as UML.Classes.Kernel.Classifier;
						if (conveyedElement != null)
						{
							//initialize if needed
							if (_conveyed == null)
							{
								_conveyed = new HashSet<UML.Classes.Kernel.Classifier>();
							}
							//add the element
							_conveyed.Add(conveyedElement);
						}
					}
				}
			}
			//nothing found, return empty list.
			if (_conveyed == null)
			{
				_conveyed = new HashSet<UML.Classes.Kernel.Classifier>();
			}
			return _conveyed;
		}
		set 
		{
			throw new NotImplementedException();
		}
	}
		public override HashSet<UML.InfomationFlows.InformationFlow> getInformationFlows()
		{
			var informationFlows = new HashSet<UML.InfomationFlows.InformationFlow>();
			informationFlows.Add(this);
			return informationFlows;
		}
	
	public HashSet<UML.Classes.Kernel.Relationship> realizations 
	{
		get 
		{
			//there is only one realization connector for each Informationflow in EA
			if (_realization == null)
			{
				// because access doesn't want to join on the description field (memo) we cannot use proper SQL join syntax
				string sqlGetRealization = @"select crel.Connector_ID from t_connector c, t_xref x, t_connector crel  
											where x.Name = 'MOFProps'
											and crel.ea_guid = x.Client
											and c.ea_guid = '" + this.guid + "'"
											+" and x.Description like '%"+this.guid+"%'";
										
				_realization = this.EAModel.getRelationsByQuery(sqlGetRealization).FirstOrDefault();
				if (_realization != null)
				{
					_realizations.Add(_realization);
				}
			}
			return _realizations;
		}
		set 
		{
			throw new NotImplementedException();
		}
	}

	#endregion
  }
}