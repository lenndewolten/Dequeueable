using Dequeueable.AzureQueueStorage.Factories;
using Dequeueable.AzureQueueStorage.Services.Queues;
using Dequeueable.AzureQueueStorage.Services.Singleton;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dequeueable.AzureQueueStorage.Configurations
{
    internal sealed class DequeueableSingletonHostBuilder(IServiceCollection services) : IDequeueableSingletonHostBuilder
    {
        public IServiceCollection AsSingleton(Action<SingletonHostOptions>? options = null)
        {
            services.AddOptions<SingletonHostOptions>()
                .BindConfiguration(SingletonHostOptions.Name)
                .Validate(SingletonHostOptions.ValidatePollingInterval, $"The '{nameof(SingletonHostOptions.MinimumPollingIntervalInSeconds)}' must not be greater than the '{nameof(SingletonHostOptions.MaximumPollingIntervalInSeconds)}'.")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            if (options is not null)
            {
                services.Configure(options);
            }

            services.AddTransient<IDistributedLockManager, DistributedLockManager>();
            services.AddTransient<IDistributedLockManagerFactory, DistributedLockManagerFactory>();
            services.AddTransient<IBlobClientProvider, BlobClientProvider>();
            services.AddTransient<ISingletonLockManager, SingletonLockManager>();
            services.AddTransient<IBlobClientFactory, BlobClientFactory>();
            services.AddTransient<QueueMessageExecutor>();
            services.AddTransient<IQueueMessageExecutor>(provider =>
            {
                var singletonManager = provider.GetRequiredService<ISingletonLockManager>();
                var executor = provider.GetRequiredService<QueueMessageExecutor>();
                var attribute = provider.GetRequiredService<IOptions<SingletonHostOptions>>();
                var timeProvider = provider.GetRequiredService<TimeProvider>();

                return new SingletonQueueMessageExecutor(singletonManager, executor, timeProvider, attribute);
            });

            services.PostConfigure<ListenerHostOptions>(storageAccountOptions => storageAccountOptions.NewBatchThreshold = 0);

            return services;
        }
    }
}