using System.Collections.Generic;
using System.Configuration;
using Firebase;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.Azure;
using Microsoft.Azure.Documents.Client;

namespace LMS.Common.DAL
{
    public class FBoperation : IFBoperation
    {
        private static string _firebaseSecret;
      
        public FBoperation()
        {
            _firebaseSecret = ConfigurationManager.AppSettings["FirebaseSecret"]??CloudConfigurationManager.GetSetting("FirebaseSecret");
        }

        public string GetFirebaseToken(string user, string uid, string data)
        {
            var tokenGenerator = new TokenGenerator(_firebaseSecret);
            var authPayload = new Dictionary<string, object>()
            {
                {"uid", uid},
                {"user", user},
                {"data", data}
            };
            string token = tokenGenerator.CreateToken(authPayload);
            return token;
        }

        public IFirebaseClient GetFirebaseClient()
        {
            //var node = "https://dazzling-inferno-4653.firebaseio.com/";
            var node =  ConfigurationManager.AppSettings["FirebaseUrl"]??CloudConfigurationManager.GetSetting("FirebaseUrl");
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = _firebaseSecret,
                BasePath = node
            };
            IFirebaseClient client = new FirebaseClient(config);
            return client;
        }

        public IFirebaseClient GetFirebaseClient(string node)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = _firebaseSecret,
                BasePath = node
            };
            IFirebaseClient client = new FirebaseClient(config);
            return client;
        }
    }
}