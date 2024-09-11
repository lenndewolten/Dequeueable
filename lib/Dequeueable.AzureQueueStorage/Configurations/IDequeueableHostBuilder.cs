namespace Dequeueable.AzureQueueStorage.Configurations
{
    /// <summary>
    /// Interface to builds and setup the dequeueable host
    /// </summary>
    public interface IDequeueableHostBuilder
    {
        /// <summary>
        /// The application will run as a job, from start to finish, and will automatically shutdown when the messages are executed.
        /// </summary>
        /// <param name="options">Action to configure the <see cref="HostOptions"/></param>
        /// <returns><see cref="IDequeueableHostBuilder"/></returns>
        IDequeueableSingletonHostBuilder RunAsJob(Action<HostOptions>? options = null);
        /// <summary>
        /// The application will run as a listener, the queue will periodically be polled for new message.
        /// </summary>
        /// <param name="options">Action to configure the <see cref="ListenerHostOptions"/></param>
        /// <returns><see cref="IDequeueableHostBuilder"/></returns>
        IDequeueableSingletonHostBuilder RunAsListener(Action<ListenerHostOptions>? options = null);
    }
}