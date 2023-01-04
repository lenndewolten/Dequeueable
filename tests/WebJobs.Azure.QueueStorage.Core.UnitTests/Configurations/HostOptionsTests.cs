using FluentAssertions;
using WebJobs.Azure.QueueStorage.Core.Configurations;

namespace WebJobs.Azure.QueueStorage.Core.UnitTests.Configurations
{
    public class HostOptionsTests
    {
        [Fact]
        public void Given_the_PoisonQueueSuffix_when_it_is_set_with_a_valid_value_then_it_set_correctly()
        {
            // Arrange
            var expected = "test";
            var sut = new HostOptions
            {
                PoisonQueueSuffix = expected,
            };

            // Act
            var actual = sut.PoisonQueueSuffix;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_the_PoisonQueueSuffix_when_it_is_set_with_an_invalid_value_then_an_ArgumentException_is_thrown()
        {
            // Arrange
            var expected = string.Empty;

            // Act
            Action act = () =>
            {
                var options = new HostOptions
                {
                    PoisonQueueSuffix = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Given_the_MaxDequeueCount_when_it_is_set_to_zero_then_it_set_correctly()
        {
            // Arrange
            var expected = 0;
            var sut = new HostOptions
            {
                MaxDequeueCount = expected,
            };

            // Act
            var actual = sut.MaxDequeueCount;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_the_MaxDequeueCount_when_it_to_twenty_then_it_set_correctly()
        {
            // Arrange
            var expected = 20;
            var sut = new HostOptions
            {
                MaxDequeueCount = expected,
            };

            // Act
            var actual = sut.MaxDequeueCount;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_the_MaxDequeueCount_when_it_is_set_to_minus_one_then_an_ArgumentException_is_thrown()
        {
            // Arrange
            var expected = -1;

            // Act
            Action act = () =>
            {
                var options = new HostOptions
                {
                    MaxDequeueCount = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Given_the_MaxDequeueCount_when_it_is_set_to_twentyone_then_an_ArgumentException_is_thrown()
        {
            // Arrange
            var expected = 21;

            // Act
            Action act = () =>
            {
                var options = new HostOptions
                {
                    MaxDequeueCount = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Given_the_VisibilityTimeoutInSeconds_when_it_to_twenty_then_it_set_correctly()
        {
            // Arrange
            var expected = 10;
            var sut = new HostOptions
            {
                VisibilityTimeoutInSeconds = expected,
            };

            // Act
            var actual = sut.VisibilityTimeoutInSeconds;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_the_VisibilityTimeoutInSeconds_when_it_is_set_to_minus_one_then_an_ArgumentException_is_thrown()
        {
            // Arrange
            var expected = 0;

            // Act
            Action act = () =>
            {
                var options = new HostOptions
                {
                    VisibilityTimeoutInSeconds = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Given_the_BatchSize_when_it_to_twenty_then_it_set_correctly()
        {
            // Arrange
            var expected = 1;
            var sut = new HostOptions
            {
                BatchSize = expected,
            };

            // Act
            var actual = sut.BatchSize;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_the_BatchSize_when_it_is_set_to_minus_one_then_an_ArgumentException_is_thrown()
        {
            // Arrange
            var expected = 0;

            // Act
            Action act = () =>
            {
                var options = new HostOptions
                {
                    BatchSize = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }
    }
}
