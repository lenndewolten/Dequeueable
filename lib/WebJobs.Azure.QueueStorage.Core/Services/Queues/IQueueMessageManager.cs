using WebJobs.Azure.QueueStorage.Core.Models;

namespace WebJobs.Azure.QueueStorage.Core.Services.Queues
{
    public interface IQueueMessageManager
    {
        Task DeleteMessageAsync(Message queueMessage, CancellationToken cancellationToken);
        Task EnqueueMessageAsync(Message queueMessage, CancellationToken cancellationToken);
        Task MoveToPoisonQueueAsync(Message queueMessage, CancellationToken cancellationToken);
        Task<IEnumerable<Message>> RetrieveMessagesAsync(CancellationToken cancellationToken);
        Task<DateTimeOffset?> UpdateVisibilityTimeOutAsync(Message queueMessage, CancellationToken cancellationToken);
    }
}