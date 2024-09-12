using Dequeueable.Models;

namespace Dequeueable.SampleJob
{
    internal sealed class SampleMessage : IQueueMessage
    {
        public string MessageId { get; set; } = string.Empty;

        public DateTimeOffset NextVisibleOn { get; set; }

        public BinaryData Body { get; set; } = BinaryData.Empty;
    }
}
