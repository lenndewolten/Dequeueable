using Microsoft.Extensions.DependencyInjection;

namespace Dequeueable.AzureQueueStorage.Configurations
{
    /// <summary>
    /// Interface to builds and setup the dequeueable singleton host
    /// </summary>
    public interface IDequeueableSingletonHostBuilder
    {
        /// <summary>
        /// This makes sure only a single instance of the function is executed at any given time (even across host instances).
        /// A blob lease is used behind the scenes to implement the lock./>
        /// </summary>
        /// <param name="options">Action to configure the <see cref="SingletonHostOptions"/></param>
        /// <returns><see cref="IServiceCollection"/></returns>
        IServiceCollection AsSingleton(Action<SingletonHostOptions>? options = null);
    }
}