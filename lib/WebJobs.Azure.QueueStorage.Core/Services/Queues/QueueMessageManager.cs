using Azure.Storage.Queues;
using Azure;
using Azure.Storage.Queues.Models;
using WebJobs.Azure.QueueStorage.Core.Configurations;
using WebJobs.Azure.QueueStorage.Core.Models;

namespace WebJobs.Azure.QueueStorage.Core.Services.Queues
{
    internal sealed class QueueMessageManager : IQueueMessageManager
    {
        private readonly QueueClient _queueClient;
        private readonly QueueClient _poisonQueueClient;
        private readonly IHostOptions _options;

        public QueueMessageManager(IQueueClientProvider queueClientProvider, IHostOptions options)
        {
            _options = options;
            _queueClient = queueClientProvider.GetQueue();
            _poisonQueueClient = queueClientProvider.GetPoisonQueue();
        }

        public async Task<IEnumerable<Message>> RetrieveMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _queueClient.ReceiveMessagesAsync(maxMessages: _options.BatchSize, visibilityTimeout: _options.VisibilityTimeout, cancellationToken);
                return response.Value.Select(m => new Message(m.MessageId, m.PopReceipt, m.DequeueCount, m.NextVisibleOn, m.Body));
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                await CreateQueue(_queueClient, cancellationToken);
                var response = await _queueClient.ReceiveMessagesAsync(maxMessages: _options.BatchSize, visibilityTimeout: _options.VisibilityTimeout, cancellationToken);
                return response.Value.Select(m => new Message(m.MessageId, m.PopReceipt, m.DequeueCount, m.NextVisibleOn, m.Body));
            }
        }

        public async Task<DateTimeOffset?> UpdateVisibilityTimeOutAsync(Message queueMessage, CancellationToken cancellationToken)
        {
            var updateReceipt = (await _queueClient.UpdateMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, visibilityTimeout: _options.VisibilityTimeout, cancellationToken: cancellationToken)).Value;
            queueMessage.PopReceipt = updateReceipt.PopReceipt;

            return updateReceipt.NextVisibleOn;
        }

        public async Task DeleteMessageAsync(Message queueMessage, CancellationToken cancellationToken)
        {
            try
            {
                await _queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, cancellationToken);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
            }
        }

        public async Task EnqueueMessageAsync(Message queueMessage, CancellationToken cancellationToken)
        {
            await _queueClient.UpdateMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, queueMessage.Body, TimeSpan.Zero, cancellationToken);
        }

        public async Task MoveToPoisonQueueAsync(Message queueMessage, CancellationToken cancellationToken)
        {
            await SendMessageAsync(_poisonQueueClient, queueMessage, cancellationToken);
            await DeleteMessageAsync(queueMessage, cancellationToken);
        }

        private static async Task<SendReceipt?> SendMessageAsync(QueueClient queueClient, Message queueMessage, CancellationToken cancellationToken)
        {
            try
            {
                return (await queueClient.SendMessageAsync(queueMessage.Body, cancellationToken: cancellationToken)).Value;
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                await CreateQueue(queueClient, cancellationToken);
                return (await queueClient.SendMessageAsync(queueMessage.Body, cancellationToken: cancellationToken)).Value;
            }
        }

        private static Task CreateQueue(QueueClient queueClient, CancellationToken cancellationToken)
        {
            return queueClient.CreateAsync(cancellationToken: cancellationToken);
        }
    }
}
