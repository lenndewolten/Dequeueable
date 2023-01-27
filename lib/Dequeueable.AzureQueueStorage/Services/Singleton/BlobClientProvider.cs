using Azure.Storage.Blobs;
using Dequeueable.AzureQueueStorage.Configurations;
using Dequeueable.AzureQueueStorage.Factories;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AzureQueueStorage.Services.Singleton
{
    internal sealed class BlobClientProvider : IBlobClientProvider
    {
        private readonly IHostOptions _options;
        private readonly IBlobClientFactory _factory;
        private readonly SingletonAttribute _singletonAttribute;
        private readonly ILogger<BlobClientProvider> _logger;

        public BlobClientProvider(
            IBlobClientFactory factory,
            IHostOptions options,
            SingletonAttribute singletonAttribute,
            ILogger<BlobClientProvider> logger)
        {
            _options = options;
            _factory = factory;
            _singletonAttribute = singletonAttribute;
            _logger = logger;
        }

        public BlobClient Get(string fileName)
        {
            if (_options.AuthenticationScheme is not null)
            {
                _logger.LogDebug("Authenticate the BlobClient through Active Directory");

                var uri = BuildUri(_singletonAttribute.BlobUriFormat, _options.AccountName, _singletonAttribute.ContainerName, fileName);
                return _factory.Create(uri, _options.AuthenticationScheme);
            }

            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            {
                throw new InvalidOperationException("No AuthenticationScheme or ConnectionString supplied. Make sure that it is defined in the app settings");
            }

            _logger.LogDebug("Authenticate the BlobClient through the ConnectionString");
            return _factory.Create(_options.ConnectionString, _singletonAttribute.ContainerName, fileName);
        }

        private Uri BuildUri(string? uriFormat, string? accountName, string containerName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(uriFormat))
            {
                throw new ArgumentException($"'{nameof(uriFormat)}' cannot be null or whitespace.", nameof(uriFormat));
            }

            if (string.IsNullOrWhiteSpace(accountName) == false)
            {
                uriFormat = uriFormat.Replace($"{{{nameof(IHostOptions.AccountName)}}}", accountName, StringComparison.InvariantCultureIgnoreCase);
            }

            uriFormat = uriFormat.Replace($"{{{nameof(SingletonAttribute.ContainerName)}}}", containerName, StringComparison.InvariantCultureIgnoreCase);
            uriFormat = uriFormat.Replace($"{{blobName}}", fileName, StringComparison.InvariantCultureIgnoreCase);

            try
            {
                return new Uri(uriFormat);
            }
            catch (UriFormatException)
            {
                _logger.LogError("Invalid Uri: The Blob Uri could not be parsed. Format: '{Uri}'", uriFormat);
                throw;
            }
        }
    }
}
