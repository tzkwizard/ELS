using System.Runtime.Serialization;

namespace LMS.model.Models
{
    [DataContract]
    public class MetricEvent
    {
        [DataMember]
        public int DeviceId { get; set; }

        [DataMember]
        public int Temperature { get; set; }
    }

    public class test
    {
        public string url { get; set; }
        public body body { get; set; }
    }

    public class body
    {
          public string user { get; set; }
          public string uid { get; set; }
          public string message { get; set; }
          public string timestamp { get; set; }

    }
    public class EHdata
    {
        public string offset { get; set; }
        public string body { get; set; }
        public string partitionId { get; set; }
    }
}