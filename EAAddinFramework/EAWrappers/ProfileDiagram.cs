using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of ProfileDiagram.
	/// </summary>
	public class ProfileDiagram : Diagram, UML.Diagrams.ProfileDiagram
	{
		public ProfileDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
