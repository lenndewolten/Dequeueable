using Amazon.SQS;
using Dequeueable.AmazonSQS.Configurations;

namespace Dequeueable.AmazonSQS.Factories
{
    internal sealed class AmazonSQSClientFactory(IHostOptions options) : IAmazonSQSClientFactory, IDisposable
    {
        private AmazonSQSClient? _client;
        public AmazonSQSClient Create()
        {
            if (options.AWSCredentials is null && options.AmazonSQSConfig is null)
            {
                return _client = new();
            }

            if (options.AWSCredentials is not null && options.AmazonSQSConfig is not null)
            {
                return _client ??= new(options.AWSCredentials, options.AmazonSQSConfig);
            }

            return _client ??= new(options.AmazonSQSConfig);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
