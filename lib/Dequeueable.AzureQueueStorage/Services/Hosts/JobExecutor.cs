using Dequeueable.AzureQueueStorage.Models;
using Dequeueable.AzureQueueStorage.Services.Queues;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AzureQueueStorage.Services.Hosts
{
    internal sealed class JobExecutor : IHost
    {
        private readonly List<Task> _processing = new();
        private readonly IQueueMessageManager _messagesManager;
        private readonly IQueueMessageHandler _queueMessageHandler;
        private readonly ILogger<JobExecutor> _logger;

        public JobExecutor(
            IQueueMessageManager messagesManager,
            IQueueMessageHandler queueMessageHandler,
            ILogger<JobExecutor> logger)
        {
            _messagesManager = messagesManager;
            _queueMessageHandler = queueMessageHandler;
            _logger = logger;
        }

        public async Task HandleAsync(CancellationToken cancellationToken)
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

            return;
        }

        private Task HandleMessages(Message[] messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var task = _queueMessageHandler.HandleAsync(message, cancellationToken);
                _processing.Add(task);
            }

            return Task.WhenAll(_processing);
        }
    }
}
