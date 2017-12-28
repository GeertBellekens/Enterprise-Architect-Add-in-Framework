/*
 * Created by SharpDevelop.
 * User: Admin
 * Date: 25.04.2012
 * Time: 19:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA.BehaviorStateMachines {
		
	/// <summary>
	/// Description of Region.
	/// </summary>
	public class Region 
	: ElementWrapper
	, UML.StateMachines.BehaviorStateMachines.Region
	{
		ElementWrapper redefinedElement = null;
		global::EA.Diagram masterDiagram = null;
		global::EA.Partition partition = null;
		string _name;
		short _regionPos;
		
		UML.StateMachines.BehaviorStateMachines.StateMachine _stateMachine = null;
		UML.StateMachines.BehaviorStateMachines.State _state = null;
		
		public Region
				( Model model
			 	, ElementWrapper redefinedElement
			 	, global::EA.Diagram masterDiagram
			 	, global::EA.Partition partition
			 	, short regionPos = 0
			 	)
		: base(model,redefinedElement.wrappedElement)
		{
			this.redefinedElement = redefinedElement;
			this.masterDiagram = masterDiagram;
			this.partition = partition;
			this._regionPos = regionPos;
			if(redefinedElement is UML.StateMachines.BehaviorStateMachines.StateMachine) {
				_stateMachine = redefinedElement as UML.StateMachines.BehaviorStateMachines.StateMachine;
			}
			else if(redefinedElement is UML.StateMachines.BehaviorStateMachines.State) {
				_state = redefinedElement as UML.StateMachines.BehaviorStateMachines.State;
			}
			else {
				throw new ArgumentException("Only StateMachine or State instances are allowed as owners.","owningElement");
			}
			if(partition != null) {
				if(!string.IsNullOrEmpty(partition.Name) &&
				   partition.Name != "<anonymous>") {
					_name = partition.Name;
				}
				else {
					_name = base.name + "Region" + regionPos.ToString();
				}
			}
			else {
				_name = base.name + "Region";
			}
		}
		
		public override string name 
		{
			get { 
				return _name; 
			}
			set { 
				_name = value; 
			}
		}

		public bool isLeaf
		{ 
			get {
				return false;
			}
			set {
				throw new NotImplementedException();
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

    	/// <summary>
		/// The StateMachine that owns the Region. If a Region is owned by a StateMachine, then it cannot also be owned by a
		/// State. {Subsets NamedElement::namespace}
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.StateMachine stateMachine 
		{
			get {
				return _stateMachine;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The State that owns the Region. If a Region is owned by a State, then it cannot also be owned by a StateMachine.
		/// {Subsets NamedElement::namespace}
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.State state {
			get {
				return _state;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The set of transitions owned by the region. {Subsets Namespace::ownedMember}
		/// </summary>
		public HashSet<UML.StateMachines.BehaviorStateMachines.Transition> transitions {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The set of vertices that are owned by this region. {Subsets Namespace::ownedMember}
		/// </summary>
		public HashSet<UML.StateMachines.BehaviorStateMachines.Vertex> subvertices {
			get {
				return ((Factory)this.EAModel.factory).createVertices(this);
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// The region of which this region is an extension. {Subsets RedefinableElement::redefinedElement}
		/// </summary>
		public UML.StateMachines.BehaviorStateMachines.Region extendedRegion {
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
		public UML.Classes.Kernel.Classifier redefinitionContext {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool Equals(object obj)
		{
			Region other = obj as Region;
			if (other == null)
				return false;
			return (this.wrappedElement.ElementGUID == other.wrappedElement.ElementGUID &&
			        this._regionPos == other._regionPos);
		}
		
		public override int GetHashCode()
		{
			string hashString = wrappedElement.ElementGUID + _regionPos.ToString();
			return hashString.GetHashCode();
		}

		internal bool isContainedElement(ElementWrapper elementWrapper)
		{
			if(elementWrapper == null) {
				return false;
			}
			
			// The element in question must be a direct child of the regions owning element
			if(elementWrapper.wrappedElement.ParentID != redefinedElement.wrappedElement.ElementID)
			{
				return false;
			}
			
			if(partition != null)
			{
				// Check if the element in question resides inside the graphical representation of this region
				//--------------------------------------------------------------------------------------------
				if(masterDiagram != null)
				{
					// Get the element in question's graphical representation from the master diagram
					global::EA.DiagramObject elementDiagramObject = getMasterDiagramObject(elementWrapper);
					if(elementDiagramObject != null)
					{
						System.Drawing.Rectangle elementRectangle = 
							new System.Drawing.Rectangle
									( elementDiagramObject.left
							 		, System.Math.Abs(elementDiagramObject.top)
							  		, elementDiagramObject.right - elementDiagramObject.left
							  		, System.Math.Abs(elementDiagramObject.bottom) - System.Math.Abs(elementDiagramObject.top)
							  		);
						// Get the owning elements graphical region representation from the master diagram
						global::EA.DiagramObject owningElementDiagramObject = getMasterDiagramObject(redefinedElement);
						if(owningElementDiagramObject != null)
						{
							int x = owningElementDiagramObject.left;
							int y = System.Math.Abs(owningElementDiagramObject.top) + getRegionTopOffset(partition);
							int width = owningElementDiagramObject.right - x;
							int height = partition.Size;
							System.Drawing.Rectangle regionRectangle = new System.Drawing.Rectangle(x,y,width,height);
							if(regionRectangle.Contains(elementRectangle))
						    {
						   		return true;
						    }
						}
					}
				}
			}
			else
			{
				return true;
			}
			return false;
		}
		
		private global::EA.DiagramObject getMasterDiagramObject(ElementWrapper elementWrapper)
		{
			foreach(global::EA.DiagramObject diagramObject in masterDiagram.DiagramObjects)
			{
				if(diagramObject.ElementID == elementWrapper.wrappedElement.ElementID)
				{
					return diagramObject;
				}
			}
			return null;
		}
		
		private int getRegionTopOffset(global::EA.Partition partition)
		{
			int regionTopOffset = 0;
			short otherRegionPos = 0;
			foreach(global::EA.Partition otherPartition in redefinedElement.wrappedElement.Partitions)
			{
				if(otherRegionPos < _regionPos)
				{
					regionTopOffset += otherPartition.Size;
				}
				else
				{
					break;
				}
				++otherRegionPos;
			}
			return regionTopOffset;
		}
	}
}
