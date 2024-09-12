using Microsoft.Extensions.DependencyInjection;

namespace Dequeueable.AmazonSQS.Configurations
{
    /// <summary>
    /// Interface to builds and setup the dequeueable singleton host
    /// </summary>
    public interface IDequeueableSingletonHostBuilder
    {
        /// <summary>
        /// Runs the function as a Distributed Singleton. Queue messages containing the same MessageGroupId will not run in parallel <see href="https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/using-messagegroupid-property.html"/>
        /// </summary>
        /// <returns><see cref="IDequeueableHostBuilder"/></returns>
        IServiceCollection AsSingleton();
    }
}
