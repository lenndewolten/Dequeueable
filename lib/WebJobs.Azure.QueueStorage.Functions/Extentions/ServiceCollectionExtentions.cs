using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WebJobs.Azure.QueueStorage.Functions.Attributes;
using WebJobs.Azure.QueueStorage.Functions.Configurations;
using WebJobs.Azure.QueueStorage.Functions.Factories;
using WebJobs.Azure.QueueStorage.Functions.Services.Hosts;
using WebJobs.Azure.QueueStorage.Functions.Services.Queues;
using WebJobs.Azure.QueueStorage.Functions.Services.Singleton;

namespace WebJobs.Azure.QueueStorage.Functions.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddAzureQueueStorageJob<TFunction>(this IServiceCollection services, Action<HostOptions>? options = null)
            where TFunction : class, IAzureQueueFunction
        {
            services.AddOptions<HostOptions>().BindConfiguration(HostOptions.WebHost);

            if (options is not null)
            {
                services.Configure(options);
            }

            services.AddAzureQueueStorageServices<TFunction>();
            services.AddHostedService<JobHost>();
            services.AddSingleton<IHostHandler, JobHostHandler>();

            var singletonAttribute = typeof(TFunction).GetSingletonAttribute();
            if (singletonAttribute is not null)
            {
                services.AddAzureQueueStorageSingletonServices(singletonAttribute);
            }

            return services;
        }

        public static IServiceCollection AddAzureQueueStorageListener<TFunction>(this IServiceCollection services, Action<ListenerOptions>? options = null)
            where TFunction : class, IAzureQueueFunction
        {
            services.AddOptions<ListenerOptions>().BindConfiguration(HostOptions.WebHost);

            if (options is not null)
            {
                services.Configure(options);
            }

            services.AddAzureQueueStorageServices<TFunction>();
            services.AddHostedService<ListenerHost>();
            services.AddSingleton<IHostHandler, ListenerHostHandler>();

            var singletonAttribute = typeof(TFunction).GetSingletonAttribute();
            if (singletonAttribute is not null)
            {
                services.AddAzureQueueStorageSingletonServices(singletonAttribute);
                services.PostConfigure<ListenerOptions>(storageAccountOptions => storageAccountOptions.NewBatchThreshold = 0);
            }

            services.AddSingleton<IHostOptions>(provider =>
            {
                var opt = provider.GetRequiredService<IOptions<ListenerOptions>>();
                return opt.Value;
            });

            return services;
        }

        private static IServiceCollection AddAzureQueueStorageServices<TFunction>(this IServiceCollection services)
            where TFunction : class, IAzureQueueFunction
        {

            services.AddSingleton<IQueueMessageManager, QueueMessageManager>();
            services.AddTransient<IQueueMessageHandler, QueueMessageHandler>();
            services.AddTransient<IQueueMessageExecutor, QueueMessageExecutor>();
            services.AddTransient<IQueueClientFactory, QueueClientFactory>();
            services.AddTransient<IAzureQueueFunction, TFunction>();
            services.TryAddTransient<IQueueClientProvider, QueueClientProvider>();

            services.TryAddSingleton<IHostOptions>(provider =>
            {
                var opt = provider.GetRequiredService<IOptions<HostOptions>>();
                return opt.Value;
            });

            return services;
        }

        private static IServiceCollection AddAzureQueueStorageSingletonServices(this IServiceCollection services, SingletonAttribute singletonAttribute)
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
