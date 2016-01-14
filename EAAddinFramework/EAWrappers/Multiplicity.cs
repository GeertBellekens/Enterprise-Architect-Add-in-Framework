
using System;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Multiplicity is an interval with a lower limit of type Unsigned Integer (Natural), and a upper limit of type UnlimitedNatural
	/// it is usually represented as [lower limit]..[upper limit] e.g. 0..1, 1..1, 0..*
	/// </summary>
	public class Multiplicity
	{
		public uint lower {get;set;}
		public UML.Classes.Kernel.UnlimitedNatural upper {get;set;}
		private const string delimiter = "..";
		public override string ToString()
		{
			return this.EACardinality;
		}

		public string EACardinality
		{
			get
			{
				return this.lower.ToString() + delimiter + this.upper.ToString();
			}
			set
			{
				string[] parts = value.Split(new string[]{delimiter},StringSplitOptions.None);
				if (value.Length > 0 && parts.Length >= 2)
				{
					this.lower = uint.Parse(parts[0]);
					this.upper = new UnlimitedNatural(parts[1]);
				}
				else if (value.Length > 0 && parts.Length == 1)
				{
					if (parts[0] == UnlimitedNatural.unlimited)
					{	//[*] means [0..*]
						this.lower = 0;
						this.upper = new UnlimitedNatural(parts[0]);
					}
					else
					{
						// [1] means [1..1] 
						this.lower = uint.Parse(parts[0]);
						this.upper = new UnlimitedNatural(parts[0]);
					}
				}
				else
				{
					throw new Exception(string.Format("Cardinality specification {0} is invalid!" ,value));
				}
			}
		}
		
		public Multiplicity(string cardinality)
		{
			this.EACardinality = cardinality;			
		}
		public Multiplicity(string lowerString, string upperString)
		{
			if (lowerString.Length > 0 && upperString.Length > 0)
		    {
		    	this.EACardinality = lowerString + ".." + upperString;
		    }
			else
			{
				this.EACardinality = lowerString+upperString;
			}
		}
		
	}
}
