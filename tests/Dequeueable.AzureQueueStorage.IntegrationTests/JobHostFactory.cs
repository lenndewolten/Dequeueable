using Dequeueable.AzureQueueStorage.Extentions;
using Dequeueable.AzureQueueStorage.IntegrationTests.TestDataBuilders;
using Dequeueable.AzureQueueStorage.Services.Hosts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dequeueable.AzureQueueStorage.IntegrationTests
{
    public class JobHostFactory<TFunction>
        where TFunction : class, IAzureQueueFunction
    {
        public readonly IHostBuilder HostBuilder;
        private readonly Action<Configurations.HostOptions>? _options;

        public JobHostFactory(string connectionString, string queueName, Action<Configurations.HostOptions>? overrideOptions = null)
        {
            _options = opt =>
            {
                opt.ConnectionString = connectionString;
                opt.QueueName = queueName;
            };

            if (overrideOptions is not null)
            {
                _options += overrideOptions;
            }

            HostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddAzureQueueStorageJob<TFunction>(_options);
                    services.AddTransient<IFakeService, FakeService>();
                });
        }

        public JobHostFactory(string connectionString, string queueName)
        {
            HostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services
                    .AddAzureQueueStorageJob<TFunction>(options =>
                    {
                        options.ConnectionString = connectionString;
                        options.QueueName = queueName;

                    });

                    services.AddTransient<IFakeService, FakeService>();
                });
        }

        public IHostBuilder ConfigureTestServices(Action<IServiceCollection> services)
        {
            HostBuilder.ConfigureServices(services);
            return HostBuilder;
        }

        public IHostHandler Build()
        {
            var host = HostBuilder.Build();
            return host.Services.GetRequiredService<IHostHandler>();
        }
    }
}
