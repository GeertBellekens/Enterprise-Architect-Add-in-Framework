/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 03.05.2012
 * Time: 18:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA.BehaviorStateMachines {
	/// <summary>
	/// Description of FinalState.
	/// </summary>
	public class FinalState
	: State, UML.StateMachines.BehaviorStateMachines.FinalState
	{
		public FinalState(Model model, global::EA.Element wrappedElement, UML.StateMachines.BehaviorStateMachines.Region containingRegion)
      	: base(model,wrappedElement,containingRegion)
		{
		}
	}
}
