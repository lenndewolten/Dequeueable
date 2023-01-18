using Dequeueable.AzureQueueStorage.Configurations;
using Dequeueable.AzureQueueStorage.Extentions;
using Dequeueable.AzureQueueStorage.IntegrationTests.TestDataBuilders;
using Dequeueable.AzureQueueStorage.Services.Hosts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dequeueable.AzureQueueStorage.IntegrationTests
{
    public class ListenerHostFactory<TFunction>
        where TFunction : class, IAzureQueueFunction
    {
        public readonly IHostBuilder HostBuilder;

        private readonly Action<ListenerOptions> _options = options =>
        {
            options.NewBatchThreshold = 0;
            options.MinimumPollingIntervalInMilliseconds = 0;
            options.MaximumPollingIntervalInMilliseconds = 50;
        };

        public ListenerHostFactory(string connectionString, string queueName, Action<ListenerOptions>? overrideOptions = null)
        {
            Action<ListenerOptions> options = opt =>
            {
                opt.ConnectionString = connectionString;
                opt.QueueName = queueName;
            };

            options += _options;

            if (overrideOptions is not null)
            {
                options += overrideOptions;
            }

            HostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddAzureQueueStorageListener<TFunction>(options);
                    services.AddTransient<IFakeService, FakeService>();
                });
        }

        public ListenerHostFactory(string connectionString, string queueName)
        {
            var options = _options;

            HostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services
                    .AddAzureQueueStorageListener<TFunction>(options =>
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
