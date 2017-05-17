using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of OpaqueExpression.
	/// </summary>
	public class OpaqueExpression:ValueSpecification,UML.Classes.Kernel.OpaqueExpression
	{
		string body {get;set;}
		string language {get;set;}
		public OpaqueExpression(string body, string language)
		{
			this.body = body;
			this.language = language;
		}

		public List<string> bodies 
		{
			get 
			{
				return new List<string>{this.body};
			}
			set 
			{
				if (value != null 
				    && value.Any())
				{
					this.body = value[0];
				}
			}
		}

		public List<string> languages 
		{
			get 
			{
				return new List<string>{this.language};
			}
			set 
			{
				if (value != null 
				    && value.Any())
				{
					this.language = value[0];
				}
			}
		}
	}
}
