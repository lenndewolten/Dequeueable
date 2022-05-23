namespace JobHandlers.AzureQueueMessage.Configurations
{
    public class StorageAccountOptions
    {
        private const string _poisenSuffix = "poisen";

        public const string StorageAccount = nameof(StorageAccount);
        public string? ConnectionString { get; set; }
        public string? QueueName { get; set; }
        public string PoisenQueueName => string.IsNullOrWhiteSpace(QueueName) ? _poisenSuffix : $"{QueueName}-{_poisenSuffix}";
        public string? AccountName { get; set; }
        public TimeSpan VisibilityTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public int MaxDequeueCount { get; set; } = 5;
    }
}
