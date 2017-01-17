using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of LiteralString.
	/// </summary>
	public class LiteralNull:ValueSpecification,UML.Classes.Kernel.LiteralNull
	{
		public override string ToString()
		{
			return "NULL";
		}
	}
}