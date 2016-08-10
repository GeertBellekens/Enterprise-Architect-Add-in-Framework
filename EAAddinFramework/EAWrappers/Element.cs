using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA {
  public abstract class Element : UML.Classes.Kernel.Element {
    public Model model { get;  internal set; }

    internal Element(Model model){
      this.model = model;
    }
    internal abstract global::EA.Collection eaTaggedValuesCollection {get;}
    public abstract String notes { get; set; }
    public abstract HashSet<UML.Classes.Kernel.Element> ownedElements 
      { get; set; }
    
    public virtual HashSet<UML.Classes.Kernel.Comment> ownedComments
      { 
    	get
    	{
    		HashSet<UML.Classes.Kernel.Comment> comments = new HashSet<TSF.UmlToolingFramework.UML.Classes.Kernel.Comment>();
    		comments.Add(((Factory)this.model.factory).createDescriptionComment(this));
    		return comments;
    	}
    	set
    	{
    		//first clear the notes
    		string newNotes = string.Empty;
    		foreach (DescriptionComment comment in value) 
    		{
    			newNotes += comment.body;
    		}
    		this.notes = newNotes;
    	}
    }
    public abstract UML.Classes.Kernel.Element owner 
      { get; set; }
    public abstract HashSet<UML.Profiles.Stereotype> stereotypes 
      { get; set; }
    public void addStereotype(UML.Profiles.Stereotype stereotype) {
      HashSet<UML.Profiles.Stereotype> newStereotypes = 
        new HashSet<UML.Profiles.Stereotype>(this.stereotypes);
      if (!newStereotypes.Contains(stereotype)) {
        newStereotypes.Add(stereotype);
        this.stereotypes = newStereotypes;
      }
    }

	public virtual string uniqueID 
	{
		get 
		{
			return null;
		}
	}

    public List<string> stereotypeNames 
    {
    	get
    	{
    		var returnedNames = new List<string>();
    		foreach (UML.Profiles.Stereotype stereotype in this.stereotypes) 
    		{
    			returnedNames.Add(stereotype.name);
    		}
    		return returnedNames;
    	}
    }
    public abstract string guid {get;}
	
	/// <summary>
	/// returns a list of diagrams that show this item.
	/// Default implementation on this level is an empty list.
	/// To be overridden by concrete subclasses
	/// </summary>
	/// <returns>all diagrams that show this item</returns>
	public virtual List<UML.Diagrams.Diagram> getDependentDiagrams()
	{
		return new List<UML.Diagrams.Diagram>();
	}

    /// returns the owner of the given type
    /// This operation will keep on looking upwards through the owners until 
    /// it finds one with the given type.
    /// NON UML
    public T getOwner<T>() where T : UML.Classes.Kernel.Element {
      if (this.owner is T || this.owner == null) {
        return (T) this.owner;
      }else {
        return ((Element)this.owner).getOwner<T>();
      }
    }

    /// default implementation returns an empty list because there is only one
    /// subclass that can actually implement this operation: ElementWrapper.
    public virtual List<UML.Classes.Kernel.Relationship> relationships {
      get { return new List<UML.Classes.Kernel.Relationship>(); }
      set { /* do nothing */ }
    }

    /// default implementation returns an empty list because there is only one
    /// subclass that can actually implement this operation: EAElementWrapper.
    public virtual List<T> getRelationships<T>() 
      where T : UML.Classes.Kernel.Relationship 
    {
      return new List<T>();
    }


    internal abstract void saveElement();

    public virtual void save(){
      this.saveElement();
      foreach (UML.Classes.Kernel.Element element in this.ownedElements) {
        ((Element)element).save();
      }
    }
	//default empty implemenation
	public virtual UML.Diagrams.Diagram compositeDiagram 
	{
		get {return null;}
		set {}//do absolutely nothing
	}
	
    //default not implemented
    public virtual HashSet<T> getUsingDiagrams<T>() where T : class, UML.Diagrams.Diagram
    {
        throw new NotImplementedException();
    }
  	
    /// <summary>
    /// selects the element. 
    /// </summary>
	public virtual void select()
	{
		this.model.selectedElement = this;
	}
  	/// <summary>
  	/// opens the element. 
  	/// </summary>
	public virtual void open()
	{
		this.model.selectedElement = this;
	}

  	
	public abstract TSF.UmlToolingFramework.UML.Extended.UMLItem getItemFromRelativePath(List<string> relativePath);
	
	public virtual string name 
	{
		get 
		{
			return string.Empty;
		}
		set{}
	}
  	/// <summary>
  	/// default empty implementation
  	/// </summary>
	public virtual HashSet<UML.Profiles.TaggedValue> taggedValues {
		get {
			return new HashSet<UML.Profiles.TaggedValue>();
		}
		set {
			//do nothing
		}
	}
  	/// <summary>
  	/// default empty implementation
  	/// </summary>
  	/// <returns>empty set</returns>
	public virtual HashSet<UML.Profiles.TaggedValue> getReferencingTaggedValues()
	{
		return new HashSet<UML.Profiles.TaggedValue>();
	}
	
	public virtual string fqn 
	{
		get 
		{
			string nodepath = string.Empty;
			if (this.owner != null)
			{
				nodepath = this.owner.fqn;
			}
			if (this.name.Length > 0)
			{
				if (nodepath.Length > 0) 
				{
					nodepath = nodepath + ".";
				}
				nodepath = nodepath + this.name;
			}			
			return nodepath;
		}
	}
	/// <summary>
	/// opens the (standard) properties dialog in EA
	/// </summary>
	public virtual void openProperties()
	{
		this.model.openProperties(this);
	}
  	/// <summary>
  	/// adds this element to the currently opened diagram
  	/// </summary>
	public virtual void addToCurrentDiagram()
	{
		UML.Diagrams.Diagram currentDiagram = this.model.currentDiagram;
		if (currentDiagram != null)
		{
			currentDiagram.addToDiagram(this);
		}
	}
  	/// <summary>
  	/// selects this element in the current diagram
  	/// </summary>
	public void selectInCurrentDiagram()
	{
		UML.Diagrams.Diagram currentDiagram = this.model.currentDiagram;
		if (currentDiagram != null)
		{
			currentDiagram.selectItem(this);
		}
	}
	/// <summary>
	/// returns the name as ToString
	/// </summary>
	/// <returns>the name as ToString</returns>
	public override string ToString()
	{
		return this.name;
	}
	/// <summary>
	/// adds a new tagged value to the element with the given name and value
	/// if a tagged value with that name already exists the value of the existing tagged value is updated./
	/// </summary>
	/// <param name="name">the name fo the tagged value to add</param>
	/// <param name="tagValue">the value of the tagged value</param>
	/// <param name = "addDuplicate"></param>
	/// <returns>the added (or updated) tagged value</returns>
		
	public virtual TaggedValue addTaggedValue(string name, string tagValue, bool addDuplicate = false)
	{
		TaggedValue newTaggedValue = null;
		if (! addDuplicate)
		{
			//we don't wan't any duplicates so we get the existing one
			newTaggedValue = this.getTaggedValue(name);
		}
		if (newTaggedValue == null) 
		{
			//no existing tagged value found, or we need to create duplicates
			newTaggedValue = (TaggedValue)this.model.factory.createNewTaggedValue(this,name);
		}
		newTaggedValue.tagValue = tagValue;
		newTaggedValue.save();
		return newTaggedValue;
	}
	/// <summary>
	/// copies the tagged values from the source as tagged values of this element
	/// if a tagged value already exists the value is copied. Else a new tagged value is created
	/// </summary>
	/// <param name="sourceElement">the element to copy the tagged values from</param>
	public virtual void copyTaggedValues(Element sourceElement)
	{
		foreach (TaggedValue taggedValue in sourceElement.taggedValues) 
		{
			this.addTaggedValue(taggedValue.name, taggedValue.eaStringValue);
			
		}
	}
	/// <summary>
	/// returns the tagged value with the given name
	/// if a tagged valeu with the given name doesn't exist null is returned
	/// </summary>
	/// <param name="name">the name of the taggev value to return</param>
	/// <returns>the tagged value with the given name</returns>
	public virtual TaggedValue getTaggedValue(string name)
	{
		foreach (TaggedValue taggedValue in this.taggedValues) 
		{
			if (taggedValue.name.Equals(name,StringComparison.InvariantCultureIgnoreCase))
			{
				return taggedValue;
			}
		}
		return null;
	}
	/// <summary>
	/// deletes this element from the model
	/// In Enterprise Architect we can only delete an element by removing it from its parents collection.
	/// </summary>
	public virtual void delete()
	{
		((Element)this.owner).deleteOwnedElement(this);
	}
	public abstract void deleteOwnedElement(Element ownedElement);
	public UML.Classes.Kernel.Package owningPackage 
	{
		get
		{
			var ownerPackage = this.owner as Package;
			return ownerPackage ?? ownerPackage.owningPackage;
				
		}
		set
		{
			throw new NotImplementedException();
		}
	}
  }
}
