

namespace WebApi
{
    /*public class Module 
    {
        // private ILogHandler _ilogHandler;
        private const string Userlog = "Userlog";
        private ConnectionMultiplexer _connection;
        private IDatabase _cache;
        private readonly HttpRequest _request;
        private readonly HttpResponse _response;
        private ELSLog esLog;
        public Module(HttpRequest request,HttpResponse response)
        {
            _request = request;
            _response = response;
            esLog = new ELSLog();
        }

        public void CallMessageApi()
        {
            String message = "";

            esLog.ElsResponse = 202;
            esLog.ElsIpaddress = _request.UserHostAddress;
            esLog.ElsRequest =
                "[" + DateTime.Now.ToString("dd/MMM/yyyy:HH:mm:ss.FFF zz") + "]" + " \"" + _request.HttpMethod + " "
                + _request.Path + "\" " + _response.StatusCode + " " + _request.TotalBytes + " \"" +
                _request.UrlReferrer + "\" " + "\"" + _request.UserAgent + "\"" + " " + _request.Form;

            message += esLog.ElsIpaddress + " - - " + esLog.ElsRequest + " " + esLog.ElsResponse + " none";


            try
            {

                var client = new MessageHandleApi();
                client.LogMessage.Post(message);
                /*ILogHandler ilogHandler = new LogHandler();
                ilogHandler.SendQueueAsyncAll(1);#1#

            }
            catch (Exception)
            {

               // Task.Run(() => Redis(message));
                string a = "44";
            }
        }

    
        private async Task Redis(string message)
        {
            if (_connection == null)
            {
                _connection =
                    ConnectionMultiplexer.Connect(ConfigurationManager.ConnectionStrings["AzureRedis"].ToString());
                if (_cache == null)
                {
                    _cache = _connection.GetDatabase();

                }
            }
            await _cache.ListLeftPushAsync("s", message);
        }

      






    }*/
}