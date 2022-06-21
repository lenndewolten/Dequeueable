using Azure.Storage.Queues.Models;

namespace JobHandlers.AzureQueueMessage.Handlers
{
    internal interface IQueueMessageExceptionHandler
    {
        Task Handle(QueueMessage queueMessage, CancellationToken cancellationToken);
    }
}