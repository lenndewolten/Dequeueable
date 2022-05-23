using JobHandlers.AzureQueueMessage.Configurations;
using JobHandlers.AzureQueueMessage.Handlers;
using JobHandlers.AzureQueueMessage.Services;
using JobHandlers.AzureQueueMessage.Services.Deleters;
using JobHandlers.AzureQueueMessage.Services.Retrievers;
using JobHandlers.AzureQueueMessage.Services.Updaters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobHandlers.AzureQueueMessage.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddAzureQueueMessageService<TExecutor>(this IServiceCollection services, Action<StorageAccountOptions>? options = null) where TExecutor : class, IQueueMessageExecutor
        {
            services.AddOptions<StorageAccountOptions>().BindConfiguration(StorageAccountOptions.StorageAccount);

            if (options is not null)
            {
                services.Configure(options);
            }

            services.AddHostedService<AzureQueueMessageHostService>();
            services.AddTransient<QueueMessageHandler>();
            services.AddTransient<QueueMessageExceptionHandler>();
            services.AddTransient<IQueueClientRetriever, QueueClientRetriever>();
            services.AddTransient<IQueueMessageRetriever, QueueMessageRetriever>();
            services.AddTransient<IQueueMessageUpdater, QueueMessageUpdater>();
            services.AddTransient<IQueueMessageDeleter, QueueMessageDeleter>();

            services.TryAddSingleton<QueueClientProvider>();
            services.TryAddTransient<IQueueMessageExecutor, TExecutor>();

            return services;
        }
    }
}
