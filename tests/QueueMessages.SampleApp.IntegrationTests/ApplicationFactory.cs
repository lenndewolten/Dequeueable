using JobHandlers.AzureQueueMessage.Extentions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace JobHandlers.AzureQueueMessage.IntegrationTests
{
    public class ApplicationFactory
    {
        private readonly IHostBuilder _hostBuilder;

        public ApplicationFactory()
        {
            _hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddAzureQueueMessageService<TestQueueMessageExecutor>());
        }

        public IHostBuilder ConfigureTestServices(Action<IServiceCollection> services)
        {
            _hostBuilder.ConfigureServices(services);
            return _hostBuilder;
        }

        public IHost Build()
        {
            return _hostBuilder.Build();
        }
    }
}
