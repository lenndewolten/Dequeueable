using Dequeueable.AmazonSQS.Models;
using Dequeueable.AmazonSQS.Services.Timers;
using Dequeueable.AzureQueueStorage.Services.Timers;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AmazonSQS.Services.Queues
{
    internal sealed class QueueMessageHandler : IQueueMessageHandler
    {
        private readonly ILogger<QueueMessageHandler> _logger;
        private readonly IQueueMessageManager _queueMessageManager;
        private readonly IQueueMessageExecutor _executor;

        internal TimeSpan MinimalVisibilityTimeoutDelay { get; set; } = TimeSpan.FromSeconds(15);

        public QueueMessageHandler(IQueueMessageManager queueMessageManager, IQueueMessageExecutor executor, ILogger<QueueMessageHandler> logger)
        {
            _logger = logger;
            _queueMessageManager = queueMessageManager;
            _executor = executor;
        }

        public async Task HandleAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                await HandleMessageAsync(message, cancellationToken);
                _logger.LogInformation("Executed message with id '{MessageId}' (Succeeded)", message.MessageId);
                await _queueMessageManager.DeleteMessageAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the queue message with id '{MessageId}'", message.MessageId);
                await HandleException(message, cancellationToken);
            }
        }

        private Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource();
            var run = Task.Factory.StartNew(() => ExecuteMessageAsync(message, taskCompletionSource, cancellationToken));

            return taskCompletionSource.Task;
        }

        private async Task ExecuteMessageAsync(Message message, TaskCompletionSource taskCompletionSource, CancellationToken cancellationToken)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var timer = new VisibilityTimeoutTimer(_queueMessageManager, new LinearDelayStrategy(MinimalVisibilityTimeoutDelay));

            timer.Start(message, onFaultedAction: () =>
            {
                cts.Cancel();
                taskCompletionSource.TrySetException(new Exception($"Unable to update the visibilty timeout for message with id '{message.MessageId}'. Invisibility cannot be guaranteed."));
            });

            try
            {
                await _executor.ExecuteAsync(message, cts.Token);
                timer.Stop();
                taskCompletionSource.TrySetResult();
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        }

        private Task HandleException(Message message, CancellationToken cancellationToken)
        {
            return _queueMessageManager.EnqueueMessageAsync(message, cancellationToken);
        }
    }
}
