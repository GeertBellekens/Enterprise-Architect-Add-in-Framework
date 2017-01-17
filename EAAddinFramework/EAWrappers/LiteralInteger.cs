using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of LiteralString.
	/// </summary>
	public class LiteralInteger:ValueSpecification,UML.Classes.Kernel.LiteralInteger
	{
		public LiteralInteger(int _value)
		{
			this._value = _value;
		}
		public int _value {get;set;}
		public override string ToString()
		{
			return _value.ToString();
		}
	}
}