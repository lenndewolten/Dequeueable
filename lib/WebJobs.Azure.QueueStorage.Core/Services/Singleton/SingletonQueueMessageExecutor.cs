using WebJobs.Azure.QueueStorage.Core.Attributes;
using WebJobs.Azure.QueueStorage.Core.Extentions;
using WebJobs.Azure.QueueStorage.Core.Models;
using WebJobs.Azure.QueueStorage.Core.Services.Queues;
using WebJobs.Azure.QueueStorage.Core.Services.Timers;

namespace WebJobs.Azure.QueueStorage.Core.Services.Singleton
{
    internal sealed class SingletonQueueMessageExecutor : IQueueMessageExecutor
    {
        private readonly ISingletonLockManager _singletonLockManager;
        private readonly IQueueMessageExecutor _queueMessageExecutor;
        private readonly SingletonAttribute _singletonAttribute;

        public SingletonQueueMessageExecutor(ISingletonLockManager singletonLockManager,
            IQueueMessageExecutor queueMessageExecutor,
            SingletonAttribute singletonAttribute)
        {
            _singletonLockManager = singletonLockManager;
            _queueMessageExecutor = queueMessageExecutor;
            _singletonAttribute = singletonAttribute;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_singletonAttribute.Scope))
            {
                throw new InvalidOperationException("The Singleton Scope cannot be empty when creating a scoped distributed lock");
            }

            await ExecuteMessageAsync(message, cancellationToken);
        }

        private Task ExecuteMessageAsync(Message message, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource();
            var run = Task.Factory.StartNew(() => ExecuteMessageAsync(message, taskCompletionSource, cancellationToken));

            return taskCompletionSource.Task;
        }

        private async Task ExecuteMessageAsync(Message message, TaskCompletionSource taskCompletionSource, CancellationToken cancellationToken)
        {
            string lockName;
            string leaseId;
            try
            {
                lockName = GetLockName(message);
                leaseId = await _singletonLockManager.AquireLockAsync(lockName, cancellationToken);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
                return;
            }

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                using var timer = new LeaseTimeoutTimer(_singletonLockManager, new LinearDelayStrategy(TimeSpan.FromSeconds(_singletonAttribute.MinimumPollingIntervalInSeconds)));

                timer.Start(leaseId, lockName, onFaultedAction: () =>
                {
                    taskCompletionSource.TrySetException(new SingletonException($"Unable to renew the lease with id '{leaseId}'. Distributed lock cannot be guaranteed."));
                    cts.Cancel();
                });

                await _queueMessageExecutor.ExecuteAsync(message, cts.Token);
                timer.Stop();
                taskCompletionSource.TrySetResult();
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
            finally
            {
                await _singletonLockManager.ReleaseLockAsync(leaseId, lockName, cancellationToken);
            }
        }

        private string GetLockName(Message message)
        {
            try
            {
                var lockName = message.GetValueByPropertyName(_singletonAttribute.Scope!);

                return string.IsNullOrWhiteSpace(lockName)
                    ? throw new SingletonException($"The provided scope name, '{_singletonAttribute.Scope}' , does not exist on the message with id '{message.MessageId}'")
                    : lockName;
            }
            catch (KeyNotFoundException ex)
            {
                throw new SingletonException($"The provided scope name, '{_singletonAttribute.Scope}' , does not exist on the message with id '{message.MessageId}'", ex);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new SingletonException($"Unable to parse the body for the message with id '{message.MessageId}'", ex);
            }
        }
    }
}
