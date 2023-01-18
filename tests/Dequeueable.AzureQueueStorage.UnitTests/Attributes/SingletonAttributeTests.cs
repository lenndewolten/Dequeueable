using Dequeueable.AzureQueueStorage.Attributes;
using FluentAssertions;

namespace Dequeueable.AzureQueueStorage.UnitTests.Attributes
{
    public class SingletonAttributeTests
    {
        [Fact]
        public void Given_a_MinimumPollingIntervalInSeconds_when_set_to_zero_then_an_ArgumentOutOfRangeException_is_thrown()
        {
            // Arrange
            var expected = -1;

            // Act
            Action act = () =>
            {
                var options = new SingletonAttribute("id")
                {
                    MinimumPollingIntervalInSeconds = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Given_a_MaximumPollingIntervalInSeconds_when_set_to_zero_then_an_ArgumentOutOfRangeException_is_thrown()
        {
            // Arrange
            var expected = 0;

            // Act
            Action act = () =>
            {
                var options = new SingletonAttribute("id")
                {
                    MaximumPollingIntervalInSeconds = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Given_a_MaximumPollingIntervalInSeconds_of_zero_seconds_when_set_then_an_ArgumentOutOfRangeException_is_thrown()
        {
            // Arrange
            var expected = 0;
            // Act
            Action act = () =>
            {
                var options = new SingletonAttribute("id")
                {
                    MaximumPollingIntervalInSeconds = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Given_a_MaximumPollingIntervalInSeconds_of_2_seconds_when_set_then_is_set_correctly()
        {
            // Arrange
            var expected = 2;

            // Act
            var options = new SingletonAttribute("id")
            {
                MaximumPollingIntervalInSeconds = expected,
            };

            // Assert
            options.MaximumPollingIntervalInSeconds.Should().Be(expected);
        }
    }
}
