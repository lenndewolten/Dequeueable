using Azure.Storage.Queues.Models;

namespace JobHandlers.AzureQueueMessage.Services.Updaters
{
    internal interface IQueueMessageUpdater
    {
        Task Enqueue(QueueMessage queueMessage, CancellationToken cancellationToken);
        Task MoveToPoisonQueue(QueueMessage queueMessage, CancellationToken cancellationToken);
    }
}