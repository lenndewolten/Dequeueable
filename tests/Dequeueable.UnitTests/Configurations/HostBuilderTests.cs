using Dequeueable.Configurations;
using Dequeueable.Hosts;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dequeueable.UnitTests.Configurations
{
    public class HostBuilderTests
    {
        [Fact]
        public void Given_a_DequeueableBuilder_when_AsJob_is_called_then_the_Host_is_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                .RegisterDequeueableServices<TestMessage>()
                .WithQueueMessageManager<TestQueueMessageManager>()
                .WithQueueMessageHandler<TestQueueMessageHandler>()
                .AsJob();
            });

            // Act
            var host = hostBuilder.Build();

            // Assert
            host.Services.GetRequiredService<IHostedService>().Should().BeOfType<JobHost>();
        }

        [Fact]
        public void Given_a_DequeueableBuilder_when_AsListener_is_called_then_the_Host_is_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                .RegisterDequeueableServices<TestMessage>()
                .WithQueueMessageManager<TestQueueMessageManager>()
                .WithQueueMessageHandler<TestQueueMessageHandler>()
                .AsListener<TestListenerHostOptions>();
            });

            // Act
            var host = hostBuilder.Build();

            // Assert
            host.Services.GetRequiredService<IHostedService>().Should().BeOfType<QueueListenerHost>();
        }
    }
}
