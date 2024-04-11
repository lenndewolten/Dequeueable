namespace Dequeueable.AzureQueueStorage.Services.Queues
{
    /// <summary>
    /// Represents an exception that occurs when there is an issue with setting the visibility timeout for a queue message.
    /// </summary>
    /// <param name="message"></param>
    public class VisibilityTimeoutException(string? message) : Exception(message)
    {
    }
}
