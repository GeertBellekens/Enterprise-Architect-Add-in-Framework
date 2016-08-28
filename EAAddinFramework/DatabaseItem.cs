using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework
{
	/// <summary>
	/// Description of DatabaseItem.
	/// </summary>
	public abstract class DatabaseItem
	{
		internal abstract Element wrappedElement {get;set;}
		internal abstract TaggedValue traceTaggedValue {get;set;}
		internal abstract void createTraceTaggedValue();
		public virtual bool isOverridden 
		{
			get 
			{
				return (this.traceTaggedValue != null 
				        && this.traceTaggedValue.comment.ToLower().Contains("dboverride=true"));
			}
			set
			{
				//create tagged value if needed if not overridden then we don't need the tagged value;
				if (value)
				{
					if (this.traceTaggedValue == null) 
					{
						createTraceTaggedValue();
					}
				}
				if (this.traceTaggedValue != null)
				{
					//set the value
					string goodString = "dboverride=" + value.ToString().ToLower();
					string replaceString = "dboverride=" + (!value).ToString().ToLower();
	
					if(this.traceTaggedValue.comment.ToLower().Contains(replaceString))
					{
						this.traceTaggedValue.comment = this.traceTaggedValue.comment.ToLower().Replace(replaceString,goodString);
					}
					else
					{
						this.traceTaggedValue.comment += goodString;
					}
				}
			}
		}
	}
}
