using Dequeueable.AmazonSQS.Services.Hosts;
using Dequeueable.AmazonSQS.Services.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dequeueable.AmazonSQS.Configurations
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

        public IDequeueableHostBuilder RunAsListener(Action<ListenerHostOptions>? options = null)
        {
            _services.AddOptions<ListenerHostOptions>().BindConfiguration(HostOptions.Dequeueable)
                .Validate(ListenerHostOptions.ValidatePollingInterval, $"The '{nameof(ListenerHostOptions.MinimumPollingIntervalInMilliseconds)}' must not be greater than the '{nameof(ListenerHostOptions.MaximumPollingIntervalInMilliseconds)}'.")
                .Validate(ListenerHostOptions.ValidateNewBatchThreshold, $"The '{nameof(ListenerHostOptions.NewBatchThreshold)}' must not be greater than the '{nameof(ListenerHostOptions.BatchSize)}'.")
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
                var opt = provider.GetRequiredService<IOptions<ListenerHostOptions>>();
                return opt.Value;
            });

            return this;
        }

        public IDequeueableHostBuilder AsSingleton()
        {
            _services.AddTransient<SingletonManager>();
            _services.AddTransient<QueueMessageExecutor>();
            _services.AddTransient<IQueueMessageExecutor>(provider =>
            {
                var singletonManager = provider.GetRequiredService<SingletonManager>();
                var executor = provider.GetRequiredService<QueueMessageExecutor>();

                return new SingletonQueueMessageExecutor(executor, singletonManager);
            });

            _services.PostConfigure<HostOptions>(options => options.AttributeNames = options.AttributeNames.Concat(new List<string> { "MessageGroupId" }).ToList());
            _services.PostConfigure<ListenerHostOptions>(options => options.AttributeNames = options.AttributeNames.Concat(new List<string> { "MessageGroupId" }).ToList());
            _services.PostConfigure<ListenerHostOptions>(options => options.NewBatchThreshold = 0);

            return this;
        }
    }
}
