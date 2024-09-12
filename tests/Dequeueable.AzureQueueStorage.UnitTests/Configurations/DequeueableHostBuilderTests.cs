using Dequeueable.AzureQueueStorage.Configurations;
using Dequeueable.AzureQueueStorage.Extentions;
using Dequeueable.AzureQueueStorage.Models;
using Dequeueable.AzureQueueStorage.Services.Queues;
using Dequeueable.AzureQueueStorage.Services.Singleton;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Dequeueable.AzureQueueStorage.UnitTests.Configurations
{
    public class DequeueableHostBuilderTests
    {
        private sealed class TestFunction : IAzureQueueFunction
        {
            public Task ExecuteAsync(Message message, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void Given_a_DequeueableHostBuilder_when_RunAsJob_is_called_then_the_Host_is_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                .AddAzureQueueStorageServices<TestFunction>()
                .RunAsJob(options =>
                {
                    options.QueueName = "test";
                    options.ConnectionString = "UseDevelopmentStorage=true";
                });
            });

            // Act
            var host = hostBuilder.Build();

            // Assert
            host.Services.GetRequiredService<IHostedService>().GetType().Name.Should().Be("JobHost");
        }

        [Fact]
        public void Given_a_DequeueableHostBuilder_when_RunAsJob_is_called_then_IHostOptions_is_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                .AddAzureQueueStorageServices<TestFunction>()
                .RunAsJob(options =>
                {
                    options.QueueName = "test";
                    options.ConnectionString = "UseDevelopmentStorage=true";
                });
            });

            // Act
            var host = hostBuilder.Build();

            // Assert
            host.Services.GetRequiredService<IHostOptions>().Should().BeOfType<AzureQueueStorage.Configurations.HostOptions>();
        }

        [Fact]
        public void Given_a_DequeueableHostBuilder_when_RunAsListener_is_called_then_the_Host_is_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                .AddAzureQueueStorageServices<TestFunction>()
                .RunAsListener(options =>
                {
                    options.QueueName = "test";
                    options.ConnectionString = "UseDevelopmentStorage=true";
                });
            });

            // Act
            var host = hostBuilder.Build();

            // Assert
            host.Services.GetRequiredService<IHostedService>().GetType().Name.Should().Be("QueueListenerHost");
        }

        [Fact]
        public void Given_a_DequeueableHostBuilder_when_RunAsListener_is_called_then_IHostOptions_is_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                .AddAzureQueueStorageServices<TestFunction>()
                .RunAsListener(options =>
                {
                    options.QueueName = "test";
                    options.ConnectionString = "UseDevelopmentStorage=true";
                });
            });

            // Act
            var host = hostBuilder.Build();

            // Assert
            host.Services.GetRequiredService<IHostOptions>().Should().BeOfType<ListenerHostOptions>();
        }

        [Fact]
        public void Given_a_DequeueableHostBuilder_when_AsSingleton_is_called_then_IQueueMessageExecutor_is_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                .AddAzureQueueStorageServices<TestFunction>()
                .RunAsJob(options =>
                {
                    options.QueueName = "test";
                    options.ConnectionString = "UseDevelopmentStorage=true";
                })
                .AsSingleton(opt => opt.Scope = "test");
            });

            // Act
            var host = hostBuilder.Build();

            // Assert
            host.Services.GetRequiredService<IQueueMessageExecutor>().Should().BeOfType<SingletonQueueMessageExecutor>();
        }

        [Fact]
        public void Given_a_DequeueableHostBuilder_when_AsSingleton_is_called_then_ListenerHostOptions_NewBatchThreshold_is_zero()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                .AddAzureQueueStorageServices<TestFunction>()
                .RunAsListener(options =>
                {
                    options.QueueName = "test";
                    options.ConnectionString = "UseDevelopmentStorage=true";
                })
                .AsSingleton(opt => opt.Scope = "test");
            });

            // Act
            var host = hostBuilder.Build();

            // Assert
            host.Services.GetRequiredService<IOptions<ListenerHostOptions>>().Value.NewBatchThreshold.Should().Be(0);
        }
    }
}
