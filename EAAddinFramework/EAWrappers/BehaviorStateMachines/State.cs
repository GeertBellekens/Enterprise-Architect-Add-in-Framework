/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 02.05.2012
 * Time: 21:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
using UML_SM = TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines;

namespace TSF.UmlToolingFramework.Wrappers.EA.BehaviorStateMachines {
	/// <summary>
	/// Description of State.
	/// </summary>
	public class State
	: Vertex, UML.StateMachines.BehaviorStateMachines.State
	{

		public State(Model model, global::EA.Element wrappedElement, UML.StateMachines.BehaviorStateMachines.Region containingRegion)
      	: base(model,wrappedElement,containingRegion)
		{
		}

		/// <summary>
		/// A state with isComposite=true is said to be a composite state. A composite state is a state that contains at least one
		/// region. Default value is false.
		/// </summary>
		public bool isComposite    
		{ 
			get {
				if(regions.Count > 0) {
					int containedSubVertices = 0;
					foreach(UML.StateMachines.BehaviorStateMachines.Region region in regions) {
						containedSubVertices += region.subvertices.Count;
					}
					return containedSubVertices > 0;
				}
				else if(ownedElements.Count > 0) {
					int ownedStates = 0;
					foreach(UML.Classes.Kernel.Element element in ownedElements) {
						if(element is State) {
							++ownedStates;
						}
					}
					return ownedStates > 0;				
				}
				global::EA._CustomProperty prop = getCustomProperty("isComposite");
				if(prop != null) {
					return prop.Value == "-1";
				}
				return false;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// A state with isOrthogonal=true is said to be an orthogonal composite state. An orthogonal composite state contains
		/// two or more regions. Default value is false.
		/// </summary>
		public bool isOrthogonal    
		{ 
			get {
				if(regions.Count > 1) {
					return true;
				}
				global::EA._CustomProperty prop = getCustomProperty("isOrthogonal");
				if(prop != null) {
					return prop.Value == "-1";
				}
				return false;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// A state with isSimple=true is said to be a simple state. A simple state does not have any regions and it does not refer
		/// to any submachine state machine. Default value is true.
		/// </summary>
		public bool isSimple    
		{ 
			get {
				if(regions.Count > 0) {
					int containedSubVertices = 0;
					foreach(UML.StateMachines.BehaviorStateMachines.Region region in regions) {
						containedSubVertices += region.subvertices.Count;
					}
					return containedSubVertices == 0;
				}
				if(ownedElements.Count > 0) {
					foreach(UML.Classes.Kernel.Element ownedElement in ownedElements) {
						if(ownedElement is UML_SM.State) {
							return false;
						}
					}
					
				}
				return true;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// A state with isSubmachineState=true is said to be a submachine state. Such a state refers to a state machine
		/// (submachine). Default value is false.
		/// </summary>
		public bool isSubmachineState  
		{ 
			get {
				global::EA._CustomProperty prop = getCustomProperty("isSubmachineState");
				if(prop != null) {
					return prop.Value == "-1";
				}
				return false;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The entry and exit connection points used in conjunction with this (submachine) state, i.e., as targets and sources,
		/// respectively, in the region with the submachine state. A connection point reference references the corresponding
		/// definition of a connection point pseudostate in the statemachine referenced by the submachinestate. {Subsets
		/// Namespace::ownedMember}
		/// </summary>
		public HashSet<UML.StateMachines.BehaviorStateMachines.ConnectionPointReference> connections    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The entry and exit pseudostates of a composite state. These can only be entry or exit Pseudostates, and they must have
		/// different names. They can only be defined for composite states. {Subsets Namespace::ownedMember}
		/// </summary>
		public HashSet<UML.StateMachines.BehaviorStateMachines.Pseudostate> connectionPoints    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// A list of triggers that are candidates to be retained by the state machine if they trigger no transitions out of the state (not
		/// consumed). A deferred trigger is retained until the state machine reaches a state configuration where it is no longer
		/// deferred.
		/// </summary>
		public HashSet<UML.CommonBehaviors.Communications.Trigger> deferrableTriggers    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// An optional behavior that is executed while being in the state. The execution starts when this state is entered, and stops
		/// either by itself or when the state is exited whichever comes first. {Subsets Element::ownedElement}
		/// </summary>
		public UML.CommonBehaviors.BasicBehaviors.Behavior doActvity    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// An optional behavior that is executed whenever this state is entered regardless of the transition taken to reach the state. If
		/// defined, entry actions are always executed to completion prior to any internal behavior or transitions performed within the
		/// state. {Subsets Element::ownedElement}
		/// </summary>
		public UML.CommonBehaviors.BasicBehaviors.Behavior entry    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// An optional behavior that is executed whenever this state is exited regardless of which transition was taken out of the
		/// state. If defined, exit actions are always executed to completion only after all internal activities and transition actions have
		/// completed execution. {Subsets Element::ownedElement}
		/// </summary>
		public UML.CommonBehaviors.BasicBehaviors.Behavior exit    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The state of which this state is a redefinition. {Subsets RedefinableElement::redefinedElement}
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.State redefinedState    
		{
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The regions owned directly by the state.
		/// </summary>
		public HashSet<UML.StateMachines.BehaviorStateMachines.Region> regions    
		{ 
			get {
				return ((Factory)this.EAModel.factory).createBehaviourStateMachineRegions(this);
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The state machine that is to be inserted in place of the (submachine) state.
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.StateMachine submachine    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Specifies conditions that are always true when this state is the current state. In protocol state machines, state invariants are
		/// additional conditions to the preconditions of the outgoing transitions, and to the postcondition of the incoming transitions.
		/// </summary>
		public UML.Classes.Kernel.Constraint stateInvariant    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// References the classifier in which context this element may be redefined. {Redefines
		/// RedefinableElement::redefinitionContext}
		/// </summary>
		public UML.Classes.Kernel.Classifier redefinitionContext    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		public bool isLeaf    
		{ 
			get {
				return wrappedElement.IsLeaf;
			}
			set {
				wrappedElement.IsLeaf = value;
			}
		}
		
		public HashSet<UML.Classes.Kernel.RedefinableElement> redefinedElements    
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
    	public HashSet<UML.Classes.Kernel.Classifier> redefinitionContexts     
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
