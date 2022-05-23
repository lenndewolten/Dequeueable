using JobHandlers.AzureQueueMessage.Handlers;
using Microsoft.Extensions.Hosting;

namespace JobHandlers.AzureQueueMessage
{
    internal sealed class AzureQueueMessageHostService : IHostedService, IDisposable
    {
        private readonly QueueMessageHandler _queueMessageHandler;
        private readonly CancellationTokenSource _stoppingCts = new();
        private Task? _executingTask;

        public AzureQueueMessageHostService(QueueMessageHandler queueMessageHandler)
        {
            _queueMessageHandler = queueMessageHandler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = _queueMessageHandler.HandleAsync(_stoppingCts.Token);

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask is null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}