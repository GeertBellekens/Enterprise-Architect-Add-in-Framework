
using System;
using WT=WorkTrackingFramework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace EAAddinFramework.WorkTracking.TFS
{
	/// <summary>
	/// Description of Project.
	/// </summary>
	public class Project:WT.Project
	{
		 //TFS constants
        const string tfsCollection = "/DefaultCollection/";
        //variables
		internal TFSSettings settings {get;set;}
		internal string TFSUrl {get;set;}
		private List<WorkItem> _workitems;
		//constructor
		public Project(string projectName,string TFSUrl, TFSSettings settings)
		{
			this.settings = settings;
			this.name = projectName;
			this.TFSUrl = TFSUrl + tfsCollection;
		}

		public string name {get;set;}


		public List<WT.Workitem> workitems {
			get 
			{
				if (_workitems == null)
				{
					_workitems = getAllWorkitems();
				}
				return _workitems.Cast<WT.Workitem>().ToList();
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		private List<WorkItem> getAllWorkitems()
        {
			List<WorkItem> allWorkitems = new List<WorkItem>();
            // create wiql object
            var wiql = new
            {
                query = "Select [State], [Title] " +
                        "From WorkItems " +
                	"Where [Work Item Type] in ( '"+ string.Join("','", settings.workitemMappings.Values.ToArray()) +"' )" +
                		"AND [System.TeamProject] = '" + this.name + "' "	+
                        "Order By [State] Asc, [Changed Date] Desc"
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TFSUrl );
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.defaultUserName + ":" + settings.defaultPassword)));

                // serialize the wiql object into a json string   
                var postValue = new StringContent(JsonConvert.SerializeObject(wiql), Encoding.UTF8, "application/json"); // mediaType needs to be application/json for a post call

                // set the httpmethod to PPOST
                var method = new HttpMethod("POST");

                // send the request               
                var httpRequestMessage = new HttpRequestMessage(method, TFSUrl + "_apis/wit/wiql?api-version=2.2") { Content = postValue };
                var httpResponseMessage = client.SendAsync(httpRequestMessage).Result;

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                	string testqueryResult = httpResponseMessage.Content.ReadAsStringAsync().Result;
                    WorkItemQueryResult workItemQueryResult = httpResponseMessage.Content.ReadAsAsync<WorkItemQueryResult>().Result;
                                     
                    // now that we have a bunch of work items, build a list of id's so we can get details
                    var builder = new System.Text.StringBuilder();
                    foreach (var item in workItemQueryResult.workItems)
                    {
                        builder.Append(item.id.ToString()).Append(",");
                    }

                    // clean up string of id's
                    string ids = builder.ToString().TrimEnd(new char[] { ',' });
                    //get the workitems
                    var workitemsResponses =  GetListOfWorkItems_ByIDs( ids,client);
                    //loop over the workitems to create actual workitems
                    foreach (var workitemResponse in workitemsResponses.value) 
                    {	
                    	WorkItem workitem = new WorkItem(workitemResponse.id,workitemResponse.fields);
                    	allWorkitems.Add(workitem);
                    }
                    
                    return allWorkitems;
                }

                return null;
            }
        }
		public ListofWorkItemsResponse.WorkItems GetListOfWorkItems_ByIDs(string ids, HttpClient client)
        {
            ListofWorkItemsResponse.WorkItems viewModel = new ListofWorkItemsResponse.WorkItems();
            HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&api-version=2.2").Result;
                        
            if (response.IsSuccessStatusCode)
            {
                viewModel = response.Content.ReadAsAsync<ListofWorkItemsResponse.WorkItems>().Result;
            }
            viewModel.HttpStatusCode = response.StatusCode;

            return viewModel;
            
        }
		
	}
}
