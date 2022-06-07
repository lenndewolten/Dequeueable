using FluentAssertions;
using JobHandlers.AzureQueueMessage.Configurations;
using System;
using Xunit;

namespace JobHandlers.AzureQueueMessage.UnitTests
{
    public class StorageAccountOptionsTests
    {
        [Fact]
        public void Given_the_PoisenQueueName_when_no_queue_name_is_provided_then_the_correct_value_is_returned()
        {
            // Arrange
            var expected = "test-poisen";
            var sut = new StorageAccountOptions
            {
                PoisenQueueSuffix = expected
            };

            // Act
            var actual = sut.PoisenQueueName;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_the_PoisenQueueName_when_a_queue_name_is_provided_then_the_correct_value_is_returned()
        {
            // Arrange
            var queueName = "test-queue";
            var suffix = "poisen";
            var sut = new StorageAccountOptions
            {
                PoisenQueueSuffix = suffix,
                QueueName = queueName
            };

            // Act
            var actual = sut.PoisenQueueName;

            // Assert
            actual.Should().Be($"{queueName}-{suffix}");
        }

        [Fact]
        public void Given_the_PoisenQueueSuffix_when_it_is_set_with_a_valid_value_then_it_set_correctly()
        {
            // Arrange
            var expected = "test";
            var sut = new StorageAccountOptions
            {
                PoisenQueueSuffix = expected,
            };

            // Act
            var actual = sut.PoisenQueueSuffix;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_the_PoisenQueueSuffix_when_it_is_set_with_an_invalid_value_then_an_ArgumentException_is_thrown()
        {
            // Arrange
            var expected = string.Empty;

            // Act
            Action act = () => new StorageAccountOptions
            {
                PoisenQueueSuffix = expected,
            };

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Given_the_QueueName_when_it_is_set_with_a_valid_value_then_it_set_correctly()
        {
            // Arrange
            var expected = "test";
            var sut = new StorageAccountOptions
            {
                QueueName = expected,
            };

            // Act
            var actual = sut.QueueName;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_the_QueueName_when_it_is_set_with_an_invalid_value_then_an_ArgumentException_is_thrown()
        {
            // Arrange
            var expected = string.Empty;

            // Act
            Action act = () => new StorageAccountOptions
            {
                QueueName = expected,
            };

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}