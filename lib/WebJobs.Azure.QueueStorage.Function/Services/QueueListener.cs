using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebJobs.Azure.QueueStorage.Core.Models;
using WebJobs.Azure.QueueStorage.Core.Services.Queues;
using WebJobs.Azure.QueueStorage.Core.Services.Timers;
using WebJobs.Azure.QueueStorage.Function.Configurations;

namespace WebJobs.Azure.QueueStorage.Function.Services
{
    internal sealed class QueueListener : IQueueListener
    {

        private readonly IDelayStrategy _delayStrategy;

        private readonly List<Task> _processing = new();
        private readonly IQueueMessageManager _messagesManager;
        private readonly IQueueMessageHandler _queueMessageHandler;
        private readonly ILogger<QueueListener> _logger;
        private readonly FunctionHostOptions _options;

        public QueueListener(
            IQueueMessageManager messagesManager,
            IQueueMessageHandler queueMessageHandler,
            IOptions<FunctionHostOptions> options,
            ILogger<QueueListener> logger)
        {
            _messagesManager = messagesManager;
            _queueMessageHandler = queueMessageHandler;
            _logger = logger;
            _options = options.Value;
            _delayStrategy = new RandomizedExponentialDelayStrategy(TimeSpan.FromMilliseconds(_options.MinimumPollingIntervalInMilliseconds),
                TimeSpan.FromMilliseconds(_options.MaximumPollingIntervalInMilliseconds),
                _options.DeltaBackOff);
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            try
            {
                var messages = (await _messagesManager.RetrieveMessagesAsync(cancellationToken)).ToArray();
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

        private Task HandleMessages(Message[] messages, CancellationToken cancellationToken)
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
            while (_processing.Count > _options.NewBatchThreshold && !cancellationToken.IsCancellationRequested)
            {
                var processed = await Task.WhenAny(_processing);
                _processing.Remove(processed);
            }
        }
    }
}
