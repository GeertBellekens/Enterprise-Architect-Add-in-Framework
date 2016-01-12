using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{  
public class RelationTag : TaggedValue
{

	internal global::EA.ConnectorTag wrappedTaggedValue {get;set;}
	internal RelationTag(Model model, global::EA.ConnectorTag eaTag):base(model)
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
			return this.model.getRelationByID(this.wrappedTaggedValue.ConnectorID);
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
