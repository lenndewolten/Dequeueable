using Azure.Core;
using Azure.Storage.Queues;

namespace JobHandlers.AzureQueueMessage.Services.Retrievers
{
    public class QueueClientFactory : IQueueClientFactory
    {
        private readonly QueueClientOptions _queueClientOptions = new()
        {
            MessageEncoding = QueueMessageEncoding.Base64
        };

        public QueueClient Create(string connectionString, string queueName)
        {
            return new QueueClient(connectionString, queueName, _queueClientOptions);
        }

        public QueueClient Create(Uri uri, TokenCredential tokenCredential)
        {
            return new QueueClient(uri, tokenCredential, _queueClientOptions);
        }
    }
}
