/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 22.06.2012
 * Time: 21:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
using UML_SM = TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace TSF.UmlToolingFramework.Wrappers.EA.BehaviorStateMachines
{
	/// <summary>
	/// Description of Transition.
	/// </summary>
	public class Trigger
	: UTF_EA.ElementWrapper
	, UML.CommonBehaviors.Communications.Trigger
	{

		UML.CommonBehaviors.Communications.Event _event = null;
		
		public Trigger(UTF_EA.Model model, global::EA.Element wrappedElement)
			: base(model,wrappedElement)
		{
			_event = new UTF_EA.BehaviorStateMachines.Event(model,wrappedElement);
		}
		
        // The event that causes the trigger.
		public UML.CommonBehaviors.Communications.Event event_ { 
        	get {
        		return _event;
        	}
        	set {
				throw new NotImplementedException();
        	}
        }
	}
}
