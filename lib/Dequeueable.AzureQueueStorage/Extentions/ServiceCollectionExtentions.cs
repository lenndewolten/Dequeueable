using Dequeueable.AzureQueueStorage.Configurations;
using Dequeueable.AzureQueueStorage.Factories;
using Dequeueable.AzureQueueStorage.Services.Hosts;
using Dequeueable.AzureQueueStorage.Services.Queues;
using Dequeueable.AzureQueueStorage.Services.Singleton;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dequeueable.AzureQueueStorage.Extentions
{
    /// <summary>
    /// Extension methods for adding configuration related of the Queue services to the DI container via <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtentions
    {
        /// <summary>
        /// Adds the Azure Queue services and the function of the type specified in <typeparamref name="TFunction"/> to the
        /// specified <see cref="IServiceCollection"/>. 
        /// The application will run as a job, from start to finish, and will automatically shutdown when the messages are executed.
        /// </summary>
        /// <typeparam name="TFunction">The type implementing the <see cref="IAzureQueueFunction"/></typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
        /// <param name="options">Optional action of type <see cref="HostOptions"/> to configure the app settings on the host.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddAzureQueueStorageJob<TFunction>(this IServiceCollection services, Action<HostOptions>? options = null)
            where TFunction : class, IAzureQueueFunction
        {
            services.AddOptions<HostOptions>().BindConfiguration(HostOptions.Dequeueable);

            if (options is not null)
            {
                services.Configure(options);
            }

            services.AddAzureQueueStorageServices<TFunction>();
            services.AddHostedService<JobHost>();
            services.AddSingleton<IHost, JobExecutor>();

            var singletonAttribute = typeof(TFunction).GetSingletonAttribute();
            if (singletonAttribute is not null)
            {
                services.AddAzureQueueStorageSingletonServices(singletonAttribute);
            }

            return services;
        }

        /// <summary>
        /// Adds the Azure Queue services and the function of the type specified in <typeparamref name="TFunction"/> to the
        /// specified <see cref="IServiceCollection"/>. 
        /// The application will run a Queue Listener as a BackGroundService />.
        /// </summary>
        /// <typeparam name="TFunction">The type implementing the <see cref="IAzureQueueFunction"/></typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
        /// <param name="options">Optional action of type <see cref="ListenerOptions"/> to configure the app settings on the host.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddAzureQueueStorageListener<TFunction>(this IServiceCollection services, Action<ListenerOptions>? options = null)
            where TFunction : class, IAzureQueueFunction
        {
            services.AddOptions<ListenerOptions>().BindConfiguration(HostOptions.Dequeueable);

            if (options is not null)
            {
                services.Configure(options);
            }

            services.AddAzureQueueStorageServices<TFunction>();
            services.AddHostedService<QueueListenerHost>();
            services.AddSingleton<IHost, QueueListenerExecutor>();

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
