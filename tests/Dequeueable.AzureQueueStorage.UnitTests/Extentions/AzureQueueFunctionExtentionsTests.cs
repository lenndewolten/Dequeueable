using Dequeueable.AzureQueueStorage.Extentions;
using Dequeueable.AzureQueueStorage.Models;
using Dequeueable.AzureQueueStorage.Services.Singleton;
using FluentAssertions;

namespace Dequeueable.AzureQueueStorage.UnitTests.Extentions
{
    public class AzureQueueFunctionExtentionsTests
    {
        [Singleton("Id")]
        private class TestSingeltonFunction : IAzureQueueFunction
        {
            public Task ExecuteAsync(Message message, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        private class TestFunction : IAzureQueueFunction
        {
            public Task ExecuteAsync(Message message, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void Given_a_SingletonFunction_when_GetSingletonAttribute_is_called_then_the_attribute_is_returned()
        {
            // Arrange
            var function = new TestFunction();

            // Act
            var attribute = function.GetSingletonAttribute();

            // Assert
            attribute.Should().BeNull();
        }

        [Fact]
        public void Given_a_Function_when_GetSingletonAttribute_is_called_then_null_is_returned()
        {
            // Arrange
            var function = new TestFunction();

            // Act
            var attribute = function.GetSingletonAttribute();

            // Assert
            attribute.Should().BeNull();
        }

        [Fact]
        public void Given_a_Type_that_is_a_SingletonFunction_when_GetSingletonAttribute_is_called_then_the_attribute_is_returned()
        {
            // Arrange
            var function = new TestSingeltonFunction();
            var type = function.GetType();

            // Act
            var attribute = type.GetSingletonAttribute();

            // Assert
            attribute.Should().NotBeNull();
        }

        [Fact]
        public void Given_a_Type_that_is_a_Function_when_GetSingletonAttribute_is_called_then_null_is_returned()
        {
            // Arrange
            var function = new TestFunction();
            var type = function.GetType();

            // Act
            var attribute = type.GetSingletonAttribute();

            // Assert
            attribute.Should().BeNull();
        }

        [Fact]
        public void Given_a_Type_that_is_not_assignable_to_IAzureQueueFunction_when_GetSingletonAttribute_is_called_then_null_is_returned()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var attribute = type.GetSingletonAttribute();

            // Assert
            attribute.Should().BeNull();
        }
    }
}
