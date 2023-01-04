using WebJobs.Azure.QueueStorage.Core.Models;

namespace WebJobs.Azure.QueueStorage.Core
{
    public interface IAzureQueueFunction
    {
        Task ExecuteAsync(Message message, CancellationToken cancellationToken);
    }
}
