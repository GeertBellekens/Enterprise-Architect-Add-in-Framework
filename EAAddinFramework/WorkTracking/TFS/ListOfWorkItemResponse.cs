
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;


namespace EAAddinFramework.WorkTracking.TFS
{
	/// <summary>
	/// Description of ListOfWorkItemResponse.
	/// </summary>
	public class ListofWorkItemsResponse
    {
        public class WorkItems
        {
        	public HttpStatusCode HttpStatusCode { get; set; }
            public int count { get; set; }
            public Value[] value { get; set; }            
        }

        public class Value
        {
            public int id { get; set; }
            public int rev { get; set; }
            public Fields fields { get; set; }
            public string url { get; set; }
        }

        public class Fields
        {
            [JsonProperty(PropertyName = "System.AreaPath")]
            public string SystemAreaPath { get; set; }

            [JsonProperty(PropertyName = "System.TeamProject")]
            public string SystemTeamProject { get; set; }

            [JsonProperty(PropertyName = "System.IterationPath")]
            public string SystemIterationPath { get; set; }

            [JsonProperty(PropertyName = "System.WorkItemType")]
            public string SystemWorkItemType { get; set; }

            [JsonProperty(PropertyName = "System.State")]
            public string SystemState { get; set; }

            [JsonProperty(PropertyName = "System.Reason")]
            public string SystemReason { get; set; }

            [JsonProperty(PropertyName = "System.CreatedDate")]
            public DateTime SystemCreatedDate { get; set; }

            [JsonProperty(PropertyName = "System.CreatedBy")]
            public string SystemCreatedBy { get; set; }

            [JsonProperty(PropertyName = "System.ChangedDate")]
            public DateTime SystemChangedDate { get; set; }

            [JsonProperty(PropertyName = "System.ChangedBy")]
            public string SystemChangedBy { get; set; }

            [JsonProperty(PropertyName="System.Title")]
            public string SystemTitle { get; set; }

            [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.Effort")]
            public int MicrosoftVSTSSchedulingEffort { get; set; }

            [JsonProperty(PropertyName = "System.Description")]
            public string SystemDescription { get; set; }

            [JsonProperty(PropertyName = "System.AssignedTo")]
            public string SystemAssignedTo { get; set; }

            [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.RemainingWork")]
            public int MicrosoftVSTSSchedulingRemainingWork { get; set; }

            [JsonProperty(PropertyName = "System.Tags")]
            public string SystemTags { get; set; }

        }       
    }
}
