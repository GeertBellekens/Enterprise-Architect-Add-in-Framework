using System;
namespace EAAddinFramework.WorkTracking.TFS
{
	public class QueryResult
    {
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }           
        public string url { get; set; }
    }

    public class WorkItemQueryResult 
    {
        public string queryType { get; set; }
        public string queryResultType { get; set; }
        public DateTime asOf { get; set; }
        public Column[] columns { get; set; }
        public TFSWorkitem[] workItems { get; set; }
    }   

    public class TFSWorkitem
    {
        public int id { get; set; }
        public string url { get; set; }
    }

    public class Column
    {
        public string referenceName { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class AttachmentReference
    {
        public string id { get; set; }
        public string url { get; set; }
    }

    public class WorkItemFields 
    {
        public int count { get; set; }
        public WorkItemField[] value { get; set; }
    }

    public class WorkItemField
    {
        public string name { get; set; }
        public string referenceName { get; set; }
        public string type { get; set; }
        public bool readOnly { get; set; }        
        public string url { get; set; }
    }
    public class WorkItemPatch
    {
        public class Field
        {
            public string op { get; set; }
            public string path { get; set; }
            public object value { get; set; }
        }

        public class Value
        {
            public string rel { get; set; }
            public string url { get; set; }
            public Attributes attributes { get; set; }
        }

        public class Attributes
        {
            public string comment { get; set; }
        }
    }

}