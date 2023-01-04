using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebJobs.Azure.QueueStorage.Core.Attributes;
using WebJobs.Azure.QueueStorage.Core.Configurations;
using WebJobs.Azure.QueueStorage.Core.Extentions;
using WebJobs.Azure.QueueStorage.Function.Configurations;
using WebJobs.Azure.QueueStorage.Function.Services;

namespace WebJobs.Azure.QueueStorage.Function.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddAzureQueueStorageFunction<TFunction>(this IServiceCollection services, Action<FunctionOptions>? options = null)
            where TFunction : class, IAzureQueueFunction
        {
            services.AddOptions<FunctionOptions>().BindConfiguration(HostOptions.WebHost);

            if (options is not null)
            {
                services.Configure(options);
            }

            services.AddAzureQueueStorageServices<TFunction>();
            services.AddHostedService<FunctionHost>();
            services.AddSingleton<IQueueListener, QueueListener>();

            var singletonAttribute = typeof(TFunction).GetSingletonAttribute();
            if (singletonAttribute is not null)
            {
                services.RegisterSingletonServices(singletonAttribute);
            }

            services.AddSingleton<IHostOptions>(provider =>
            {
                var opt = provider.GetRequiredService<IOptions<FunctionOptions>>();
                return opt.Value;
            });

            return services;
        }

        private static IServiceCollection RegisterSingletonServices(this IServiceCollection services, SingletonAttribute singletonAttribute)
        {
            services.AddAzureQueueStorageSingletonServices(singletonAttribute);
            services.PostConfigure<FunctionOptions>(storageAccountOptions => storageAccountOptions.NewBatchThreshold = 0);
            return services;
        }
    }
}
