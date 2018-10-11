
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using EAAddinFramework.Utilities;
using WT=WorkTrackingFramework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML=TSF.UmlToolingFramework.UML;
using System.Configuration;

namespace EAAddinFramework.WorkTracking.TFS
{
	/// <summary>
	/// Description of Project.
	/// </summary>
	public class TFSProject:Project
	{
        
        //variables
        internal TFSSettings settings {get;set;}
		internal string TFSUrl {get;set;}
		private List<WorkItem> _workitems;
		AuthenticationHeaderValue _authorization;
		internal AuthenticationHeaderValue authorization
		{
			get
			{
				if (_authorization == null)
				{
					_authorization = getBasicAuthorization(false) ;
				}
				return _authorization;
			}
			private set
			{
				_authorization = value;
			}
		}

		

		//constructor
		public TFSProject(Package packageToWrap,string TFSUrl, TFSSettings settings):base(packageToWrap)
		{
			this.settings = settings;
			this.TFSUrl = TFSUrl + this.settings.defaultCollection;
		}
		public TFSProject(string projectName,string TFSUrl, TFSSettings settings)
		{
			this.settings = settings;
			this.name = projectName;
			this.TFSUrl = TFSUrl + this.settings.defaultCollection;
		}
		public static TFSProject getCurrentProject(Element currentElement,string TFSUrl, TFSSettings settings)
		{
			var currentProjectPackage = Project.getCurrentProjectPackage(currentElement);
			if (currentProjectPackage != null) return new TFSProject(currentProjectPackage,TFSUrl,settings);
			//no project package found
			return null;
		}
		public string url
		{
			get
			{
				return Uri.EscapeUriString(this.TFSUrl + this.name + "/");
			}
		}

