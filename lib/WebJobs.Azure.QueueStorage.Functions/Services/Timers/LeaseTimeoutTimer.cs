using WebJobs.Azure.QueueStorage.Functions.Services.Singleton;

namespace WebJobs.Azure.QueueStorage.Functions.Services.Timers
{
    public sealed class LeaseTimeoutTimer : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly ISingletonLockManager _singletonLockManager;
        private readonly IDelayStrategy _delayStrategy;

        private bool _disposed;

        public LeaseTimeoutTimer(ISingletonLockManager singletonLockManager, IDelayStrategy delayStrategy)
        {
            _cts = new CancellationTokenSource();
            _singletonLockManager = singletonLockManager;
            _delayStrategy = delayStrategy;
        }

        public void Start(string leaseId, string fileName, Action? onFaultedAction = null)
        {
            StartAsync(leaseId, fileName, _cts.Token)
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

        private async Task StartAsync(string leaseId, string fileName, CancellationToken cancellationToken)
        {
            await Task.Yield();
            var nextVisibleOn = DateTimeOffset.UtcNow.Add(_delayStrategy.MinimalRenewalDelay);

            TaskCompletionSource<object> cancellationTaskSource = new();
            using (cancellationToken.Register(() => cancellationTaskSource.SetCanceled()))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        nextVisibleOn = await UpdateTimeout(leaseId, fileName, nextVisibleOn, cancellationToken);
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

        private async Task<DateTimeOffset> UpdateTimeout(string leaseId, string fileName, DateTimeOffset nextVisibleOn, CancellationToken cancellationToken)
        {
            var delay = _delayStrategy.GetNextDelay(nextVisibleOn);
            await Task.Delay(delay, cancellationToken);

            nextVisibleOn = await _singletonLockManager.RenewLockAsync(leaseId, fileName, cancellationToken);

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
