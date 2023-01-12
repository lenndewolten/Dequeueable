namespace WebJobs.Azure.QueueStorage.Functions.Models
{
    public class Message
    {
        public string MessageId { get; }
        public string PopReceipt { get; internal set; }
        public long DequeueCount { get; }
        public DateTimeOffset? NextVisibleOn { get; }
        public BinaryData Body { get; }

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
