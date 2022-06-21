using JobHandlers.AzureQueueMessage.Extentions;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.GuardiansOfTheGalaxy.Executors;
using SampleApp.GuardiansOfTheGalaxy.Services;

namespace SampleApp.GuardiansOfTheGalaxy
{
    internal static class ServiceCollectionExtentions
    {
        public static IServiceCollection ConfigureDefaultServices(this IServiceCollection services)
        {
            services.AddAzureQueueMessageService<CreateGuardianEventExecutor>(options => options.VisibilityTimeout = TimeSpan.FromSeconds(60));
            services.AddTransient<IGuardianCreator, GuardianCreator>();

            return services;
        }
    }
}
