using JobHandlers.AzureQueueMessage.Configurations;
using Microsoft.Extensions.Logging;

namespace JobHandlers.AzureQueueMessage.Services.Builders
{
    internal class StorageAccountUriBuilder : IStorageAccountUriBuilder
    {
        private readonly ILogger<StorageAccountUriBuilder> _logger;

        public StorageAccountUriBuilder(ILogger<StorageAccountUriBuilder> logger)
        {
            _logger = logger;
        }

        public Uri Build(string uriFormat, string queueName, string? accountName)
        {
            if (string.IsNullOrWhiteSpace(uriFormat))
            {
                throw new ArgumentException($"'{nameof(uriFormat)}' cannot be null or whitespace.", nameof(uriFormat));
            }

            if (string.IsNullOrWhiteSpace(accountName) == false)
            {
                uriFormat = uriFormat.Replace($"{{{nameof(StorageAccountOptions.AccountName)}}}", accountName, StringComparison.InvariantCultureIgnoreCase);
            }

            if (string.IsNullOrWhiteSpace(queueName) == false)
            {
                uriFormat = uriFormat.Replace($"{{{nameof(StorageAccountOptions.QueueName)}}}", queueName, StringComparison.InvariantCultureIgnoreCase);
            }

            try
            {
                return new Uri(uriFormat);
            }
            catch (UriFormatException)
            {
                _logger.LogError("Invalid Uri: The Storage account Uri could not be parsed. Format: '{Uri}'", uriFormat);
                throw;
            }
        }
    }
}
