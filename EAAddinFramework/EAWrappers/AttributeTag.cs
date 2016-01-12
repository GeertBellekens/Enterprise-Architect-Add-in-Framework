using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{  
public class AttributeTag : TaggedValue
{

	internal global::EA.AttributeTag wrappedTaggedValue {get;set;}
	internal AttributeTag(Model model, global::EA.AttributeTag eaTag):base(model)
    {
      this.wrappedTaggedValue = eaTag;
    }

	
	public override string eaStringValue 
	{
		get 
		{
			return this.wrappedTaggedValue.Value;
		}
		set 
		{
			this.wrappedTaggedValue.Value = value;
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
	
	public override TSF.UmlToolingFramework.UML.Classes.Kernel.Element owner {
		get 
		{
			return this.model.getAttributeByID(this.wrappedTaggedValue.AttributeID);
		}
		set {
			throw new NotImplementedException();
		}
	}
	
	public override string ea_guid 
	{
		get 
		{
			return this.wrappedTaggedValue.TagGUID;
		}
	}
	
	public override void save()
	{
		this.wrappedTaggedValue.Update();
	}
}
}
