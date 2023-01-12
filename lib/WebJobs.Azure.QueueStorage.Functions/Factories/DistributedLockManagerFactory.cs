using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using WebJobs.Azure.QueueStorage.Functions.Services.Singleton;

namespace WebJobs.Azure.QueueStorage.Functions.Factories
{
    internal sealed class DistributedLockManagerFactory : IDistributedLockManagerFactory
    {
        public IDistributedLockManager Create(BlobClient blobClient, ILogger logger)
        {
            return new DistributedLockManager(blobClient, logger);
        }
    }
}
