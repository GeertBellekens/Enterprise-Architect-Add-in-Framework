using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLEA = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.UML.Diagrams;

namespace EAAddinFramework.DiagramLayout
{
    public class LateralHorizontalLayout : DiagramLayout
    {
        public LateralHorizontalLayout(UMLEA.Diagram diagram) : base(diagram) { }

        public override void layout(IEnumerable<DiagramElement> diagramElements)
        {
            this.setLinkStyle(diagramElements, UMLEA.LinkStyle.lsLateralHorizontalTree);
        }
    }
}
