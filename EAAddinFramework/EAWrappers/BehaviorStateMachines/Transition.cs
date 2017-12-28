/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 13.05.2012
 * Time: 18:47
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
	public class Transition
	: ConnectorWrapper, UML_SM.Transition
	{
		private UML_SM.TransitionKind _kind = UML_SM.TransitionKind._external;
		
		public Transition(Model model, global::EA.Connector transition)
			: base(model,transition)
		{
		}
		
		/// <summary>
		/// See definition of TransitionKind. Default value is external.
		/// </summary>
		public UML_SM.TransitionKind kind
		{ 
			get {
				return _kind;
			}
			set {
				_kind = value;
			}
		}
		
		/// <summary>
		/// Specifies the triggers that may fire the transition.
		/// </summary>
		public HashSet<UML.CommonBehaviors.Communications.Trigger> triggers  
		{ 
			get {
				return ((UTF_EA.Factory)EAModel.factory).createTransitionTriggers(this);
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// A guard is a constraint that provides a fine-grained control over the firing of the transition. The guard is evaluated
		/// when an event occurrence is dispatched by the state machine. If the guard is true at that time, the transition may be
		/// enabled; otherwise, it is disabled. Guards should be pure expressions without side effects. Guard expressions with
		/// side effects are ill-formed. {Subsets Namespace::ownedRule}
		/// </summary>
		public UML.Classes.Kernel.Constraint guard  
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Specifies an optional behavior to be performed when the transition fires.
		/// </summary>
		public UML.CommonBehaviors.BasicBehaviors.Behavior effect  
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Designates the originating vertex (state or pseudostate) of the transition.
		/// </summary>
		public UML_SM.Vertex source  
		{ 
			get {
				return base.source as UML_SM.Vertex;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Designates the target vertex that is reached when the transition is taken.
		/// </summary>
		public UML_SM.Vertex target  
		{ 
			get {
				return base.target as UML_SM.Vertex;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The transition of which this is a replacement. {Subsets RedefinableElement::redefinedElement}
		/// </summary>
		public UML_SM.Transition redefinedTransition  
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
		
		/// <summary>
		/// Designates the region that owns this transition. (Subsets Namespace.namespace)
		/// </summary>
		public UML_SM.Region container  
		{ 
			get {
				return source.container;
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
