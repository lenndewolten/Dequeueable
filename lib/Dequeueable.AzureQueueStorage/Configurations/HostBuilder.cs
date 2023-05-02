using Dequeueable.AzureQueueStorage.Factories;
using Dequeueable.AzureQueueStorage.Services.Hosts;
using Dequeueable.AzureQueueStorage.Services.Queues;
using Dequeueable.AzureQueueStorage.Services.Singleton;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dequeueable.AzureQueueStorage.Configurations
{
    internal class HostBuilder : IDequeueableHostBuilder
    {
        private readonly IServiceCollection _services;

        public HostBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public IDequeueableHostBuilder RunAsJob(Action<HostOptions>? options = null)
        {
            _services.AddOptions<HostOptions>().BindConfiguration(HostOptions.Dequeueable)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            if (options is not null)
            {
                _services.Configure(options);
            }

            _services.AddHostedService<JobHost>();
            _services.AddSingleton<IHostExecutor, JobExecutor>();

            _services.TryAddSingleton<IHostOptions>(provider =>
            {
                var opt = provider.GetRequiredService<IOptions<HostOptions>>();
                return opt.Value;
            });

            return this;
        }

        public IDequeueableHostBuilder RunAsListener(Action<ListenerOptions>? options = null)
        {
            _services.AddOptions<ListenerOptions>().BindConfiguration(HostOptions.Dequeueable)
                .Validate(ListenerOptions.ValidatePollingInterval, $"The '{nameof(ListenerOptions.MinimumPollingIntervalInMilliseconds)}' must not be greater than the '{nameof(ListenerOptions.MaximumPollingIntervalInMilliseconds)}'.")
                .Validate(ListenerOptions.ValidateNewBatchThreshold, $"The '{nameof(ListenerOptions.NewBatchThreshold)}' must not be greater than the '{nameof(ListenerOptions.BatchSize)}'.")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            if (options is not null)
            {
                _services.Configure(options);
            }

            _services.AddHostedService<QueueListenerHost>();
            _services.AddSingleton<IHostExecutor, QueueListenerExecutor>();

            _services.TryAddSingleton<IHostOptions>(provider =>
            {
                var opt = provider.GetRequiredService<IOptions<ListenerOptions>>();
                return opt.Value;
            });

            return this;
        }

        public IDequeueableHostBuilder AsSingleton(Action<SingletonOptions>? options = null)
        {
            _services.AddOptions<SingletonOptions>().BindConfiguration(SingletonOptions.Name)
                .Validate(SingletonOptions.ValidatePollingInterval, $"The '{nameof(SingletonOptions.MinimumPollingIntervalInSeconds)}' must not be greater than the '{nameof(SingletonOptions.MaximumPollingIntervalInSeconds)}'.")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            if (options is not null)
            {
                _services.Configure(options);
            }

            _services.AddTransient<IDistributedLockManager, DistributedLockManager>();
            _services.AddTransient<IDistributedLockManagerFactory, DistributedLockManagerFactory>();
            _services.AddTransient<IBlobClientProvider, BlobClientProvider>();
            _services.AddTransient<ISingletonLockManager, SingletonLockManager>();
            _services.AddTransient<IBlobClientFactory, BlobClientFactory>();
            _services.AddTransient<QueueMessageExecutor>();
            _services.AddTransient<IQueueMessageExecutor>(provider =>
            {
                var singletonManager = provider.GetRequiredService<ISingletonLockManager>();
                var executor = provider.GetRequiredService<QueueMessageExecutor>();
                var attribute = provider.GetRequiredService<IOptions<SingletonOptions>>();

                return new SingletonQueueMessageExecutor(singletonManager, executor, attribute);
            });

            _services.PostConfigure<ListenerOptions>(storageAccountOptions => storageAccountOptions.NewBatchThreshold = 0);

            return this;
        }
    }
}
