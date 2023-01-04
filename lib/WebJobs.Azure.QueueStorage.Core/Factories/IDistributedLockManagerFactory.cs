using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using WebJobs.Azure.QueueStorage.Core.Services.Singleton;

namespace WebJobs.Azure.QueueStorage.Core.Factories
{
    internal interface IDistributedLockManagerFactory
    {
        IDistributedLockManager Create(BlobClient blobClient, ILogger logger);
    }
}