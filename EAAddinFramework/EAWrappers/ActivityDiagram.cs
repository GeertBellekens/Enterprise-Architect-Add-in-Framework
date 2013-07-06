using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of ActivityDiagram.
	/// </summary>
	public class ActivityDiagram : Diagram, UML.Diagrams.ActivityDiagram
	{
		public ActivityDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
