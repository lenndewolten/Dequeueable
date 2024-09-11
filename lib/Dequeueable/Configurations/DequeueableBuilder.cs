using Dequeueable.Hosts;
using Dequeueable.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dequeueable.Configurations
{
    public interface IDequeueableBuilder<TMessage> where TMessage : class
    {
        IQueueMessageHandlerBuilder<TMessage> WithQueueMessageManager<TImplementation>()
            where TImplementation : class, IQueueMessageManager<TMessage>;
    }

    internal sealed class DequeueableBuilder<TMessage>(IServiceCollection services) : IDequeueableBuilder<TMessage> where TMessage : class
    {
        public IQueueMessageHandlerBuilder<TMessage> WithQueueMessageManager<TImplementation>()
            where TImplementation : class, IQueueMessageManager<TMessage>
        {
            services.AddSingleton<IQueueMessageManager<TMessage>, TImplementation>();
            return new QueueMessageHandlerBuilder<TMessage>(services);
        }
    }

    public interface IQueueMessageHandlerBuilder<TMessage> where TMessage : class
    {
        IDequeueableHostBuilder<TMessage> WithQueueMessageHandler<TImplementation>()
            where TImplementation : class, IQueueMessageHandler<TMessage>;
    }

    internal sealed class QueueMessageHandlerBuilder<TMessage>(IServiceCollection services) : IQueueMessageHandlerBuilder<TMessage> where TMessage : class
    {
        public IDequeueableHostBuilder<TMessage> WithQueueMessageHandler<TImplementation>()
            where TImplementation : class, IQueueMessageHandler<TMessage>
        {
            services.AddTransient<IQueueMessageHandler<TMessage>, TImplementation>();
            return new DequeueableHostBuilder<TMessage>(services);
        }
    }

    public interface IDequeueableHostBuilder<TMessage> where TMessage : class
    {
        IServiceCollection AsListener<TOptions>(Action<TOptions>? options = null)
            where TOptions : class, IListenerHostOptions;

        IServiceCollection AsListener<TOptions, TDep1>(Action<TOptions, TDep1>? options = null)
           where TOptions : class, IListenerHostOptions
           where TDep1 : class;

        IServiceCollection AsJob();
    }

    internal sealed class DequeueableHostBuilder<TMessage>(IServiceCollection services) : IDequeueableHostBuilder<TMessage> where TMessage : class
    {
        public IServiceCollection AsListener<TOptions>(Action<TOptions>? options = null)
            where TOptions : class, IListenerHostOptions
        {
            services.AddOptions<TOptions>()
                .Configure(options ?? (opt => { }))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            RegisterListenerServices<TOptions>();

            return services;
        }

        public IServiceCollection AsListener<TOptions, TDep1>(Action<TOptions, TDep1>? options = null)
            where TOptions : class, IListenerHostOptions
            where TDep1 : class
        {
            services.AddOptions<TOptions>()
                .Configure(options ?? ((opt, dep1) => { }))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            RegisterListenerServices<TOptions>();

            return services;
        }

        public IServiceCollection AsJob()
        {
            services.AddHostedService<JobHost>();
            services.AddSingleton<IHostExecutor, JobExecutor<TMessage>>();

            return services;
        }

        private IServiceCollection RegisterListenerServices<TOptions>()
            where TOptions : class, IListenerHostOptions
        {
            services.AddHostedService<QueueListenerHost>();
            services.AddSingleton<IHostExecutor, QueueListenerExecutor<TMessage, TOptions>>();

            return services;
        }
    }

    public static class ServiceCollectionExtentions
    {
        public static IDequeueableBuilder<TMessage> RegisterDequeueableServices<TMessage>(this IServiceCollection services) where TMessage : class
        {
            services.TryAddSingleton(TimeProvider.System);

            return new DequeueableBuilder<TMessage>(services);
        }
    }
}
