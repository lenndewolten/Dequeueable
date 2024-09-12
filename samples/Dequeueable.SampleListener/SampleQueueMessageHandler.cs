using Dequeueable.Queues;
using Microsoft.Extensions.Logging;

namespace Dequeueable.SampleListener
{
    internal sealed class SampleQueueMessageHandler(ILogger<SampleQueueMessageHandler> logger) : IQueueMessageHandler<SampleMessage>
    {
        private static readonly Random _random = new();

        public async Task HandleAsync(SampleMessage message, CancellationToken cancellationToken)
        {
            const int baseDelay = 5000;
            const int randomFactor = 15000;

            for (var i = 0; i < 6; i++)
            {
                logger.LogInformation("Executing handler loop {I} for message {MessageId}", i, message.MessageId);

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Handler cancelled!");
                    break;
                }

#pragma warning disable CA5394 // Do not use insecure randomness
                var delay = baseDelay + _random.Next(0, randomFactor);
#pragma warning restore CA5394 // Do not use insecure randomness
                await Task.Delay(delay, cancellationToken);
            }

            logger.LogInformation("Executing message {MessageId} finished", message.MessageId);
        }
    }
}
