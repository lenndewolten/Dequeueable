using Azure.Core;
using Azure.Storage.Queues;

namespace WebJobs.Azure.QueueStorage.Core.Configurations
{
    public interface IHostOptions
    {
        string? AccountName { get; set; }
        TokenCredential? AuthenticationScheme { get; set; }
        int BatchSize { get; set; }
        string? ConnectionString { get; set; }
        long MaxDequeueCount { get; set; }
        string PoisonQueueName { get; }
        string PoisonQueueSuffix { get; set; }
        QueueClientOptions? QueueClientOptions { get; set; }
        string QueueName { get; set; }
        string QueueUriFormat { get; set; }
        TimeSpan VisibilityTimeout { get; set; }
    }
}