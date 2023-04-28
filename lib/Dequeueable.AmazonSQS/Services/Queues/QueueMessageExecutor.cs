using Dequeueable.AmazonSQS.Models;

namespace Dequeueable.AmazonSQS.Services.Queues
{
    internal sealed class QueueMessageExecutor : IQueueMessageExecutor
    {
        private readonly IAmazonSQSFunction _function;

        public QueueMessageExecutor(IAmazonSQSFunction function)
        {
            _function = function;
        }

        public Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            return _function.ExecuteAsync(message, cancellationToken);
        }
    }
}
