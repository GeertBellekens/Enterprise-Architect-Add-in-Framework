
using System;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;
using EAAddinFramework.Utilities;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Multiplicity is an interval with a lower limit of type Unsigned Integer (Natural), and a upper limit of type UnlimitedNatural
	/// it is usually represented as [lower limit]..[upper limit] e.g. 0..1, 1..1, 0..*
	/// </summary>
	public class Multiplicity:UML.Classes.Kernel.Multiplicity
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
				return this.lower + delimiter + this.upper ?? string.Empty;
			}
			set
			{
				try
				{
					string[] parts = value.Split(new string[]{delimiter},StringSplitOptions.None);
					if (value.Length > 0 && parts.Length >= 2)
					{
						if (string.IsNullOrEmpty(parts[0]))
					    {
					    	parts[0] = "0";
					    }
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
				}
				catch(Exception)
				{
					throw new FormatException(string.Format("Cannot create multiplicity based on string '{0}'",value));
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
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			Multiplicity other = obj as Multiplicity;
				if (other == null)
					return false;
						return this.EACardinality == other.EACardinality;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * lower.GetHashCode();
				if (upper != null)
					hashCode += 1000000009 * upper.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(Multiplicity lhs, Multiplicity rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Multiplicity lhs, Multiplicity rhs) {
			return !(lhs == rhs);
		}

		#endregion
		
	}
}
