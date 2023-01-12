using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using WebJobs.Azure.QueueStorage.Functions.Services.Singleton;

namespace WebJobs.Azure.QueueStorage.Functions.Factories
{
    internal interface IDistributedLockManagerFactory
    {
        IDistributedLockManager Create(BlobClient blobClient, ILogger logger);
    }
}