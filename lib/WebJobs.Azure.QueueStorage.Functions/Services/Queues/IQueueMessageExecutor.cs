using WebJobs.Azure.QueueStorage.Functions.Models;

namespace WebJobs.Azure.QueueStorage.Functions.Services.Queues
{
    internal interface IQueueMessageExecutor
    {
        Task ExecuteAsync(Message message, CancellationToken cancellationToken);
    }
}
