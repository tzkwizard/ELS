using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using RenWeb.Framework.Interfaces;
using WebApi.Models;
using WebApi.Services;
using Context = Microsoft.Ajax.Utilities.Context;

namespace WebApi.Controllers
{
    public class ContactController : ApiController
    {
        private readonly ContactRepository _contactRepository;

        public ContactController()
        {
            this._contactRepository = new ContactRepository();
        }
        public Contact[] Get()
        {
            return _contactRepository.GetAllContacts();
        }

        public HttpResponseMessage Post(Contact contact, HttpRequestMessage requestMessage)
        {
            _contactRepository.SaveContact(contact);

            var response = requestMessage.CreateResponse<Contact>(System.Net.HttpStatusCode.Created, contact);
            

            //Queuestore q = new Queuestore();
            
            //string queuename = "hahaha";
            // Create or reference an existing queue 
           // CloudQueue queue = q.CreateQueueAsync(queuename).Result;
         
            int num = Convert.ToInt32(contact.Number);

           
            ITestLogHandler ilogHandler = new TestLogHandler();
      
            ilogHandler.SendQueueAsyncAll(num);

           //q.SendQueueAsyncAll(num, responselog);
            return response;
        }


        
    }


}