		public override List<WT.Workitem> workitems {
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


        /// <summary>
        /// returns all owned workitems for this package and if requested for all owned packages recursively
        /// </summary>
        /// <param name="ownerPackage">the owner package</param>
        /// <param name="recursive">indicates whether or not we should recursively search workitems in owned packages</param>
        /// <returns>a list of owned workitems for the given package</returns>
        public override List<WT.Workitem> getOwnedWorkitems(UML.Classes.Kernel.Package ownerPackage, bool recursive)
		{
			List<WT.Workitem> foundWorkItems = new List<WT.Workitem>();
			var elementTypes = settings.mappedElementTypes;
			var stereotypes = settings.mappedStereotypes;
			string elementTypeClause = "o.Object_Type in ('" + string.Join("','",elementTypes) + "')";
			string stereotypeClause = "o.Stereotype in ('"+  string.Join("','",stereotypes)  +"')";
			string getWorkitemsSQL = @"select o.Object_ID from t_object o
									where o.Package_ID =" + ((Package)ownerPackage).packageID;
			if (elementTypes.Count > 0)
			{
				if(stereotypes.Count > 0)
				{
					getWorkitemsSQL += " and ( " + elementTypeClause;
					getWorkitemsSQL += " or " + stereotypeClause + ")";
				}
				else
				{
					getWorkitemsSQL += " and " + elementTypeClause;
				}
			}
			else
			{
				if (stereotypes.Count > 0)
				{
					getWorkitemsSQL += " and " + stereotypeClause;
				}
			}
			//add the direct owned workitems
			foreach (var workitemElement in ((Package)ownerPackage).EAModel.getElementWrappersByQuery(getWorkitemsSQL))
			{
				foundWorkItems.Add( new TFSWorkItem(this, workitemElement));
			}
			//recurse owned packages
			if (recursive)
			{
				foreach (var ownedPackage in ownerPackage.ownedElements.OfType<Package>()) 
				{
					foundWorkItems.AddRange(getOwnedWorkitems(ownedPackage,recursive));
				}
			}
			return foundWorkItems;
		}
		private List<WorkItem> getAllWorkitems()
        {
			List<WorkItem> allWorkitems = new List<WorkItem>();
            // create wiql object
            string wiqlQuery = 	"Select [State], [Title] " +
                        	   	" From WorkItems " +
                				" Where [Work Item Type] in ( '"+ string.Join("','", settings.workitemMappings.Values.ToArray()) +"' )" +
                				" AND [System.TeamProject] = '" + this.name + "' ";
            //if the filter tag is set we filter items on this tag
            if (!string.IsNullOrEmpty(settings.TFSFilterTag))
            {
            	wiqlQuery += "AND [Tags] contains '" +settings.TFSFilterTag + "'";
            }
            //add default ordering
            wiqlQuery += "Order By [State] Asc, [Changed Date] Desc";
            var wiql = new
            {	
            	query = wiqlQuery      
            };
			//TODO: check if code below is needed for default credentials
            //new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true })
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(TFSUrl );
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var httpResponseMessage = postWiql( client, wiql);

                if (httpResponseMessage != null &&
                    httpResponseMessage.IsSuccessStatusCode)
                {
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
                    	WorkItem workitem = new TFSWorkItem(this,workitemResponse.id,workitemResponse.fields);
                    	allWorkitems.Add(workitem);
                    }
                }
              return allWorkitems;
            }
        }
		private HttpResponseMessage postWiql(HttpClient client, object wiql)
		{
            // serialize the wiql object into a json string   
            var postValue = new StringContent(JsonConvert.SerializeObject(wiql), Encoding.UTF8, "application/json"); // mediaType needs to be application/json for a post call

            // set the httpmethod to PPOST
            var method = new HttpMethod("POST");

            // send the request               
            var httpRequestMessage = new HttpRequestMessage(method, TFSUrl + "_apis/wit/wiql?api-version=2.2") { Content = postValue };
            //send the actual request to TFS
            return sendToTFS(client,httpRequestMessage);
		}
		internal HttpResponseMessage sendToTFS(HttpClient client, HttpRequestMessage httpRequestMessage,bool useBasicAuthentication = false, bool askUser = false)
		{
			if ( useBasicAuthentication) 
			{
				authorization = getBasicAuthorization(askUser);
				if (authorization != null ) client.DefaultRequestHeaders.Authorization = authorization;
			}
			//user pressed cancel
			if (useBasicAuthentication && authorization == null) return null;
			//make a clone of the request message
			var cloneRequestMessage = cloneHttpRequestMessage(httpRequestMessage).Result;
			//send http message
			var httpResponseMessage = client.SendAsync(httpRequestMessage).Result;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
            	if (!useBasicAuthentication && !askUser)
            	{
            		//if it didn't work using default authentication then try using basic authentication
            		httpRequestMessage = cloneHttpRequestMessage(cloneRequestMessage).Result;
            		httpResponseMessage = sendToTFS(client,httpRequestMessage,true);
                }
            	if (useBasicAuthentication && !askUser)
            	{
            		httpRequestMessage = cloneHttpRequestMessage(cloneRequestMessage).Result;
            		httpResponseMessage = sendToTFS(client,httpRequestMessage,true,true);
            	}
            }
            return httpResponseMessage;
		}
		/// <summary>
		/// clone the httpRequestMessage in order to be able to send it again if it failed the first time
		/// </summary>
		/// <param name="req">the httpRequestMessage</param>
		/// <returns></returns>
		private static async Task<HttpRequestMessage> cloneHttpRequestMessage(HttpRequestMessage req)
	    {
	        HttpRequestMessage clone = new HttpRequestMessage(req.Method, req.RequestUri);
	
	        // Copy the request's content (via a MemoryStream) into the cloned object
	        var ms = new MemoryStream();
	        if (req.Content != null)
	        {
	            await req.Content.CopyToAsync(ms).ConfigureAwait(false);
	            ms.Position = 0;
	            clone.Content = new StreamContent(ms);
	
	            // Copy the content headers
	            if (req.Content.Headers != null)
	                foreach (var h in req.Content.Headers)
	                    clone.Content.Headers.Add(h.Key, h.Value);
	        }
	
	
	        clone.Version = req.Version;
	
	        foreach (KeyValuePair<string, object> prop in req.Properties)
	            clone.Properties.Add(prop);
	
	        foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
	            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
	
	        return clone;
	    }
		private AuthenticationHeaderValue getBasicAuthorization(bool askUser)
		{
			AuthenticationHeaderValue authenticationHeader = null;
			if (! string.IsNullOrEmpty(settings.defaultPassword) && ! askUser)
			{
				authenticationHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.defaultUserName+ ":" + settings.defaultPassword)));
			}
			else
			{
				//ask user for password
				GetAuthorizationForm getAuthorizationForm = new GetAuthorizationForm();
				getAuthorizationForm.userName = settings.defaultUserName;
				DialogResult dialogResponse;
				if (this.wrappedPackage != null) 
				{
					dialogResponse = getAuthorizationForm.ShowDialog(this.wrappedPackage.EAModel.mainEAWindow);
				}
				else
				{
					dialogResponse = getAuthorizationForm.ShowDialog();
				}
				if (dialogResponse == DialogResult.OK)
				{
					settings.defaultPassword = getAuthorizationForm.passWord;
					authenticationHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(getAuthorizationForm.userName + ":" + settings.defaultPassword)));
				}
				else
				{
					settings.defaultPassword = null;
					throw new OperationCanceledException("User pressed cancel");
				}
			}
			return authenticationHeader;
		}
		public ListofWorkItemsResponse.WorkItems GetListOfWorkItems_ByIDs(string ids, HttpClient client)
        {
            ListofWorkItemsResponse.WorkItems viewModel = new ListofWorkItemsResponse.WorkItems();
            HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&api-version=2.2").Result;
                        
            if (response.IsSuccessStatusCode)
            {
                viewModel = response.Content.ReadAsAsync<ListofWorkItemsResponse.WorkItems>().Result;
            }
            else
            {
            	Logger.logError("Could not get workitems from TFS because of error \nStatuscode: " 
	                                + response.StatusCode + " Reasonphrase: " +response.ReasonPhrase);
            }
            viewModel.HttpStatusCode = response.StatusCode;

            return viewModel;
            
        }
		
	}
}
