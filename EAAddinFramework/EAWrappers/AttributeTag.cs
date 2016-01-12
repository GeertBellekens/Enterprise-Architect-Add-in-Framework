using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{  
public class AttributeTag : TaggedValue
{

	internal global::EA.AttributeTag wrappedAttributeTag {get;set;}
	internal AttributeTag(Model model, global::EA.AttributeTag eaTag):base(model)
    {
      this.wrappedAttributeTag = eaTag;
    }

	
	public override string eaStringValue 
	{
		get 
		{
			return this.wrappedAttributeTag.Value;
		}
		set {
			throw new NotImplementedException();
		}
	}
	
	public override string name 
	{
		get 
		{
			return this.wrappedAttributeTag.Name;
		}
		set 
		{
			throw new NotImplementedException();
		}
	}
	
	public override TSF.UmlToolingFramework.UML.Classes.Kernel.Element owner {
		get 
		{
			return this.model.getAttributeByID(this.wrappedAttributeTag.AttributeID);
		}
		set {
			throw new NotImplementedException();
		}
	}
	
	public override string ea_guid 
	{
		get 
		{
			return this.wrappedAttributeTag.TagGUID;
		}
	}
	
	public override void save()
	{
		this.wrappedAttributeTag.Update();
	}
}
}
