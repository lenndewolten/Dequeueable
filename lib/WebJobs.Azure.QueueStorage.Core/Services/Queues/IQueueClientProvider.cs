using Azure.Storage.Queues;

namespace WebJobs.Azure.QueueStorage.Core.Services.Queues
{
    public interface IQueueClientProvider
    {
        QueueClient GetQueue();
        QueueClient GetPoisonQueue();
    }
}