namespace JobHandlers.AzureQueueMessage.Services
{
    internal static class QueueClientUriProvider
    {
        public static Uri CreateQueueClientUri(string accountName, string queueName)
        {
            return new Uri($"https://{accountName}.queue.core.windows.net/{queueName}");
        }
    }
}
