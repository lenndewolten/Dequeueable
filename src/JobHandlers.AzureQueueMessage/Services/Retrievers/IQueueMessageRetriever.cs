using Azure.Storage.Queues.Models;

namespace JobHandlers.AzureQueueMessage.Services.Retrievers
{
    internal interface IQueueMessageRetriever
    {
        Task<QueueMessage> Retrieve(CancellationToken cancellationToken);
    }
}