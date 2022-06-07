using Azure.Core;
using Azure.Storage.Queues;

namespace JobHandlers.AzureQueueMessage.Services.Retrievers
{
    internal interface IQueueClientFactory
    {
        QueueClient Create(string connectionString, string queueName);
        QueueClient Create(string accountName, string queueName, TokenCredential tokenCredential);
    }
}
