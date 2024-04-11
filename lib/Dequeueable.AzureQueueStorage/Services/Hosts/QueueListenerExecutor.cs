using Dequeueable.AzureQueueStorage.Configurations;
using Dequeueable.AzureQueueStorage.Models;
using Dequeueable.AzureQueueStorage.Services.Queues;
using Dequeueable.AzureQueueStorage.Services.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dequeueable.AzureQueueStorage.Services.Hosts
{
    internal sealed class QueueListenerExecutor(
        IQueueMessageManager messagesManager,
        IQueueMessageHandler queueMessageHandler,
        IOptions<ListenerHostOptions> options,
        ILogger<QueueListenerExecutor> logger) : IHostExecutor
    {

        private RandomizedExponentialDelayStrategy DelayStrategy => new(TimeSpan.FromMilliseconds(_options.MinimumPollingIntervalInMilliseconds),
                TimeSpan.FromMilliseconds(_options.MaximumPollingIntervalInMilliseconds),
                _options.DeltaBackOff);

        private readonly List<Task> _processing = [];
        private readonly ListenerHostOptions _options = options.Value;

        public async Task HandleAsync(CancellationToken cancellationToken)
        {
            var messages = (await messagesManager.RetrieveMessagesAsync(cancellationToken)).ToArray();
            var messagesFound = messages.Length > 0;
            if (messagesFound)
            {
                await HandleMessages(messages!, cancellationToken);
            }
            else
            {
                logger.LogDebug("No messages found");
            }

            await WaitForDelay(messagesFound, cancellationToken);
        }

        private Task HandleMessages(Message[] messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var task = queueMessageHandler.HandleAsync(message, cancellationToken);
                _processing.Add(task);
            }

            return WaitForNewBatchThreshold(cancellationToken);
        }

        private Task WaitForDelay(bool messageFound, CancellationToken cancellationToken)
        {
            var delay = DelayStrategy.GetNextDelay(executionSucceeded: messageFound);
            return Task.Delay(delay, cancellationToken);
        }

        private async Task WaitForNewBatchThreshold(CancellationToken cancellationToken)
        {
            while (_processing.Count > _options.NewBatchThreshold && !cancellationToken.IsCancellationRequested)
            {
                var processed = await Task.WhenAny(_processing);
                _processing.Remove(processed);
            }
        }
    }
}