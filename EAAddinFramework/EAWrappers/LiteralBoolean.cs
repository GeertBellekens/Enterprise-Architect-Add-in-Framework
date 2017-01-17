using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of LiteralString.
	/// </summary>
	public class LiteralBoolean:ValueSpecification,UML.Classes.Kernel.LiteralBoolean
	{
		public LiteralBoolean( bool _value)
		{
			this._value = _value;
		}
		public bool _value {get;set;}
		public override string ToString()
		{
			return _value.ToString();
		}

	}
}