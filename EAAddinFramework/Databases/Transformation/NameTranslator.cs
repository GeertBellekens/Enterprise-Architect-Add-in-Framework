﻿using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using UTF_EA=TSF.UmlToolingFramework.Wrappers.EA;
using DB=DatabaseFramework;
using DB_EA = EAAddinFramework.Databases;
using EAAddinFramework.Utilities;

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
				foreach (string translationLine in System.IO.File.ReadAllLines(csvFilepath))
				{
					var lineParts = translationLine.Split(';');
					if (lineParts.Count() > 1)
					{
						var sourceName = lineParts[0];
						var targetName = lineParts[1];
						bool suffix = false;
						if (lineParts.Count() > 2)
						{
							suffix = lineParts[2].ToLower().Contains("suffix");
						}
						if (! translations.ContainsKey(sourceName))
						translations.Add(sourceName,new NameTranslation(sourceName,targetName,suffix));
					}
					
				} 
				_separator = separator;
				
			}catch (Exception e)
			{
				Logger.logError(string.Format("Error: {0} Stacktrace: {1}",e.Message, e.StackTrace));
			}
		}
		public string translate(string source)
		{
			var translatedParts = this.translateParts(source.Split(' ').ToList());
			//first get the parts that are not suffixes
			string translation = string.Join(this._separator, translatedParts.Where(x => ! x.isSuffix)
			                                 .Select(y => y.translation).ToArray());
			//then get the parts that are suffixes
			string translationSuffix = string.Join(this._separator, translatedParts.Where(x => x.isSuffix).Select(y => y.target).ToArray());
			return translation + translationSuffix;
			
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
			//return unchanged if the size of the group is 0
			if (groupsize < 1) return nameParts;
			//make a new list
			List<TranslatedItem> translatedGroups = new List<TranslatedItem>();
			//loop original list
			for (int i = 0; i < nameParts.Count; i++) 
			{
				//each time try to make a group of items of the given size that are not translated yet and try to translate
				List<TranslatedItem> group = new List<TranslatedItem>();
				for (int k = 0; k < groupsize && k + i < nameParts.Count;k++)
				{
					var currentItem = nameParts[i+k];
					if (! currentItem.isTranslated) 
					{
						group.Add(nameParts[i+k]);
					}
					else
					{
						//item is already translated
						//in that case can add this one and all previous ones to the translated groups and continue after after the translated item
						translatedGroups.AddRange(translatedGroups.GetRange(i, i+k));
						//start with the next item
						i = i + k ; //the i++ adds the +1 itself so it starts with the item after the translated one?
						break;
					}
				}
				//check if the group is the required size
				if (group.Count == groupsize)
				{
					var groupTranslation = translateGroup(group);
					//check if translation has succeeded
					if (groupTranslation != null)
					{
						//add the grouptranslation to the new list
						translatedGroups.Add(groupTranslation);
						//if possible we continue after this group with the same groupsize, 
						if (i + groupsize < nameParts.Count)
						{
							i += groupsize;
						}
						else
						{
							//if not we add the individual items to the new list and we try to translate with a smaller groupsize
							translatedGroups.AddRange(translatedGroups.GetRange( i + groupsize, nameParts.Count - (i + groupsize)));
							return translateParts(translatedGroups, groupsize -1);
						}
					}
					else
					{
						//could not translate this group. We add the first item of the group to the translated groups so we can try with the next items
						translatedGroups.Add(group[0]);
					}					
				}
				else
				{
					//group did not reach the required size, the first item so we can try with the next iteration
					translatedGroups.AddRange(group);
					//move i so we start checking after this group
					i = i + group.Count;
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
