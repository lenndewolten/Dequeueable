using Azure.Storage.Queues.Models;
using JobHandlers.AzureQueueMessage.Configurations;
using Microsoft.Extensions.Options;

namespace JobHandlers.AzureQueueMessage.Services.Retrievers
{
    internal class QueueMessageRetriever : IQueueMessageRetriever
    {
        private readonly QueueClientProvider _queueClientProvider;
        private readonly StorageAccountOptions _storageAccountOptions;

        public QueueMessageRetriever(QueueClientProvider queueClientProvider,
            IOptions<StorageAccountOptions> storageAccountOptionsAccessor)
        {
            _queueClientProvider = queueClientProvider;
            _storageAccountOptions = storageAccountOptionsAccessor.Value;
        }

        public async Task<QueueMessage?> Retrieve(CancellationToken cancellationToken)
        {
            var queueClient = await _queueClientProvider.Get(_storageAccountOptions.QueueName, cancellationToken);
            var message = await queueClient.ReceiveMessageAsync(visibilityTimeout: _storageAccountOptions.VisibilityTimeout, cancellationToken: cancellationToken);

            return message.Value;
        }
    }
}
