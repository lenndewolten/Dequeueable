using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebJobs.Azure.QueueStorage.Job.Configurations;
using WebJobs.Azure.QueueStorage.Job.Extentions;
using WebJobs.Azure.QueueStorage.Job.IntegrationTests.TestDataBuilders;
using WebJobs.Azure.QueueStorage.Job.Services;

namespace WebJobs.Azure.QueueStorage.Job.IntegrationTests
{
    public class ApplicationFactory<TFunction>
        where TFunction : class, IAzureQueueJob
    {
        public readonly IHostBuilder HostBuilder;
        private readonly Action<JobHostOptions>? _options;


        public ApplicationFactory(string connectionString, string queueName, Action<JobHostOptions>? overrideOptions = null)
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

        public ApplicationFactory(string connectionString, string queueName)
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

        public IQueueMessagesHandler Build()
        {
            var host = HostBuilder.Build();
            return host.Services.GetRequiredService<IQueueMessagesHandler>();
        }
    }
}
