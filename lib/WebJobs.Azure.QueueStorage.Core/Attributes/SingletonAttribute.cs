namespace WebJobs.Azure.QueueStorage.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SingletonAttribute : Attribute
    {
        private int _minimumPollingIntervalInSeconds = 10;
        private int _maximumPollingIntervalInSeconds = 120;

        public string Scope { get; set; }

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

        public int MaxRetries { get; set; } = 3;
        public string ContainerName { get; set; } = "webjobshost";
        public string BlobUriFormat { get; set; } = "https://{accountName}.blob.core.windows.net/{containerName}/{blobName}";

        public SingletonAttribute(string scope)
        {
            Scope = scope;
        }

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
