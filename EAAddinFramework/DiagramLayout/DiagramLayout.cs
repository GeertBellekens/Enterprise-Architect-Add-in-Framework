using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UMLEA = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.DiagramLayout
{
    public abstract class DiagramLayout
    {
        protected DiagramLayout(UMLEA.Diagram diagram )
        {
            this.diagram = diagram;
        }
        protected UMLEA.Diagram diagram { get;private set; }
        public void layout()
        {
            this.layout(this.diagram.diagramElements);
        }
        public abstract void layout(IEnumerable<UML.Diagrams.DiagramElement> diagramElements);

        protected void setLinkStyle(IEnumerable<UML.Diagrams.DiagramElement> diagramElements, UMLEA.LinkStyle linkStyle)
        {
            foreach (var diagramLink in diagramElements.OfType<UMLEA.DiagramLinkWrapper>())
            {
                diagramLink.setStyle(linkStyle);
                diagramLink.save();
            }
        }
    }
}
