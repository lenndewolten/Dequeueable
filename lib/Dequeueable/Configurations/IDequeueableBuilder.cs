using Dequeueable.Models;
using Dequeueable.Queues;

namespace Dequeueable.Configurations
{
    /// <summary>
    /// Provides a builder interface for configuring the message dequeueing process.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the message that implements <see cref="IQueueMessage"/>. This represents the message being dequeued.
    /// </typeparam>
    public interface IDequeueableBuilder<TMessage> where TMessage : class, IQueueMessage
    {
        /// <summary>
        /// Configures the builder to use a specific implementation of the <see cref="IQueueMessageManager{TMessage}"/> interface.
        /// </summary>
        /// <typeparam name="TImplementation">
        /// The type of the class that implements <see cref="IQueueMessageManager{TMessage}"/>. 
        /// This class is responsible for managing the lifecycle of the messages in the queue.
        /// </typeparam>
        /// <returns>
        /// Returns an <see cref="IQueueMessageHandlerBuilder{TMessage}"/> to continue the configuration process.
        /// </returns>
        IQueueMessageHandlerBuilder<TMessage> WithQueueMessageManager<TImplementation>()
            where TImplementation : class, IQueueMessageManager<TMessage>;
    }
}
