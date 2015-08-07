
using System;
using System.Collections.Generic;

namespace MessageHandleApi.Models
{
    public class QueryInfo
    {
        public string[] Index { get; set; }
        public string Type { get; set; }
        public int Size { get; set; }
        public string SearchText { get; set; }
        public string Field { get; set; }
        public string AggField { get; set; }
        public int SubSize { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Span { get; set; }
        public string[] MultiField { get; set; }
        public FilterInfo[] Filterdata { get; set; }
        public Coordinates Location { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string GeoDistance { get; set; } 
        public string Username { get; set; }
        public string Password { get; set; }
        public string token { get; set; }
    }

    public class Coordinates
    {
        public string Lat { get; set; }
        public string Lon { get; set; }
    }

    public class FilterInfo
    {
        public string Text { get; set; }
        public string Field { get; set; }
        public string Condition { get; set; }
    }
}
