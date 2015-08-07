using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class FoodModel
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public IEnumerable<MeasureModel> Measures { get; set; }
    }
}