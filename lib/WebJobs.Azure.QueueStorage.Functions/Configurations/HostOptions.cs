using Azure.Core;
using Azure.Storage.Queues;

namespace WebJobs.Azure.QueueStorage.Functions.Configurations
{
    /// <summary>
    /// Use the HostOptions to configure the settings of the host
    /// </summary>
    public class HostOptions : IHostOptions
    {
        /// <summary>
        /// Constant string used to bind the appsettings.*.json
        /// </summary>
        public static string WebHost => nameof(WebHost);
        private long _visibilityTimeoutInSeconds = 300;
        private long _maxDequeueCount = 5;
        private int _batchSize = 16;
        private string _poisonQueueSuffix = "poison";

        /// <summary>
        /// The connection string used to authenticate to the queue. 
        /// </summary>
        public string? ConnectionString { get; set; }
        /// <summary>
        /// The storage account name, used for identity flow.
        /// </summary>
        public string? AccountName { get; set; }
        /// <summary>
        /// The queue used to retrieve the messages.
        /// </summary>
        public string QueueName { get; set; } = string.Empty;

        /// <summary>
        /// The poisen queue used to post queue message that reach the <see cref="MaxDequeueCount">MaxDequeueCount</see>.
        /// </summary>
        public string PoisonQueueName => $"{QueueName}-{_poisonQueueSuffix}";

        /// <summary>
        /// Suffix that will be used after the <see cref="QueueName">QueueName</see>, eg queuename-suffix.
        /// </summary>
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

        /// <summary>
        /// The uri format to the queue storage. Used for identity flow. Use ` {accountName}` and `{queueName}` for variable substitution.
        /// </summary>
        public string QueueUriFormat { get; set; } = "https://{accountName}.queue.core.windows.net/{queueName}";

        /// <summary>
        /// Token credential used to authenticate via AD, Any token credential provider can be used that inherits the abstract class <see cref="TokenCredential">TokenCredential</see>.
        /// </summary>
        public TokenCredential? AuthenticationScheme { get; set; }

        /// <summary>
        /// Provides the client configuration options for connecting to Azure Queue Storage, see <see cref="QueueClientOptions">QueueClientOptions</see>.
        /// </summary>
        public QueueClientOptions? QueueClientOptions { get; set; } = new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 };

        /// <summary>
        /// The maximum number of messages processed in parallel.
        /// </summary>
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

        /// <summary>
        /// Max dequeue count before moving to the poison queue. 
        /// </summary>
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

        /// <summary>
        /// The timeout after the queue message is visible again for other services.
        /// </summary>
        public long VisibilityTimeoutInSeconds
        {
            get => _visibilityTimeoutInSeconds;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(VisibilityTimeoutInSeconds), $"'{nameof(VisibilityTimeoutInSeconds)}' must not be negative or zero.");
                }

                _visibilityTimeoutInSeconds = value;
            }
        }
    }
}
