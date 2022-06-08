using Azure.Storage.Queues;
using JobHandlers.AzureQueueMessage.Configurations;
using JobHandlers.AzureQueueMessage.Services.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JobHandlers.AzureQueueMessage.Services.Retrievers
{
    internal class QueueClientRetriever : IQueueClientRetriever
    {
        private readonly StorageAccountOptions _storageAccountOptions;
        private readonly IQueueClientFactory _queueClientFactory;
        private readonly ILogger<QueueClientRetriever> _logger;
        private readonly IStorageAccountUriBuilder _storageAccountUriBuilder;

        public QueueClientRetriever(IQueueClientFactory queueClientFactory,
            IOptions<StorageAccountOptions> storageAccountOptionsAccessor,
            ILogger<QueueClientRetriever> logger,
            IStorageAccountUriBuilder storageAccountUriBuilder)
        {
            _storageAccountOptions = storageAccountOptionsAccessor.Value;
            _queueClientFactory = queueClientFactory;
            _logger = logger;
            _storageAccountUriBuilder = storageAccountUriBuilder;
        }

        public QueueClient Retrieve(string queueName)
        {
            if (_storageAccountOptions.AuthenticationScheme is not null)
            {
                _logger.LogDebug("Authenticate the QueueClient through Active Directory");

                var uri = _storageAccountUriBuilder.Build(
                _storageAccountOptions.StorageAccountUriFormat,
                queueName,
                _storageAccountOptions.AccountName);

                return _queueClientFactory.Create(uri, _storageAccountOptions.AuthenticationScheme);
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
