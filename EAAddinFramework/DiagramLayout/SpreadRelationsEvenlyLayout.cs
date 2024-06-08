using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using UML = TSF.UmlToolingFramework.UML;
using TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinFramework.DiagramLayout
{
    public class SpreadRelationsEvenlyLayout: DiagramLayout
    {
        public SpreadRelationsEvenlyLayout(Diagram diagram): base(diagram) { }

        /// <summary>
        /// lays out all relations with formatting orthogonal, and evenly spread out against the whole connecting side.
        /// All relations go top down from side to top.
        /// </summary>
        public override void layout(IEnumerable<UML.Diagrams.DiagramElement> diagramElements)
        {
            this.setLinkStyle(diagramElements, LinkStyle.lsOrthogonalRoundedTree);
            //first we make a map of all diagram objects, and the relations they have
            var diagramObjectLayouts = this.getDiagramObjectLayouts(diagramElements.OfType<DiagramLinkWrapper>());
            //for each diagram object we have a list of relations on the top, left, bottom and right.
            //then we can, based on the number of relations on each side, determine the coördinates of the relations in order to spread the relations evenly
            foreach(var diagramLinkWrapper in diagramElements.OfType<DiagramLinkWrapper>())
            {
                diagramObjectLayouts.TryGetValue(diagramLinkWrapper.sourceDiagramObject.uniqueID, out var sourceDiagramObjectLayout);
                diagramObjectLayouts.TryGetValue(diagramLinkWrapper.targetDiagramObject.uniqueID, out var targetDiagramObjectLayout);
                //layout diagramlink only if both source and target are found
                if (sourceDiagramObjectLayout != null && targetDiagramObjectLayout != null)
                {
                    //get the relative positions
                    var SX = sourceDiagramObjectLayout.getHorizontalRelativePosition(diagramLinkWrapper);
                    var SY = sourceDiagramObjectLayout.getVerticalRelativePosition(diagramLinkWrapper);
                    var EX = targetDiagramObjectLayout.getHorizontalRelativePosition(diagramLinkWrapper);
                    var EY = targetDiagramObjectLayout.getVerticalRelativePosition(diagramLinkWrapper);

                    //set the link geometry properties
                    diagramLinkWrapper.startXPosition = SX;
                    diagramLinkWrapper.startYPosition = SY;
                    diagramLinkWrapper.endXPosition = EX;
                    diagramLinkWrapper.endYPosition = EY;

                    //set the bend
                    var bendX = sourceDiagramObjectLayout.getBendHorizontalPosition(targetDiagramObjectLayout, diagramLinkWrapper);
                    var bendY = sourceDiagramObjectLayout.getBendVerticalPosition(targetDiagramObjectLayout, diagramLinkWrapper);
                    diagramLinkWrapper.setBend(bendX, bendY);

                    diagramLinkWrapper.save();
                }
            }
        }

        private Dictionary<string, DiagramObjectWrapper> getRelatedDiagramObjects(IEnumerable<DiagramLinkWrapper> diagramLinks)
        {
            Dictionary<string, DiagramObjectWrapper> diagramObjects = new Dictionary<string, DiagramObjectWrapper>();
            foreach (var diagramLink in diagramLinks)
            {
                diagramObjects[diagramLink.sourceDiagramObject.uniqueID] = diagramLink.sourceDiagramObject;
                diagramObjects[diagramLink.targetDiagramObject.uniqueID] = diagramLink.targetDiagramObject;
            }
            return diagramObjects;
        }
        private Dictionary<string, DiagramObjectLayout> getDiagramObjectLayouts(IEnumerable<DiagramLinkWrapper> diagramLinks)
        {
            var diagramObjectLayouts = new Dictionary<string, DiagramObjectLayout> ();
            //check all the connections between the different diagramElements, and determine the side it's on
            foreach (var diagramLink in diagramLinks)
            {
                if (!diagramObjectLayouts.ContainsKey(diagramLink.sourceDiagramObject.uniqueID ))
                {
                    diagramObjectLayouts.Add(diagramLink.sourceDiagramObject.uniqueID, 
                                            new DiagramObjectLayout(diagramLink.sourceDiagramObject));
                }
                if (!diagramObjectLayouts.ContainsKey(diagramLink.targetDiagramObject.uniqueID))
                {
                    diagramObjectLayouts.Add(diagramLink.targetDiagramObject.uniqueID, 
                                            new DiagramObjectLayout(diagramLink.targetDiagramObject));
                }
                var sourceDiagramObjectLayout = diagramObjectLayouts[diagramLink.sourceDiagramObject.uniqueID];
                var targetDiagramObjectLayout = diagramObjectLayouts[diagramLink.targetDiagramObject.uniqueID];
                //figure out direction. setLinkSides will set the linksides on the target diagramObjectLayout as well.
                sourceDiagramObjectLayout.setLinkSides(diagramLink, targetDiagramObjectLayout);
            }
            

            return diagramObjectLayouts;
        }
        //helper class to remember the connections on each side
        private class DiagramObjectLayout
        {
            public DiagramObjectWrapper diagramObjectWrapper { get;private set; }
            internal DiagramObjectLayout(DiagramObjectWrapper diagramObjectWrapper)
            {
                this.diagramObjectWrapper = diagramObjectWrapper;
            }
            public List<DiagramLinkWrapper> topLinks { get; private set; } = new List<DiagramLinkWrapper>();
            public List<DiagramLinkWrapper> leftLinks { get; private set; } = new List<DiagramLinkWrapper>();
            public List<DiagramLinkWrapper> rightLinks { get; private set; } = new List<DiagramLinkWrapper>();
            public List<DiagramLinkWrapper> bottomLinks { get; private set; } = new List<DiagramLinkWrapper>();
            public int topIndex { get; set; } = 0;
            public int leftIndex { get; set; } = 0;
            public int rightIndex { get; set; } = 0;
            public int bottomIndex { get; set; } = 0;

            public int getVerticalRelativePosition(DiagramLinkWrapper diagramLink)
            {
                if (this.leftLinks.Contains(diagramLink))
                {
                    var index = this.leftLinks.IndexOf(diagramLink);
                    return getVerticalRelativePosition(index, this.leftLinks.Count);
                }
                if (this.rightLinks.Contains(diagramLink))
                {
                    var index = this.rightLinks.IndexOf(diagramLink);
                    return getVerticalRelativePosition(index, this.rightLinks.Count);
                }
                return 0;
            }
            public int getHorizontalRelativePosition(DiagramLinkWrapper diagramLink)
            {
                //check in which side this diagramLink sits
                if (this.topLinks.Contains(diagramLink))
                {
                    var index = this.topLinks.IndexOf(diagramLink);
                    return getHorizontalRelativePosition(index, this.topLinks.Count);
                }
                if (this.bottomLinks.Contains(diagramLink))
                {
                    var index = this.bottomLinks.IndexOf(diagramLink);
                    return getHorizontalRelativePosition(index, this.bottomLinks.Count);
                }
                return 0; 
            }

            internal int getBendVerticalPosition (DiagramObjectLayout targetDiagramObjectLayout, DiagramLinkWrapper diagramLink)
            {
                if (this.leftLinks.Contains(diagramLink))
                {
                    var index = this.leftLinks.IndexOf(diagramLink);
                    return getYCordinate(index, this.leftLinks.Count);
                }
                if (this.rightLinks.Contains(diagramLink))
                {
                    var index = this.rightLinks.IndexOf(diagramLink);
                    return getYCordinate(index, this.rightLinks.Count);
                }
                if (targetDiagramObjectLayout != null)
                {
                    return targetDiagramObjectLayout.getBendVerticalPosition(null, diagramLink);
                }
                return 0;
            }
            internal int getBendHorizontalPosition(DiagramObjectLayout targetDiagramObjectLayout, DiagramLinkWrapper diagramLink)
            {
                if (this.topLinks.Contains(diagramLink))
                {
                    var index = this.topLinks.IndexOf(diagramLink);
                    return getXCordinate(index, this.topLinks.Count);
                }
                if (this.bottomLinks.Contains(diagramLink))
                {
                    var index = this.bottomLinks.IndexOf(diagramLink);
                    return getXCordinate(index, this.bottomLinks.Count);
                }
                if (targetDiagramObjectLayout != null)
                {
                    return targetDiagramObjectLayout.getBendHorizontalPosition(null, diagramLink);
                }
                return 0;
            }

            public int getYCordinate(int index, int totalLinks)
            {
                var gap = getYGap(totalLinks);
                var yCord = this.diagramObjectWrapper.top + gap * (index + 1);
                return yCord;
            }

            public int getXCordinate(int index, int totalLinks)
            {
                var gap = getXGap(totalLinks);
                var xCord = this.diagramObjectWrapper.right - gap * (index + 1);
                return xCord;
            }

            public int getYGap(int totalLinks)
            {
                return Decimal.ToInt16(this.diagramObjectWrapper.height / totalLinks);
            }
            public int getXGap(int totalLinks)
            {
                return Decimal.ToInt16(this.diagramObjectWrapper.width / totalLinks);
            }

            public int getVerticalRelativePosition(int index, int totalLinks)
            {
                //determine xCordindate
                var yCord = this.getYCordinate(index, totalLinks);
                //determine middle
                int middle = this.diagramObjectWrapper.top + (this.diagramObjectWrapper.height / 2);
                var relativePosition = middle - yCord;
                return relativePosition;
            }

            public int getHorizontalRelativePosition(int index, int totalLinks)
            {
                //determine xCordindate
                var xCord = this.getXCordinate(index, totalLinks);
                //determine middle
                int middle = this.diagramObjectWrapper.left + (this.diagramObjectWrapper.width / 2);
                var relativePosition = middle - xCord;
                return relativePosition;
            }
            public void setLinkSides(DiagramLinkWrapper link, DiagramObjectLayout targetDiagramObjectLayout) 
            {
                var targetDiagramObjectWrapper = targetDiagramObjectLayout.diagramObjectWrapper;
                //set links
                if (this.diagramObjectWrapper.right < targetDiagramObjectWrapper.left)
                {
                    //from left to right
                    this.rightLinks.Add(link);
                    if (this.diagramObjectWrapper.top > targetDiagramObjectWrapper.bottom)
                    {
                        //going up
                        targetDiagramObjectLayout.bottomLinks.Add(link);
                    }
                    else if (this.diagramObjectWrapper.bottom < targetDiagramObjectWrapper.top)
                    {
                        //going down
                        targetDiagramObjectLayout.topLinks.Add(link);
                    }
                    else
                    {
                        //going sideways
                        targetDiagramObjectLayout.leftLinks.Add(link);
                    }
                }
                else if (this.diagramObjectWrapper.left > targetDiagramObjectWrapper.right)
                {
                    //right to left
                    this.leftLinks.Add(link);
                    if (this.diagramObjectWrapper.top > targetDiagramObjectWrapper.bottom)
                    {
                        //going up
                        targetDiagramObjectLayout.bottomLinks.Add(link);
                    }
                    else if (this.diagramObjectWrapper.bottom < targetDiagramObjectWrapper.top)
                    {
                        //going down
                        targetDiagramObjectLayout.topLinks.Add(link);
                    }
                    else
                    {
                        //going sideways
                        targetDiagramObjectLayout.rightLinks.Add(link);
                    }
                }
                else
                {
                    if (this.diagramObjectWrapper.top > targetDiagramObjectWrapper.bottom)
                    {
                        //going up
                        this.topLinks.Add(link);
                        targetDiagramObjectLayout.bottomLinks.Add(link);
                    }
                    else if (this.diagramObjectWrapper.bottom < targetDiagramObjectWrapper.top)
                    {
                        //going down
                        this.bottomLinks.Add(link);
                        targetDiagramObjectLayout.topLinks.Add(link);
                    }
                }
            }

        }
    }
}
