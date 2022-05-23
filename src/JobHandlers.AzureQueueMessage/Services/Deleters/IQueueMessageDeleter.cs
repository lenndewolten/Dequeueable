using Azure.Storage.Queues.Models;

namespace JobHandlers.AzureQueueMessage.Services.Deleters
{
    internal interface IQueueMessageDeleter
    {
        Task Delete(QueueMessage queueMessage, CancellationToken cancellationToken);
    }
}