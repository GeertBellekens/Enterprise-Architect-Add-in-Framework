using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of DeploymentDiagram.
	/// </summary>
	public class DeploymentDiagram:Diagram,UML.Diagrams.DeploymentDiagram
	{
		public DeploymentDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
	}
}
