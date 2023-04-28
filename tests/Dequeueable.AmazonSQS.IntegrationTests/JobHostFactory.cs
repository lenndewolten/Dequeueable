﻿using Dequeueable.AmazonSQS.Extentions;
using Dequeueable.AmazonSQS.IntegrationTests.TestDataBuilders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dequeueable.AmazonSQS.IntegrationTests
{
    public class JobHostFactory<TFunction>
        where TFunction : class, IAmazonSQSFunction
    {
        public readonly IHostBuilder HostBuilder;
        private readonly Action<Configurations.HostOptions>? _options;

        public JobHostFactory(Action<Configurations.HostOptions>? overrideOptions = null, bool runAsSingleton = false)
        {
            if (overrideOptions is not null)
            {
                _options += overrideOptions;
            }

            HostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    var hostBuilder = services.AddAmazonSQSServices<TestFunction>()
                    .RunAsJob(_options);

                    if (runAsSingleton)
                    {
                        hostBuilder.AsSingleton();
                    }

                    services.AddTransient<IFakeService, FakeService>();
                });
        }

        public IHostBuilder ConfigureTestServices(Action<IServiceCollection> services)
        {
            HostBuilder.ConfigureServices(services);
            return HostBuilder;
        }

        public Services.Hosts.IHostExecutor Build()
        {
            var host = HostBuilder.Build();
            return host.Services.GetRequiredService<Services.Hosts.IHostExecutor>();
        }
    }
}
