using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA=TSF.UmlToolingFramework.Wrappers.EA;
using DB=DatabaseFramework;
using DB_EA = EAAddinFramework.Databases;
using EAAddinFramework.Utilities;
using System.IO;

namespace EAAddinFramework.Databases.Transformation
{
	/// <summary>
	/// Description of NameTranslator.
	/// </summary>
	public class NameTranslator
	{
		protected Dictionary<string, NameTranslation> translations = new Dictionary<string, NameTranslation>();
		protected string _separator = " ";
		public NameTranslator(string csvFilepath, string separator)
		{
			try
			{
				//workaround to make sure it also works when the file is open
				var fileStream = new FileStream(csvFilepath,FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				var reader = new StreamReader(fileStream);
				string translationLine;
				while ( (translationLine = reader.ReadLine()) != null)
				{
					var lineParts = translationLine.Split(';');
					if (lineParts.Count() > 1)
					{
						var sourceName = lineParts[0].ToLower();
						var targetName = lineParts[1];
						bool suffix = false;
						if (lineParts.Count() > 2)
						{
							suffix = lineParts[2].ToLower().Contains("suffix");
						}
						if (! translations.ContainsKey(sourceName))
						{
							translations.Add(sourceName,new NameTranslation(sourceName,targetName,suffix));
						}
					}
					
				} 
				_separator = separator;
				
			}catch (Exception e)
			{
				Logger.logError(string.Format("Error: {0} Stacktrace: {1}",e.Message, e.StackTrace));
			}
		}
		public string translate(string source, string logicalTableName)
		{
			var translatedParts = this.translateParts(source.ToLower().Split(' ').ToList());
			//first get the parts that are not suffixes
			string translation = string.Join(this._separator, translatedParts.Where(x => ! x.isSuffix)
			                                 .Select(y => y.translation).ToArray());
			//then get the parts that are suffixes
			string translationSuffix = string.Join(this._separator, translatedParts.Where(x => x.isSuffix).Select(y => y.translation).ToArray());
			if (string.IsNullOrEmpty(translationSuffix)) return translation;
			//if we have only a suffix then we add the translation for the logical table name befor the suffix
			if (string.IsNullOrEmpty(translation) 
			    && !source.Equals(logicalTableName,StringComparison.InvariantCultureIgnoreCase)) return  translate(logicalTableName,string.Empty) + _separator + translationSuffix;
			//if there is a suffix then return suffix as well
			return translation + _separator + translationSuffix;
			
		}
		private List<TranslatedItem> translateParts(List<string> nameParts)
		{
			//Transforma into translated items
			List<TranslatedItem> translatedItems = new List<TranslatedItem>();
			foreach (var namePart in nameParts) 
			{
				translatedItems.Add(new TranslatedItem(namePart));
			}
			return translateParts(translatedItems, translatedItems.Count);
		}
		private List<TranslatedItem> translateParts(List<TranslatedItem> nameParts, int groupsize)
		{
			//return unchanged if the size of the group is 0 or if all parts are already translated
			if (groupsize < 1 || nameParts.All(x => x.isTranslated)) return nameParts;
			//make a new list
			List<TranslatedItem> translatedGroups = new List<TranslatedItem>();
			//loop original list
			for (int i = 0; i < nameParts.Count; i++) 
			{
				//each time try to make a group of items of the given size that are not translated yet and try to translate
				List<TranslatedItem> untranslatedGroup = new List<TranslatedItem>();
				for (int k = 0; k < groupsize && k + i < nameParts.Count;k++)
				{
					var currentItem = nameParts[i+k];
					if (! currentItem.isTranslated) 
					{
						untranslatedGroup.Add(nameParts[i+k]);
					}
					else
					{
						//item is already translated
						
						//in that case can add this one and all previous ones to the translated groups and continue after after the translated item
						translatedGroups.AddRange(nameParts.GetRange(i, k +1)); 
						//remove everyting from the group so it won't get processed
						untranslatedGroup.Clear();
						//start with the next item
						i = i + k ; //the i++ adds the +1 itself so it starts with the item after the translated one?
						break;
					}
				}
				//check if the group is the required size
				if (untranslatedGroup.Count == groupsize)
				{
					var groupTranslation = translateGroup(untranslatedGroup);
					//check if translation has succeeded
					if (groupTranslation != null)
					{
						//add the grouptranslation to the new list
						translatedGroups.Add(groupTranslation);
						//up i with the size of the group
						i += groupsize -1;
					}
					else
					{
						//could not translate this group. We add the first item of the group to the translated groups so we can try with the next items
						translatedGroups.Add(untranslatedGroup[0]);
					}					
				}
				else if (untranslatedGroup.Count > 0)
				{
					//untranslated group did not reach the required size, the first item so we can try with the next iteration
					translatedGroups.AddRange(untranslatedGroup);
					//move i so we start checking after this group
					i = i + untranslatedGroup.Count;
				}
			}
			return translateParts(translatedGroups, groupsize -1);
		}
		private TranslatedItem translateGroup(List<TranslatedItem> group)
		{
			//join the group again and try to translate
			string term = string.Join(" ",group.Select(x => x.source).ToArray());
			if (translations.ContainsKey(term))
		    {
				return new TranslatedItem(term, translations[term].target,translations[term].suffix);
		    }
			else
			{
				return null;
			}
		}
		
		
	}
}
