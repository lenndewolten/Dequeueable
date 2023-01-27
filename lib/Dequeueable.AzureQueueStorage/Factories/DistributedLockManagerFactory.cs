using Azure.Storage.Blobs;
using Dequeueable.AzureQueueStorage.Services.Singleton;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AzureQueueStorage.Factories
{
    internal sealed class DistributedLockManagerFactory : IDistributedLockManagerFactory
    {
        public IDistributedLockManager Create(BlobClient blobClient, ILogger logger)
        {
            return new DistributedLockManager(blobClient, logger);
        }
    }
}
