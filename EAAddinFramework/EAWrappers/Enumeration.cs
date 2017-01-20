using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Description of Enumeration.
	/// </summary>
	public class Enumeration:DataType,UML.Classes.Kernel.Enumeration
	{
		
		public Enumeration(Model model, global::EA.Element elementToWrap)
      : base(model, elementToWrap)
		{}
		

	}
}
