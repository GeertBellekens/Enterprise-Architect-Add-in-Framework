using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of UseCaseDiagram.
	/// </summary>
	public class UseCaseDiagram: Diagram, UML.Diagrams.UseCaseDiagram
	{
		public UseCaseDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
