using Azure.Storage.Queues;

namespace WebJobs.Azure.QueueStorage.Functions.Services.Queues
{
    public interface IQueueClientProvider
    {
        QueueClient GetQueue();
        QueueClient GetPoisonQueue();
    }
}