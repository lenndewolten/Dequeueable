using Dequeueable.Hosts;
using Dequeueable.Queues;
using Dequeueable.UnitTests.Configurations;
using Dequeueable.UnitTests.TestDataBuilders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Dequeueable.UnitTests.Hosts
{
    public class QueueListenerExecutorTests
    {
        [Fact]
        public async Task Given_a_QueueListenerExecutor_when_HandleAsync_is_called_but_no_messages_are_retrieved_then_the_handler_is_not_called()
        {
            // Arrange
            var queueMessageManagerMock = new Mock<IQueueMessageManager<TestMessage>>(MockBehavior.Strict);
            var queueMessageHandlerMock = new Mock<IQueueMessageHandler<TestMessage>>(MockBehavior.Strict);
            var options = new TestListenerHostOptions { MinimumPollingIntervalInMilliseconds = 0, MaximumPollingIntervalInMilliseconds = 1 };
            var optionsMock = new Mock<IOptions<TestListenerHostOptions>>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<QueueListenerExecutor<TestMessage, TestListenerHostOptions>>>(MockBehavior.Strict);

            queueMessageManagerMock.Setup(m => m.RetrieveMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<TestMessage>());
            optionsMock.SetupGet(o => o.Value).Returns(options);

            loggerMock.Setup(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No messages found")),
                null,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));

            var sut = new QueueListenerExecutor<TestMessage, TestListenerHostOptions>(queueMessageManagerMock.Object, queueMessageHandlerMock.Object, loggerMock.Object, optionsMock.Object);

            // Act
            await sut.HandleAsync(CancellationToken.None);

            // Assert
            queueMessageHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Given_a_QueueListenerExecutor_when_HandleAsync_is_called_and_messages_are_retrieved_then_the_handler_is_called_correctly()
        {
            // Arrange
            var messages = new[] { new MessageTestDataBuilder().WithmessageId("1").Build(), new MessageTestDataBuilder().WithmessageId("2").Build() };
            var queueMessageManagerMock = new Mock<IQueueMessageManager<TestMessage>>(MockBehavior.Strict);
            var queueMessageHandlerMock = new Mock<IQueueMessageHandler<TestMessage>>(MockBehavior.Strict);
            var options = new TestListenerHostOptions { MinimumPollingIntervalInMilliseconds = 0, MaximumPollingIntervalInMilliseconds = 1 };
            var optionsMock = new Mock<IOptions<TestListenerHostOptions>>(MockBehavior.Strict);
            //var loggerMock = new Mock<ILogger<QueueListenerExecutor<TestMessage, TestListenerHostOptions>>>(MockBehavior.Strict);

            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
            .SetMinimumLevel(LogLevel.Error));

            var loggerMock = loggerFactory.CreateLogger<QueueListenerExecutor<TestMessage, TestListenerHostOptions>>();

            queueMessageManagerMock.Setup(m => m.RetrieveMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(messages);
            optionsMock.SetupGet(o => o.Value).Returns(options);

            queueMessageHandlerMock.Setup(h => h.HandleAsync(It.Is<TestMessage>(m => messages.Any(ma => ma.MessageId == m.MessageId)), CancellationToken.None)).Returns(Task.CompletedTask);

            var sut = new QueueListenerExecutor<TestMessage, TestListenerHostOptions>(queueMessageManagerMock.Object, queueMessageHandlerMock.Object, loggerMock, optionsMock.Object);

            // Act
            await sut.HandleAsync(CancellationToken.None);

            // Assert
            queueMessageHandlerMock.Verify(e => e.HandleAsync(It.Is<TestMessage>(m => messages.Any(ma => ma.MessageId == m.MessageId)), It.IsAny<CancellationToken>()), Times.Exactly(messages.Length));
        }
    }
}
