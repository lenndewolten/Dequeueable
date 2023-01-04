using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using WebJobs.Azure.QueueStorage.Core.Attributes;
using WebJobs.Azure.QueueStorage.Core.Factories;
using WebJobs.Azure.QueueStorage.Core.Models;
using WebJobs.Azure.QueueStorage.Core.Services.Queues;
using WebJobs.Azure.QueueStorage.Function.Configurations;
using WebJobs.Azure.QueueStorage.Function.Extentions;
using WebJobs.Azure.QueueStorage.Function.Services;

namespace WebJobs.Azure.QueueStorage.Function.UnitTests.Extentions
{
    public class ServiceCollectionExtentionsTests
    {
        [Singleton("Id")]
        private class TestSingeltonFunction : IAzureQueueFunction
        {
            public Task ExecuteAsync(Message message, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        private class TestFunction : IAzureQueueFunction
        {
            public Task ExecuteAsync(Message message, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void Given_a_Host_when_AddAzureQueueStorageFunction_is_called_with_a_function_during_DI_then_all_the_host_services_are_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddAzureQueueStorageFunction<TestFunction>();
                    services.PostConfigure<FunctionHostOptions>(options =>
                    {
                        options.ConnectionString = "test";
                        options.QueueName = "testqueue";
                    });
                    services.AddSingleton(_ => new Mock<IQueueClientFactory>().Object);
                    services.AddSingleton(_ => new Mock<IBlobClientFactory>().Object);
                });

            // Act
            using var host = hostBuilder.Build();
            var hostServices = host.Services.GetServices<IHostedService>();

            // Assert
            hostServices.Any(s => s.GetType() == typeof(FunctionHost)).Should().BeTrue();
        }

        [Fact]
        public void Given_a_Host_when_AddAzureQueueStorageFunction_is_called_with_a_singleton_function_during_DI_then_all_the_host_services_are_registered_correctly()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddAzureQueueStorageFunction<TestSingeltonFunction>(options => options.NewBatchThreshold = 6);
                    services.PostConfigure<FunctionHostOptions>(options =>
                    {
                        options.ConnectionString = "test";
                        options.QueueName = "testqueue";
                    });
                    services.AddSingleton(_ => new Mock<IQueueClientFactory>().Object);
                    services.AddSingleton(_ => new Mock<IBlobClientFactory>().Object);
                });

            // Act
            using var host = hostBuilder.Build();
            var services = host.Services;

            // Assert
            services.GetServices<IHostedService>().Any(s => s.GetType() == typeof(FunctionHost)).Should().BeTrue();
            services.GetServices<IQueueMessageExecutor>().Should().HaveCount(2);
            services.GetRequiredService<IQueueListener>().Should().BeOfType(typeof(QueueListener));
            services.GetRequiredService<IOptions<FunctionHostOptions>>().Value.NewBatchThreshold.Should().Be(0);
        }
    }
}
