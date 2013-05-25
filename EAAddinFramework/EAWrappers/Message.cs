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
			Diagram diagram = this.model.getDiagramByID(this.wrappedConnector.DiagramID);
			if (diagram != null)
			{
				diagram.open();
				//TODO add selection of message in the diagram
			}
		}
        public UML.Interactions.BasicInteractions.MessageSort messageSort
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
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
                        returnedOperation = this.model.getOperationByGUID(tag.Value);
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
            T diagram = this.model.getDiagramByID(this.wrappedConnector.DiagramID) as T;
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
    }
 }
