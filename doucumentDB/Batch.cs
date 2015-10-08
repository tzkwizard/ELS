using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace doucumentDB
{

    public class Batch
    {
        private static string storageConnectionString =
         "DefaultEndpointsProtocol=https;AccountName=elsaotuo;AccountKey=AV49N0PZ1Qlz42b0w47EPoPbNLULgxYOWxsO4IvFmrAkZPzkdGCKKOJqyiHVGfAPex6HhkDSWpNQAIuPmBHBMA==";

        public static void RunBatch(string[] args)
        {
            //CreateStorage();
            //CreateFiles();
            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials("https://testbatch.southcentralus.batch.azure.com", "testbatch", "O+w9Brj9NKnb2RXt13T0HaJLQ3v8HMf6RuojDkeAHVMTDQ/tdgjvim3pFiiH+ekqWiYppDDQ6M0rvAyqaIIUaw==");
            BatchClient client = BatchClient.Open(cred);
           // CreatePool(client);
            //ListPools(client);
            //CreateJob(client);
           // ListJobs(client);
            DeleteTasks(client);
            AddTasks(client);
            ListTasks(client);
           // DeleteJob(client);
           // DeletePool(client);
        }
        static void DeletePool(BatchClient client)
        {
            client.PoolOperations.DeletePool("testpool1");
            Console.WriteLine("Pool was deleted.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
        private static void DeleteJob(BatchClient client)
        {
            client.JobOperations.DeleteJob("testjob1");
            Console.WriteLine("Job was deleted.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
        private static void DeleteTasks(BatchClient client)
        {
            CloudJob job = client.JobOperations.GetJob("testjob1");
            foreach (CloudTask task in job.ListTasks())
            {
                task.Delete();
            }
            Console.WriteLine("All tasks deleted.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
        private static void ListTasks(BatchClient client)
        {
            IPagedEnumerable<CloudTask> tasks = client.JobOperations.ListTasks("testjob1");
            foreach (CloudTask task in tasks)
            {
                Console.WriteLine("Task id: " + task.Id);
                Console.WriteLine("   Task status: " + task.State);
                Console.WriteLine("   Task start: " + task.ExecutionInformation.StartTime);
            }
            Console.ReadLine();
        }

        private static void AddTasks(BatchClient client)
        {
            CloudJob job = client.JobOperations.GetJob("testjob1");
            ResourceFile programFile = new ResourceFile(
                "https://elsaotuo.blob.core.windows.net/testcon1/ProcessTaskData.exe",
                "ProcessTaskData.exe");
            ResourceFile assemblyFile = new ResourceFile(
                  "https://elsaotuo.blob.core.windows.net/testcon1/Microsoft.WindowsAzure.Storage.dll",
                  "Microsoft.WindowsAzure.Storage.dll");
            for (int i = 1; i < 4; ++i)
            {
                string blobName = "taskdata" + i;
                string taskName = "mytask" + i;
                ResourceFile taskData = new ResourceFile("https://elsaotuo.blob.core.windows.net/testcon1/" +
                  blobName, blobName);
                CloudTask task = new CloudTask(taskName, "ProcessTaskData.exe https://elsaotuo.blob.core.windows.net/testcon1/" +
                  blobName + " 3");
                List<ResourceFile> taskFiles = new List<ResourceFile>();
                taskFiles.Add(taskData);
                taskFiles.Add(programFile);
                taskFiles.Add(assemblyFile);
                task.ResourceFiles = taskFiles;
                job.AddTask(task);
                job.Commit();
                job.Refresh();
            }

            client.Utilities.CreateTaskStateMonitor().WaitAll(job.ListTasks(),
        TaskState.Completed, new TimeSpan(0, 15, 0));
            Console.WriteLine("The tasks completed successfully.");
            
            foreach (CloudTask task in job.ListTasks())
            {
                Console.WriteLine("Task " + task.Id + " says:\n" + task.GetNodeFile(Constants.StandardOutFileName).ReadAsString());
            }
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
        private static void ListJobs(BatchClient client)
        {
            IPagedEnumerable<CloudJob> jobs = client.JobOperations.ListJobs();
            foreach (CloudJob job in jobs)
            {
                Console.WriteLine("Job id: " + job.Id);
                Console.WriteLine("   Job status: " + job.State);
            }
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
        private static CloudJob CreateJob(BatchClient client)
        {
            CloudJob newJob = client.JobOperations.CreateJob();
            newJob.Id = "testjob1";
            newJob.PoolInformation = new PoolInformation() { PoolId = "testpool1" };
            newJob.Commit();
            Console.WriteLine("Created the job. Press Enter to continue.");
            Console.ReadLine();

            return newJob;
        }
        private static void ListPools(BatchClient client)
        {
            IPagedEnumerable<CloudPool> pools = client.PoolOperations.ListPools();
            foreach (CloudPool pool in pools)
            {
                Console.WriteLine("Pool name: " + pool.Id);
                Console.WriteLine("   Pool status: " + pool.State);
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }
        private static void CreatePool(BatchClient client)
        {
            CloudPool newPool = client.PoolOperations.CreatePool(
              "testpool1",
              "3",
              "small",
              3);
            newPool.Commit();
            Console.WriteLine("Created the pool. Press Enter to continue.");
            Console.ReadLine();
        }
        private static void CreateStorage()
        {
            // Get the storage connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                storageConnectionString);

            // Create the container
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("testcon1");
            container.CreateIfNotExists();

            // Set permissions on the container
            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            container.SetPermissions(containerPermissions);
            Console.WriteLine("Created the container. Press Enter to continue.");
            Console.ReadLine();
        }
        private static void CreateFiles()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                storageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("testcon1");

            CloudBlockBlob taskData1 = container.GetBlockBlobReference("taskdata1");
            CloudBlockBlob taskData2 = container.GetBlockBlobReference("taskdata2");
            CloudBlockBlob taskData3 = container.GetBlockBlobReference("taskdata3");
            taskData1.UploadFromFile("..\\..\\taskdata1.txt", FileMode.Open);
            taskData2.UploadFromFile("..\\..\\taskdata2.txt", FileMode.Open);
            taskData3.UploadFromFile("..\\..\\taskdata3.txt", FileMode.Open);

            CloudBlockBlob storageassembly = container.GetBlockBlobReference("Microsoft.WindowsAzure.Storage.dll");
            storageassembly.UploadFromFile("Microsoft.WindowsAzure.Storage.dll", FileMode.Open);

            CloudBlockBlob dataprocessor = container.GetBlockBlobReference("ProcessTaskData.exe");
            dataprocessor.UploadFromFile("..\\..\\..\\ProcessTaskData\\bin\\debug\\ProcessTaskData.exe", FileMode.Open);

            Console.WriteLine("Uploaded the files. Press Enter to continue.");
            Console.ReadLine();
        }
    }
}
