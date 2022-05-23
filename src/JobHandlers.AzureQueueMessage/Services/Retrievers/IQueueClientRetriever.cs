using Azure.Storage.Queues;

namespace JobHandlers.AzureQueueMessage.Services.Retrievers
{
    internal interface IQueueClientRetriever
    {
        QueueClient Retrieve(string queueName);
    }
}