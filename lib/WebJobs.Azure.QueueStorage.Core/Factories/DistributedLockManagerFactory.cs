using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using WebJobs.Azure.QueueStorage.Core.Services.Singleton;

namespace WebJobs.Azure.QueueStorage.Core.Factories
{
    internal sealed class DistributedLockManagerFactory : IDistributedLockManagerFactory
    {
        public IDistributedLockManager Create(BlobClient blobClient, ILogger logger)
        {
            return new DistributedLockManager(blobClient, logger);
        }
    }
}
