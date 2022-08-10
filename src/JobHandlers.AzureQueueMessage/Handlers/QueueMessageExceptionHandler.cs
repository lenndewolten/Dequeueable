using Azure.Storage.Queues.Models;
using JobHandlers.AzureQueueMessage.Configurations;
using JobHandlers.AzureQueueMessage.Services.Updaters;
using Microsoft.Extensions.Options;

namespace JobHandlers.AzureQueueMessage.Handlers
{
    internal sealed class QueueMessageExceptionHandler : IQueueMessageExceptionHandler
    {
        private readonly int _maxDequeueCount;
        private readonly IQueueMessageUpdater _queueMessageUpdater;

        public QueueMessageExceptionHandler(IOptions<StorageAccountOptions> storageAccountOptionsAccessor,
            IQueueMessageUpdater queueMessageUpdater)
        {
            _maxDequeueCount = storageAccountOptionsAccessor.Value.MaxDequeueCount;
            _queueMessageUpdater = queueMessageUpdater;
        }

        public Task Handle(QueueMessage queueMessage, CancellationToken cancellationToken)
        {
            if (queueMessage.DequeueCount >= _maxDequeueCount)
            {
                return _queueMessageUpdater.MoveToPoisonQueue(queueMessage, cancellationToken);
            }

            return _queueMessageUpdater.Enqueue(queueMessage, cancellationToken);
        }
    }
}
