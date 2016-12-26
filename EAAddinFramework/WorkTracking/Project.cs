
using System;
using WT=WorkTrackingFramework;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.WorkTracking
{
	/// <summary>
	/// Description of Project.
	/// </summary>
	public abstract class Project:WT.Project
	{
		Package _wrappedPackage;
		internal Package wrappedPackage {
			get {
				return _wrappedPackage;
			}
			set {
				_wrappedPackage = value;
				this.resetProperties();
			}
		}
		/// <summary>
		/// default constructor
		/// </summary>
		public Project(){}
		public Project(Package wrappedPackage)
		{
			this._wrappedPackage = wrappedPackage;
		}
		protected static Package getCurrentProjectPackage(Element currentElement)
		{
			//if the current element is null then we can't find the project
			if (currentElement == null) return null;
			//get the owning package
			if (!( currentElement is Package)) currentElement = currentElement.owningPackage as Element;
			if (currentElement is RootPackage)
			{
				if (currentElement.notes.Contains("project=")) return currentElement as Package;
			}
			//no rootPackage
			if (currentElement.taggedValues.Any( x => x.name == "project" && x.tagValue.ToString().Length > 0))
			{
				return currentElement as Package;
			}
			//no project found on this level, go one level up
			return getCurrentProjectPackage(currentElement.owningPackage as Element);
		}

		/// <summary>
		/// returns all owned workitems for this package and if requested for all owned packages recursively
		/// </summary>
		/// <param name="ownerPackage">the owner package</param>
		/// <param name="recursive">indicates whether or not we should recursively search workitems in owned packages</param>
		/// <returns>a list of owned workitems for the given package</returns>
		public abstract List<WT.Workitem> getOwnedWorkitems(UML.Classes.Kernel.Package ownerPackage, bool recursive);
		

		void resetProperties()
		{
			if (this.wrappedPackage is RootPackage)
			{
				this.wrappedPackage.notes ="project=" + _name;
			}
			else
			{
				this.wrappedPackage.addTaggedValue("project",_name);
			}
		}
		#region Project implementation

		List<WorkItem> _workitems;
		public virtual List<WT.Workitem> workitems {
			get 
			{
				return _workitems.Cast<WT.Workitem>().ToList();
			}
			set 
			{
				_workitems = value.Cast<WorkItem>().ToList();
			}
		}

		string _name;
		public string name 
		{
			get 
			{
				if (this.wrappedPackage != null)
				{
					if (wrappedPackage is RootPackage)
					{
						var keyValue = this.wrappedPackage.notes.Split('=');
						if (keyValue.Count() == 2)
						{
							_name = keyValue[1];
						}
					}
					else
					{
						var projectTag = wrappedPackage.getTaggedValue("project");
						if (projectTag != null)
						{
							_name = projectTag.eaStringValue;
						}
					}
				}
				return _name;
			}
			set 
			{
				if (this.wrappedPackage != null)
				{
					if (wrappedPackage is RootPackage)
					{
						this.wrappedPackage.notes ="project=" + value;
					}
					else
					{
						this.wrappedPackage.addTaggedValue("project",value);
					}
				}
				_name = value;
			}
		}

		#endregion
	}
}
