using WebJobs.Azure.QueueStorage.Core.Models;

namespace WebJobs.Azure.QueueStorage.Core.Services.Queues
{
    public interface IQueueMessageHandler
    {
        Task HandleAsync(Message message, CancellationToken cancellationToken);
    }
}
