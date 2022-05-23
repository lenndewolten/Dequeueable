using Azure.Storage.Queues.Models;
using JobHandlers.AzureQueueMessage.Configurations;
using Microsoft.Extensions.Options;

namespace JobHandlers.AzureQueueMessage.Services.Deleters
{
    internal class QueueMessageDeleter : IQueueMessageDeleter
    {
        private readonly QueueClientProvider _queueClientProvider;
        private readonly string? _queueName;

        public QueueMessageDeleter(QueueClientProvider queueClientProvider,
            IOptions<StorageAccountOptions> storageAccountOptionsAccessor)
        {
            _queueClientProvider = queueClientProvider;
            _queueName = storageAccountOptionsAccessor.Value.QueueName;
        }

        public async Task Delete(QueueMessage queueMessage, CancellationToken cancellationToken)
        {
            var queueClient = await _queueClientProvider.Get(_queueName, cancellationToken);

            await queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, cancellationToken);
        }


    }
}
