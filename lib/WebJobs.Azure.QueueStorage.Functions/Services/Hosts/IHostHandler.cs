namespace WebJobs.Azure.QueueStorage.Functions.Services.Hosts
{
    /// <summary>
    /// The handler that will be called when the host is started. This interface can be used for integration testing.
    /// </summary>
    public interface IHostHandler
    {
        /// <summary>
        /// The method that will be called when the host is started.
        /// </summary>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/> to propagate
        /// notifications that the operation should be cancelled.
        /// </param>
        Task HandleAsync(CancellationToken cancellationToken);
    }
}