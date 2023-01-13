using WebJobs.Azure.QueueStorage.Functions.Models;

namespace WebJobs.Azure.QueueStorage.Functions
{
    /// <summary>
    /// Interface to bind a function to the framework
    /// </summary>
    public interface IAzureQueueFunction
    {
        /// <summary>
        /// The method that will be invoked when a message is found on the queue
        /// </summary>
        /// <param name="message">
        ///  The Queue Message on the queue
        /// 
        /// </param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/> to propagate
        /// notifications that the operation should be cancelled.
        /// </param>
        Task ExecuteAsync(Message message, CancellationToken cancellationToken);
    }
}
