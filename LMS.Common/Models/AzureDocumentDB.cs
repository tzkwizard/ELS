using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Partitioning;

namespace LMS.Common.Models
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
}