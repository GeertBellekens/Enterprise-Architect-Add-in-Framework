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
		public override string EAElementType 
		{
			get 
			{
				return "Package";
			}
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
		public override UML.Classes.Kernel.Package owningPackage 
		{
			get 
			{
				return null;
			}
			set 
			{
				// do nothing
			}
		}
	    public override TSF.UmlToolingFramework.UML.Classes.Kernel.Element owner
		{
			get 
			{ 
				return null;
			}
			set 
			{ 
				//do nothing, rotpackages don't have an owner
			}
		}
		public override HashSet<UML.Profiles.Stereotype> stereotypes 
		{
			get 
			{
				//rootpackges don't have stereotypes
				return new HashSet<UML.Profiles.Stereotype>();
			}
			set 
			{
				//do nothing
			}
		}
		public override HashSet<UML.Profiles.TaggedValue> taggedValues 
		{
			//in EA the rootpackage can't have tagged values
			get 
			{ 
				return new HashSet<UML.Profiles.TaggedValue>();
			}
			set 
			{
				//do nothing
			}
		}
		
		/// <summary>
		/// Rootpackages don't have relationships
		/// </summary>
		/// <returns>an empty list</returns>
		public override List<T> getRelationships<T>(bool outgoing = true, bool incoming = true)
		{
			return new List<T>();
		}
	}
}
