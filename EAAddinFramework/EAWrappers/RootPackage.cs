using System;
using System.Collections.Generic;
using System.Linq;

using UML=TSF.UmlToolingFramework.UML;

namespace TSF.UmlToolingFramework.Wrappers.EA
{
	/// <summary>
	/// Description of rootPackage.
	/// </summary>
	public class RootPackage:Package
	{
		public RootPackage(Model model,global::EA.Package package):base(model,package)
		{
			this.wrappedPackage = package;
		}
		/// <summary>
		/// return the hashcode based on the elements guid
		/// </summary>
		/// <returns>hashcode based on the elements guid</returns>
	    public override int GetHashCode()
	    {
	      return new Guid(this.wrappedPackage.PackageGUID).GetHashCode();
	    }
	    
	    public override String name {
	      get { return this.wrappedPackage.Name;  }
	      set { this.wrappedPackage.Name = value; }
	    }
	    public override HashSet<UML.Diagrams.Diagram> ownedDiagrams 
	    {
			get 
			{
	    		return new HashSet<UML.Diagrams.Diagram>();
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
	}
}
