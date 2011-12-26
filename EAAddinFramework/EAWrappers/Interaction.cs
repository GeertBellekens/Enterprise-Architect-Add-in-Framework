using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
	/// <summary>
	/// Description of InterAction.
	/// </summary>
	public class Interaction: ElementWrapper,UML.Interactions.BasicInteractions.Interaction
	{
		/// <summary>
		/// default constructor, calls parent constructor
		/// </summary>
		/// <param name="model">the model containing the element</param>
		/// <param name="wrappedElement">the EA.Element to be wrapped</param>
		public Interaction(Model model, global::EA.Element wrappedElement)
      	: base(model,wrappedElement)
    	{
    	}
		
		public HashSet<TSF.UmlToolingFramework.UML.Interactions.Fragments.Gate> formalGates {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Interactions.BasicInteractions.Lifeline> lifelines {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Interactions.BasicInteractions.Message> messages {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Interactions.BasicInteractions.InteractionFragment> fragments {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Actions.BasicActions.Action> actions {
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
		
		public TSF.UmlToolingFramework.UML.Interactions.Fragments.InteractionOperand enclosingOperand {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Interactions.BasicInteractions.Lifeline> covered {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Interactions.BasicInteractions.GeneralOrdering> generalOrderings {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public HashSet<TSF.UmlToolingFramework.UML.Interactions.BasicInteractions.Interaction> enclosingInteraction {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
