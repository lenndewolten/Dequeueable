using WebJobs.Azure.QueueStorage.Functions.Models;

namespace WebJobs.Azure.QueueStorage.Functions
{
    public interface IAzureQueueFunction
    {
        Task ExecuteAsync(Message message, CancellationToken cancellationToken);
    }
}
