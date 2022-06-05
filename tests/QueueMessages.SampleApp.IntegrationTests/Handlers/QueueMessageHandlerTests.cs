using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using FluentAssertions;
using JobHandlers.AzureQueueMessage.Configurations;
using JobHandlers.AzureQueueMessage.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JobHandlers.AzureQueueMessage.IntegrationTests.Handlers
{
    public sealed class QueueMessageHandlerTests : IAsyncLifetime
    {
        private const int _blobPort = 4000;
        private const int _queuePort = 4001;
        private const int _tablePort = 4002;
        private const string _queueName = "dockerqueue";
        private const string _message = "integration test messages";

        private readonly string _connectionString = $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:{_blobPort}/devstoreaccount1;QueueEndpoint=http://127.0.0.1:{_queuePort}/devstoreaccount1;TableEndpoint=http://127.0.0.1:{_tablePort}/devstoreaccount1;";
        private readonly QueueClient _queueClient;

        private readonly IDockerContainer _testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("mcr.microsoft.com/azure-storage/azurite")
                .WithPortBinding(_blobPort, 10000)
                .WithPortBinding(_queuePort, 10001)
                .WithPortBinding(_tablePort, 10002)
                .WithCleanUp(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(10001))
                .Build();

        public QueueMessageHandlerTests()
        {
            _queueClient = new QueueClient(_connectionString, _queueName, new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
        }

        public Task DisposeAsync()
        {
            return _testcontainersBuilder.DisposeAsync().AsTask();
        }

        public async Task InitializeAsync()
        {
            await _testcontainersBuilder.StartAsync();
            await _queueClient.CreateIfNotExistsAsync();
            await _queueClient.SendMessageAsync(_message);
        }

        [Fact]
        public async Task Given_a_QueueMessageHandler_when_HandleAsync_is_called_then_the_executor_is_called_correctly_and_the_message_is_deleted()
        {
            // Arrange
            var factory = new ApplicationFactory();
            var queueMessageExecutorMock = new Mock<IQueueMessageExecutor>(MockBehavior.Strict);

            queueMessageExecutorMock.Setup(e => e.Execute(It.Is<QueueMessage>(m => m.Body.ToString() == _message.ToString()), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            factory.ConfigureTestServices(services =>
            {
                services.AddTransient(_ => queueMessageExecutorMock.Object);
                services.AddOptions<StorageAccountOptions>().Configure(opt =>
                {
                    opt.ConnectionString = _connectionString;
                    opt.QueueName = _queueName;
                });
            });

            var host = factory.Build();

            // Act
            var queueMessageHandler = host.GetQueueMessageHandler();
            await queueMessageHandler.HandleAsync(CancellationToken.None);

            // Assert
            queueMessageExecutorMock.Verify();
            var peekedMessage = await _queueClient.PeekMessageAsync();
            peekedMessage.Value.Should().BeNull();
        }

        [Fact]
        public async Task Given_a_QueueMessageHandler_when_HandleAsync_is_called_and_a_message_is_retrieved_but_an_exception_occurred_then_the_message_is_enqueued_correctly()
        {
            // Arrange
            var factory = new ApplicationFactory();
            var queueMessageExecutorMock = new Mock<IQueueMessageExecutor>(MockBehavior.Strict);

            queueMessageExecutorMock.Setup(e => e.Execute(It.Is<QueueMessage>(m => m.Body.ToString() == _message.ToString()), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test error"));

            factory.ConfigureTestServices(services =>
            {
                services.AddTransient(_ => queueMessageExecutorMock.Object);
                services.AddOptions<StorageAccountOptions>().Configure(opt =>
                {
                    opt.ConnectionString = _connectionString;
                    opt.QueueName = _queueName;
                    opt.MaxDequeueCount = 5;
                });
            });

            var host = factory.Build();

            // Act
            var queueMessageHandler = host.GetQueueMessageHandler();
            Func<Task> act = () => queueMessageHandler.HandleAsync(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            var peekedMessage = await _queueClient.PeekMessageAsync();
            peekedMessage.Value.DequeueCount.Should().Be(1);
        }

        [Fact]
        public async Task Given_a_QueueMessageHandler_with_max_dequeue_count_1_when_HandleAsync_is_called_and_a_message_is_retrieved_with_dequeue_count_1_and_an_exception_occurred_then_the_message_is_moved_to_the_poisen_queue()
        {
            // Arrange
            var factory = new ApplicationFactory();
            var queueMessageExecutorMock = new Mock<IQueueMessageExecutor>(MockBehavior.Strict);

            queueMessageExecutorMock.Setup(e => e.Execute(It.Is<QueueMessage>(m => m.Body.ToString() == _message.ToString()), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test error"));

            factory.ConfigureTestServices(services =>
            {
                services.AddTransient(_ => queueMessageExecutorMock.Object);
                services.AddOptions<StorageAccountOptions>().Configure(opt =>
                {
                    opt.ConnectionString = _connectionString;
                    opt.QueueName = _queueName;
                    opt.MaxDequeueCount = 1;
                });
            });

            var host = factory.Build();

            // Act
            var queueMessageHandler = host.GetQueueMessageHandler();
            Func<Task> act = () => queueMessageHandler.HandleAsync(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            var poisenQueueClient = new QueueClient(_connectionString, $"{_queueName}-poisen", new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
            var peekedMessage = await poisenQueueClient.PeekMessageAsync();
            peekedMessage.Value.Body.ToString().Should().Be(_message);
        }
    }
}