using Dequeueable.AmazonSQS.Models;
using Dequeueable.AmazonSQS.Services.Queues;
using Dequeueable.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Dequeueable.AmazonSQS.Configurations
{
    internal sealed class DequeueableHostBuilder(IServiceCollection services) : IDequeueableHostBuilder
    {
        public IDequeueableSingletonHostBuilder RunAsJob(Action<HostOptions>? options = null)
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

            return new DequeueableSingletonHostBuilder(services);
        }

        public IDequeueableSingletonHostBuilder RunAsListener(Action<ListenerHostOptions>? options = null)
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

            return new DequeueableSingletonHostBuilder(services);
        }
    }
}
