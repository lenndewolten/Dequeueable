using Dequeueable.Queues;

namespace Dequeueable.UnitTests.Configurations
{
    public sealed class TestQueueMessageHandler : IQueueMessageHandler<TestMessage>
    {
        public Task HandleAsync(TestMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
