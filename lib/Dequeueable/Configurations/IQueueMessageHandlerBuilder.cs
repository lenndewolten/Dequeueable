using Dequeueable.Models;
using Dequeueable.Queues;

namespace Dequeueable.Configurations
{
    /// <summary>
    /// Provides a builder interface for configuring the message handler in the dequeueing process.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the message that implements <see cref="IQueueMessage"/>. This represents the message being dequeued and processed.
    /// </typeparam>
    public interface IQueueMessageHandlerBuilder<TMessage> where TMessage : class, IQueueMessage
    {
        /// <summary>
        /// Configures the builder to use a specific implementation of the <see cref="IQueueMessageHandler{TMessage}"/> interface.
        /// </summary>
        /// <typeparam name="TImplementation">
        /// The type of the class that implements <see cref="IQueueMessageHandler{TMessage}"/>. 
        /// This class is responsible for handling the processing of the dequeued messages.
        /// </typeparam>
        /// <returns>
        /// Returns an <see cref="IDequeueableHostBuilder{TMessage}"/> to continue the configuration process.
        /// </returns>
        IDequeueableHostBuilder<TMessage> WithQueueMessageHandler<TImplementation>()
            where TImplementation : class, IQueueMessageHandler<TMessage>;
    }
}
