using Azure.Storage.Blobs;
using Dequeueable.AzureQueueStorage.Configurations;
using Dequeueable.AzureQueueStorage.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dequeueable.AzureQueueStorage.Services.Singleton
{
    internal sealed class BlobClientProvider : IBlobClientProvider
    {
        private readonly IHostOptions _options;
        private readonly SingletonHostOptions _singletonHostOptions;
        private readonly IBlobClientFactory _factory;
        private readonly ILogger<BlobClientProvider> _logger;

        public BlobClientProvider(
            IBlobClientFactory factory,
            IHostOptions options,
            IOptions<SingletonHostOptions> singletonHostOptions,
            ILogger<BlobClientProvider> logger)
        {
            _options = options;
            _singletonHostOptions = singletonHostOptions.Value;
            _factory = factory;
            _logger = logger;
        }

        public BlobClient Get(string fileName)
        {
            if (_options.AuthenticationScheme is not null)
            {
                _logger.LogDebug("Authenticate the BlobClient through Active Directory");

                var uri = BuildUri(_singletonHostOptions.BlobUriFormat, _options.AccountName, _singletonHostOptions.ContainerName, fileName);
                return _factory.Create(uri, _options.AuthenticationScheme);
            }

            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            {
                throw new InvalidOperationException("No AuthenticationScheme or ConnectionString supplied. Make sure that it is defined in the app settings");
            }

            _logger.LogDebug("Authenticate the BlobClient through the ConnectionString");
            return _factory.Create(_options.ConnectionString, _singletonHostOptions.ContainerName, fileName);
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

            uriFormat = uriFormat.Replace($"{{{nameof(_singletonHostOptions.ContainerName)}}}", containerName, StringComparison.InvariantCultureIgnoreCase);
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
