﻿using Dequeueable.AzureQueueStorage.Configurations;
using Dequeueable.AzureQueueStorage.Models;
using Dequeueable.AzureQueueStorage.Services.Hosts;
using Dequeueable.AzureQueueStorage.Services.Queues;
using Dequeueable.AzureQueueStorage.UnitTests.TestDataBuilders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Dequeueable.AzureQueueStorage.UnitTests.Services.Hosts
{
    public class QueueListenerTests
    {

        [Fact]
        public async Task Given_a_QueueListener_when_HandleAsync_is_called_but_no_messages_are_retrieved_then_the_handler_is_not_called()
        {
            // Arrange
            var queueMessageManagerMock = new Mock<IQueueMessageManager>(MockBehavior.Strict);
            var queueMessageHandlerMock = new Mock<IQueueMessageHandler>(MockBehavior.Strict);
            var options = new ListenerOptions { MinimumPollingIntervalInMilliseconds = 0, MaximumPollingIntervalInMilliseconds = 1, QueueName = "TestQueue" };
            var optionsMock = new Mock<IOptions<ListenerOptions>>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<QueueListener>>(MockBehavior.Strict);

            queueMessageManagerMock.Setup(m => m.RetrieveMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Message>());
            optionsMock.SetupGet(o => o.Value).Returns(options);

            loggerMock.Setup(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No messages found")),
                null,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));

            var sut = new QueueListener(queueMessageManagerMock.Object, queueMessageHandlerMock.Object, optionsMock.Object, loggerMock.Object);

            // Act
            await sut.HandleAsync(CancellationToken.None);

            // Assert
            queueMessageHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Given_a_QueueListener_when_HandleAsync_is_called_and_messages_are_retrieved_then_the_handler_is_called_correctly()
        {
            // Arrange
            var messages = new[] { new MessageTestDataBuilder().WithmessageId("1").Build(), new MessageTestDataBuilder().WithmessageId("2").Build() };
            var queueMessageManagerMock = new Mock<IQueueMessageManager>(MockBehavior.Strict);
            var queueMessageHandlerMock = new Mock<IQueueMessageHandler>(MockBehavior.Strict);
            var options = new ListenerOptions { MinimumPollingIntervalInMilliseconds = 0, MaximumPollingIntervalInMilliseconds = 1, QueueName = "TestQueue" };
            var optionsMock = new Mock<IOptions<ListenerOptions>>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<QueueListener>>(MockBehavior.Strict);

            queueMessageManagerMock.Setup(m => m.RetrieveMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(messages);
            optionsMock.SetupGet(o => o.Value).Returns(options);

            queueMessageHandlerMock.Setup(h => h.HandleAsync(It.Is<Message>(m => messages.Any(ma => ma.MessageId == m.MessageId)), CancellationToken.None)).Returns(Task.CompletedTask);

            var sut = new QueueListener(queueMessageManagerMock.Object, queueMessageHandlerMock.Object, optionsMock.Object, loggerMock.Object);

            // Act
            await sut.HandleAsync(CancellationToken.None);

            // Assert
            queueMessageHandlerMock.Verify(e => e.HandleAsync(It.Is<Message>(m => messages.Any(ma => ma.MessageId == m.MessageId)), It.IsAny<CancellationToken>()), Times.Exactly(messages.Length));
        }
    }
}
