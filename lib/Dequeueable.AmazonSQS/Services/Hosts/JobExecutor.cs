using Dequeueable.AmazonSQS.Services.Queues;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AmazonSQS.Services.Hosts
{
    internal sealed class JobExecutor : IHostExecutor
    {
        private readonly IQueueMessageManager _queueMessageManager;
        private readonly IQueueMessageHandler _queueMessageHandler;
        private readonly ILogger<JobExecutor> _logger;

        private readonly List<Task> _processing = new();

        public JobExecutor(IQueueMessageManager queueMessageManager, IQueueMessageHandler queueMessageHandler, ILogger<JobExecutor> logger)
        {
            _queueMessageManager = queueMessageManager;
            _queueMessageHandler = queueMessageHandler;
            _logger = logger;
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

                return;
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

            return Task.WhenAll(_processing);
        }
    }
}
