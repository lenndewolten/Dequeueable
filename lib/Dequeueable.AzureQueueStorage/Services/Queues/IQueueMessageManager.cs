using Dequeueable.AzureQueueStorage.Models;

namespace Dequeueable.AzureQueueStorage.Services.Queues
{
    internal interface IQueueMessageManager : Dequeueable.Queues.IQueueMessageManager<Message>
    {
        Task EnqueueMessageAsync(Message queueMessage, CancellationToken cancellationToken);
        Task MoveToPoisonQueueAsync(Message queueMessage, CancellationToken cancellationToken);
    }
}
