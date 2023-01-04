using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebJobs.Azure.QueueStorage.Function.Configurations;
using WebJobs.Azure.QueueStorage.Function.Extentions;
using WebJobs.Azure.QueueStorage.Function.IntegrationTests.TestDataBuilders;
using WebJobs.Azure.QueueStorage.Function.Services;

namespace WebJobs.Azure.QueueStorage.Function.IntegrationTests
{
    public class ApplicationFactory<TFunction>
        where TFunction : class, IAzureQueueFunction
    {
        public readonly IHostBuilder HostBuilder;

        private readonly Action<FunctionHostOptions> _options = options =>
        {
            options.NewBatchThreshold = 0;
            options.MinimumPollingIntervalInMilliseconds = 0;
            options.MaximumPollingIntervalInMilliseconds = 50;
        };

        public ApplicationFactory(string connectionString, string queueName, Action<FunctionHostOptions>? overrideOptions = null)
        {
            Action<FunctionHostOptions> options = opt =>
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
                    services.AddAzureQueueStorageFunction<TFunction>(options);
                    services.AddTransient<IFakeService, FakeService>();
                });
        }

        public ApplicationFactory(string connectionString, string queueName)
        {
            var options = _options;

            HostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services
                    .AddAzureQueueStorageFunction<TFunction>(options =>
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

        public IQueueListener Build()
        {
            var host = HostBuilder.Build();
            return host.Services.GetRequiredService<IQueueListener>();
        }
    }
}
