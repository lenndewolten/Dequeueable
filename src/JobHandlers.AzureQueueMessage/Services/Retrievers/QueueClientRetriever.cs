using Azure.Identity;
using Azure.Storage.Queues;
using JobHandlers.AzureQueueMessage.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JobHandlers.AzureQueueMessage.Services.Retrievers
{
    internal class QueueClientRetriever : IQueueClientRetriever
    {
        private readonly StorageAccountOptions _storageAccountOptions;
        private readonly IQueueClientFactory _queueClientFactory;
        private readonly ILogger<QueueClientRetriever> _logger;

        public QueueClientRetriever(IQueueClientFactory queueClientFactory, IOptions<StorageAccountOptions> storageAccountOptionsAccessor,
            ILogger<QueueClientRetriever> logger)
        {
            _storageAccountOptions = storageAccountOptionsAccessor.Value;
            _queueClientFactory = queueClientFactory;
            _logger = logger;
        }

        public QueueClient Retrieve(string queueName)
        {
            if (string.IsNullOrWhiteSpace(_storageAccountOptions.AccountName) == false)
            {
                _logger.LogDebug("Authenticate the QueueClient through Active Directory");

                return _queueClientFactory.Create(_storageAccountOptions.AccountName, queueName, new DefaultAzureCredential());
            }

            if (string.IsNullOrWhiteSpace(_storageAccountOptions.ConnectionString))
            {
                throw new InvalidOperationException("No AccountName or ConnectionString supplied. Make sure that it is defined in the app settings");
            }

            _logger.LogDebug("Authenticate the QueueClient through the ConnectionString");
            return _queueClientFactory.Create(_storageAccountOptions.ConnectionString, queueName);
        }
    }
}
