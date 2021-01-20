
using System;
using EAAddinFramework.Utilities;
using WT = WorkTrackingFramework;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
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
			//set default type
			if (string.IsNullOrEmpty(this.type))
		    {
				if (this._TFSOwnerProject.settings.workitemMappings.ContainsKey(this.wrappedElement.wrappedElement.FQStereotype)) //TODO add to ElementWrapper?
				{
					this.type = _TFSOwnerProject.settings.workitemMappings[this.wrappedElement.wrappedElement.FQStereotype];
				}
			 	else if(this._TFSOwnerProject.settings.workitemMappings.ContainsKey(this.wrappedElement.EAElementType))
				{
					this.type = _TFSOwnerProject.settings.workitemMappings[this.wrappedElement.EAElementType];
				}
			 	else
			 	{
			 		this.type = _TFSOwnerProject.settings.defaultWorkitemType;
			 	}
		    }
			//set default status
			if (string.IsNullOrEmpty( this.state))
			{
				this.state = this._TFSOwnerProject.settings.defaultStatus;
			}
		}
		private void updateTFSFields(ListofWorkItemsResponse.Fields fields)
		{
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
		public TFSWorkItem(TFSProject ownerProject, int workitemID, ListofWorkItemsResponse.Fields fields):base(ownerProject)
		{
			_TFSOwnerProject = ownerProject;
			this.ID = workitemID.ToString();
			this.updateTFSFields(fields);
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
		public string TFSIteration
		{get
			{
				if (this.iteration.Length > 0)
					return this.ownerProject.name + @"\" + this.iteration;
				return this.ownerProject.name;
			}
		}
		public string TFSArea
		{
			get
			{
				if (this.area.Length > 0)
					return this.ownerProject.name + @"\" + this.area;
				return this.ownerProject.name ;
			}
			
		}
		
		/// <summary>
		/// mapped to tagged value TFS_ID
		/// </summary>
		public override string ID 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					
					var IDTag = this.wrappedElement.getTaggedValue("TFS_ID");
					if (IDTag != null)
					{
						_ID = IDTag.tagValue.ToString();
					}
				}
				return _ID;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.addTaggedValue("TFS_ID",value);
				}
				_ID = value;
			}
		}
				/// <summary>
		/// mapped to notes of wrapped element
		/// </summary>
		public override string description 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					_description = this.wrappedElement.convertFromEANotes("HTML");
				}
				return _description;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.convertToEANotes(value,"HTML");
				}
				_description = value;
			}
		}
		
		/// <summary>
		/// mapped to tagged value TFS_type
		/// </summary>
		public override string type 
		{
			get 
			{
				if (this.wrappedElement != null)
				{
					var typeTag = this.wrappedElement.getTaggedValue("TFS_type");
					if (typeTag != null)
					{
						_type = typeTag.tagValue.ToString();
					}
				}
				return _type;
			}
			set 
			{
				if (this.wrappedElement != null)
				{
					this.wrappedElement.addTaggedValue("TFS_type",value);
				}
				_type = value;
			}
		}
		public bool synchronizeToTFS()
		{
			if (string.IsNullOrEmpty(this.ID))
			{
				return createNewOnTFS();
			}
			else
			{
				return updateToTFS();
			}

		}
		
        public bool createNewOnTFS()
        {
        	if (string.IsNullOrEmpty(this.ID))
	        {
	            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
	            
	            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[7];
	
	           	fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Title", value = this.title };
	            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Description", value = this.description };
	            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.IterationPath", value = this.TFSIteration };
	            fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AreaPath", value = this.TFSArea };
	            fields[4] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AssignedTo", value = this.assignedTo };
	            fields[5] = new WorkItemPatch.Field() { op = "add", path = "/fields/Ext_RefId", value = this.wrappedElement.uniqueID };
	            fields[6] = new WorkItemPatch.Field() { op = "add", path = "/fields/State", value = this.state};
	                     
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
	                //var request = new HttpRequestMessage(method, _TFSOwnerProject.TFSUrl + Uri.EscapeDataString(_TFSOwnerProject.name) + "/_apis/wit/workitems/$" + this.type + "?api-version=2.2") { Content = patchValue };
	                var request = new HttpRequestMessage(method, _TFSOwnerProject.TFSUrl + Uri.EscapeDataString(_TFSOwnerProject.name) +"/_apis/wit/workitems/$"+ this.type +"?api-version=2.2") { Content = patchValue };
	                var response = _TFSOwnerProject.sendToTFS(client,request);
	
	                if (response != null)
	                {
		                if (response.IsSuccessStatusCode)
		                {
		                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
		                    this.ID = viewModel.id.ToString();
		                    return true;
		                }
		                Logger.logError("Could not create workitem on TFS with title: '" + this.title + " because of error \nStatuscode: " 
		                                + response.StatusCode + " Reasonphrase: " +response.ReasonPhrase);
	                }
	               return false;
	            }
        	}
        	return false;
        }
		
        public bool updateToTFS()
        {
        	if (this.wrappedElement != null
        	    && ! string.IsNullOrEmpty(this.ID))
        	{
	            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
	            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[7];
	
	           	fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Title", value = this.title };
	            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Description", value = this.description };
	            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.IterationPath", value = this.TFSIteration };
	            fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AreaPath", value = this.TFSArea };
	            fields[4] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AssignedTo", value = this.assignedTo };
	            fields[5] = new WorkItemPatch.Field() { op = "add", path = "/fields/Ext_RefId", value = this.wrappedElement.uniqueID };
	            fields[6] = new WorkItemPatch.Field() { op = "add", path = "/fields/State", value = this.wrappedElement.status };
	                      
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
	                var response = _TFSOwnerProject.sendToTFS(client,request);
	                if (! response.IsSuccessStatusCode)
	                {
	                	Logger.logError("Could not update workitem to TFS with ID: '" + this.ID + " because of error \nStatuscode: " 
	                                + response.StatusCode + " Reasonphrase: " +response.ReasonPhrase);
	                }
	                return response.IsSuccessStatusCode;
	            }
        	}
        	return false;
        }
        
        public bool updateEAGUIDToTFS()
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
	                var response = _TFSOwnerProject.sendToTFS(client,request);
	
	                return response.IsSuccessStatusCode;
	            }
        	}
        	return false;
        }
        public bool synchronizeToEA()
        {
        	if (this.ID.Length > 0)
        	{
	       		using (var client = new HttpClient())
	            {
					client.BaseAddress = new Uri(this._TFSOwnerProject.TFSUrl);
	                client.DefaultRequestHeaders.Accept.Clear();
	                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")); 
	                client.DefaultRequestHeaders.Authorization = _TFSOwnerProject.authorization;
	                var workitemResponses = this._TFSOwnerProject.GetListOfWorkItems_ByIDs(this.ID ,client);
	                if (workitemResponses.value != null && workitemResponses.value.Any())
	                {
	                	var workitemValues = workitemResponses.value[0].fields;
	                	this.updateTFSFields(workitemResponses.value[0].fields);
	                	this.save();
	                	return true;
	                }
	        	}
        	}
        	return false;
        }
		public override bool synchronizeToEA(Package ownerPackage, string elementType)
		{
			bool result = base.synchronizeToEA(ownerPackage, elementType);
			this.updateEAGUIDToTFS();
			return result;
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
                var response = _TFSOwnerProject.sendToTFS(client,request);
                               
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
