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
		public override HashSet<UML.Profiles.TaggedValue> getReferencingTaggedValues()
		{
			return this.model.getTaggedValuesWithValue(this.wrappedPackage.PackageGUID);
		}
		/// <summary>
		/// Rootpackages don't have relationships
		/// </summary>
		/// <returns>an empty list</returns>
		public override List<T> getRelationships<T>()
		{
			return new List<T>();
		}
	}
}
