
namespace JobHandlers.AzureQueueMessage.Services.Builders
{
    internal interface IStorageAccountUriBuilder
    {
        Uri Build(string uriFormat, string queueName, string? accountName);
    }
}