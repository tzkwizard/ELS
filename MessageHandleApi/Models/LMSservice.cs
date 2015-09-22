namespace MessageHandleApi.Models
{
    public class ChatMessage
    {
        public string Type { get; set; }
        public Info Info { get; set; }
        public ChatRoom Room { get; set; }
    }

    public class ChatRoom
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PostMessage
    {
        public string Type { get; set; }
        public PostPath Path { get; set; }
        public Info Info { get; set; }
      
    }

    public class PostPath
    {
        public string District { get; set; }
        public string School { get; set; }
        public string Classes { get; set; }
    }
    public class Info
    {
        public string message { get; set; }
        public string user { get; set; }
        public string timestamp { get; set; }
        public string uid { get; set; }
    }



}
