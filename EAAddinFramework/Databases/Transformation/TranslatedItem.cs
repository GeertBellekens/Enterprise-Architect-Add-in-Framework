
using System;
using System.Collections.Generic;

namespace EAAddinFramework.Databases.Transformation
{
	/// <summary>
	/// Description of translatedItem.
	/// </summary>
	public class TranslatedItem
	{
		public string source {get;set;}
		public string target {get;set;}
		public bool isTranslated{get;set;}
		public bool isSuffix{get;set;}
		public TranslatedItem(string source, string target, bool isSuffix)
		{
			this.source = source;
			this.target = target;
			this.isTranslated = true;
			this.isSuffix = isSuffix;
		}
		public TranslatedItem(string source)
		{
			this.source = source;
			this.target = string.Empty;
			this.isTranslated = false;
		}
		public string translation
		{
			get
			{
				if (isTranslated) return target;
				return source;
			}
		}
			
	}
}
