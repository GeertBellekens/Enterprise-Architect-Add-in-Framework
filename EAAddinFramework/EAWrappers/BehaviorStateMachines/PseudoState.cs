/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 03.05.2012
 * Time: 18:30
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
	/// Description of PseudoState.
	/// </summary>
	public class PseudoState
	: Vertex, UML.StateMachines.BehaviorStateMachines.Pseudostate
	{
		internal enum EASubTypes
		{
			Synchronization = 0 ,
			Initial = 3 ,
			ShallowHistory = 5 ,
			Synch = 6 ,
			Junction = 10 ,
			Choice = 11 ,
			Terminate = 12 ,
			EntryPoint = 13 ,
			ExitPoint = 14 ,
			DeepHistory = 15 ,
		}
		public PseudoState(Model model, global::EA.Element wrappedElement, UML.StateMachines.BehaviorStateMachines.Region containingRegion)
      	: base(model,wrappedElement,containingRegion)
		{
		}

		/// <summary>
		/// Determines the precise type of the Pseudostate. Default value is initial.
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.PseudostateKind kind
		{
			get {
				switch((EASubTypes)wrappedElement.Subtype)
				{
				case EASubTypes.Initial:
					return UML.StateMachines.BehaviorStateMachines.PseudostateKind.initial;
				case EASubTypes.ShallowHistory:
					return UML.StateMachines.BehaviorStateMachines.PseudostateKind.shallowHistory;
				case EASubTypes.DeepHistory:
					return UML.StateMachines.BehaviorStateMachines.PseudostateKind.deepHistory;
				case EASubTypes.EntryPoint:
					return UML.StateMachines.BehaviorStateMachines.PseudostateKind.entryPoint;
				case EASubTypes.ExitPoint:
					return UML.StateMachines.BehaviorStateMachines.PseudostateKind.exitPoint;
				case EASubTypes.Terminate:
					return UML.StateMachines.BehaviorStateMachines.PseudostateKind.terminate;
				case EASubTypes.Junction:
					return UML.StateMachines.BehaviorStateMachines.PseudostateKind.junction;
				case EASubTypes.Synchronization:
					foreach(global::EA.Property property in wrappedElement.Properties)
					{
						if(property.ToString() == "Fork") {
							return UML.StateMachines.BehaviorStateMachines.PseudostateKind.fork;
						}
						else if(property.ToString() == "Join") {
							return UML.StateMachines.BehaviorStateMachines.PseudostateKind.join;
						}
					}
					break;
				case EASubTypes.Choice:
					return UML.StateMachines.BehaviorStateMachines.PseudostateKind.choice;
				}
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The StateMachine in which this Pseudostate is defined. This only applies to Pseudostates of the kind entryPoint or
		/// exitPoint. {Subsets NamedElement::namespace}
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.StateMachine statemachine
		{
			get {
				return this.owner as StateMachine;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// State that owns the Pseudostate. {Subsets Element::owner}
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.State state 
		{
			get {
				return this.owner as State;
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
