using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of ObjectDiagram.
	/// </summary>
	public class ObjectDiagram:Diagram, UML.Diagrams.ObjectDiagram
	{
		public ObjectDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
