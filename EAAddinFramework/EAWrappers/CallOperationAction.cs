
using System;
using System.Collections.Generic;
using System.Xml;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
	/// <summary>
	/// CallOperationAction is an action that transmits an operation call request to the target object, where it may cause the
    /// invocation of associated behavior. The argument values of the action are available to the execution of the invoked
    /// behavior. If the action is marked synchronous, the execution of the call operation action waits until the execution of the
    /// invoked behavior completes and a reply transmission is returned to the caller; otherwise, execution of the action is
    /// complete when the invocation of the operation is established and the execution of the invoked operation proceeds
    /// concurrently with the execution of the calling behavior. Any values returned as part of the reply transmission are put on
    /// the result output pins of the call operation action. Upon receipt of the reply transmission, execution of the call operation
    /// action is complete.
	/// </summary>
	public class CallOperationAction: Action,UML.Actions.BasicActions.CallOperationAction
	{
		/// <summary>
		/// constructor, no special treatment
		/// </summary>
		/// <param name="model">the model</param>
		/// <param name="wrappedElement">the EA.Element to wrap</param>
		public CallOperationAction(Model model, global::EA.Element wrappedElement) 
      : base(model,wrappedElement)
    	{
    	}
		
        /// <summary>
        /// The operation to be invoked by the action execution.
        /// </summary>
        public UML.Classes.Kernel.Operation operation { 
        	get{
        		// first get the operations guid which is stored in the Classifier_guid column
        		XmlDocument operationGUIDxml = this.EAModel.SQLQuery(@"select o.Classifier_guid from t_object o
									where o.Object_ID = " + this.id.ToString());
        		XmlNode operationGUIDNode = operationGUIDxml.SelectSingleNode(this.EAModel.formatXPath("//Classifier_guid"));
        	    return this.EAModel.getOperationByGUID(operationGUIDNode.InnerText);
        	}
        	set{
        		// no API method available, so we need to update the database directly
//        		this.model.executeSQL(@"update t_object
//        								set Classifier_guid = "+ ((Operation)value).GUID
//        		                      + "where Object_ID = " + this.id.ToString();
				//TODO add GUID property to Operation
				throw new NotImplementedException();
        		}
        }

        
        /// <summary>
        /// The target object to which the request is sent. The classifier of the target object is used to dynamically determine a
        /// behavior to invoke. This object constitutes the context of the execution of the operation. {Subsets Action::input} 
        /// </summary>
       public UML.Actions.BasicActions.InputPin target{ 
        	get{throw new NotImplementedException();} 
        	set{throw new NotImplementedException();} 
        }
            
        /// <summary>
        /// If true, the call is synchronous and the caller waits for completion of the invoked behavior. If false, the call is
        /// asynchronous and the caller proceeds immediately and does not expect a return value.
        /// </summary>
		public bool isSynchronous { 
        	get{throw new NotImplementedException();} 
        	set{throw new NotImplementedException();} 
        }

        
        /// <summary>
        /// A list of output pins where the results of performing the invocation are placed. {Subsets Action::input} 
        /// </summary>
		public HashSet<UML.Actions.BasicActions.OutputPin> result{ 
        	get{throw new NotImplementedException();} 
        	set{throw new NotImplementedException();} 
        }
        
        
        /// <summary>
        /// Specification of the ordered set of argument values that appear during execution. 
        /// </summary>
		public HashSet<UML.Actions.BasicActions.InputPin> arguments{ 
        	get{throw new NotImplementedException();} 
        	set{throw new NotImplementedException();} 
        }
	}
}
