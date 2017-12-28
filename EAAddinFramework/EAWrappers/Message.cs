using System;
using System.Collections.Generic;
using System.Linq;  

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  
    //Wraps an EA connector of type "message" and implements the Message interface 
    public class Message : ConnectorWrapper, UML.Interactions.BasicInteractions.Message{
        
        //creates a new Message based on the given EA.connector of type message
        public Message(Model model, global::EA.Connector message)
          : base(model, message)
        {}
        
        //EA only knows messages of kind Complete
        public UML.Interactions.BasicInteractions.MessageKind  messageKind
        {
        	  get 
        	{
                return UML.Interactions.BasicInteractions.MessageKind.complete;
        	}
        	  set 
        	{ 
        		// do nothing
        	}
        }
		public override void open()
		{
			Diagram diagram = this.EAModel.getDiagramByID(this.wrappedConnector.DiagramID);
			if (diagram != null)
			{
				diagram.open();
				//TODO add selection of message in the diagram
			}
		}
        public UML.Interactions.BasicInteractions.MessageSort messageSort
        {
            get 
            {
            	//only implemented for synchronous/asyncronous calls
            	string pdata1 = this.wrappedConnector.MiscData[0].ToString();
            	return pdata1.Equals("Synchronous",StringComparison.InvariantCultureIgnoreCase) ? 
            		UML.Interactions.BasicInteractions.MessageSort.synchCall : UML.Interactions.BasicInteractions.MessageSort.asynchCall;
        	}
            set 
            {
            	//only implemented to make a difference between Synchronous and Asynchronous.
            	//property is read-only in the API so we have to use an SQL update to set it.
            	string pdata1;
            	string pdata3;
            	pdata3 = value == UML.Interactions.BasicInteractions.MessageSort.asynchSignal ? "Signal" : "Call";
            	if (value == UML.Interactions.BasicInteractions.MessageSort.asynchCall
            	    || value == UML.Interactions.BasicInteractions.MessageSort.asynchSignal)
            	{
            		pdata1 = "Asynchronous";
            	}
            	else
            	{
            		pdata1 = "Synchronous";
            	}
            	string sqlUpdatePdata1 = "update t_connector set PDATA1 = '"+pdata1+"' ,PDATA3 = '"+pdata3+"'  ,PDATA4 ='0' "
            							+" where ea_guid = '"+this.uniqueID+"'";
            	this.EAModel.executeSQL(sqlUpdatePdata1);
            }
        }

        public UML.Interactions.BasicInteractions.Interaction interaction
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public UML.Interactions.BasicInteractions.MessageEnd  receiveEvent{
        	  get {throw new NotImplementedException();}
        	  set {throw new NotImplementedException();}
        }

        public UML.Interactions.BasicInteractions.MessageEnd sendEvent
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public UML.CompositeStructures.InternalStructures.Connector connector
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public HashSet<UML.Classes.Kernel.ValueSpecification> arguments
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        //derived property, returns the sendEvent if present, otherwise the receiveEvent
        public UML.Classes.Kernel.NamedElement  signature
        {
        	  get 
        	{ 
        		if (this.messageKind == UML.Interactions.BasicInteractions.MessageKind.lost){
                    return this.sendEvent;
                }else{
                    return this.receiveEvent;
                }
        	}
            
        	  set 
        	{ 
        		if (this.messageKind == UML.Interactions.BasicInteractions.MessageKind.lost){
                    this.sendEvent = value as UML.Interactions.BasicInteractions.MessageEnd;
                }else{
                    this.receiveEvent = value as UML.Interactions.BasicInteractions.MessageEnd;;
                }
        	}
        }


        public UML.Classes.Kernel.Operation calledOperation
        {
            get
            {
                UML.Classes.Kernel.Operation returnedOperation = null;
                foreach (global::EA.ConnectorTag tag in this.wrappedConnector.TaggedValues)
                {
                    if (tag.Name == "operation_guid")
                    {
                        returnedOperation = this.EAModel.getOperationByGUID(tag.Value);
                    }
                }
                return returnedOperation;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        
        public override HashSet<T> getUsingDiagrams<T>()
        {
            //messages only occur on sequence diagrams
            HashSet<T> diagrams = new HashSet<T>();
            T diagram = this.EAModel.getDiagramByID(this.wrappedConnector.DiagramID) as T;
            if (null != diagram)
                diagrams.Add(diagram);

            return diagrams;
        }
        /// <summary>
        /// does nothing
        /// </summary>
		public override void addToCurrentDiagram()
		{
			//do nothing. An existing message cannot be added to a diagram in EA
			//because in EA messages are bound to a diagram.
		}
		/// <summary>
		/// the sequence number of this message within the Interaction.
		/// </summary>
		public int sequence 
		{
			get
			{
				return this.WrappedConnector.SequenceNo;
			}
			set
			{
				this.WrappedConnector.SequenceNo = value;
			}
		}
		public int y 
		{
			get
			{
				return this.wrappedConnector.StartPointY;
			}
			set
			{
				this.WrappedConnector.StartPointY = value;
				this.WrappedConnector.EndPointY = value;
			}
		}
    }
 }
