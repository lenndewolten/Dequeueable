using FluentAssertions;
using JobHandlers.AzureQueueMessage.Services.Builders;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace JobHandlers.AzureQueueMessage.UnitTests
{
    public class StorageAccountUriBuilderTests
    {
        [Fact]
        public void Given_a_StorageAccountUriBuilder_when_Build_is_called_with_an_invalid_format_then_an_ArgumentException_is_thrown()
        {
            // Arrange
            var format = string.Empty;
            var queueName = "test-queue";
            var accountName = "test-account";

            var loggerMock = new Mock<ILogger<StorageAccountUriBuilder>>();
            var sut = new StorageAccountUriBuilder(loggerMock.Object);

            // Act
            Action act = () => sut.Build(format, queueName, accountName);

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Given_a_StorageAccountUriBuilder_when_Build_is_called_and_an_invalid_format_is_created_then_an_UriFormatException_is_thrown()
        {
            // Arrange
            var format = "https://{accountName}.queue.core.windows.net/{queueName}";
            var queueName = "test-queue";
            string? accountName = null;

            var loggerMock = new Mock<ILogger<StorageAccountUriBuilder>>();
            var sut = new StorageAccountUriBuilder(loggerMock.Object);

            // Act
            Action act = () => sut.Build(format, queueName, accountName);

            // Assert
            act.Should().ThrowExactly<UriFormatException>();
            loggerMock.Verify(
                x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Invalid Uri: The Storage account Uri could not be parsed. Format: 'https://{{accountName}}.queue.core.windows.net/{queueName}'")),
                It.IsAny<UriFormatException>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
        }

        [Fact]
        public void Given_a_StorageAccountUriBuilder_when_Build_is_called_without_accountName_but_with_a_valid_format_then_the_uri_is_build_correctly()
        {
            // Arrange
            var format = "https://127.0.0.1:10000/devstoreaccount1/{queueName}";
            var queueName = "test-queue";
            string? accountName = null;

            var loggerMock = new Mock<ILogger<StorageAccountUriBuilder>>();
            var sut = new StorageAccountUriBuilder(loggerMock.Object);

            // Act
            var actual = sut.Build(format, queueName, accountName);

            // Assert
            actual.AbsoluteUri.Should().Be($"https://127.0.0.1:10000/devstoreaccount1/{queueName}");
        }

        [Fact]
        public void Given_a_StorageAccountUriBuilder_when_Build_is_called_without_queueName_but_with_a_valid_format_then_the_uri_is_build_correctly()
        {
            // Arrange
            var format = "https://{accountName}.queue.core.windows.net/test-queue";
            var queueName = string.Empty;
            var accountName = "test-account";

            var loggerMock = new Mock<ILogger<StorageAccountUriBuilder>>();
            var sut = new StorageAccountUriBuilder(loggerMock.Object);

            // Act
            var actual = sut.Build(format, queueName, accountName);

            // Assert
            actual.AbsoluteUri.Should().Be($"https://{accountName}.queue.core.windows.net/test-queue");
        }

        [Fact]
        public void Given_a_StorageAccountUriBuilder_when_Build_is_called_with_queueName_and_accountName_and_a_valid_format_then_the_uri_is_build_correctly()
        {
            // Arrange
            var format = "https://{aCCouNtNaMe}.queue.core.windows.net/{QueUEnaMe}";
            var queueName = "test-queue";
            var accountName = "test-account";

            var loggerMock = new Mock<ILogger<StorageAccountUriBuilder>>();
            var sut = new StorageAccountUriBuilder(loggerMock.Object);

            // Act
            var actual = sut.Build(format, queueName, accountName);

            // Assert
            actual.AbsoluteUri.Should().Be($"https://{accountName}.queue.core.windows.net/{queueName}");
        }
    }
}