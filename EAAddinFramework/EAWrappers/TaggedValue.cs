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
			throw new NotImplementedException();
		}
	}
	
	private bool isGUID(string guidString)
	{
		Guid dummy;
		return Guid.TryParse(guidString,out dummy);
	}
	public abstract string eaStringValue { get;  set; }
		
	
	public abstract UML.Classes.Kernel.Element owner { get;  set; }
	
}
}
