
using System;
using WT=WorkTrackingFramework;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace EAAddinFramework.WorkTracking.TFS
{
	/// <summary>
	/// Description of WorkItem.
	/// </summary>
	public class WorkItem:WT.Workitem
	{
		public WorkItem(int workitemID, ListofWorkItemsResponse.Fields fields)
		{
			this.ID = workitemID.ToString();
			this.type = fields.SystemWorkItemType;
			this.title = fields.SystemTitle;
			this.state = fields.SystemState;
			this.description = fields.SystemDescription;
			this.assignedTo = fields.SystemAssignedTo;
			this.area = fields.SystemAreaPath;
			this.iteration = fields.SystemIterationPath;
		}

		#region Workitem implementation
		 string _ID;
		public string ID 
		{
			get 
			{
				return _ID;
			}
			set 
			{
				_ID = value;
			}
		}
		 string _type;
		public string type 
		{
			get 
			{
				return _type;
			}
			set 
			{
				_type = value;
			}
		}
		 string _title;
		public string title 
		{
			get 
			{
				return _title;
			}
			set 
			{
				_title = value;
			}
		}

		string _state;
		public string state 
		{
			get 
			{
				return _state;
			}
			set {
				_state = value;
			}
		}

		string _description;
		public string description 
		{
			get {
				return _description;
			}
			set {
				_description = value;
			}
		}
		string _assignedTo;
		public string assignedTo {
			get {
				return _assignedTo;
			}
			set {
				_assignedTo = value;
			}
		}

		string _area;
		public string area {
			get {
				return _area;
			}
			set {
				_area = value;
			}
		}

		string _iteration;
		public string iteration {
			get {
				return _iteration;
			}
			set {
				_iteration = value;
			}
		}

		#endregion
	}
}
