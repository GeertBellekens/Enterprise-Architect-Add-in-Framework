
using System;
using UML=TSF.UmlToolingFramework.UML;


namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Description of UnlimitedNatural.
	/// </summary>
	public class UnlimitedNatural:UML.Classes.Kernel.UnlimitedNatural
	{
		public override bool isUnlimited {get;set;}
		public uint numericValue {get;private set;}
		public UnlimitedNatural(string valueString)
		{
			if (valueString == unlimited
			    || valueString.StartsWith("-"))
			{
				this.isUnlimited = true;
			}
			else
			{
				this.numericValue = uint.Parse(valueString);
				this.integerValue = (int)this.numericValue;
				this.isUnlimited = false;
			}
		}
		public const string unlimited = "*";
		public override string ToString()
		{
			if (isUnlimited)
			{
				return unlimited;
			}
			else
			{
				return numericValue.ToString();
			}
		}
		public static bool TryParse(string stringRepresenation, out UnlimitedNatural result)
		{
			try 
			{
				result = new UnlimitedNatural(stringRepresenation);
				return true;
			}
			catch(Exception)
			{
				result = null;
				return false;
			}
		}

	}
}
