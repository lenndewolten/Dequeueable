using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebJobs.Azure.QueueStorage.Core.Configurations;
using WebJobs.Azure.QueueStorage.Core.Extentions;
using WebJobs.Azure.QueueStorage.Core.Attributes;
using WebJobs.Azure.QueueStorage.Job.Configurations;
using WebJobs.Azure.QueueStorage.Job.Services;

namespace WebJobs.Azure.QueueStorage.Job.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddAzureQueueStorageJob<TJob>(this IServiceCollection services, Action<JobHostOptions>? options = null)
            where TJob : class, IAzureQueueJob
        {
            services.AddOptions<JobHostOptions>().BindConfiguration(HostOptions.WebHost);

            if (options is not null)
            {
                services.Configure(options);
            }

            services.AddAzureQueueStorageServices<TJob>();
            services.AddHostedService<JobHost>();
            services.AddSingleton<IQueueMessagesHandler, QueueMessagesHandler>();

            var singletonAttribute = typeof(TJob).GetSingletonAttribute();
            if (singletonAttribute is not null)
            {
                services.RegisterSingletonServices(singletonAttribute);
            }

            services.AddSingleton<IHostOptions>(provider =>
            {
                var opt = provider.GetRequiredService<IOptions<JobHostOptions>>();
                return opt.Value;
            });

            return services;
        }

        private static IServiceCollection RegisterSingletonServices(this IServiceCollection services, SingletonAttribute singletonAttribute)
        {
            return services.AddAzureQueueStorageSingletonServices(singletonAttribute);
        }
    }
}
