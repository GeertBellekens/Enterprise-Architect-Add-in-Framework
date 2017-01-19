using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of LiteralString.
	/// </summary>
	public class LiteralString:ValueSpecification,UML.Classes.Kernel.LiteralString
	{	
		public LiteralString (string _value)
		{
			this._value = _value;
		}
		public string _value {get;set;}
		public override string ToString()
		{
			return _value ?? string.Empty;
		}
	}
}
