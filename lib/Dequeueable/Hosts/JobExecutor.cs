using Dequeueable.Models;
using Dequeueable.Queues;
using Microsoft.Extensions.Logging;

namespace Dequeueable.Hosts
{
    internal sealed class JobExecutor<TMessage>(
        IQueueMessageManager<TMessage> messagesManager,
        IQueueMessageHandler<TMessage> queueMessageHandler,
        ILogger<JobExecutor<TMessage>> logger) : IHostExecutor
        where TMessage : class, IQueueMessage
    {
        private readonly List<Task> _processing = [];

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

            return;
        }

        private Task HandleMessages(TMessage[] messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var task = queueMessageHandler.HandleAsync(message, cancellationToken);
                _processing.Add(task);
            }

            return Task.WhenAll(_processing);
        }
    }
}
