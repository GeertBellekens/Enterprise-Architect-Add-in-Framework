using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{  
public class ParameterTag : TaggedValue
{

	internal global::EA.ParamTag wrappedTaggedValue {get;set;}
	internal ParameterTag(Model model, global::EA.ParamTag eaTag):base(model)
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
			this.wrappedTaggedValue.Value = value;
		}
	}
	
	public override string name 
	{
		get 
		{
			return this.wrappedTaggedValue.Tag;
		}
		set 
		{
			throw new NotImplementedException();
		}
	}
	
	public override UML.Classes.Kernel.Element owner {
		get 
		{
			return this.model.getParameterByGUID(this.wrappedTaggedValue.ElementGUID);
		}
		set {
			throw new NotImplementedException();
		}
	}
	
	public override string ea_guid 
	{
		get 
		{
			return this.wrappedTaggedValue.PropertyGUID;
		}
	}
	
	public override void save()
	{
		this.wrappedTaggedValue.Update();
	}
}
}
