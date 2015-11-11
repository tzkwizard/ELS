using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.Practices.TransientFaultHandling;

namespace LMS.Common.Service
{
    public class RetryService
    {
        private const int RetryTimes = 4;
        public static async Task ExecuteWithRetries(Func<object> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);
            var retryTimes = RetryTimes;
            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    function();
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
        }

        public static async Task<ResourceResponse<Document>> ExecuteWithRetries(
            Func<Task<ResourceResponse<Document>>> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);
            var retryTimes = RetryTimes;
            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    return await function();
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                    if (e.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
            return null;
        }

        public static async Task ExecuteWithRetries(
            Func<Task> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);
            var retryTimes = RetryTimes;
            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    await function();
                    break;
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
        }

        public static async Task<ResourceResponse<Document>> ExecuteWithRetries(int retryTimes,
            Func<Task<ResourceResponse<Document>>> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);

            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    return await function();
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
            return null;
        }

        public static async Task ExecuteWithRetries(int retryTimes,
            Func<Task> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);

            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    await function();
                    break;
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
        }

        public static async Task<StoredProcedureResponse<List<Document>>> ExecuteWithRetries(
            Func<Task<StoredProcedureResponse<List<Document>>>> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);
            var retryTimes = RetryTimes;
            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    return await function();
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
            return null;
        }

        public static RetryPolicy<StorageTransientErrorDetectionStrategy> GetRetryPolicy()
        {
            ExponentialBackoff retryStrategy = new ExponentialBackoff(5, TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2));

            RetryPolicy<StorageTransientErrorDetectionStrategy> retryPolicy =
                new RetryPolicy<StorageTransientErrorDetectionStrategy>(retryStrategy);
            retryPolicy.Retrying += (sender, args) =>
            {
                // Log details of the retry.
                var msg = String.Format("Retry - Count:{0}, Delay:{1}, Exception:{2}",
                    args.CurrentRetryCount, args.Delay, args.LastException);
                Trace.TraceInformation(msg);
            };
            return retryPolicy;
        }
    }
}