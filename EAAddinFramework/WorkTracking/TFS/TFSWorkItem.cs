
using System;
using System.Windows.Forms;
using WT=WorkTrackingFramework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
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
			this.area = fields.SystemAreaPath
							.Replace(_TFSOwnerProject.name + @"\", string.Empty)
							.Replace(_TFSOwnerProject.name, string.Empty);;
			this.iteration = fields.SystemIterationPath
							.Replace(_TFSOwnerProject.name + @"\", string.Empty)
							.Replace(_TFSOwnerProject.name, string.Empty);;
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
		
		// / <summary>
        // / Create a bug
        // / </summary>
        // / <param name="projectName"></param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem CreateNewOnTFS()
        {
        	if (string.IsNullOrEmpty(this.ID))
	        {
	            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
	            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[6];
	
	            // set some field values like title and description
	            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Title", value = this.title };
	            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Description", value = this.iteration };
	            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.IterationPath", value = this.iteration };
	            fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AreaPath", value = this.area };
	            fields[4] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AssignedTo", value = this.assignedTo };
	            fields[5] = new WorkItemPatch.Field() { op = "add", path = "/fields/Ext_RefId", value = this.wrappedElement.uniqueID };
	            
	            using (var client = new HttpClient())
	            {
	                client.DefaultRequestHeaders.Accept.Clear();
	                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	                client.DefaultRequestHeaders.Authorization = _TFSOwnerProject.authorization;
	
	                // serialize the fields array into a json string          
	                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call
	
	                // set the httpmethod to Patch
	                var method = new HttpMethod("PATCH");
	               
	                // send the request
	                var request = new HttpRequestMessage(method, _TFSOwnerProject.TFSUrl + Uri.EscapeUriString(_TFSOwnerProject.name) + "/_apis/wit/workitems/$" + this.type + "?api-version=2.2") { Content = patchValue };
	                var response = client.SendAsync(request).Result;
	
	                var me = response.ToString();
	
	                if (response.IsSuccessStatusCode)
	                {
	                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
	                }
	
	                viewModel.HttpStatusCode = response.StatusCode;
	
	                return viewModel;
	            }
        	}
        	return null;
        }
		
        

        // / <summary>
        // / update fields on work item using bypass rules
        // / </summary>
        // / <param name="id">work item id</param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public bool UpdateEAGUIDToTFS()
        {
        	if (this.wrappedElement != null)
        	{
	            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
	            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[1];
	
	            // replace value on a field that you normally cannot change, like system.createdby
	            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/Ext_RefId", value = this.wrappedElement.uniqueID};
	                      
	            using (var client = new HttpClient())
	            {
	                client.DefaultRequestHeaders.Accept.Clear();
	                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	                client.DefaultRequestHeaders.Authorization = _TFSOwnerProject.authorization;
	
	                // serialize the fields array into a json string          
	                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call
	
	                // set the httpmethod to Patch
	                var method = new HttpMethod("PATCH");
	
	                // send the request
	                var request = new HttpRequestMessage(method, _TFSOwnerProject.TFSUrl  + "_apis/wit/workitems/" + this.ID + "?api-version=2.2") { Content = patchValue };
	                var response = client.SendAsync(request).Result;
	
	                return response.IsSuccessStatusCode;
	            }
        	}
        	return false;
        }
		public override void synchronizeToEA(Package ownerPackage, string elementType)
		{
			base.synchronizeToEA(ownerPackage, elementType);
			this.UpdateEAGUIDToTFS();
		}
		        // / <summary>
        // / update a specific work item by id and return that changed worked item
        // / </summary>
        // / <param name="id"></param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem UpdateWorkItemFields(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[4];

            // change some values on a few fields
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.History", value = "adding some history" };
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "2" };
            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.BusinessValue", value = "100" };
            fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.ValueArea", value = "Architectural" };
                      
            using (var client = new HttpClient())
            {               
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")); 
                client.DefaultRequestHeaders.Authorization = _TFSOwnerProject.authorization;
                
                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH"); 

                // send the request
                var request = new HttpRequestMessage(method, _TFSOwnerProject.TFSUrl  + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;
                               
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

	}
}
