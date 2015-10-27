using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace eventHub
{
    public class firedata
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }
    }
    public class EHdata
    {
        public string offset { get; set; }
        public string body { get; set; }
        public string partitionId { get; set; }
    }
        [DataContract]
        public class MetricEvent
        {
            [DataMember]
            public int DeviceId { get; set; }
            [DataMember]
            public int Temperature { get; set; }
        }
}
