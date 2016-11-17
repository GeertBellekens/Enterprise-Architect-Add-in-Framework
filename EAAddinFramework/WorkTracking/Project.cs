
using System;
using WT=WorkTrackingFramework;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace EAAddinFramework.WorkTracking
{
	/// <summary>
	/// Description of Project.
	/// </summary>
	public class Project:WT.Project
	{
		RootPackage _wrappedRootPackage;
		internal RootPackage wrappedRootPackage {
			get {
				return _wrappedRootPackage;
			}
			set {
				_wrappedRootPackage = value;
				this.resetProperties();
			}
		}
		/// <summary>
		/// default constructor
		/// </summary>
		public Project(){}
		public Project(RootPackage wrappedRootPackage)
		{
			this._wrappedRootPackage = wrappedRootPackage;
		}

		void resetProperties()
		{
			this.wrappedRootPackage.notes ="project=" + _name;
		}
		#region Project implementation

		List<WorkItem> _workitems;
		public List<WT.Workitem> workitems {
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
				if (this.wrappedRootPackage != null)
				{
					var keyValue = this.wrappedRootPackage.notes.Split('=');
					if (keyValue.Count() == 2)
					{
						_name = keyValue[1];
					}
				}
				return _name;
			}
			set 
			{
				if (this.wrappedRootPackage != null)
				{
					this.wrappedRootPackage.notes ="project=" + value;
				}
				_name = value;
			}
		}

		#endregion
	}
}
