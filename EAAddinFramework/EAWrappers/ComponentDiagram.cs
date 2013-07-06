using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of ComponentDiagram.
	/// </summary>
	public class ComponentDiagram:Diagram, UML.Diagrams.ComponentDiagram
	{
		public ComponentDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
