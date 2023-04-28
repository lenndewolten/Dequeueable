using Dequeueable.AmazonSQS.Models;

namespace Dequeueable.AmazonSQS.Services.Queues
{
    internal sealed class SingletonQueueMessageExecutor : IQueueMessageExecutor
    {
        private readonly IQueueMessageExecutor _executor;
        private readonly SingletonManager _singletonManager;

        public SingletonQueueMessageExecutor(IQueueMessageExecutor executor, SingletonManager singletonManager)
        {
            _executor = executor;
            _singletonManager = singletonManager;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(message.MessageGroupId))
            {
                await _executor.ExecuteAsync(message, cancellationToken);
                return;
            }

            await _singletonManager.WaitAsync(message.MessageGroupId, cancellationToken);
            try
            {
                await _executor.ExecuteAsync(message, cancellationToken);
            }
            finally
            {
                _singletonManager.Release(message.MessageGroupId);
            }
        }
    }
}
