using Azure.Storage.Queues.Models;
using FluentAssertions;
using JobHandlers.AzureQueueMessage.Handlers;
using JobHandlers.AzureQueueMessage.Services;
using JobHandlers.AzureQueueMessage.Services.Deleters;
using JobHandlers.AzureQueueMessage.Services.Retrievers;
using JobHandlers.AzureQueueMessage.UnitTests.TestDataBuilders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JobHandlers.AzureQueueMessage.UnitTests
{
    public class QueueMessageHandlerTests
    {
        [Fact]
        public async Task Given_a_QueueMessageHandler_when_HandleAsync_is_called_but_no_QueueMessage_is_retrieved_then_the_application_stops_correctly()
        {
            // Arrange
            var queueMessageRetrieverMock = new Mock<IQueueMessageRetriever>(MockBehavior.Strict);
            var queueMessageExecutorMock = new Mock<IQueueMessageExecutor>(MockBehavior.Strict);
            var queueMessageDeleterMock = new Mock<IQueueMessageDeleter>(MockBehavior.Strict);
            var queueMessageExceptionHandlerMock = new Mock<IQueueMessageExceptionHandler>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<QueueMessageHandler>>();
            var hostApplicationLifetimeMock = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);

            queueMessageRetrieverMock.Setup(u => u.Retrieve(It.IsAny<CancellationToken>()))
                .ReturnsAsync((QueueMessage?)null);

            hostApplicationLifetimeMock.Setup(l => l.StopApplication()).Verifiable();

            var sut = new QueueMessageHandler(queueMessageRetrieverMock.Object,
                queueMessageExecutorMock.Object,
                queueMessageDeleterMock.Object,
                queueMessageExceptionHandlerMock.Object,
                loggerMock.Object,
                hostApplicationLifetimeMock.Object);

            // Act
            await sut.HandleAsync(CancellationToken.None);

            // Assert
            queueMessageExecutorMock.VerifyNoOtherCalls();
            hostApplicationLifetimeMock.Verify();
            loggerMock.Verify(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No message detected on the queue")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        }

        [Fact]
        public async Task Given_a_QueueMessageHandler_when_HandleAsync_is_called_and_a_QueueMessage_is_retrieved_then_message_is_handled_correctly()
        {
            // Arrange
            var queueMessage = new QueueMesssageTestDataBuilder().Build();

            var queueMessageRetrieverMock = new Mock<IQueueMessageRetriever>(MockBehavior.Strict);
            var queueMessageExecutorMock = new Mock<IQueueMessageExecutor>(MockBehavior.Strict);
            var queueMessageDeleterMock = new Mock<IQueueMessageDeleter>(MockBehavior.Strict);
            var queueMessageExceptionHandlerMock = new Mock<IQueueMessageExceptionHandler>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<QueueMessageHandler>>();
            var hostApplicationLifetimeMock = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);

            queueMessageRetrieverMock.Setup(u => u.Retrieve(It.IsAny<CancellationToken>()))
                .ReturnsAsync(queueMessage);

            queueMessageExecutorMock.Setup(e => e.Execute(queueMessage, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            queueMessageDeleterMock.Setup(d => d.Delete(queueMessage, CancellationToken.None))
                .Returns(Task.CompletedTask)
                .Verifiable();

            hostApplicationLifetimeMock.Setup(l => l.StopApplication()).Verifiable();

            var sut = new QueueMessageHandler(queueMessageRetrieverMock.Object,
                queueMessageExecutorMock.Object,
                queueMessageDeleterMock.Object,
                queueMessageExceptionHandlerMock.Object,
                loggerMock.Object,
                hostApplicationLifetimeMock.Object);

            // Act
            await sut.HandleAsync(CancellationToken.None);

            // Assert
            queueMessageExecutorMock.Verify();
            queueMessageDeleterMock.Verify();
            hostApplicationLifetimeMock.Verify();
            loggerMock.Verify(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Finished executing message with id: '{queueMessage.MessageId}'")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        }

        [Fact]
        public async Task Given_a_QueueMessageHandler_when_HandleAsync_is_called_but_executing_the_message_throws_an_exception_then_the_exception_is_handled_correctly()
        {
            // Arrange
            var queueMessage = new QueueMesssageTestDataBuilder().Build();
            var exception = new Exception("test exception");

            var queueMessageRetrieverMock = new Mock<IQueueMessageRetriever>(MockBehavior.Strict);
            var queueMessageExecutorMock = new Mock<IQueueMessageExecutor>(MockBehavior.Strict);
            var queueMessageDeleterMock = new Mock<IQueueMessageDeleter>(MockBehavior.Strict);
            var queueMessageExceptionHandlerMock = new Mock<IQueueMessageExceptionHandler>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<QueueMessageHandler>>();
            var hostApplicationLifetimeMock = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);

            queueMessageRetrieverMock.Setup(u => u.Retrieve(It.IsAny<CancellationToken>()))
                .ReturnsAsync(queueMessage);

            queueMessageExecutorMock.Setup(e => e.Execute(queueMessage, It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            queueMessageExceptionHandlerMock.Setup(q => q.Handle(queueMessage, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            hostApplicationLifetimeMock.Setup(l => l.StopApplication()).Verifiable();

            var sut = new QueueMessageHandler(queueMessageRetrieverMock.Object,
                queueMessageExecutorMock.Object,
                queueMessageDeleterMock.Object,
                queueMessageExceptionHandlerMock.Object,
                loggerMock.Object,
                hostApplicationLifetimeMock.Object);

            // Act
            Func<Task> act = () => sut.HandleAsync(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>();

            queueMessageDeleterMock.VerifyNoOtherCalls();
            queueMessageExceptionHandlerMock.Verify();
            hostApplicationLifetimeMock.Verify();
            loggerMock.Verify(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Processing message '{queueMessage.MessageId}' failed")),
                It.Is<Exception>(ex => ex.Message == exception.Message),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        }
    }
}