
using System;

namespace WebApi.Models
{
    public class QueryInfo
    {
        public string[] Index { get; set; }
        public string Type { get; set; }
        public int Size { get; set; }
        public string Field { get; set; }
        public string SearchText { get; set; }
        public string AggField { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
