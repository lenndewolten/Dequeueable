using Dequeueable.Models;

namespace Dequeueable.Queues
{
    public interface IQueueMessageHandler<TMessage> where TMessage : class, IQueueMessage
    {
        Task HandleAsync(TMessage message, CancellationToken cancellationToken);
    }
}
