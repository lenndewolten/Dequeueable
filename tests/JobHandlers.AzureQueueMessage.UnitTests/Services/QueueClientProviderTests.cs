using Azure;
using Azure.Storage.Queues;
using FluentAssertions;
using JobHandlers.AzureQueueMessage.Services;
using JobHandlers.AzureQueueMessage.Services.Retrievers;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JobHandlers.AzureQueueMessage.UnitTests
{
    public class QueueClientProviderTests
    {
        [Fact]
        public async Task Given_a_QueueClientProvider_when_Get_is_called_without_a_queueName_then_an_InvalidOperationException_is_thrown()
        {
            // Arrange
            string? queueName = null;
            var queueClientRetrieverMock = new Mock<IQueueClientRetriever>(MockBehavior.Strict);

            var sut = new QueueClientProvider(queueClientRetrieverMock.Object);

            // Act
            Func<Task> act = () => sut.Get(queueName, CancellationToken.None);

            // Assert
            await act.Should().ThrowExactlyAsync<InvalidOperationException>()
                .WithMessage("Invalid Queue Name. Make sure that it is defined in the app settings");
        }

        [Fact]
        public async Task Given_a_QueueClientProvider_when_Get_is_called_once_then_the_client_is_retrieved_correctly()
        {
            // Arrange
            var queueName = "test-queue";
            var queueClientRetrieverMock = new Mock<IQueueClientRetriever>(MockBehavior.Strict);
            var queueClientMock = new Mock<QueueClient>(MockBehavior.Strict);

            queueClientMock.Setup(c => c.CreateIfNotExistsAsync(null, CancellationToken.None))
                .ReturnsAsync(new Mock<Response>().Object)
                .Verifiable();

            queueClientRetrieverMock.Setup(q => q.Retrieve(queueName))
                .Returns(queueClientMock.Object);

            var sut = new QueueClientProvider(queueClientRetrieverMock.Object);

            // Act
            var actual = await sut.Get(queueName, CancellationToken.None);

            // Assert
            actual.Should().NotBeNull();
            queueClientMock.Verify();
        }

        [Fact]
        public async Task Given_a_QueueClientProvider_when_Get_is_called_more_than_once_then_the_client_is_retrieved_correctly()
        {
            // Arrange
            var queueName = "test-queue";
            var queueClientRetrieverMock = new Mock<IQueueClientRetriever>(MockBehavior.Strict);
            var queueClientMock = new Mock<QueueClient>(MockBehavior.Strict);

            queueClientMock.Setup(c => c.CreateIfNotExistsAsync(null, CancellationToken.None))
                .ReturnsAsync(new Mock<Response>().Object);

            queueClientRetrieverMock.Setup(q => q.Retrieve(queueName))
                .Returns(queueClientMock.Object);

            var sut = new QueueClientProvider(queueClientRetrieverMock.Object);

            // Act
            var clients = new List<QueueClient>();

            for (int i = 0; i < 4; i++)
            {
                clients.Add(await sut.Get(queueName, CancellationToken.None));
            }

            // Assert
            clients.Should().AllBeEquivalentTo(queueClientMock.Object);
            queueClientRetrieverMock.Verify(q => q.Retrieve(queueName), Times.Once);
        }
    }
}
