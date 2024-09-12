using Dequeueable.AmazonSQS.Services.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace Dequeueable.AmazonSQS.Configurations
{
    internal sealed class DequeueableSingletonHostBuilder(IServiceCollection services) : IDequeueableSingletonHostBuilder
    {
        public IServiceCollection AsSingleton()
        {
            services.AddTransient<SingletonManager>();
            services.AddTransient<QueueMessageExecutor>();
            services.AddTransient<IQueueMessageExecutor>(provider =>
            {
                var singletonManager = provider.GetRequiredService<SingletonManager>();
                var executor = provider.GetRequiredService<QueueMessageExecutor>();

                return new SingletonQueueMessageExecutor(executor, singletonManager);
            });

            services.PostConfigure<HostOptions>(options => options.AttributeNames = [.. options.AttributeNames, .. new List<string> { "MessageGroupId" }]);
            services.PostConfigure<ListenerHostOptions>(options => options.AttributeNames = [.. options.AttributeNames, .. new List<string> { "MessageGroupId" }]);
            services.PostConfigure<ListenerHostOptions>(options => options.NewBatchThreshold = 0);

            return services;
        }
    }
}
