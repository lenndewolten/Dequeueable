using Amazon.SQS;
using Amazon.SQS.Model;
using Dequeueable.AmazonSQS.Factories;
using Dequeueable.AmazonSQS.IntegrationTests.TestDataBuilders;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Dequeueable.AmazonSQS.IntegrationTests.Functions
{
    public class HostTests
    {
        [Fact]
        public async Task Given_a_JobFunction_when_two_messages_are_retrieved_then_they_are_handled_correctly()
        {
            // Arrange
            var options = new Configurations.HostOptions
            {
                VisibilityTimeoutInSeconds = 500,
                QueueUrl = "http://mutesturl.com"
            };
            var factory = new JobHostFactory<TestFunction>(opt =>
            {
                opt.VisibilityTimeoutInSeconds = options.VisibilityTimeoutInSeconds;
                opt.QueueUrl = options.QueueUrl;
            });

            var messages = new Message[] { new Message { Body = "message1", ReceiptHandle = "1" }, new Message { Body = "message2", ReceiptHandle = "2" } };

            var fakeServiceMock = new Mock<IFakeService>();
            var amazonSQSClientFactoryMock = new Mock<IAmazonSQSClientFactory>();
            var clientFake = new Mock<AmazonSQSClient>("TESTKEY", "TESTSECRET", Amazon.RegionEndpoint.EUCentral1);

            clientFake.Setup(c => c.ReceiveMessageAsync(It.Is<ReceiveMessageRequest>(r => r.QueueUrl == options.QueueUrl), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiveMessageResponse
                {
                    Messages = messages.ToList()
                });

            amazonSQSClientFactoryMock.Setup(c => c.Create()).Returns(clientFake.Object);

            factory.ConfigureTestServices(services =>
            {
                services.AddTransient(_ => fakeServiceMock.Object);
                services.AddTransient(_ => amazonSQSClientFactoryMock.Object);
            });

            // Act
            var host = factory.Build();
            await host.HandleAsync(CancellationToken.None);

            // Assert
            foreach (var message in messages)
            {
                fakeServiceMock.Verify(f => f.Execute(It.Is<Models.Message>(m => m.Body.ToString() == message.Body)), Times.Once());
                clientFake.Verify(c => c.ChangeMessageVisibilityAsync(It.IsAny<ChangeMessageVisibilityRequest>(), It.IsAny<CancellationToken>()), Times.Never());
            }
        }

        [Fact]
        public async Task Given_a_ListenerFunction_when_two_messages_are_retieved_then_they_are_handled_correctly()
        {
            // Arrange
            var options = new Configurations.ListenerHostOptions
            {
                VisibilityTimeoutInSeconds = 500,
                QueueUrl = "http://mutesturl.com"
            };
            var factory = new ListenerHostFactory<TestFunction>(opt =>
            {
                opt.VisibilityTimeoutInSeconds = options.VisibilityTimeoutInSeconds;
                opt.MinimumPollingIntervalInMilliseconds = 1;
                opt.MaximumPollingIntervalInMilliseconds = 2;
                opt.QueueUrl = options.QueueUrl;
            });

            var messages = new Message[] { new Message { Body = "message1", ReceiptHandle = "1" }, new Message { Body = "message2", ReceiptHandle = "2" } };

            var fakeServiceMock = new Mock<IFakeService>();
            var amazonSQSClientFactoryMock = new Mock<IAmazonSQSClientFactory>();
            var clientFake = new Mock<AmazonSQSClient>("TESTKEY", "TESTSECRET", Amazon.RegionEndpoint.EUCentral1);

            clientFake.Setup(c => c.ReceiveMessageAsync(It.Is<ReceiveMessageRequest>(r => r.QueueUrl == options.QueueUrl), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiveMessageResponse
                {
                    Messages = messages.ToList()
                });

            amazonSQSClientFactoryMock.Setup(c => c.Create()).Returns(clientFake.Object);

            factory.ConfigureTestServices(services =>
            {
                services.AddTransient(_ => fakeServiceMock.Object);
                services.AddTransient(_ => amazonSQSClientFactoryMock.Object);
            });

            // Act
            var host = factory.Build();
            await host.HandleAsync(CancellationToken.None);

            // Assert
            foreach (var message in messages)
            {
                fakeServiceMock.Verify(f => f.Execute(It.Is<Models.Message>(m => m.Body.ToString() == message.Body)), Times.Once());
                clientFake.Verify(c => c.ChangeMessageVisibilityAsync(It.IsAny<ChangeMessageVisibilityRequest>(), It.IsAny<CancellationToken>()), Times.Never());
            }
        }

        [Fact]
        public async Task Given_a_Singleton_Job_when_a_Queue_has_two_messages_then_they_are_handled_correctly()
        {
            // Arrange
            var options = new Configurations.ListenerHostOptions
            {
                VisibilityTimeoutInSeconds = 500,
                QueueUrl = "http://mutesturl.com"
            };
            var factory = new ListenerHostFactory<TestFunction>(opt =>
            {
                opt.VisibilityTimeoutInSeconds = options.VisibilityTimeoutInSeconds;
                opt.MinimumPollingIntervalInMilliseconds = 1;
                opt.MaximumPollingIntervalInMilliseconds = 2;
                opt.QueueUrl = options.QueueUrl;
            }, runAsSingleton: true);

            var messages = new Message[] { new Message { Body = "message1", ReceiptHandle = "5", Attributes = new Dictionary<string, string> { { "MessageGroupId", "1" } } }, new Message { Body = "message2", ReceiptHandle = "6", Attributes = new Dictionary<string, string> { { "MessageGroupId", "1" } } } };

            var amazonSQSClientFactoryMock = new Mock<IAmazonSQSClientFactory>();
            var clientFake = new Mock<AmazonSQSClient>("TESTKEY", "TESTSECRET", Amazon.RegionEndpoint.EUCentral1);

            clientFake.Setup(c => c.ReceiveMessageAsync(It.Is<ReceiveMessageRequest>(r => r.QueueUrl == options.QueueUrl), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiveMessageResponse
                {
                    Messages = messages.ToList()
                });

            amazonSQSClientFactoryMock.Setup(c => c.Create()).Returns(clientFake.Object);

            factory.ConfigureTestServices(services =>
            {
                services.AddTransient<IFakeService, SingletonFakeService>();
                services.AddTransient(_ => amazonSQSClientFactoryMock.Object);
            });

            // Act
            var host = factory.Build();
            await host.HandleAsync(CancellationToken.None);

            // Assert
            foreach (var message in messages)
            {
                clientFake.Verify(c => c.DeleteMessageAsync(options.QueueUrl, message.ReceiptHandle, It.IsAny<CancellationToken>()), Times.Once());
                clientFake.Verify(c => c.ChangeMessageVisibilityAsync(It.IsAny<ChangeMessageVisibilityRequest>(), It.IsAny<CancellationToken>()), Times.Never());
            }
        }

        [Fact]
        public async Task Given_a_Singleton_Listener_when_a_Queue_has_two_messages_then_they_are_handled_correctly()
        {
            // Arrange
            var options = new Configurations.ListenerHostOptions
            {
                VisibilityTimeoutInSeconds = 500,
                QueueUrl = "http://mutesturl.com"
            };
            var factory = new ListenerHostFactory<TestFunction>(opt =>
            {
                opt.VisibilityTimeoutInSeconds = options.VisibilityTimeoutInSeconds;
                opt.MinimumPollingIntervalInMilliseconds = 1;
                opt.MaximumPollingIntervalInMilliseconds = 2;
                opt.QueueUrl = options.QueueUrl;
            }, runAsSingleton: true);

            var messages = new Message[] { new Message { Body = "message1", ReceiptHandle = "7", Attributes = new Dictionary<string, string> { { "MessageGroupId", "1" } } }, new Message { Body = "message2", ReceiptHandle = "8", Attributes = new Dictionary<string, string> { { "MessageGroupId", "1" } } } };

            var amazonSQSClientFactoryMock = new Mock<IAmazonSQSClientFactory>();
            var clientFake = new Mock<AmazonSQSClient>("TESTKEY", "TESTSECRET", Amazon.RegionEndpoint.EUCentral1);

            clientFake.Setup(c => c.ReceiveMessageAsync(It.Is<ReceiveMessageRequest>(r => r.QueueUrl == options.QueueUrl), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReceiveMessageResponse
                {
                    Messages = messages.ToList()
                });

            amazonSQSClientFactoryMock.Setup(c => c.Create()).Returns(clientFake.Object);

            factory.ConfigureTestServices(services =>
            {
                services.AddTransient<IFakeService, SingletonFakeService>();
                services.AddTransient(_ => amazonSQSClientFactoryMock.Object);
            });

            // Act
            var host = factory.Build();
            await host.HandleAsync(CancellationToken.None);

            // Assert
            foreach (var message in messages)
            {
                clientFake.Verify(c => c.DeleteMessageAsync(options.QueueUrl, message.ReceiptHandle, It.IsAny<CancellationToken>()), Times.Once());
                clientFake.Verify(c => c.ChangeMessageVisibilityAsync(It.IsAny<ChangeMessageVisibilityRequest>(), It.IsAny<CancellationToken>()), Times.Never());
            }
        }
    }
}
