using Dequeueable.Configurations;
using Dequeueable.Queues;
using Dequeueable.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dequeueable.Hosts
{
    internal sealed class QueueListenerExecutor<TMessage, TOptions>(
        IQueueMessageManager<TMessage> messagesManager,
        IQueueMessageHandler<TMessage> queueMessageHandler,
        ILogger<QueueListenerExecutor<TMessage, TOptions>> logger,
        IOptions<TOptions> options) : IHostExecutor
            where TMessage : class
            where TOptions : class, IListenerHostOptions
    {

        private RandomizedExponentialDelayStrategy DelayStrategy => new(TimeSpan.FromMilliseconds(_options.MinimumPollingIntervalInMilliseconds),
                TimeSpan.FromMilliseconds(_options.MaximumPollingIntervalInMilliseconds),
                _options.DeltaBackOff);

        private readonly List<Task> _processing = [];
        private readonly TOptions _options = options.Value;

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

        private Task HandleMessages(TMessage[] messages, CancellationToken cancellationToken)
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