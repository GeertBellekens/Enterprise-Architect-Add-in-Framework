using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA.BehaviorStateMachines {
	/// <summary>
	/// Description of StateMachine.
	/// </summary>
	public class StateMachine:ElementWrapper,UML.StateMachines.BehaviorStateMachines.StateMachine
	{
				
		/// <summary>
		/// default constructor, calls parent constructor
		/// </summary>
		/// <param name="model">the model containing the element</param>
		/// <param name="wrappedElement">the EA.Element to be wrapped</param>
		public StateMachine(Model model, global::EA.Element wrappedElement)
      	: base(model,wrappedElement)
    	{
    	}
		
		public HashSet<TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines.Region> regions {
			get {
				return ((Factory)this.EAModel.factory).createBehaviourStateMachineRegions(this);
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines.Pseudostate> connectionPoints {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines.StateMachine> extendedStateMachines {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool isReentrant {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		

		
		public TSF.UmlToolingFramework.UML.CommonBehaviors.BasicBehaviors.BehavioredClassifier context {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Parameter> ownedParameters {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public TSF.UmlToolingFramework.UML.CommonBehaviors.BasicBehaviors.Behavior redefinedBehavior {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Constraint> preconditions {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Constraint> postconditions {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Classifier> nestedClassifiers {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Feature> features {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Dependencies.Substitution> substitutions {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool isLeaf {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.RedefinableElement> redefinedElements {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Classifier> redefinitionContexts {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
