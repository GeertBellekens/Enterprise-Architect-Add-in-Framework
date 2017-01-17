using System.Collections.Generic;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA 
{
	/// <summary>
	/// Description of LiteralString.
	/// </summary>
	public class LiteralUnlimitedNatural:ValueSpecification,UML.Classes.Kernel.LiteralUnlimitedNatural
	{
		public LiteralUnlimitedNatural(UnlimitedNatural __value)
		{
			this.__value = __value;
		}
		UnlimitedNatural __value{get;set;}
		public UML.Classes.Kernel.UnlimitedNatural _value 
		{
			get
			{
				return __value;
			}
			set
			{
				__value = (UnlimitedNatural)value ;
			}
		}
		public override string ToString()
		{
			return __value.ToString();
		}
	}
}