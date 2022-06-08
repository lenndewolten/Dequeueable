using Azure.Storage.Queues.Models;
using JobHandlers.AzureQueueMessage.Extentions;
using JobHandlers.AzureQueueMessage.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JobHandlers.AzureQueueMessage.UnitTests.Extentions
{
    public class ServiceCollectionExtentionsTests
    {
        [Fact]
        public void Given_the_ServiceCollectionExtentions_when_AddAzureQueueMessageService_is_called_then_the_DI_for_the_hostservice_are_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services => services.AddAzureQueueMessageService<TestQueueMessageExecutor>());

            // Act
            var host = hostBuilder.Build();

            // Assert
            host.Services.GetRequiredService<IHostedService>();
        }
    }

    internal class TestQueueMessageExecutor : IQueueMessageExecutor
    {
        public Task Execute(QueueMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
