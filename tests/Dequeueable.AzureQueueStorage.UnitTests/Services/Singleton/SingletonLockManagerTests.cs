using Azure.Storage.Blobs;
using Dequeueable.AzureQueueStorage.Configurations;
using Dequeueable.AzureQueueStorage.Factories;
using Dequeueable.AzureQueueStorage.Services.Singleton;
using Dequeueable.AzureQueueStorage.UnitTests.TestDataBuilders;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dequeueable.AzureQueueStorage.UnitTests.Services.Singleton
{
    public class SingletonLockManagerTests
    {
        [Fact]
        public async Task Given_a_LockManager_when_AquireLockAsync_is_called_and_the_lock_is_acquired_then_the_leaseId_is_returned()
        {
            // Arrange
            var leaseId = "someId";
            var fileName = "someName";
            var options = new HostOptions { ConnectionString = "some string" };
            var loggerMock = new Mock<ILogger<SingletonLockManager>>(MockBehavior.Strict);

            var blobClientProviderMock = new Mock<IBlobClientProvider>(MockBehavior.Strict);
            var distributedLockManagerFactoryMock = new Mock<IDistributedLockManagerFactory>(MockBehavior.Strict);
            var distributedLockManagerMock = new Mock<IDistributedLockManager>(MockBehavior.Strict);
            var singletonAttribute = new SingletonAttributeTestDataBuilder().WithMaxRetries(3).Build();
            var blobClientFake = new Mock<BlobClient>();

            blobClientProviderMock.Setup(c => c.Get(fileName)).Returns(blobClientFake.Object);
            distributedLockManagerFactoryMock.Setup(f => f.Create(blobClientFake.Object, loggerMock.Object)).Returns(distributedLockManagerMock.Object);
            loggerMock.Setup(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Lock with Id '{leaseId}' acquired for '{fileName}'")),
                null,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)))
                .Verifiable();
            distributedLockManagerMock.Setup(m => m.AcquireAsync(CancellationToken.None)).ReturnsAsync(leaseId);

            var sut = new SingletonLockManager(loggerMock.Object, blobClientProviderMock.Object, distributedLockManagerFactoryMock.Object, singletonAttribute);

            // Act
            var result = await sut.AquireLockAsync(fileName, CancellationToken.None);

            // Assert
            result.Should().Be(leaseId);
        }

        [Fact]
        public async Task Given_a_LockManager_when_AquireLockAsync_is_called_and_the_lock_cannot_be_acquired_at_first_then_it_is_retried_correctly()
        {
            // Arrange
            var leaseId = "someId";
            var fileName = "someName";
            var options = new HostOptions { ConnectionString = "some string" };
            var loggerMock = new Mock<ILogger<SingletonLockManager>>(MockBehavior.Strict);

            var blobClientProviderMock = new Mock<IBlobClientProvider>(MockBehavior.Strict);
            var distributedLockManagerFactoryMock = new Mock<IDistributedLockManagerFactory>(MockBehavior.Strict);
            var distributedLockManagerMock = new Mock<IDistributedLockManager>(MockBehavior.Strict);
            var singletonAttribute = new SingletonAttributeTestDataBuilder().WithMaxRetries(1).Build();
            var blobClientFake = new Mock<BlobClient>();

            blobClientProviderMock.Setup(c => c.Get(fileName)).Returns(blobClientFake.Object);
            distributedLockManagerFactoryMock.Setup(f => f.Create(blobClientFake.Object, loggerMock.Object)).Returns(distributedLockManagerMock.Object);
            loggerMock.Setup(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Lock with Id '{leaseId}' acquired for '{fileName}'")),
                null,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)))
                .Verifiable();
            distributedLockManagerMock.SetupSequence(m => m.AcquireAsync(CancellationToken.None))
                .ReturnsAsync((string?)null)
                .ReturnsAsync(leaseId);

            var sut = new SingletonLockManager(loggerMock.Object, blobClientProviderMock.Object, distributedLockManagerFactoryMock.Object, singletonAttribute);

            // Act
            var result = await sut.AquireLockAsync(fileName, CancellationToken.None);

            // Assert
            result.Should().Be(leaseId);
        }

        [Fact]
        public async Task Given_a_LockManager_when_AquireLockAsync_is_called_and_the_lock_cannot_be_acquired_and_the_MaxRetries_is_reached_then_a_SingletonException_is_thrown()
        {
            // Arrange
            var fileName = "someName";
            var options = new HostOptions { ConnectionString = "some string" };
            var loggerMock = new Mock<ILogger<SingletonLockManager>>(MockBehavior.Strict);

            var blobClientProviderMock = new Mock<IBlobClientProvider>(MockBehavior.Strict);
            var distributedLockManagerFactoryMock = new Mock<IDistributedLockManagerFactory>(MockBehavior.Strict);
            var distributedLockManagerMock = new Mock<IDistributedLockManager>(MockBehavior.Strict);
            var singletonAttribute = new SingletonAttributeTestDataBuilder().WithMaxRetries(1).WithMinimumInterval(1).WithMaximumInterval(1).Build();
            var blobClientFake = new Mock<BlobClient>();

            blobClientProviderMock.Setup(c => c.Get(fileName)).Returns(blobClientFake.Object);
            distributedLockManagerFactoryMock.Setup(f => f.Create(blobClientFake.Object, loggerMock.Object)).Returns(distributedLockManagerMock.Object);
            distributedLockManagerMock.SetupSequence(m => m.AcquireAsync(CancellationToken.None))
                .ReturnsAsync((string?)null)
                .ReturnsAsync((string?)null);

            var sut = new SingletonLockManager(loggerMock.Object, blobClientProviderMock.Object, distributedLockManagerFactoryMock.Object, singletonAttribute);

            // Act
            Func<Task> act = () => sut.AquireLockAsync(fileName, CancellationToken.None);

            // Assert
            await act.Should().ThrowExactlyAsync<SingletonException>().WithMessage($"Unable to acquire lock, max retries of '{singletonAttribute.MaxRetries}' reached");
        }
    }
}
