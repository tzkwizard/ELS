using System.Collections.Generic;

namespace LMS.model.Models.Api
{
    public class LMSMessage
    {
        public string url { get; set; }

        public LMSbody body { get; set; }
    }

    public class LMSbody
    {
        public string user { get; set; }
        public string uid { get; set; }
        public bool show { get; set; }
        public string message { get; set; }
        public string comment { get; set; }
        public long timestamp { get; set; }
    }

    public class User
    {
        public string uid { get; set; }
        public string name { get; set; }
    }

    public class LMSresult
    {
        public bool moreData { get; set; }
        public long time { get; set; }
        public List<PostMessage> list { get; set; }
    }

    public class LMSChatresult
    {
        public bool moreData { get; set; }
        public long time { get; set; }
        public List<TableChat> list { get; set; }
    }
}