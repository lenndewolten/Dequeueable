using Azure.Identity;
using Azure.Storage.Queues;
using JobHandlers.AzureQueueMessage.Configurations;
using JobHandlers.AzureQueueMessage.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JobHandlers.AzureQueueMessage.Services.Retrievers
{
    internal class QueueClientRetriever : IQueueClientRetriever
    {
        private readonly StorageAccountOptions _storageAccountOptions;
        private readonly ILogger<QueueClientRetriever> _logger;

        public QueueClientRetriever(IOptions<StorageAccountOptions> storageAccountOptionsAccessor,
            ILogger<QueueClientRetriever> logger)
        {
            _storageAccountOptions = storageAccountOptionsAccessor.Value;
            _logger = logger;
        }

        public QueueClient Retrieve(string queueName)
        {
            var queueClientOptions = new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 };

            if (string.IsNullOrWhiteSpace(_storageAccountOptions.AccountName) == false)
            {
                _logger.LogInformation("Authenticate the QueueClient through Active Directory");

                var uri = QueueClientUriProvider.CreateQueueClientUri(_storageAccountOptions.AccountName, queueName);
                return new QueueClient(uri, new DefaultAzureCredential(), queueClientOptions);
            }

            if (string.IsNullOrWhiteSpace(_storageAccountOptions.ConnectionString))
            {
                throw new InvalidOperationException("Invalid StorageAccount ConnectionString. Make sure that it is defined in the app settings");
            }

            _logger.LogInformation("Authenticate the QueueClient through the ConnectionString");
            return new QueueClient(_storageAccountOptions.ConnectionString, queueName, queueClientOptions);
        }
    }
}
