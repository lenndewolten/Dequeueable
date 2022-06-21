using Azure.Storage.Queues.Models;

namespace JobHandlers.AzureQueueMessage.Services
{
    public interface IQueueMessageExecutor
    {
        public Task Execute(QueueMessage message, CancellationToken cancellationToken);
    }
}
