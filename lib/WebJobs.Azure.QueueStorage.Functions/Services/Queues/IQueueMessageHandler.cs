using WebJobs.Azure.QueueStorage.Functions.Models;

namespace WebJobs.Azure.QueueStorage.Functions.Services.Queues
{
    internal interface IQueueMessageHandler
    {
        Task HandleAsync(Message message, CancellationToken cancellationToken);
    }
}
