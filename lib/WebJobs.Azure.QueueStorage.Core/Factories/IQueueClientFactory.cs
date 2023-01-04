using Azure.Core;
using Azure.Storage.Queues;

namespace WebJobs.Azure.QueueStorage.Core.Factories
{
    public interface IQueueClientFactory
    {
        QueueClient Create(string connectionString, string queueName, QueueClientOptions? queueClientOptions = null);
        QueueClient Create(Uri uri, TokenCredential tokenCredential, QueueClientOptions? queueClientOptions = null);
    }
}