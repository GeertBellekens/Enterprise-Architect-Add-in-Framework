
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace EAAddinFramework.WorkTracking.TFS
{
	/// <summary>
	/// Description of TFSSettings.
	/// </summary>
	public interface TFSSettings
	{
		Dictionary<string,string> projectConnections{get;set;}
        Dictionary<string,string> workitemMappings{get;set;}
        string defaultProject{get;set;}
        string defaultUserName{get;set;}
        string defaultPassword {get;set;}
        List<string> mappedElementTypes{get;}
        List<string> mappedStereotypes{get;}
        string defaultWorkitemType {get;set;}
        string defaultStatus {get;set;}
        string TFSFilterTag {get;set;}
        string defaultCollection {get;set;}
	}
}
