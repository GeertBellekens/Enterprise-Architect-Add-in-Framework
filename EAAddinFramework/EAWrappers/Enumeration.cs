using System;
using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Description of Enumeration.
	/// </summary>
	public class Enumeration:DataType,UML.Classes.Kernel.Enumeration
	{
		private HashSet<UML.Classes.Kernel.EnumerationLiteral> _ownedLiterals;
		public Enumeration(Model model, global::EA.Element elementToWrap)
      : base(model, elementToWrap)
		{}
		
		public HashSet<UML.Classes.Kernel.EnumerationLiteral> ownedLiterals 
		{
			get 
			{
		      	if (this._ownedLiterals == null)
		      	{
		      		this._ownedLiterals = new HashSet<UML.Classes.Kernel.EnumerationLiteral>(this.attributeWrappers.OfType<UML.Classes.Kernel.EnumerationLiteral>());
		      	}
		        return this._ownedLiterals;
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}
