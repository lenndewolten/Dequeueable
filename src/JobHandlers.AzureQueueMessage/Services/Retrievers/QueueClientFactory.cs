using Azure.Core;
using Azure.Identity;
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

        public QueueClient Create(string accountName, string queueName, TokenCredential tokenCredential)
        {
            var uri = QueueClientUriProvider.CreateQueueClientUri(accountName, queueName);
            return new QueueClient(uri, new DefaultAzureCredential(), _queueClientOptions);
        }
    }
}
