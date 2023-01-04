using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebJobs.Azure.QueueStorage.Core.Attributes;
using WebJobs.Azure.QueueStorage.Core.Factories;
using WebJobs.Azure.QueueStorage.Core.Services.Queues;
using WebJobs.Azure.QueueStorage.Core.Services.Singleton;

namespace WebJobs.Azure.QueueStorage.Core.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddAzureQueueStorageServices<TFunction>(this IServiceCollection services)
            where TFunction : class, IAzureQueueFunction
        {
            services.AddSingleton<IQueueMessageManager, QueueMessageManager>();
            services.AddTransient<IQueueMessageHandler, QueueMessageHandler>();
            services.AddTransient<IQueueMessageExecutor, QueueMessageExecutor>();
            services.AddTransient<IQueueClientFactory, QueueClientFactory>();
            services.AddTransient<IAzureQueueFunction, TFunction>();
            services.TryAddTransient<IQueueClientProvider, QueueClientProvider>();

            return services;
        }

        public static IServiceCollection AddAzureQueueStorageSingletonServices(this IServiceCollection services, SingletonAttribute singletonAttribute)
        {
            services.AddTransient<IDistributedLockManager, DistributedLockManager>();
            services.AddTransient<IDistributedLockManagerFactory, DistributedLockManagerFactory>();
            services.AddTransient<IBlobClientProvider, BlobClientProvider>();
            services.AddTransient<ISingletonLockManager, SingletonLockManager>();
            services.AddTransient<IBlobClientFactory, BlobClientFactory>();
            services.AddSingleton(_ => singletonAttribute);

            services.AddTransient<QueueMessageExecutor>();
            services.AddTransient<IQueueMessageExecutor>(provider =>
            {
                var singletonManager = provider.GetRequiredService<ISingletonLockManager>();
                var executor = provider.GetRequiredService<QueueMessageExecutor>();
                var attribute = provider.GetRequiredService<SingletonAttribute>();

                return new SingletonQueueMessageExecutor(singletonManager, executor, attribute);
            });

            return services;
        }
    }
}
