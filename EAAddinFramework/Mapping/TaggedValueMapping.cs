using System;
using MP = MappingFramework;
using UML=TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Collections.Generic;
using System.Linq;

namespace EAAddinFramework.Mapping
{
	/// <summary>
	/// Description of TaggedValueMapping.
	/// </summary>
	public class TaggedValueMapping:Mapping
	{
		internal  TaggedValue wrappedTaggedValue{get;private set;}
		public TaggedValueMapping(TaggedValue wrappedTaggedValue,string basePath):base((Element)wrappedTaggedValue.owner,wrappedTaggedValue.tagValue as Element,basePath)
		{
			this.wrappedTaggedValue = wrappedTaggedValue;
		}
	}
}
