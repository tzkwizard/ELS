using Microsoft.WindowsAzure.Storage.Table;

namespace LMS.model.Models
{
    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public CustomerEntity() { }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }

    public class TableChat : TableEntity
    {
         public TableChat(string roomId, string messageId)
        {
            this.PartitionKey = roomId;
            this.RowKey = messageId;
        }

         public TableChat() { }
         public string roomName { get; set; }
         public string message { get; set; }
         public string user { get; set; }
         public string uid { get; set; }
         public long timestamp { get; set; }

    }
    public class TablePost : TableEntity
    {
        public TablePost(string type, string messageId)
        {
            this.PartitionKey = type;
            this.RowKey = messageId;
        }

        public TablePost() { }
        public string district { get; set; }
        public string school { get; set; }
        public string classes { get; set; }
        public string unit { get; set; }

        public string message { get; set; }
        public string user { get; set; }
        public string uid { get; set; }
        public long timestamp { get; set; }

    }
}
