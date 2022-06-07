using JobHandlers.AzureQueueMessage.Configurations;
using JobHandlers.AzureQueueMessage.Handlers;
using JobHandlers.AzureQueueMessage.Services.Updaters;
using JobHandlers.AzureQueueMessage.UnitTests.TestDataBuilders;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JobHandlers.AzureQueueMessage.UnitTests
{
    public class QueueMessageExceptionHandlerTests
    {
        [Fact]
        public async Task Given_a_QueueMessageExceptionHandler_with_MaxDequeueCount_of_four_when_Handle_is_called_with_a_QueueMessage_with_DequeueCount_of_two_then_it_is_Enqueued_correctly()
        {
            // Arrange
            var maxDequeueCount = 4;
            var queueMessage = new QueueMesssageTestDataBuilder().WithDequeueCount(2).Build();

            var queueMessageUpdaterMock = new Mock<IQueueMessageUpdater>(MockBehavior.Strict);
            queueMessageUpdaterMock.Setup(u => u.Enqueue(queueMessage, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var options = new Mock<IOptions<StorageAccountOptions>>(MockBehavior.Strict);
            options.SetupGet(o => o.Value).Returns(new StorageAccountOptions { MaxDequeueCount = maxDequeueCount });

            var sut = new QueueMessageExceptionHandler(options.Object, queueMessageUpdaterMock.Object);

            // Act
            await sut.Handle(queueMessage, CancellationToken.None);

            // Assert
            queueMessageUpdaterMock.Verify();
        }

        [Fact]
        public async Task Given_a_QueueMessageExceptionHandler_with_MaxDequeueCount_of_four_when_Handle_is_called_with_a_QueueMessage_with_DequeueCount_of_four_then_it_is_moved_to_the_poisen_queue_correctly()
        {
            // Arrange
            var maxDequeueCount = 4;
            var queueMessage = new QueueMesssageTestDataBuilder().WithDequeueCount(maxDequeueCount).Build();

            var queueMessageUpdaterMock = new Mock<IQueueMessageUpdater>(MockBehavior.Strict);
            queueMessageUpdaterMock.Setup(u => u.MoveToPoisenQueue(queueMessage, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var options = new Mock<IOptions<StorageAccountOptions>>(MockBehavior.Strict);
            options.SetupGet(o => o.Value).Returns(new StorageAccountOptions { MaxDequeueCount = maxDequeueCount });

            var sut = new QueueMessageExceptionHandler(options.Object, queueMessageUpdaterMock.Object);

            // Act
            await sut.Handle(queueMessage, CancellationToken.None);

            // Assert
            queueMessageUpdaterMock.Verify();
        }
    }
}