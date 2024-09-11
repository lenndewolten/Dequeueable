using Dequeueable.Hosts;
using Dequeueable.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Dequeueable.Configurations
{
    internal sealed class DequeueableHostBuilder<TMessage>(IServiceCollection services) : IDequeueableHostBuilder<TMessage> where TMessage : class, IQueueMessage
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
}
