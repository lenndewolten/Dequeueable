
using Dequeueable.AmazonSQS.Models;

namespace Dequeueable.AmazonSQS.Services.Queues
{
    internal interface IQueueMessageManager
    {
        Task DeleteMessageAsync(Message message, CancellationToken cancellationToken);
        Task EnqueueMessageAsync(Message message, CancellationToken cancellationToken);
        Task EnqueueMessageAsync(Message[] messages, CancellationToken cancellationToken);
        Task<Message[]> RetreiveMessagesAsync(CancellationToken cancellationToken = default);
        Task<DateTimeOffset> UpdateVisibilityTimeOutAsync(Message message, CancellationToken cancellationToken);
        Task<DateTimeOffset> UpdateVisibilityTimeOutAsync(Message[] messages, CancellationToken cancellationToken);
    }
}
