using Dequeueable.AmazonSQS.Models;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AmazonSQS.SampleListener.Functions
{
    internal class TestFunction : IAmazonSQSFunction
    {
        private readonly ILogger<TestFunction> _logger;

        public TestFunction(ILogger<TestFunction> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Function called with MessageId {MessageId} and content {MessageBody}", message.MessageId, message.Body.ToString());
            return Task.Delay(TimeSpan.FromMinutes(2), cancellationToken);
        }
    }
}
