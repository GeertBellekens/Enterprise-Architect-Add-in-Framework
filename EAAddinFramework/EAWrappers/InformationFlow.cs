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
	
	private UML.Classes.Kernel.Classifier _conveyedClassifier = null;
	private HashSet<UML.Classes.Kernel.Classifier> _conveyed = new HashSet<UML.Classes.Kernel.Classifier>();
	private ConnectorWrapper _realization = null;
	public HashSet<UML.Classes.Kernel.Relationship> _realizations  = new HashSet<UML.Classes.Kernel.Relationship>();
	#region InformationFlow implementation
	
	public HashSet<UML.Classes.Kernel.Classifier> conveyed 
	{
		get 
		{
			// in EA there will only be one classifier for each InformationFlow
			if (_conveyedClassifier == null)
			{
				// because access doesn't want to join on the description field (memo) we cannot use proper SQL join syntax
				string sqlGetClassifier = @"select o.Object_ID 
							from t_connector c, t_xref x, t_object o 
							where c.ea_guid = x.client
							and o.ea_guid like x.description
							and c.ea_guid = '" + this.guid + "'";
				_conveyedClassifier = this.model.getElementWrappersByQuery(sqlGetClassifier).FirstOrDefault() as UML.Classes.Kernel.Classifier;
				if(_conveyedClassifier != null)
				{
					_conveyed.Add(_conveyedClassifier);
				}
			}
			return _conveyed;
		}
		set 
		{
			throw new NotImplementedException();
		}
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
										
				_realization = this.model.getRelationsByQuery(sqlGetRealization).FirstOrDefault();
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