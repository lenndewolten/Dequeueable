using Azure.Identity;
using FluentAssertions;
using JobHandlers.AzureQueueMessage.Configurations;
using JobHandlers.AzureQueueMessage.Services.Builders;
using JobHandlers.AzureQueueMessage.Services.Retrievers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace JobHandlers.AzureQueueMessage.UnitTests
{
    public class QueueClientRetrieverTests
    {
        [Fact]
        public void Given_a_QueueClientRetriever_when_Retrieve_is_called_with_a_ConnectionString_as_option_then_the_client_is_created_correctly()
        {
            // Arrange
            var queueName = "test queue";
            var options = new StorageAccountOptions
            {
                ConnectionString = "unit-test"
            };
            var loggerMock = new Mock<ILogger<QueueClientRetriever>>();
            var optionsMock = new Mock<IOptions<StorageAccountOptions>>(MockBehavior.Strict);
            var factoryMock = new Mock<IQueueClientFactory>();
            var storageAccountUriBuilderMock = new Mock<IStorageAccountUriBuilder>(MockBehavior.Strict);

            optionsMock.SetupGet(o => o.Value).Returns(options);
            factoryMock.Setup(f => f.Create(options.ConnectionString, queueName)).Verifiable();

            var sut = new QueueClientRetriever(factoryMock.Object, optionsMock.Object, loggerMock.Object, storageAccountUriBuilderMock.Object);

            // Act
            sut.Retrieve(queueName);

            // Assert
            factoryMock.Verify();
            loggerMock.Verify(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Authenticate the QueueClient through the ConnectionString")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        }

        [Fact]
        public void Given_a_QueueClientRetriever_when_Retrieve_is_called_with_an_AuthScheme_as_option_then_the_client_is_created_correctly()
        {
            // Arrange
            var queueName = "test queue";
            var options = new StorageAccountOptions
            {
                AuthenticationScheme = new DefaultAzureCredential(),
            };
            var loggerMock = new Mock<ILogger<QueueClientRetriever>>();
            var optionsMock = new Mock<IOptions<StorageAccountOptions>>(MockBehavior.Strict);
            var factoryMock = new Mock<IQueueClientFactory>();
            var storageAccountUriBuilderMock = new Mock<IStorageAccountUriBuilder>(MockBehavior.Strict);

            optionsMock.SetupGet(o => o.Value).Returns(options);
            storageAccountUriBuilderMock.Setup(b => b.Build(options.StorageAccountUriFormat, queueName, options.AccountName))
                .Returns(new Uri("https://localhost"))
                .Verifiable()
                ;
            factoryMock.Setup(f => f.Create(It.IsAny<Uri>(), options.AuthenticationScheme)).Verifiable();

            var sut = new QueueClientRetriever(factoryMock.Object, optionsMock.Object, loggerMock.Object, storageAccountUriBuilderMock.Object);

            // Act
            sut.Retrieve(queueName);

            // Assert
            storageAccountUriBuilderMock.Verify();
            factoryMock.Verify();
            loggerMock.Verify(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Authenticate the QueueClient through Active Director")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        }


        [Fact]
        public void Given_a_QueueClientRetriever_when_Retrieve_is_called_without_an_AuthScheme_or_ConnectionString_as_an_option_then_an_InvalidOperationException_is_thrown()
        {
            // Arrange
            var queueName = "test queue";
            var options = new StorageAccountOptions
            {
                AuthenticationScheme = null
            };
            var loggerMock = new Mock<ILogger<QueueClientRetriever>>();
            var optionsMock = new Mock<IOptions<StorageAccountOptions>>(MockBehavior.Strict);
            var factoryMock = new Mock<IQueueClientFactory>(MockBehavior.Strict);
            var storageAccountUriBuilderMock = new Mock<IStorageAccountUriBuilder>(MockBehavior.Strict);

            optionsMock.SetupGet(o => o.Value).Returns(options);

            var sut = new QueueClientRetriever(factoryMock.Object, optionsMock.Object, loggerMock.Object, storageAccountUriBuilderMock.Object);

            // Act
            Action act = () => sut.Retrieve(queueName);

            // Assert
            act.Should().ThrowExactly<InvalidOperationException>().WithMessage("No AccountName or ConnectionString supplied. Make sure that it is defined in the app settings");
        }
    }
}