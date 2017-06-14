using System.Collections.Generic;
using System.Linq;
using System;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of LinkStyles.
	/// </summary>
	public enum LinkStyle : int
	{
		lsDirectMode, 
		lsAutoRouteMode, 
		lsCustomMode, 
		lsTreeVerticalTree, 
		lsTreeHorizontalTree,
		lsLateralHorizontalTree, 
		lsLateralVerticalTree, 
		lsOrthogonalSquareTree, 
		lsOrthogonalRoundedTree
	}
}
