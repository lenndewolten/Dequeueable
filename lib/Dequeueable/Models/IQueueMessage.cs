namespace Dequeueable.Models
{
    /// <summary>
    /// Represents a queue message with essential properties that all messages must implement.
    /// </summary>
    public interface IQueueMessage
    {
        /// <summary>
        /// The unqiue id of the message
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// <see cref="DateTimeOffset"/> of the queue message when it is visibile again for other clients.
        /// </summary>
        public DateTimeOffset NextVisibleOn { get; }

        /// <summary>
        /// <see cref="BinaryData"/> of the body.
        /// </summary>
        public BinaryData Body { get; }
    }
}
