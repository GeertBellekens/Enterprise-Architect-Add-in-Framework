using System;
using System.Collections.Generic;
using System.Linq;

using EAAddinFramework.Utilities;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  /// The EAoperation wraps the EA.Method and is both an operation from Kernel 
  /// as an Operation from Interfaces
  public class Operation : Element, UML.Classes.Kernel.Operation {
    internal global::EA.Method wrappedOperation {get; set; }
    public int id
    {
    	get{return this.wrappedOperation.MethodID;}
    }

    public Operation(Model model, global::EA.Method wrappedOperation) 
      : base(model) 
    {
		this.wrappedOperation = wrappedOperation;
		this.isDirty = true;
    }
    public override string name 
	{
      get { return this.wrappedOperation.Name;   }
      set { this.wrappedOperation.Name = value; }
    }
	public override UML.Classes.Kernel.Package owningPackage {
		get {
			return base.owningPackage;
		}
		set {
			((ElementWrapper)this.owner).owningPackage = value;
		}
	}
    public global::EA.Method WrappedOperation {
    	get {
    		return wrappedOperation;
    	}
    }
    public void setStereotype(string stereotype)
    {
    	this.wrappedOperation.StereotypeEx = stereotype;
    }

	public Parameter addOwnedParameter(string name)
	{
		return this.EAModel.factory.createElement(this.wrappedOperation.Parameters.AddNew(name, "Parameter")) as Parameter;
	}
	public string code
	{
		get
		{
			return this.wrappedOperation.Code;
		}
		set
		{
			this.wrappedOperation.Code = value;
		}
	}
	/// <summary>
    /// return the unique ID of this element
    /// </summary>
	public override string uniqueID 
	{
		get 
		{
			return this.wrappedOperation.MethodGUID;
		}
	}	    
    public override HashSet<UML.Classes.Kernel.Element> ownedElements {
      get {
        return new HashSet<UML.Classes.Kernel.Element>
          (this.ownedParameters.Cast<UML.Classes.Kernel.Element>());
      }
      set { throw new NotImplementedException(); }
    }
    
	internal ElementWrapper _owner;
    public override UML.Classes.Kernel.Element owner 
    {
      get 
      {
      	if (_owner == null)
      	{
      		_owner = this.EAModel.getElementWrapperByID( this.wrappedOperation.ParentID);
      	}
      	return _owner;
      }
      set 
      {
      	throw new NotImplementedException();
      }
    }
    
    public override HashSet<UML.Profiles.Stereotype> stereotypes {
      get {
        return ((Factory)this.EAModel.factory).createStereotypes
          ( this, this.wrappedOperation.StereotypeEx );
      }
      set { throw new NotImplementedException(); }
    }
    
    public bool isQuery {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public bool isOrdered {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public bool isUnique {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public int lower {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.UnlimitedNatural upper {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.Type type {
      get {
        ParameterReturnType returnParameter =
          ((Factory)this.EAModel.factory).createEAParameterReturnType(this);
        return returnParameter != null ? returnParameter.type : null;
      }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Operation> redefinedOperations {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.Constraint bodyCondition {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Constraint> postcondition {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Constraint> precondition {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Type> raisedExceptions {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Parameter> ownedParameters {
      get {
        // get the regular parameters
        HashSet<UML.Classes.Kernel.Parameter> parameters = 
          new HashSet<UML.Classes.Kernel.Parameter>
          ( this.EAModel.factory.createElements
            (this.wrappedOperation.Parameters)
            .Cast<UML.Classes.Kernel.Parameter>() );
        // get the returntype
        ParameterReturnType returntype =
          ((Factory)this.EAModel.factory).createEAParameterReturnType(this);
        if( returntype != null ) {
          parameters.Add(returntype);
        }
        return parameters;
      }
      set { 
			//only implemented to remove all parameters
			if (value.Count > 0) throw new NotImplementedException();
			this.wrappedOperation.Parameters.Refresh();
			for (short i = (short)(this.wrappedOperation.Parameters.Count-1); i >= 0; i--)
			{
				this.wrappedOperation.Parameters.DeleteAt(i,false);
			}
			this.wrappedOperation.Parameters.Refresh();
		}
    }
    
    public bool isStatic {
      get { return this.wrappedOperation.IsStatic;  }
      set { this.wrappedOperation.IsStatic = value; }
    }
    
    public HashSet<UML.Classes.Kernel.Classifier> featuringClassifiers {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public bool isLeaf {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.RedefinableElement> redefinedElements {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public HashSet<UML.Classes.Kernel.Classifier> redefinitionContexts {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    

    
    public UML.Classes.Kernel.VisibilityKind visibility {
      get {
        return VisibilityKind.getUMLVisibilityKind
          ( this.wrappedOperation.Visibility, 
            UML.Classes.Kernel.VisibilityKind._public );
      }
      set { throw new NotImplementedException(); }
    }
    
    public String qualifiedName {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Kernel.Namespace owningNamespace {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public UML.Classes.Interfaces.Interface _interface {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Dependencies.Dependency> clientDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    
    public List<UML.Classes.Dependencies.Dependency> supplierDependencies {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
    

    /// <summary>
    /// A behavioral description that implements the behavioral feature. There may be at most one behavior for a particular
    /// pairing of a classifier (as owner of the behavior) and a behavioral feature (as specification of the behavior).
    /// </summary>
	public HashSet<UML.CommonBehaviors.BasicBehaviors.Behavior> methods
	{
		get
		{
			//in EA we will find only one behavior
			HashSet<UML.CommonBehaviors.BasicBehaviors.Behavior> behaviors = new HashSet<TSF.UmlToolingFramework.UML.CommonBehaviors.BasicBehaviors.Behavior>();
			// the Behavior property of an operation either contains some text, or a GUID of a Behavior element.
			// we try to find the element 
			EA.ElementWrapper behavior = this.EAModel.getElementWrapperByGUID (this.wrappedOperation.Behavior);
			if (behavior != null && behavior is UML.CommonBehaviors.BasicBehaviors.Behavior) behaviors.Add((UML.CommonBehaviors.BasicBehaviors.Behavior)behavior);
			return behaviors;
		}
    		
		set {throw new NotImplementedException(); }
	}
    
    internal override void saveElement(){
      this.wrappedOperation.Update();
    }

    public override String notes {
      get { return this.wrappedOperation.Notes;  }
      set { this.wrappedOperation.Notes = value; }
    }
    public bool isAbstract {
      get { return this.wrappedOperation.Abstract;  }
      set { this.wrappedOperation.Abstract = value; }
    }

	public override List<TSF.UmlToolingFramework.UML.Classes.Kernel.Relationship> relationships 
	{
		get 
		{ 
			string selectRelationsSQL = @"select c.Connector_ID from t_connector c
,t_operation o where o.ea_guid = '"+this.wrappedOperation.MethodGUID +@"' 
and c.StyleEx like '%LF_P="+this.wrappedOperation.MethodGUID+"%'"
+@" and ((c.Start_Object_ID = o.Object_ID and c.End_Object_ID <> o.Object_ID)
    or (c.Start_Object_ID <> o.Object_ID and c.End_Object_ID = o.Object_ID))";
			return this.EAModel.getRelationsByQuery(selectRelationsSQL).Cast<UML.Classes.Kernel.Relationship>().ToList();
		}
		set { base.relationships = value; }
	}
    public HashSet<UML.Interactions.BasicInteractions.Message> getCallingMessages()
    {
          //this one we will try to find with a cunning sql query directly
            //sometimes the guid of an operation is not the same as the guid mentioned in the tag
            //to be sure the try to get both.
            string sqlCallingMessages =
                @"select c.Connector_ID  from ((t_connector c 
                inner join t_connectortag ct on ct.ElementID = c.Connector_ID) 
                inner join t_operation o on ct.[VALUE] = o.ea_guid) 
                where c.Connector_Type in ('Sequence','Collaboration')
                and ct.Property = 'operation_guid' 
                and ct.[VALUE] = '" + this.wrappedOperation.MethodGUID + @"'
                Union 
                select c.Connector_ID  from ((t_connector c 
                inner join t_connectortag ct on ct.ElementID = c.Connector_ID) 
                inner join t_operationTag ot on ct.[VALUE] = ot.[VALUE])
                where c.Connector_Type in ('Sequence','Collaboration') 
                and ct.Property = 'operation_guid' 
                and ot.ElementID = " + this.wrappedOperation.MethodID;
           // return this.model.getRelationsFromQuery(sqlQuery).Cast<UMLMessage>().ToList();
            HashSet<UML.Interactions.BasicInteractions.Message> returnedMessages = new HashSet<UML.Interactions.BasicInteractions.Message>();
           foreach (Message message in this.EAModel.getRelationsByQuery(sqlCallingMessages).Cast<Message>().ToList())
           {
               if (!returnedMessages.Contains(message))
               {
                   returnedMessages.Add(message);
               }
           }
           return returnedMessages;
          
    }
    /// <summary>
    /// returns all CallOperationActions that call this operation
    /// </summary>
    /// <returns>all CallOperationActions that call this operation</returns>
    public HashSet<UML.Actions.BasicActions.CallOperationAction> getDependentCallOperationActions()
    {
    	string sqlCallOperationActions = 
    		@"SELECT a.Object_ID FROM t_operation op 
			inner join t_object a on op.ea_guid = a.Classifier_guid
			where op.OperationID = " +this.wrappedOperation.MethodID;
    	HashSet<UML.Actions.BasicActions.CallOperationAction> callOperationActions = new HashSet<UML.Actions.BasicActions.CallOperationAction>();
    	foreach (ElementWrapper callOperationAction in this.EAModel.getElementWrappersByQuery(sqlCallOperationActions)) 
    	{
    		if (callOperationAction is UML.Actions.BasicActions.CallOperationAction)
    		{
    			callOperationActions.Add((UML.Actions.BasicActions.CallOperationAction)callOperationAction);
    		}
    	}
    	return callOperationActions;
    }
    /// <summary>
    /// returns all diagrams that somehow use this operation
    /// </summary>
    /// <returns>all diagrams that use this operation</returns>
    public override HashSet<T> getUsingDiagrams<T>()
    {
        HashSet<T> diagrams = new HashSet<T>();
        //sequence diagrams that contain a message calling this operation
        foreach (Message message in this.getCallingMessages())
        {
            foreach (T diagram in message.getUsingDiagrams<T>())
            {
                if (!diagrams.Contains(diagram))
                {
                    diagrams.Add(diagram);
                }
            }
        }
        //Activity diagrams containing a CallOperationAction calling this operation
        foreach (CallOperationAction action in this.getDependentCallOperationActions()) 
        {
        	foreach (T diagram in  action.getUsingDiagrams<T>()) 
        	{
        		if (!diagrams.Contains(diagram))
                {
                    diagrams.Add(diagram);
                }
        	}
        	
        }
        return diagrams;
    }
  	
	public HashSet<TSF.UmlToolingFramework.UML.Diagrams.Diagram> ownedDiagrams {
		get {
			throw new NotImplementedException();
		}
		set {
			throw new NotImplementedException();
		}
	}
  	
	public override TSF.UmlToolingFramework.UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
	{
		UML.Extended.UMLItem item = null;
		List<string> filteredPath = new List<string>(relativePath);
		if (ElementWrapper.filterName(filteredPath,this.name))
		{
	    	if (filteredPath.Count ==1)
	    	{
	    		item = this;
	    	}
		}
		return item; 
	}
	
	#region Equals and GetHashCode implementation
	public override bool Equals(object obj)
	{
		Operation other = obj as Operation;
		if (other != null)
		{
			if (other.wrappedOperation.MethodGUID == this.wrappedOperation.MethodGUID)
			{
				return true;	
			}
		}
		return false;
	}
	
	public override int GetHashCode()
	{
		return new Guid(this.wrappedOperation.MethodGUID).GetHashCode();
	}
	
	#endregion
	
  	
  	
	internal override global::EA.Collection eaTaggedValuesCollection {
		get 
		{
			return this.WrappedOperation.TaggedValues;
		}
	}
  	
	public override string guid {
		get 
		{
			return this.WrappedOperation.MethodGUID;
		}
	}

		#region implemented abstract members of Element

	public override void deleteOwnedElement(Element ownedElement)
	{
		throw new NotImplementedException();
	}

	#endregion

		#region implemented abstract members of Element

	public override bool makeWritable(bool overrideLocks)
	{
		return this.owner.makeWritable(overrideLocks);
	}

	public override string getLockedUser()
	{
		return ((Element)this.owner).getLockedUser();
	}

	public override string getLockedUserID()
	{
		return ((Element)this.owner).getLockedUserID();
	}
	protected override string getTaggedValueQuery(string taggedValueName)
	{
		return @"select tv.ea_guid from t_operationtag tv
			where 
			tv.Property = '"+taggedValueName+"' and tv.ElementID = "+ this.id;
	}

	#endregion
	public void exportAllDiagrams(string imagePath)
	{
		// do nothing as operations don't have diagram
	}
  }
}
