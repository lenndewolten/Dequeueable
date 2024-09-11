using Dequeueable.Queues;
using Microsoft.Extensions.Logging;

namespace Dequeueable.SampleJob
{
    internal sealed class SampleQueueMessageHandler(ILogger<SampleQueueMessageHandler> logger) : IQueueMessageHandler<SampleMessage>
    {
        public async Task HandleAsync(SampleMessage message, CancellationToken cancellationToken)
        {
            for (var i = 0; i < 6; i++)
            {
                logger.LogInformation("Executing handler loop {I} for message {MessageId}", i, message.MessageId);
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Handler cancelled!");
                    break;
                }
                await Task.Delay(10000, cancellationToken);
            }

            logger.LogInformation("Executing message {MessageId} finished", message.MessageId);
        }
    }
}
