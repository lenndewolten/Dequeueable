using Dequeueable.Models;

namespace Dequeueable.Queues
{
    /// <summary>
    /// Provides an interface for managing queue messages of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the queue message to be managed. This type must implement the <see cref="IQueueMessage"/> interface.
    /// </typeparam>
    public interface IQueueMessageManager<TMessage> where TMessage : class, IQueueMessage
    {
        /// <summary>
        /// Asynchronously deletes the specified message from the queue.
        /// </summary>
        /// <param name="queueMessage">The message to delete.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous delete operation.</returns>
        Task DeleteMessageAsync(TMessage queueMessage, CancellationToken cancellationToken);



        /// <summary>
        /// Asynchronously retrieves messages from the queue.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous retrieval operation, returning a collection of messages.</returns>
        Task<IEnumerable<TMessage>> RetrieveMessagesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously updates the visibility timeout of the specified message in the queue.
        /// </summary>
        /// <param name="queueMessage">The message for which to update the visibility timeout.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous update operation, returning the new visibility timeout as a <see cref="DateTimeOffset"/>.
        /// </returns>
        Task<DateTimeOffset> UpdateVisibilityTimeOutAsync(TMessage queueMessage, CancellationToken cancellationToken);
    }
}