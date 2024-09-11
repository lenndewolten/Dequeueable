using Dequeueable.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Dequeueable.Configurations
{
    /// <summary>
    /// Provides a builder interface for configuring the dequeueable host, including setting up listeners and jobs.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the message that implements <see cref="IQueueMessage"/>. This represents the message being dequeued and processed by the host.
    /// </typeparam>
    public interface IDequeueableHostBuilder<TMessage> where TMessage : class, IQueueMessage
    {
        /// <summary>
        /// Configures the dequeueable host as a listener with the specified options.
        /// </summary>
        /// /// <returns>
        /// Returns an <see cref="IServiceCollection"/> to allow further service configuration.
        /// </returns>
        IServiceCollection AsListener(Action<ListenerHostOptions>? options = null);

        /// <summary>
        /// Configures the dequeueable host as a listener with the specified options.
        /// </summary>
        /// <typeparam name="TOptions">
        /// The type of the options that implement <see cref="IListenerHostOptions"/>. These options define the configuration for the listener.
        /// </typeparam>
        /// <param name="options">
        /// An optional action to configure the listener options. If not provided, default options will be used.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IServiceCollection"/> to allow further service configuration.
        /// </returns>
        IServiceCollection AsListener<TOptions>(Action<TOptions>? options = null)
            where TOptions : class, IListenerHostOptions;

        /// <summary>
        /// Configures the dequeueable host as a listener with the specified options and an additional dependency.
        /// </summary>
        /// <typeparam name="TOptions">
        /// The type of the options that implement <see cref="IListenerHostOptions"/>. These options define the configuration for the listener.
        /// </typeparam>
        /// <typeparam name="TDep1">
        /// The type of the additional dependency required by the listener, which must be a class.
        /// </typeparam>
        /// <param name="options">
        /// An optional action to configure the listener options along with the additional dependency. If not provided, default options will be used.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IServiceCollection"/> to allow further service configuration.
        /// </returns>
        IServiceCollection AsListener<TOptions, TDep1>(Action<TOptions, TDep1>? options = null)
            where TOptions : class, IListenerHostOptions
            where TDep1 : class;

        /// <summary>
        /// Configures the dequeueable host as a job, which processes messages in a background service.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IServiceCollection"/> to allow further service configuration.
        /// </returns>
        IServiceCollection AsJob();
    }
}
