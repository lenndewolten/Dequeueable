using Azure.Storage.Queues.Models;

namespace JobHandlers.AzureQueueMessage.UnitTests.TestDataBuilders
{
    internal class QueueMesssageTestDataBuilder
    {
        private readonly string _messageId = "some id";
        private readonly string _popReceipt = "some receipt";
        private readonly string _body = "sme message";
        private long _dequeueCount = 5;

        public QueueMessage Build()
        {
            return QueuesModelFactory.QueueMessage(_messageId, _popReceipt, _body, _dequeueCount);
        }

        public QueueMesssageTestDataBuilder WithDequeueCount(int dequeueCount)
        {
            _dequeueCount = dequeueCount;
            return this;
        }
    }
}
