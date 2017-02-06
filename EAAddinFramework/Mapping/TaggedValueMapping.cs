using System;
using EAAddinFramework.Utilities;
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
		public TaggedValueMapping(Element source, Element target,string sourcePath,string targetPath,MappingSettings settings):base(source,target,sourcePath,targetPath)
		{
			string tagName = settings.linkedAttributeTagName;
			if (target is ConnectorWrapper)
			{
				tagName = settings.linkedAssociationTagName;
			}
			string tagComments =string.Empty;
			if (!string.IsNullOrEmpty(sourcePath)) tagComments = 
				KeyValuePairsHelper.setValueForKey("mappingSourcePath", this.stripPathFromElementName(source,sourcePath),tagComments);
			if (!string.IsNullOrEmpty(targetPath)) tagComments = 
				KeyValuePairsHelper.setValueForKey("mappingTargetPath", this.stripPathFromElementName(target, targetPath),tagComments);
			this.wrappedTaggedValue = source.addTaggedValue(tagName,target.uniqueID,tagComments,true);
		}
		
		

		#region implemented abstract members of Mapping
		public override MP.MappingLogic mappingLogic 
		{
			get 
			{	
				if (_mappingLogic == null)
				{
					Guid mappingElementGUID;
					string mappingLogicString = KeyValuePairsHelper.getValueForKey("mappingLogic",this.wrappedTaggedValue.comment);
					if (Guid.TryParse(mappingLogicString,out mappingElementGUID))
					{
						ElementWrapper mappingLogicElement = this.wrappedTaggedValue.model.getElementByGUID(mappingLogicString) as ElementWrapper;
						if (mappingLogicElement != null) _mappingLogic = new MappingLogic(mappingLogicElement);
					}
					if (_mappingLogic == null && !string.IsNullOrEmpty(mappingLogicString))
					{
						_mappingLogic = new MappingLogic(mappingLogicString);
					}
				}
				return _mappingLogic;				                            
			}
			set 
			{
				string logicString = value.description;
				if (value.mappingElement != null) logicString = value.mappingElement.uniqueID;
				this.wrappedTaggedValue.comment = KeyValuePairsHelper.setValueForKey("mappingLogic",logicString,this.wrappedTaggedValue.comment);
			}
		}
		#endregion

		#region implemented abstract members of Mapping

		public override void save()
		{
			this.wrappedTaggedValue.save();
		}

		#endregion
	}
}
