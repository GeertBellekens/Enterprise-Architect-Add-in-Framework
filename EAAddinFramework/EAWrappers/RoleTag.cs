using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{  
public class RoleTag : TaggedValue
{

	internal global::EA.RoleTag wrappedTaggedValue {get;set;}
	internal RoleTag(Model model, global::EA.RoleTag eaTag):base(model)
    {
      this.wrappedTaggedValue = eaTag;
    }

	/// <summary>
    /// return the unique ID of this element
    /// </summary>
	public override string uniqueID 
	{
		get 
		{
			return this.wrappedTaggedValue.PropertyGUID;
		}
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
	public override string comment {
		get {
			//apparently not no notes implemented in EA.RoleTag
			return string.Empty;
		}
		set {
			//do nothing
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
			var owningRelation = this.model.getRelationByGUID(this.wrappedTaggedValue.ElementGUID) as ConnectorWrapper;
			if (this.wrappedTaggedValue.BaseClass == "ASSOCIATION_TARGET")
			{
				return owningRelation.targetEnd;
			}
			else
			{
				return owningRelation.sourceEnd;
			}
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
