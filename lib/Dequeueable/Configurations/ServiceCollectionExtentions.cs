using Dequeueable.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dequeueable.Configurations
{
    /// <summary>
    /// Provides extension methods for registering dequeueable services with an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtentions
    {
        /// <summary>
        /// Registers the necessary services for dequeueing messages of type <typeparamref name="TMessage"/> and returns a builder for further configuration.
        /// </summary>
        /// <typeparam name="TMessage">
        /// The type of the message that implements <see cref="IQueueMessage"/>. This represents the type of messages that will be dequeued and processed.
        /// </typeparam>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to which the dequeueable services will be registered.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IDequeueableBuilder{TMessage}"/> that can be used to further configure the dequeueable services.
        /// </returns>
        public static IDequeueableBuilder<TMessage> RegisterDequeueableServices<TMessage>(this IServiceCollection services) where TMessage : class, IQueueMessage
        {
            services.TryAddSingleton(TimeProvider.System);

            return new DequeueableBuilder<TMessage>(services);
        }
    }
}
