using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of TimingDiagram.
	/// </summary>
	public class TimingDiagram : Diagram, UML.Diagrams.TimingDiagram
	{
		public TimingDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
