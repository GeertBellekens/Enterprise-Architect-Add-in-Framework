
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
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.WorkTracking.TFS
{
	/// <summary>
	/// Description of Project.
	/// </summary>
	public class TFSProject:Project
	{
		 //TFS constants
        const string tfsCollection = "DefaultCollection/";
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
			this.TFSUrl = TFSUrl + tfsCollection;
		}
		public TFSProject(string projectName,string TFSUrl, TFSSettings settings)
		{
			this.settings = settings;
			this.name = projectName;
			this.TFSUrl = TFSUrl + tfsCollection;
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
			foreach (var workitemElement in ((Package)ownerPackage).model.getElementWrappersByQuery(getWorkitemsSQL))
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
            var wiql = new
            {	
                query = "Select [State], [Title] " +
                        "From WorkItems " +
                	"Where [Work Item Type] in ( '"+ string.Join("','", settings.workitemMappings.Values.ToArray()) +"' )" +
                		"AND [System.TeamProject] = '" + this.name + "' "	+
                        "Order By [State] Asc, [Changed Date] Desc"
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
		private HttpResponseMessage postWiql(HttpClient client, object wiql,bool useBasicAuthentication = false, bool askUser = false)
		{
			if ( useBasicAuthentication) 
			{
				authorization = getBasicAuthorization(askUser);
				if (authorization != null ) client.DefaultRequestHeaders.Authorization = authorization;
			}
			//user pressed cancel
			if (useBasicAuthentication && authorization == null) return null;

            // serialize the wiql object into a json string   
            var postValue = new StringContent(JsonConvert.SerializeObject(wiql), Encoding.UTF8, "application/json"); // mediaType needs to be application/json for a post call

            // set the httpmethod to PPOST
            var method = new HttpMethod("POST");

            // send the request               
            var httpRequestMessage = new HttpRequestMessage(method, TFSUrl + "_apis/wit/wiql?api-version=2.2") { Content = postValue };
            var httpResponseMessage = client.SendAsync(httpRequestMessage).Result;
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
            	if (!useBasicAuthentication && !askUser)
            	{
            		//if it didn't work using default authentication then try using basic authentication
            		httpResponseMessage = postWiql(client,wiql,true);
                }
            	if (useBasicAuthentication && !askUser)
            	{
            		httpResponseMessage = postWiql(client,wiql,true,true);
            	}
            }
            return httpResponseMessage;
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
				var dialogResponse = getAuthorizationForm.ShowDialog();
				if (dialogResponse == DialogResult.OK)
				{
					settings.defaultPassword = getAuthorizationForm.passWord;
					authenticationHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(getAuthorizationForm.userName + ":" + settings.defaultPassword)));
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
            viewModel.HttpStatusCode = response.StatusCode;

            return viewModel;
            
        }
		
	}
}
