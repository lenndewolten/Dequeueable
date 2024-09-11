using Dequeueable.AmazonSQS.Models;
using Dequeueable.Queues;

namespace Dequeueable.AmazonSQS.Services.Queues
{
    internal interface IQueueMessageManager : IQueueMessageManager<Message>
    {
        Task EnqueueMessageAsync(Message queueMessage, CancellationToken cancellationToken);
    }
}
