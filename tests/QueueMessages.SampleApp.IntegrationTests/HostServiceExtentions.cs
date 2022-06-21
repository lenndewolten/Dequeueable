using JobHandlers.AzureQueueMessage.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace JobHandlers.AzureQueueMessage.IntegrationTests
{
    internal static class HostServiceExtentions
    {
        public static IHostedService GetTestHostService<THostService>(this IHost host) where THostService : IHostedService
        {
            return host.Services.GetServices<IHostedService>().Single(service => service.GetType() == typeof(THostService));
        }

        public static QueueMessageHandler GetQueueMessageHandler(this IHost host)
        {
            return host.Services.GetRequiredService<QueueMessageHandler>();
        }
    }
}
