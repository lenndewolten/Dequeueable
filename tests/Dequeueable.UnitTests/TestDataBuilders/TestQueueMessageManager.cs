using Dequeueable.Queues;

namespace Dequeueable.UnitTests.Configurations
{
    public sealed class TestQueueMessageManager : IQueueMessageManager<TestMessage>
    {
        public Task DeleteMessageAsync(TestMessage queueMessage, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task EnqueueMessageAsync(TestMessage queueMessage, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task MoveToPoisonQueueAsync(TestMessage queueMessage, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestMessage>> RetrieveMessagesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<DateTimeOffset> UpdateVisibilityTimeOutAsync(TestMessage queueMessage, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
