using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nest;

namespace MessageHandleApi.Models
{
    public class ELSresult
    {
        public int Time { get; set; }
        public int Document { set; get; }
        public int Total { set; get; }
        public int AggTotal { get; set; }
        public double MaxScore { get; set; }
        public IEnumerable<IHit<logs>> Data { set; get; }
        public IList<KeyItem> AggData { get; set; }     
        public IList<HistogramItem> DateHistData { get; set; }
        public IList<RangeItem> GeoAggdata { get; set; }
        public ArrayList AutoData { get; set; }
    }
}