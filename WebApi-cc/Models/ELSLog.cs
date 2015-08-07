using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class ELSLog
    {
        public string ElsRequest { get; set; }
        public int ElsResponse { get; set; }
        public string ElsIpaddress { get; set; }
        public string ElsUserInfromation { get; set; }
        public StudentInfo student { get; set; }
    }

    public class StudentInfo
    {
        public int studentId { get; set; }
        public int familyId { get; set; }
    }
}