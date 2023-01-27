using Azure.Storage.Blobs;
using Dequeueable.AzureQueueStorage.Services.Singleton;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AzureQueueStorage.Factories
{
    internal interface IDistributedLockManagerFactory
    {
        IDistributedLockManager Create(BlobClient blobClient, ILogger logger);
    }
}