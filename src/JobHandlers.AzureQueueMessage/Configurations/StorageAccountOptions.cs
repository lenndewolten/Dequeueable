using Azure.Core;

namespace JobHandlers.AzureQueueMessage.Configurations
{
    public class StorageAccountOptions
    {
        private string _poisenQueueSuffix = "poisen";
        private string _queueName = string.Empty;
        private int _maxDequeueCount = 5;
        private string? _connectionString;
        private string? _accountName;
        private string _storageAccountUriFormat = "https://{accountName}.queue.core.windows.net/{queueName}";

        internal string PoisenQueueName => string.IsNullOrWhiteSpace(QueueName) ? _poisenQueueSuffix : $"{QueueName}-{_poisenQueueSuffix}";

        public string PoisenQueueSuffix
        {
            get { return _poisenQueueSuffix; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"'{nameof(PoisenQueueSuffix)}' cannot be null or whitespace.", nameof(PoisenQueueSuffix));
                }

                _poisenQueueSuffix = value;
            }
        }

        public string QueueName
        {
            get { return _queueName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"'{nameof(QueueName)}' cannot be null or whitespace.", nameof(QueueName));
                }

                _queueName = value;
            }
        }

        public string? ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"'{nameof(ConnectionString)}' cannot be null or whitespace.", nameof(ConnectionString));
                }

                _connectionString = value;
            }
        }

        public string? AccountName
        {
            get { return _accountName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"'{nameof(AccountName)}' cannot be null or whitespace.", nameof(AccountName));
                }

                _accountName = value;
            }
        }

        public string StorageAccountUriFormat
        {
            get { return _storageAccountUriFormat; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"'{nameof(StorageAccountUriFormat)}' cannot be null or whitespace.", nameof(StorageAccountUriFormat));
                }

                _storageAccountUriFormat = value;
            }
        }

        public int MaxDequeueCount
        {
            get { return _maxDequeueCount; }
            set
            {
                if (value < 0 || value > 20)
                {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                    throw new ArgumentOutOfRangeException(
                          $"{nameof(MaxDequeueCount)} must be between 0 and 20.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
                }

                _maxDequeueCount = value;
            }
        }

        public TimeSpan VisibilityTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public const string StorageAccount = nameof(StorageAccount);

        public TokenCredential? AuthenticationScheme { get; set; }
    }
}
