namespace Dequeueable.AzureQueueStorage.Services.Singleton
{
    /// <summary>
    /// This attribute can be applied to a job functions to ensure that only a single
    /// instance of the function is executed at any given time (even across host instances).
    /// A blob lease is used behind the scenes to implement the lock.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SingletonAttribute : Attribute
    {
        private int _minimumPollingIntervalInSeconds = 10;
        private int _maximumPollingIntervalInSeconds = 120;

        /// <summary>
        /// Gets the scope indentifier of the lock. This will be the blob file name to implement the lock.
        /// </summary>
        public string Scope { get; }

        /// <summary>
        /// The minimum polling interval to check if a new lease can be acquired.
        /// </summary>
        public int MinimumPollingIntervalInSeconds
        {
            get => _minimumPollingIntervalInSeconds;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinimumPollingIntervalInSeconds), $"'{nameof(MinimumPollingIntervalInSeconds)}' must not be negative or zero.");
                }

                _minimumPollingIntervalInSeconds = value;
            }
        }

        /// <summary>
        /// The maximum polling interval to check if a new lease can be acquired.
        /// </summary>
        public int MaximumPollingIntervalInSeconds
        {
            get => _maximumPollingIntervalInSeconds;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaximumPollingIntervalInSeconds), $"'{nameof(MaximumPollingIntervalInSeconds)}' must not be negative or zero.");
                }

                _maximumPollingIntervalInSeconds = value;
            }
        }

        /// <summary>
        /// The max retries to acquire a lease. When reached, the host will shutdown.
        /// </summary>
        public int MaxRetries { get; } = 3;
        /// <summary>
        /// The name of the container used for the lock files. 
        /// </summary>
        public string ContainerName { get; } = "webjobshost";
        /// <summary>
        /// The uri format to the blob storage. Used for identity flow. Use ` {accountName}`, `{containerName}` and `{blobName}` for variable substitution.
        /// </summary>
        public string BlobUriFormat { get; } = "https://{accountName}.blob.core.windows.net/{containerName}/{blobName}";

        /// <summary>
        /// Constructs a new instance using the specified scope settings.
        /// </summary>
        /// <param name="scope">The scope value for the singleton lock. This will be the blob file name to implement the lock.</param>
        public SingletonAttribute(string scope)
        {
            Scope = scope;
        }

        /// <summary>
        /// Constructs a new instance using the specified scope and the default settings.
        /// </summary>
        /// <param name="scope">The scope value for the singleton lock. This will be the blob file name to implement the lock.</param>
        /// <param name="containerName">The name of the container used for the lock files. </param>
        /// <param name="blobUriFormat">The uri format to the blob storage. Used for identity flow. Use ` {accountName}`, `{containerName}` and `{blobName}` for variable substitution.</param>
        /// <param name="maxRetries">The max retries to acquire a lease. When reached, the host will shutdown.</param>
        /// <param name="minimumIntervalInSeconds">The minimum polling interval to check if a new lease can be acquired.</param>
        /// <param name="maximumIntervalInSeconds">The maximum polling interval to check if a new lease can be acquired.</param>
        public SingletonAttribute(string scope,
            string? containerName = null,
            string? blobUriFormat = null,
            int maxRetries = default,
            int minimumIntervalInSeconds = default,
            int maximumIntervalInSeconds = default)
        {
            Scope = scope;
            ContainerName = containerName ?? ContainerName;
            BlobUriFormat = blobUriFormat ?? BlobUriFormat;
            MinimumPollingIntervalInSeconds = minimumIntervalInSeconds == default ? MinimumPollingIntervalInSeconds : minimumIntervalInSeconds;
            MaximumPollingIntervalInSeconds = maximumIntervalInSeconds == default ? MaximumPollingIntervalInSeconds : maximumIntervalInSeconds;
            MaxRetries = maxRetries == default ? MaxRetries : maxRetries;
            Scope = scope;
        }
    }
}
