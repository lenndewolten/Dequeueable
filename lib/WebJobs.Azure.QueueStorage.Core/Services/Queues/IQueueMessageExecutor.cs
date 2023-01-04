using WebJobs.Azure.QueueStorage.Core.Models;

namespace WebJobs.Azure.QueueStorage.Core.Services.Queues
{
    public interface IQueueMessageExecutor
    {
        Task ExecuteAsync(Message message, CancellationToken cancellationToken);
    }
}
