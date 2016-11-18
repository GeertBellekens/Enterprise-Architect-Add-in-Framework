
using System;
using WT=WorkTrackingFramework;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using EAAddinFramework.WorkTracking;
using TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinFramework.WorkTracking.TFS
{
	/// <summary>
	/// Description of WorkItem.
	/// </summary>
	public class TFSWorkItem:WorkItem
	{
		private TFSProject _TFSOwnerProject;
		public TFSWorkItem(TFSProject ownerProject, ElementWrapper elementToWrap):base(ownerProject,elementToWrap)
		{
			_TFSOwnerProject = ownerProject;
		}
		public TFSWorkItem(TFSProject ownerProject, int workitemID, ListofWorkItemsResponse.Fields fields):base(ownerProject)
		{
			_TFSOwnerProject = ownerProject;
			this.ID = workitemID.ToString();
			this.type = fields.SystemWorkItemType;
			this.title = fields.SystemTitle;
			this.state = fields.SystemState;
			this.description = fields.SystemDescription;
			this.assignedTo = fields.SystemAssignedTo;
			this.area = fields.SystemAreaPath;
			this.iteration = fields.SystemIterationPath;
		}
		public string url
		{
			get
			{
				return this._TFSOwnerProject.url + "/_workitems?_a=edit&id="+this.ID;
			}
		}
		public override WT.Project ownerProject {
			get 
			{
				return base.ownerProject;
			}
			set {
				base.ownerProject = value;
				this._TFSOwnerProject = (TFSProject)value;
			}
		}
		public override string iteration 
		{
			get 
			{
				return this.ownerProject.name + @"\" + base.iteration;
			}
			set 
			{
				base.iteration = value.Replace(@"\" +_TFSOwnerProject.name,string.Empty);
			}
		}
		public override string area 
		{
			get 
			{
				return this.ownerProject.name + @"\" + base.area;
			}
			set 
			{
				base.area = value.Replace(@"\" +_TFSOwnerProject.name,string.Empty);
			}
		}

	}
}
