using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{  
public abstract class TaggedValue : UML.Profiles.TaggedValue 
{

	internal Model model { get;  set; }

    internal TaggedValue(Model model)
    {
      this.model = model;
    }

	
	public abstract string name { get;  set; }
	public object tagValue
	{
		get
		{
			UML.Classes.Kernel.Element tagElement = null;
			if (this.isGUID(this.eaStringValue))
			{
				//try to get the object using the guid
				tagElement = this.model.getElementByGUID(this.eaStringValue);
			}
			if (tagElement != null)
			{
				return tagElement;
			}else
			{
				return this.eaStringValue;
			}
				
		}
		set
		{
			if (value is string)
			{
				this.eaStringValue = (string)value;
			}
			else if (value is Element)
			{
				this.eaStringValue = ((Element)value).guid;
			}
			else
			{
				this.eaStringValue = value.ToString();
			}
		}
	}
	
	private bool isGUID(string guidString)
	{
		Guid dummy;
		return Guid.TryParse(guidString,out dummy);
	}
	public abstract string eaStringValue { get;  set; }
		
	
	public abstract UML.Classes.Kernel.Element owner { get;  set; }
	
	
	public void select()
	{
		this.owner.select();
	}
	
	public void open()
	{
		this.owner.open();
	}
	public abstract string ea_guid {get;}
		
	public string fqn 
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
	/// opens the properties dialog in EA. 
	/// For tagged values the properties dialog of the owner will be opened
	/// </summary>
	public void openProperties()
	{
		this.owner.openProperties();
	}
	
	public HashSet<UML.Profiles.Stereotype> stereotypes {
		get 
		{
			//tagged values never have stereotypes in EA
			// return an empty collection
			return new HashSet<UML.Profiles.Stereotype>();
		}
	}
	/// <summary>
	/// add the owner of the tagged value to the current diagram
	/// </summary>
	public void addToCurrentDiagram()
	{
		this.owner.addToCurrentDiagram();
	}
	/// <summary>
	/// select the owner of this tagged value in the current diagram
	/// </summary>
	public void selectInCurrentDiagram()
	{
		this.owner.selectInCurrentDiagram();
	}
	
	public abstract void save();
}
}
