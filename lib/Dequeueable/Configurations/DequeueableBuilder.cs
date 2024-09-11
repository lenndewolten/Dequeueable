using Dequeueable.Models;
using Dequeueable.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace Dequeueable.Configurations
{
    internal sealed class DequeueableBuilder<TMessage>(IServiceCollection services) : IDequeueableBuilder<TMessage> where TMessage : class, IQueueMessage
    {
        public IQueueMessageHandlerBuilder<TMessage> WithQueueMessageManager<TImplementation>()
            where TImplementation : class, IQueueMessageManager<TMessage>
        {
            services.AddSingleton<IQueueMessageManager<TMessage>, TImplementation>();
            return new QueueMessageHandlerBuilder<TMessage>(services);
        }
    }
}
