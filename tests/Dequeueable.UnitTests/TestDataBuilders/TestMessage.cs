using Dequeueable.Models;

namespace Dequeueable.UnitTests.Configurations
{
    public sealed class TestMessage : IQueueMessage
    {
        public string MessageId { get; set; } = string.Empty;

        public DateTimeOffset NextVisibleOn { get; set; }

        public BinaryData Body { get; set; } = BinaryData.Empty;
    }
}
