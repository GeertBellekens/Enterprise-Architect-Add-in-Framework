using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of StateMachineDiagram.
	/// </summary>
	public class StateMachineDiagram : Diagram, UML.Diagrams.StateMachineDiagram
	{
		public StateMachineDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
