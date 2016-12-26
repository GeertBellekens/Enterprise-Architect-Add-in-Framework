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
		public TaggedValueMapping(TaggedValue wrappedTaggedValue,string basePath,ElementWrapper targetRootElement):base((Element)wrappedTaggedValue.owner,wrappedTaggedValue.tagValue as Element,basePath,targetRootElement)
		{
			this.wrappedTaggedValue = wrappedTaggedValue;
		}
		public TaggedValueMapping(TaggedValue wrappedTaggedValue,string basePath,string targetBasePath):base((Element)wrappedTaggedValue.owner,wrappedTaggedValue.tagValue as Element,basePath,targetBasePath)
		{
			this.wrappedTaggedValue = wrappedTaggedValue;
		}
	}
}
