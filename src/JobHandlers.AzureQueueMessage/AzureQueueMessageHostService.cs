using JobHandlers.AzureQueueMessage.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobHandlers.AzureQueueMessage
{
    internal sealed class AzureQueueMessageHostService : BackgroundService
    {
        private readonly QueueMessageHandler _queueMessageHandler;
        private readonly ILogger<AzureQueueMessageHostService> _logger;

        public AzureQueueMessageHostService(QueueMessageHandler queueMessageHandler,
            ILogger<AzureQueueMessageHostService> logger)
        {
            _queueMessageHandler = queueMessageHandler;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Azure Queue Message service started");

            await _queueMessageHandler.HandleAsync(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Azure Queue Message service stopping");
            return base.StopAsync(cancellationToken);
        }
    }
}