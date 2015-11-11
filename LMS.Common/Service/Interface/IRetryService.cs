using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.Practices.TransientFaultHandling;

namespace LMS.Common.Service.Interface
{
    public interface IRetryService
    {
        Task ExecuteWithRetries(Func<object> function);

        Task<ResourceResponse<Document>> ExecuteWithRetries(
            Func<Task<ResourceResponse<Document>>> function);

        Task ExecuteWithRetries(Func<Task> function);

        Task<ResourceResponse<Document>> ExecuteWithRetries(int retryTimes,
            Func<Task<ResourceResponse<Document>>> function);

        Task ExecuteWithRetries(int retryTimes, Func<Task> function);

        Task<StoredProcedureResponse<List<Document>>> ExecuteWithRetries(
            Func<Task<StoredProcedureResponse<List<Document>>>> function);
        RetryPolicy<StorageTransientErrorDetectionStrategy> GetRetryPolicy();

    }
}