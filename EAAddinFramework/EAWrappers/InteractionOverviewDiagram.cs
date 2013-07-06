using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of InteractionOverviewDiagram.
	/// </summary>
	public class InteractionOverviewDiagram:Diagram,UML.Diagrams.InteractionOverviewDiagram
	{
		public InteractionOverviewDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
