using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
	/// <summary>
	/// Description of Action.
	/// </summary>
	public class Action: ElementWrapper,UML.Actions.BasicActions.Action
	{
		
		
		public Action(Model model, global::EA.Element wrappedElement)
      	: base(model,wrappedElement)
    	{
    	}
		 /// <summary>
		 /// The classifier that owns the behavior of which this action is a part.
		 /// </summary>
		 public UML.Classes.Kernel.Classifier context
		 {
        	get{throw new NotImplementedException();}
        	set{throw new NotImplementedException();} 
         }
        
        /// <summary>
        /// The ordered set of input pins connected to the Action. These are among the total set of inputs. {Specializes
        /// Element::ownedElement}
        /// </summary>
         public HashSet<UML.Actions.BasicActions.InputPin> input
         {
        	get{throw new NotImplementedException();}
        	set{throw new NotImplementedException();} 
         }
		
        /// <summary>
        ///The ordered set of output pins connected to the Action. The action places its results onto pins in this set.
        ///{Specializes Element::ownedElement}
        /// </summary>
		 public HashSet<UML.Actions.BasicActions.OutputPin> output
		 {
        	get{throw new NotImplementedException();}
        	set{throw new NotImplementedException();} 
         }
		 
		 public ActionKind kind
		 {
		 	get
		 	{
		 		//
		 	}
		 	set
		 	{
		 		
		 	}
		 }
		
		 
		

	}
}
