using System.Collections.Generic;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using LMS.Common.DAL;
using LMS.Common.Models;
using LMS.Common.Models.Api;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace LMS.Common.Service.Interface
{
    public interface ICollectionService
    {
        Task CollectionTransfer(DocumentCollection dc1, DocumentCollection dc2);
        Task UpdateCurrentCollection(DocumentCollection newDc);
      
    }
}