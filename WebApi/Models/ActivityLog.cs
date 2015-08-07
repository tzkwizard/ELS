using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class ActivityLog
    {
        public ActivityLog(string ipAddress,string time,string desciption)
        {
            this.Desciption = desciption;
            this.Time = time;
            this.IpAddress = ipAddress;
        }

        public string IpAddress { get; set; }
        public string Time { get; set; }
        public string Desciption { get; set; } 
    }
}