using Azure.Storage.Queues.Models;
using JobHandlers.AzureQueueMessage.Services;
using Microsoft.Extensions.Logging;

namespace QueueMessages.SampleApp
{
    internal class QueueMessageExecutor : IQueueMessageExecutor
    {
        private readonly ILogger<QueueMessageExecutor> _logger;

        public QueueMessageExecutor(ILogger<QueueMessageExecutor> logger)
        {
            _logger = logger;
        }

        public async Task Execute(QueueMessage message, CancellationToken cancellationToken)
        {
            var random = new Random();
            var iterations = random.Next(5, 20);

            for (int i = 1; i < iterations; i++)
            {
                _logger.LogInformation("Executing iteration {Current}/{Iterations}", i, iterations);
                await Task.Delay(2000, cancellationToken);
            }

            _logger.LogInformation("Executed long running job");
        }
    }
}
