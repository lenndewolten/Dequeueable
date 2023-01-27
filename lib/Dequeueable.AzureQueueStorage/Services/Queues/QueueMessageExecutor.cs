using Dequeueable.AzureQueueStorage.Models;

namespace Dequeueable.AzureQueueStorage.Services.Queues
{
    internal sealed class QueueMessageExecutor : IQueueMessageExecutor
    {
        private readonly IAzureQueueFunction _function;

        public QueueMessageExecutor(IAzureQueueFunction function)
        {
            _function = function;
        }

        public async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            await _function.ExecuteAsync(message, cancellationToken);
        }
    }
}
