using Dequeueable.AmazonSQS.Models;
using Dequeueable.AmazonSQS.Services.Queues;
using Dequeueable.AzureQueueStorage.Services.Timers;

namespace Dequeueable.AmazonSQS.Services.Timers
{
    internal sealed class VisibilityTimeoutTimer : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly IQueueMessageManager _queueMessagesManager;
        private readonly IDelayStrategy _delayStrategy;

        private bool _disposed;

        public VisibilityTimeoutTimer(IQueueMessageManager queueMessagesManager, IDelayStrategy delayStrategy)
        {
            _cts = new CancellationTokenSource();
            _queueMessagesManager = queueMessagesManager;
            _delayStrategy = delayStrategy;
        }

        public void Start(Message message, Action? onFaultedAction = null)
        {
            StartAsync(message, _cts.Token)
            .ContinueWith(_ =>
            {
                onFaultedAction?.Invoke();
            }, TaskContinuationOptions.OnlyOnFaulted)
            .ConfigureAwait(false);
        }

        public void Start(Message[] messages, Action? onFaultedAction = null)
        {
            StartAsync(messages, _cts.Token)
            .ContinueWith(_ =>
            {
                onFaultedAction?.Invoke();
            }, TaskContinuationOptions.OnlyOnFaulted)
            .ConfigureAwait(false);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        private async Task StartAsync(Message message, CancellationToken cancellationToken)
        {
            await Task.Yield();
            var nextVisibleOn = message.NextVisibleOn;

            TaskCompletionSource<object> cancellationTaskSource = new();
            using (cancellationToken.Register(() => cancellationTaskSource.SetCanceled()))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        nextVisibleOn = await UpdateVisbility(message, nextVisibleOn, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex) when (ex.InnerException is OperationCanceledException)
                    {
                    }
                }
            }
        }

        private async Task StartAsync(Message[] messages, CancellationToken cancellationToken)
        {
            await Task.Yield();
            var nextVisibleOn = messages.Min(m => m.NextVisibleOn);

            TaskCompletionSource<object> cancellationTaskSource = new();
            using (cancellationToken.Register(() => cancellationTaskSource.SetCanceled()))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        nextVisibleOn = await UpdateVisbility(messages, nextVisibleOn, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex) when (ex.InnerException is OperationCanceledException)
                    {
                    }
                }
            }
        }

        private async Task<DateTimeOffset> UpdateVisbility(Message message, DateTimeOffset nextVisibleOn, CancellationToken cancellationToken)
        {
            var delay = _delayStrategy.GetNextDelay(nextVisibleOn);
            await Task.Delay(delay, cancellationToken);
            nextVisibleOn = await _queueMessagesManager.UpdateVisibilityTimeOutAsync(message, cancellationToken);
            return nextVisibleOn;
        }

        private async Task<DateTimeOffset> UpdateVisbility(Message[] messages, DateTimeOffset nextVisibleOn, CancellationToken cancellationToken)
        {
            var delay = _delayStrategy.GetNextDelay(nextVisibleOn);
            await Task.Delay(delay, cancellationToken);
            nextVisibleOn = await _queueMessagesManager.UpdateVisibilityTimeOutAsync(messages, cancellationToken);
            return nextVisibleOn;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            _disposed = true;
        }
    }
}
