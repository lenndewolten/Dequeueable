using Dequeueable.AzureQueueStorage.Configurations;
using Dequeueable.AzureQueueStorage.Models;
using Dequeueable.AzureQueueStorage.Services.Queues;
using Dequeueable.AzureQueueStorage.Services.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dequeueable.AzureQueueStorage.Services.Hosts
{
    internal sealed class ListenerHostHandler : IHostHandler
    {

        private readonly IDelayStrategy _delayStrategy;

        private readonly List<Task> _processing = new();
        private readonly IQueueMessageManager _messagesManager;
        private readonly IQueueMessageHandler _queueMessageHandler;
        private readonly ILogger<ListenerHostHandler> _logger;
        private readonly ListenerOptions _options;

        public ListenerHostHandler(
            IQueueMessageManager messagesManager,
            IQueueMessageHandler queueMessageHandler,
            IOptions<ListenerOptions> options,
            ILogger<ListenerHostHandler> logger)
        {
            _messagesManager = messagesManager;
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
