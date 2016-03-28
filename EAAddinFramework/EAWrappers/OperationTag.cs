using System;
using System.Collections.Generic;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{  
public class OperationTag : TaggedValue
{

	internal global::EA.MethodTag wrappedTaggedValue {get;set;}
	internal OperationTag(Model model, global::EA.MethodTag eaTag):base(model)
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
			return this.wrappedTaggedValue.TagGUID;
		}
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
	
	public override UML.Classes.Kernel.Element owner {
		get 
		{
			return this.model.getOperationByID(this.wrappedTaggedValue.MethodID);
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
