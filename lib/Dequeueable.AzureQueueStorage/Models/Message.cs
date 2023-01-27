namespace Dequeueable.AzureQueueStorage.Models
{
    /// <summary>
    /// Queue message retrieved from the qeueue.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The unqiue id of the message
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// Unique receipt of the Queue Message. 
        /// </summary>
        public string PopReceipt { get; internal set; }

        /// <summary>
        /// The Dequeue count of the queue message.
        /// </summary>
        public long DequeueCount { get; }

        /// <summary>
        /// <see cref="DateTimeOffset"/> of the queue message when it is visibile again for other clients.
        /// </summary>
        public DateTimeOffset? NextVisibleOn { get; }

        /// <summary>
        /// <see cref="BinaryData"/> of the body.
        /// </summary>
        public BinaryData Body { get; }

        /// <summary>
        /// Creates an instance of the queue message.
        /// </summary>
        /// <param name="messageId">Unique receipt of the Queue Message. </param>
        /// <param name="popReceipt">Unique receipt of the Queue Message. </param>
        /// <param name="dequeueCount">The Dequeue count of the queue message.</param>
        /// <param name="nextVisibleOn"> <see cref="DateTimeOffset"/> of the queue message when it is visibile again for other clients.</param>
        /// <param name="body"><see cref="BinaryData"/> of the body.</param>
        public Message(string messageId, string popReceipt, long dequeueCount, DateTimeOffset? nextVisibleOn, BinaryData body)
        {
            MessageId = messageId;
            PopReceipt = popReceipt;
            DequeueCount = dequeueCount;
            NextVisibleOn = nextVisibleOn;
            Body = body;
        }
    }
}
