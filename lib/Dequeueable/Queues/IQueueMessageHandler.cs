using Dequeueable.Models;

namespace Dequeueable.Queues
{
    /// <summary>
    /// Defines a handler for processing queue messages of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the queue message to be processed. This type must implement the <see cref="IQueueMessage"/> interface.
    /// </typeparam>
    public interface IQueueMessageHandler<TMessage> where TMessage : class, IQueueMessage
    {
        /// <summary>
        /// Asynchronously handles the processing of a queue message.
        /// </summary>
        /// <param name="message">The message to be processed.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Implementations of this method should include the logic for processing the message.
        /// The <paramref name="cancellationToken"/> should be observed regularly to handle cancellation requests promptly.
        /// </remarks>
        Task HandleAsync(TMessage message, CancellationToken cancellationToken);
    }
}
