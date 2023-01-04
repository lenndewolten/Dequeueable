using Azure.Core;
using Azure.Storage.Queues;

namespace WebJobs.Azure.QueueStorage.Core.Configurations
{
    public class HostOptions : IHostOptions
    {
        public static string WebHost => nameof(WebHost);
        private TimeSpan _visibilityTimeout = TimeSpan.FromSeconds(300);
        private long _maxDequeueCount = 5;
        private int _batchSize = 16;
        private string _poisonQueueSuffix = "poison";

        public string? ConnectionString { get; set; }
        public string? AccountName { get; set; }
        public string QueueName { get; set; } = string.Empty;

        public string PoisonQueueName => $"{QueueName}-{_poisonQueueSuffix}";

        public string PoisonQueueSuffix
        {
            get { return _poisonQueueSuffix; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"'{nameof(PoisonQueueSuffix)}' cannot be null or whitespace.", nameof(PoisonQueueSuffix));
                }

                _poisonQueueSuffix = value;
            }
        }

        public string QueueUriFormat { get; set; } = "https://{accountName}.queue.core.windows.net/{queueName}";

        public TokenCredential? AuthenticationScheme { get; set; }
        public QueueClientOptions? QueueClientOptions { get; set; } = new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 };

        public int BatchSize
        {
            get => _batchSize;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(BatchSize), $"'{nameof(BatchSize)}' must not be negative or zero.");
                }

                _batchSize = value;
            }
        }

        public long MaxDequeueCount
        {
            get => _maxDequeueCount;
            set
            {
                if (value < 0 || value > 20)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxDequeueCount),
                          $"{nameof(MaxDequeueCount)} must be between 0 and 20.");
                }

                _maxDequeueCount = value;
            }
        }

        public TimeSpan VisibilityTimeout
        {
            get => _visibilityTimeout;
            set
            {
                if (value.Ticks < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(VisibilityTimeout), $"'{nameof(VisibilityTimeout)}' must not be negative or zero.");
                }

                _visibilityTimeout = value;
            }
        }
    }
}
