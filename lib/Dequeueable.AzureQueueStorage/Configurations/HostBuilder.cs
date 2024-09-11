using Dequeueable.AzureQueueStorage.Factories;
using Dequeueable.AzureQueueStorage.Models;
using Dequeueable.AzureQueueStorage.Services.Queues;
using Dequeueable.AzureQueueStorage.Services.Singleton;
using Dequeueable.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dequeueable.AzureQueueStorage.Configurations
{
    internal class HostBuilder(IServiceCollection services) : IDequeueableHostBuilder
    {
        public IDequeueableHostBuilder RunAsJob(Action<HostOptions>? options = null)
        {
            services.AddOptions<HostOptions>().BindConfiguration(HostOptions.Dequeueable)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            if (options is not null)
            {
                services.Configure(options);
            }

            services.RegisterDequeueableServices<Message>()
                .WithQueueMessageManager<QueueMessageManager>()
                .WithQueueMessageHandler<QueueMessageHandler>()
                .AsJob();

            services.TryAddSingleton<IHostOptions>(provider =>
            {
                var opt = provider.GetRequiredService<IOptions<HostOptions>>();
                return opt.Value;
            });

            return this;
        }

        public IDequeueableHostBuilder RunAsListener(Action<ListenerHostOptions>? options = null)
        {
            services.AddOptions<ListenerHostOptions>().BindConfiguration(HostOptions.Dequeueable)
                .Validate(ListenerHostOptions.ValidatePollingInterval, $"The '{nameof(ListenerHostOptions.MinimumPollingIntervalInMilliseconds)}' must not be greater than the '{nameof(ListenerHostOptions.MaximumPollingIntervalInMilliseconds)}'.")
                .Validate(ListenerHostOptions.ValidateNewBatchThreshold, $"The '{nameof(ListenerHostOptions.NewBatchThreshold)}' must not be greater than the '{nameof(ListenerHostOptions.BatchSize)}'.")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            if (options is not null)
            {
                services.Configure(options);
            }

            services.RegisterDequeueableServices<Message>()
                .WithQueueMessageManager<QueueMessageManager>()
                .WithQueueMessageHandler<QueueMessageHandler>()
                .AsListener(options);

            services.TryAddSingleton<IHostOptions>(provider =>
            {
                var opt = provider.GetRequiredService<IOptions<ListenerHostOptions>>();
                return opt.Value;
            });

            return this;
        }

        public IDequeueableHostBuilder AsSingleton(Action<SingletonHostOptions>? options = null)
        {
            services.AddOptions<SingletonHostOptions>().BindConfiguration(SingletonHostOptions.Name)
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

            return this;
        }
    }
}
