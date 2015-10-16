using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Partitioning;
using Newtonsoft.Json;

namespace LMS.model.Models
{

    public class CurrentCollection
    {
        public string id { get; set; }
        public string name { get; set; }
        public string _self { get; set; }
    }

    public class RangeResolver
    {
        public string id { get; set; }
        public RangePartitionResolver<long> resolver { get; set; }
        public string resolverString { get; set; }
    }

    public class DcAllocate : Document
    {
        public string Type { get; set; }
        public string DcName { get; set; }
        public string DcSelfLink { get; set; }
        public List<string> District { get; set; }
        public string _self { get; set; }
    }
    public class DClist
    {
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public class ChatMessage
    {
        public string Type { get; set; }
        public Info Info { get; set; }
        public ChatRoom Room { get; set; }

        //public string _self { get; set; }
    }

    public class ChatRoom
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PostMessage
    {
        public string Type { get; set; }
        public PostPath Path { get; set; }
        public Info Info { get; set; }
        public string _self { get; set; }
        public string id { get; set; }
    }

    public class PostPath
    {
        public string District { get; set; }
        public string School { get; set; }
        public string Classes { get; set; }
        public string Unit { get; set; }
    }
    public class Info
    {
        public string message { get; set; }
        public string user { get; set; }
        public long timestamp { get; set; }
        public string uid { get; set; }
    }

    public class Topic
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Type { get; set; }
        public PostPath Path { get; set; }
        public InfoPost Info { get; set; }
        public string _self { get; set; }
    }

    public class InfoPost
    {
        public string teacher { get; set; }
        public string description { get; set; }
        public string start { get; set; }
        public string due { get; set; }
    }
    public class LMSresult
    {
        public bool moreData { get; set; }
        public long time { get; set; }
        public List<PostMessage> list { get; set; } 
    }

    public class LMSChatresult
    {
        public bool moreData { get; set; }
        public long time { get; set; }
        public List<TableChat> list { get; set; }
    }

    public class EHdata
    {
        public string offset { get; set; }
        public string body { get; set; }
        public string partitionId { get; set; }
    }
}
