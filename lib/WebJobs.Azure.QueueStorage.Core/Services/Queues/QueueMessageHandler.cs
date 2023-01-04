using Microsoft.Extensions.Logging;
using WebJobs.Azure.QueueStorage.Core.Configurations;
using WebJobs.Azure.QueueStorage.Core.Models;
using WebJobs.Azure.QueueStorage.Core.Services.Singleton;
using WebJobs.Azure.QueueStorage.Core.Services.Timers;

namespace WebJobs.Azure.QueueStorage.Core.Services.Queues
{
    internal sealed class QueueMessageHandler : IQueueMessageHandler
    {
        private readonly IQueueMessageExecutor _queueMessageExecutor;
        private readonly IQueueMessageManager _queueMessageManager;
        private readonly ILogger<QueueMessageHandler> _logger;
        private readonly IHostOptions _options;

        public QueueMessageHandler(IQueueMessageExecutor queueMessageExecutor,
            IQueueMessageManager queueMessageManager,
            ILogger<QueueMessageHandler> logger,
            IHostOptions options)
        {
            _queueMessageExecutor = queueMessageExecutor;
            _queueMessageManager = queueMessageManager;
            _logger = logger;
            _options = options;
        }

        internal TimeSpan MinimalVisibilityTimeoutDelay { get; set; } = TimeSpan.FromSeconds(15);

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
                taskCompletionSource.TrySetException(new SingletonException($"Unable to update the visibilty timeout for message with id '{message.MessageId}'. Invisibility cannot be guaranteed."));
            });

            try
            {
                await _queueMessageExecutor.ExecuteAsync(message, cts.Token);
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
            if (message.DequeueCount >= _options.MaxDequeueCount)
            {
                return _queueMessageManager.MoveToPoisonQueueAsync(message, cancellationToken);
            }

            return _queueMessageManager.EnqueueMessageAsync(message, cancellationToken);
        }
    }
}
