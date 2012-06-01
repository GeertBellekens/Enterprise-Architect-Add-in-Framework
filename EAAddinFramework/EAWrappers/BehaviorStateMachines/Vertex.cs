/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 03.05.2012
 * Time: 20:25
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
	/// Description of Vertex.
	/// </summary>
	public class Vertex
	: ElementWrapper, UML.StateMachines.BehaviorStateMachines.Vertex
	{
		private UML.StateMachines.BehaviorStateMachines.Region _container;
		
		public Vertex(Model model, global::EA.Element wrappedElement, UML.StateMachines.BehaviorStateMachines.Region containingRegion)
      	: base(model,wrappedElement)
		{
			this._container = containingRegion;
		}
		/// <summary>
		/// Specifies the transitions departing from this vertex. Derived in the following way:
		/// context Vertex::outgoing derive:
		/// Transition.allInstances() -> select(t | t.source = self)
		/// </summary>
		public HashSet<UML.StateMachines.BehaviorStateMachines.Transition> outgoings  
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Specifies the transitions entering this vertex. Derived in the following way:
		/// context Vertex::incoming derive:
		/// Transition.allInstances() -> select(t | t.target = self)
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.Transition incomings  
		{ 
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The region that contains this vertex. {Subsets Element::owner}
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.Region container  
		{ 
			get {
				return _container;
			}
			set {
				_container = value;
			}
		}
	}
}
