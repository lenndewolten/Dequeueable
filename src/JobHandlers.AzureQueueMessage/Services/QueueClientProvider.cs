using Azure.Storage.Queues;
using JobHandlers.AzureQueueMessage.Services.Retrievers;
using System.Collections.Concurrent;

namespace JobHandlers.AzureQueueMessage.Services
{
    internal class QueueClientProvider
    {
        private readonly IQueueClientRetriever _queueClientRetriever;
        private readonly ConcurrentDictionary<string, QueueClient> _clients = new(StringComparer.OrdinalIgnoreCase);

        public QueueClientProvider(IQueueClientRetriever queueClientRetriever)
        {
            _queueClientRetriever = queueClientRetriever;
        }

        public async Task<QueueClient> Get(string? queueName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new InvalidOperationException("Invalid Queue Name. Make sure that it is defined in the app settings");
            }

            var queueClient = _clients.GetOrAdd(queueName, key => _queueClientRetriever.Retrieve(key));

            // Check/Create if the queue still exist, it could be deleted in the mean time.
            await queueClient!.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            return queueClient;
        }
    }
}
