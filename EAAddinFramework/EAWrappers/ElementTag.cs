using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{  
public class ElementTag : TaggedValue
{

	internal global::EA.TaggedValue wrappedTaggedValue {get;set;}
	internal ElementTag(Model model, global::EA.TaggedValue eaTag):base(model)
    {
      this.wrappedTaggedValue = eaTag;
    }

	
	public override string eaStringValue 
	{
		get 
		{
			return this.wrappedTaggedValue.Value;
		}
		set {
			throw new NotImplementedException();
		}
	}
	
	public override string name 
	{
		get 
		{
			return this.wrappedTaggedValue.Name;
		}
		set 
		{
			throw new NotImplementedException();
		}
	}
	
	public override UML.Classes.Kernel.Element owner {
		get 
		{
			return this.model.getElementWrapperByID(this.wrappedTaggedValue.ElementID);
		}
		set {
			throw new NotImplementedException();
		}
	}
}
}
