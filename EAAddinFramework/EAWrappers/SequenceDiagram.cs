using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of SequenceDiagram.
	/// </summary>
	public class SequenceDiagram:Diagram,UML.Diagrams.SequenceDiagram
	{
		public SequenceDiagram(Model model, global::EA.Diagram wrappedDiagram ):base(model,wrappedDiagram)
		{
		}
		
				/// <summary>
		/// gets all relations that are specific to this sequence diagram.
		/// </summary>
		/// <returns>all messages and other relations of the diagram</returns>
		public override List<ConnectorWrapper> getRelations()
        {
            string SQLQuery = @"SELECT c.Connector_ID
                                FROM  t_Connector c 
                                WHERE c.DiagramID = "
                            + this.wrappedDiagram.DiagramID + " order by c.SeqNo;";
            return this.model.getRelationsByQuery(SQLQuery);
        } 
		
	}
}
