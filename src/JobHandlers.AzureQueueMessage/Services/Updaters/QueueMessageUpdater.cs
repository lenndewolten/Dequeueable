using Azure.Storage.Queues.Models;
using JobHandlers.AzureQueueMessage.Configurations;
using JobHandlers.AzureQueueMessage.Services.Deleters;
using Microsoft.Extensions.Options;

namespace JobHandlers.AzureQueueMessage.Services.Updaters
{
    internal class QueueMessageUpdater : IQueueMessageUpdater
    {
        private readonly QueueClientProvider _queueClientProvider;
        private readonly IQueueMessageDeleter _queueMessageDeleter;
        private readonly StorageAccountOptions _storageAccountOptions;

        public QueueMessageUpdater(QueueClientProvider queueClientProvider,
            IQueueMessageDeleter queueMessageDeleter,
            IOptions<StorageAccountOptions> storageAccountOptionsAccessor)
        {
            _queueClientProvider = queueClientProvider;
            _queueMessageDeleter = queueMessageDeleter;
            _storageAccountOptions = storageAccountOptionsAccessor.Value;
        }

        public async Task Enqueue(QueueMessage queueMessage, CancellationToken cancellationToken)
        {
            var queueClient = await _queueClientProvider.Get(_storageAccountOptions.QueueName, cancellationToken);

            await queueClient.UpdateMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, queueMessage.Body, TimeSpan.Zero, cancellationToken);
        }

        public async Task MoveToPoisonQueue(QueueMessage queueMessage, CancellationToken cancellationToken)
        {
            var poisonQueueClient = await _queueClientProvider.Get(_storageAccountOptions.PoisonQueueName, cancellationToken);

            await poisonQueueClient.SendMessageAsync(queueMessage.Body, cancellationToken: cancellationToken);
            await _queueMessageDeleter.Delete(queueMessage, CancellationToken.None);
        }
    }
}
