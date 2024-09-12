using Dequeueable.Models;
using Dequeueable.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace Dequeueable.Configurations
{
    internal sealed class QueueMessageHandlerBuilder<TMessage>(IServiceCollection services) : IQueueMessageHandlerBuilder<TMessage> where TMessage : class, IQueueMessage
    {
        public IDequeueableHostBuilder<TMessage> WithQueueMessageHandler<TImplementation>()
            where TImplementation : class, IQueueMessageHandler<TMessage>
        {
            services.AddTransient<IQueueMessageHandler<TMessage>, TImplementation>();
            return new DequeueableHostBuilder<TMessage>(services);
        }
    }
}
