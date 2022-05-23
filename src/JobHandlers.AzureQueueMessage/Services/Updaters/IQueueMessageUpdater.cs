using Azure.Storage.Queues.Models;

namespace JobHandlers.AzureQueueMessage.Services.Updaters
{
    internal interface IQueueMessageUpdater
    {
        Task Enqueue(QueueMessage queueMessage, CancellationToken cancellationToken);
        Task MoveToPoisenQueue(QueueMessage queueMessage, CancellationToken cancellationToken);
    }
}