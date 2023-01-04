using FluentAssertions;
using WebJobs.Azure.QueueStorage.Function.Configurations;

namespace WebJobs.Azure.QueueStorage.Function.UnitTests.Configurations
{
    public class FunctionOptionsTests
    {
        [Fact]
        public void Given_a_BatchSize_of_five_when_NewBatchThreshold_is_set_to_four_then_it_set_correctly()
        {
            // Arrange
            var expected = 4;
            var sut = new FunctionOptions
            {
                BatchSize = 5,
                NewBatchThreshold = expected
            };

            // Act
            var actual = sut.NewBatchThreshold;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_a_BatchSize_of_five_when_NewBatchThreshold_is_set_to_six_then_an_ArgumentException_is_thrown()
        {
            // Arrange
            var expected = 6;

            // Act
            Action act = () =>
            {
                var options = new FunctionOptions
                {
                    BatchSize = 5,
                    NewBatchThreshold = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Given_a_BatchSize_of_five_when_NewBatchThreshold_is_not_set_then_it_the_getter_returns_the_correct_value()
        {
            // Arrange
            var expected = 3;
            var sut = new FunctionOptions
            {
                BatchSize = 5,
            };

            // Act
            var actual = sut.NewBatchThreshold;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Given_a_MinimumPollingInterval_when_set_to_zero_then_an_ArgumentOutOfRangeException_is_thrown()
        {
            // Arrange
            var expected = TimeSpan.FromSeconds(-1);

            // Act
            Action act = () =>
            {
                var options = new FunctionOptions
                {
                    MinimumPollingInterval = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Given_a_MinimumPollingInterval_of_2_seconds_when_set_higer_than_MaximumPollingInterval_then_an_ArgumentOutOfRangeException_is_thrown()
        {
            // Arrange
            var expected = TimeSpan.FromSeconds(2);

            // Act
            Action act = () =>
            {
                var options = new FunctionOptions
                {
                    MaximumPollingInterval = expected.Add(TimeSpan.FromSeconds(-1)),
                    MinimumPollingInterval = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Given_a_MinimumPollingInterval_of_2_seconds_when_set_lower_than_MaximumPollingInterval_then_is_set_correctly()
        {
            // Arrange
            var expected = TimeSpan.FromSeconds(2);

            // Act
            var options = new FunctionOptions
            {
                MaximumPollingInterval = expected.Add(TimeSpan.FromSeconds(1)),
                MinimumPollingInterval = expected,
            };

            // Assert
            options.MinimumPollingInterval.Should().Be(expected);
        }

        [Fact]
        public void Given_a_MaximumPollingInterval_when_set_to_zero_then_an_ArgumentOutOfRangeException_is_thrown()
        {
            // Arrange
            var expected = TimeSpan.Zero;

            // Act
            Action act = () =>
            {
                var options = new FunctionOptions
                {
                    MaximumPollingInterval = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Given_a_MaximumPollingInterval_of_2_seconds_when_set_lower_than_MinimumPollingInterval_then_an_ArgumentOutOfRangeException_is_thrown()
        {
            // Arrange
            var expected = TimeSpan.FromSeconds(2);

            // Act
            Action act = () =>
            {
                var options = new FunctionOptions
                {
                    MinimumPollingInterval = expected.Add(TimeSpan.FromSeconds(1)),
                    MaximumPollingInterval = expected,
                };
            };

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Given_a_MaximumPollingInterval_of_2_seconds_when_set_higher_than_MaximumPollingInterval_then_is_set_correctly()
        {
            // Arrange
            var expected = TimeSpan.FromSeconds(2);

            // Act
            var options = new FunctionOptions
            {
                MaximumPollingInterval = expected,
                MinimumPollingInterval = expected.Add(TimeSpan.FromSeconds(-1)),
            };

            // Assert
            options.MaximumPollingInterval.Should().Be(expected);
        }
    }
}
