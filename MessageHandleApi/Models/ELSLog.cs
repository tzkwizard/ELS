namespace MessageHandleApi.Models
{
    public class ELSLogs
    {
        public LogRequest ElsRequest { get; set; }
        public LogResponse ElsResponse { get; set; }
        public LogUserInfromation ElsUser { get; set; }
     
       
    }

    public class LogUserInfromation
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string District { get; set; }
        public short ConfigSchoolID { get; set; }
        public bool AccountingIsDistrictWide { get; set; }
        public string DatabaseServer { get; set; }
        public int PersonID { get; set; }
        public string SchoolCode { get; set; }
        public int SchoolYearID { get; set; }
    }

    public class LogResponse
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }

    }
    public class LogRequest
    {
        public string IpAddress { get; set; }
        public string Time { get; set; }
        public string HttpMethod { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
        public string TotalBytes { get; set; }
        public string UrlReferrer { get; set; }
        public string UserAgent { get; set; }
        public string Form { get; set; }

    }
}