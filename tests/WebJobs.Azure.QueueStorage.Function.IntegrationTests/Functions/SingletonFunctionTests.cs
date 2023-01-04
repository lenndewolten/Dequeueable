using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebJobs.Azure.QueueStorage.Core.Models;
using WebJobs.Azure.QueueStorage.Function.IntegrationTests.Fixtures;
using WebJobs.Azure.QueueStorage.Function.IntegrationTests.TestDataBuilders;

namespace WebJobs.Azure.QueueStorage.Function.IntegrationTests.Functions
{
    [Collection("Azurite collection")]
    public class SingletonFunctionTests
    {
        private readonly QueueClientOptions _queueClientOptions = new() { MessageEncoding = QueueMessageEncoding.Base64 };

        [Fact]
        public async Task Given_a_Function_with_a_singleton_attribute_when_a_queue_has_two_messages_then_they_are_handled_correctly()
        {
            // Arrange
            var queueName = "scopedsingletontestsqueue";
            var scope = "testscope";
            var factory = new ApplicationFactory<SingletonFunction>(AzuriteFixture.ConnectionString, queueName);

            var fakeServiceMock = new Mock<IFakeService>();

            factory.ConfigureTestServices(services =>
            {
                services.AddTransient(_ => fakeServiceMock.Object);
            });

            var queueClient = new QueueClient(AzuriteFixture.ConnectionString, queueName, _queueClientOptions);
            var blobContainerClient = new BlobContainerClient(AzuriteFixture.ConnectionString, SingletonFunction.ContainerName);

            await queueClient.CreateAsync();

            var messages = new[] { new { Id = scope }, new { Id = scope } };
            foreach (var message in messages)
            {
                await queueClient.SendMessageAsync(BinaryData.FromObjectAsJson(message));
            }

            // Act
            var listener = factory.Build();
            await listener.ListenAsync(CancellationToken.None);

            // Assert
            fakeServiceMock.Verify(f => f.Execute(It.IsAny<Message>()), Times.Exactly(messages.Length));
            var peekedMessage = await queueClient.PeekMessageAsync();
            peekedMessage.Value.Should().BeNull();

            var blobclient = blobContainerClient.GetBlobClient(scope);
            (await blobclient.ExistsAsync()).Value.Should().BeTrue();

        }
    }
}
