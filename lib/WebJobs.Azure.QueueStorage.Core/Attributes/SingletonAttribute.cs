namespace WebJobs.Azure.QueueStorage.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SingletonAttribute : Attribute
    {
        public string Scope { get; set; }
        public TimeSpan MinimumInterval { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan MaximumInterval { get; set; } = TimeSpan.FromMinutes(2);

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
            long minimumIntervalSeconds = default,
            long maximumIntervalSeconds = default)
        {
            Scope = scope;
            ContainerName = containerName ?? ContainerName;
            BlobUriFormat = blobUriFormat ?? BlobUriFormat;
            MinimumInterval = minimumIntervalSeconds == default ? MinimumInterval : TimeSpan.FromSeconds(minimumIntervalSeconds);
            MaximumInterval = maximumIntervalSeconds == default ? MaximumInterval : TimeSpan.FromSeconds(maximumIntervalSeconds);
            MaxRetries = maxRetries == default ? MaxRetries : maxRetries;
            Scope = scope;
        }
    }
}
