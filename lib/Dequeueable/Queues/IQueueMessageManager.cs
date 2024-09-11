namespace Dequeueable.Queues
{
    public interface IQueueMessageManager<TMessage> where TMessage : class
    {
        Task DeleteMessageAsync(TMessage queueMessage, CancellationToken cancellationToken);
        Task EnqueueMessageAsync(TMessage queueMessage, CancellationToken cancellationToken);
        Task MoveToPoisonQueueAsync(TMessage queueMessage, CancellationToken cancellationToken);
        Task<IEnumerable<TMessage>> RetrieveMessagesAsync(CancellationToken cancellationToken);
        Task<DateTimeOffset> UpdateVisibilityTimeOutAsync(TMessage queueMessage, CancellationToken cancellationToken);
    }
}