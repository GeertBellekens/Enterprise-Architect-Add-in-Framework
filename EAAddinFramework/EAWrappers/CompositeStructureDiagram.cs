using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of CompositeStructureDiagram.
	/// </summary>
	public class CompositeStructureDiagram:Diagram,UML.Diagrams.CompositeStructureDiagram
	{
		public CompositeStructureDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
