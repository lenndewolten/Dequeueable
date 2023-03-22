using Dequeueable.AmazonSQS.Configurations;
using Dequeueable.AmazonSQS.Services.Queues;
using Dequeueable.AmazonSQS.Services.Timers;
using Dequeueable.AzureQueueStorage.Services.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dequeueable.AmazonSQS.Services.Hosts
{
    internal sealed class QueueListenerExecutor : IHostExecutor
    {
        private readonly IQueueMessageManager _queueMessageManager;
        private readonly IQueueMessageHandler _queueMessageHandler;
        private readonly ILogger<QueueListenerExecutor> _logger;
        private readonly ListenerHostOptions _options;
        private readonly IDelayStrategy _delayStrategy;
        private readonly List<Task> _processing = new();

        public QueueListenerExecutor(IQueueMessageManager queueMessageManager,
            IQueueMessageHandler queueMessageHandler,
            IOptions<ListenerHostOptions> options,
            ILogger<QueueListenerExecutor> logger)
        {
            _queueMessageManager = queueMessageManager;
            _queueMessageHandler = queueMessageHandler;
            _logger = logger;
            _options = options.Value;
            _delayStrategy = new RandomizedExponentialDelayStrategy(TimeSpan.FromMilliseconds(_options.MinimumPollingIntervalInMilliseconds),
                TimeSpan.FromMilliseconds(_options.MaximumPollingIntervalInMilliseconds),
                _options.DeltaBackOff);
        }

        public async Task HandleAsync(CancellationToken cancellationToken)
        {
            try
            {
                var messages = await _queueMessageManager.RetrieveMessagesAsync(cancellationToken: cancellationToken);
                var messagesFound = messages.Length > 0;
                if (messagesFound)
                {
                    await HandleMessages(messages!, cancellationToken);
                }
                else
                {
                    _logger.LogDebug("No messages found");
                }

                await WaitForDelay(messagesFound, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occured");
                throw;
            }
        }

        private Task HandleMessages(Models.Message[] messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var task = _queueMessageHandler.HandleAsync(message, cancellationToken);
                _processing.Add(task);
            }

            return WaitForNewBatchThreshold(cancellationToken);
        }

        private Task WaitForDelay(bool messageFound, CancellationToken cancellationToken)
        {
            var delay = _delayStrategy.GetNextDelay(executionSucceeded: messageFound);
            return Task.Delay(delay, cancellationToken);
        }

        private async Task WaitForNewBatchThreshold(CancellationToken cancellationToken)
        {
            var newBatchThreshold = _options.NewBatchThreshold ?? Convert.ToInt32(Math.Ceiling(_options.BatchSize / (double)2));

            while (_processing.Count > newBatchThreshold && !cancellationToken.IsCancellationRequested)
            {
                var processed = await Task.WhenAny(_processing);
                _processing.Remove(processed);
            }
        }
    }
}
