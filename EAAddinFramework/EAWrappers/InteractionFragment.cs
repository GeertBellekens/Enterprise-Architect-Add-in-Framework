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

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Description of Transition.
	/// </summary>
	public class InteractionFragment: UTF_EA.ElementWrapper
	, UML.Interactions.BasicInteractions.InteractionFragment
	{
		
		public InteractionFragment(UTF_EA.Model model, global::EA.Element wrappedElement)
			: base(model,wrappedElement)
		{

		}
		public UML.Interactions.Fragments.InteractionOperand enclosingOperand {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public HashSet<UML.Interactions.BasicInteractions.Lifeline> covered {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public HashSet<UML.Interactions.BasicInteractions.GeneralOrdering> generalOrderings {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public HashSet<UML.Interactions.BasicInteractions.Interaction> enclosingInteraction {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
