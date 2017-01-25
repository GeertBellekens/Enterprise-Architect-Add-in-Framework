using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of AttributeWrapper.
	/// </summary>
	public class AttributeWrapper :Element
	{
		private UML.Classes.Kernel.Type _type;
		internal global::EA.Attribute wrappedAttribute { get; set; }
	    public int id
	    {
	    	get{return this.wrappedAttribute.AttributeID;}
	    }

	    public AttributeWrapper(Model model, global::EA.Attribute wrappedAttribute) 
	      : base(model)
	    {
	      this.wrappedAttribute = wrappedAttribute;
	      var dummy = this.type; //make sure we get the type here to avoid multithreading errors
	    }
	
	    public global::EA.Attribute WrappedAttribute
	    {
	    	get { return wrappedAttribute; }
	    }
	   	public string alias
		{
			get{return this.wrappedAttribute.Alias;}
			set{this.wrappedAttribute.Alias = value;}
		}
		public override int position 
		{
			get 
			{
				return this.wrappedAttribute.Pos;
			}
			set 
			{
				this.wrappedAttribute.Pos = value;
			}
		}
	    	
		/// <summary>
	    /// return the unique ID of this element
	    /// </summary>
		public override string uniqueID 
		{
			get 
			{
				return this.wrappedAttribute.AttributeGUID;
			}
		}	
		public override string name 
		{
	      	get { return this.wrappedAttribute.Name;  }
	      	set { this.wrappedAttribute.Name = value; }
    	}
		public UML.Classes.Kernel.Type type 
	    {
	      	get 
	      	{
	    		if (this._type == null)
	    		{
			        this._type = this.model.getElementWrapperByID( this.wrappedAttribute.ClassifierID ) as UML.Classes.Kernel.Type;
			        // check if the type is defined as an element in the model.
			        if(this._type == null ) 
			        {
			          // no element, create primitive type based on the name of the type
			          this._type = this.model.factory.createPrimitiveType(this.wrappedAttribute.Type);
			        }
	    		}
	        	return this._type;
	      	}
	      	set 
	      	{
	      		this._type = value;
		    	if (value != null)
		    	{
		    		//set classifier if needed
		    		ElementWrapper elementWrapper = value as ElementWrapper;
			    	if( elementWrapper != null) 
		    		{
			          this.wrappedAttribute.ClassifierID = ((ElementWrapper)value).id;
			        }
		    	   	//always set type field
			        this.wrappedAttribute.Type = value.name;
		    	}
	
	      	}
	    }
		public override HashSet<UML.Classes.Kernel.Element> ownedElements {
	      get { return new HashSet<UML.Classes.Kernel.Element>(); }
	      set { throw new NotImplementedException(); }
	    }
	
	    
		internal Element _owner;
	    public override UML.Classes.Kernel.Element owner 
	    {
	      get 
	      {
				if (_owner == null)
				{
					_owner = this.model.getElementWrapperByID(this.wrappedAttribute.ParentID);
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
	        return ((Factory)this.model.factory).createStereotypes
	          ( this, this.wrappedAttribute.StereotypeEx );
	      }
	      set 
	      {
	      	this.wrappedAttribute.StereotypeEx = Stereotype.getStereotypeEx(value);
	      }
	    }
		internal override void saveElement(){
	      this.wrappedAttribute.Update();
	    }
	
	    public override String notes {
	      get { return this.wrappedAttribute.Notes;  }
	      set { this.wrappedAttribute.Notes = value; }
	    }
		public override TSF.UmlToolingFramework.UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath)
		{
			UML.Extended.UMLItem item = null;
			List<string> filteredPath = new List<string>(relativePath);
			if (ElementWrapper.filterName( filteredPath,this.name))
			{
		    	if (filteredPath.Count ==1)
		    	{
		    		item = this;
		    	}
			}
			return item; 
		}
		
		public override HashSet<UML.Profiles.TaggedValue> taggedValues
		{
			get 
			{
				//make sure we have an up-to date collection
				this.wrappedAttribute.TaggedValues.Refresh();
				return new HashSet<UML.Profiles.TaggedValue>(this.model.factory.createTaggedValues(this.wrappedAttribute.TaggedValues));
			}
			set { throw new NotImplementedException();}
		}
		public override HashSet<TSF.UmlToolingFramework.UML.Profiles.TaggedValue> getReferencingTaggedValues()
		{
			return this.model.getTaggedValuesWithValue(this.wrappedAttribute.AttributeGUID);
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			var other = obj as AttributeWrapper;
			if (other != null)
			{
				if (other.wrappedAttribute.AttributeGUID == this.wrappedAttribute.AttributeGUID)
				{
					return true;	
				}
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return new Guid(this.wrappedAttribute.AttributeGUID).GetHashCode();
		}
		#endregion
	
	  	
		internal override global::EA.Collection eaTaggedValuesCollection {
			get {
				return this.WrappedAttribute.TaggedValues;
			}
		}
	  	
		public override string guid {
			get 
			{
				return this.WrappedAttribute.AttributeGUID;
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
		#endregion
		public override List<UML.Extended.UMLItem> findOwnedItems(List<string> descriptionParts)
		{
			//return the owned items of the type of the attribute
			var ownedItems = new List<UML.Extended.UMLItem>();
			var elementType = this.type as Element;
			if (elementType != null)
			{
				ownedItems.AddRange(((Element)this.type).findOwnedItems(descriptionParts));
			}
			return ownedItems;
		}
	}
}
