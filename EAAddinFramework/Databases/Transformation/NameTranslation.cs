
using System;

namespace EAAddinFramework.Databases.Transformation
{
	/// <summary>
	/// Description of NameTranslation.
	/// </summary>
	public class NameTranslation
	{
		public string source {get;private set;}
		public string target {get;private set;}
		public bool suffix {get;private set;}
		public NameTranslation(string source, string target, bool suffix = false)
		{
			this.source = source;
			this.target = target;
			this.suffix = suffix;
		}
	}
}
