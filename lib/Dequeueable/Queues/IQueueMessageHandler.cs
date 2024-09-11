namespace Dequeueable.Queues
{
    public interface IQueueMessageHandler<TMessage> where TMessage : class
    {
        Task HandleAsync(TMessage message, CancellationToken cancellationToken);
    }
}
