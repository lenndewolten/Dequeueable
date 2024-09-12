using AutoFixture;
using Dequeueable.Queues;

namespace Dequeueable.SampleJob
{
    internal sealed class SampleQueueMessageManager : IQueueMessageManager<SampleMessage>
    {
        private static readonly Fixture _fixture = new();
        private readonly Dictionary<string, SampleMessage> _messages = [];

        public Task DeleteMessageAsync(SampleMessage queueMessage, CancellationToken cancellationToken)
        {
            _messages.Remove(queueMessage.MessageId);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<SampleMessage>> RetrieveMessagesAsync(CancellationToken cancellationToken)
        {
            var messages = _fixture.CreateMany<SampleMessage>();

            foreach (var message in messages)
            {
                _messages[message.MessageId] = message;
            }

            return Task.FromResult(messages);
        }

        public Task<DateTimeOffset> UpdateVisibilityTimeOutAsync(SampleMessage queueMessage, CancellationToken cancellationToken)
        {
            if (!_messages.TryGetValue(queueMessage.MessageId, out var message))
            {
                throw new InvalidDataException();
            }

            message.NextVisibleOn = DateTimeOffset.UtcNow.AddMinutes(1);

            return Task.FromResult(message.NextVisibleOn);
        }
    }
}
