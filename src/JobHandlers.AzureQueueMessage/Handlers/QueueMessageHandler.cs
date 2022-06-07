using Azure.Storage.Queues.Models;
using JobHandlers.AzureQueueMessage.Services;
using JobHandlers.AzureQueueMessage.Services.Deleters;
using JobHandlers.AzureQueueMessage.Services.Retrievers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobHandlers.AzureQueueMessage.Handlers
{
    internal class QueueMessageHandler
    {
        private readonly IQueueMessageRetriever _queueMessageRetriever;
        private readonly IQueueMessageExecutor _executor;
        private readonly IQueueMessageDeleter _queueMessageDeleter;
        private readonly IQueueMessageExceptionHandler _queueMessageExceptionHandler;
        private readonly ILogger<QueueMessageHandler> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public QueueMessageHandler(IQueueMessageRetriever queueMessageRetriever,
            IQueueMessageExecutor executor,
            IQueueMessageDeleter queueMessageDeleter,
            IQueueMessageExceptionHandler queueMessageExceptionHandler,
            ILogger<QueueMessageHandler> logger,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _queueMessageRetriever = queueMessageRetriever;
            _executor = executor;
            _queueMessageDeleter = queueMessageDeleter;
            _queueMessageExceptionHandler = queueMessageExceptionHandler;
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task HandleAsync(CancellationToken stoppingToken)
        {
            try
            {
                var message = await _queueMessageRetriever.Retrieve(stoppingToken);
                if (message == null)
                {
                    _logger.LogInformation("No message detected on the queue");
                    return;
                }

                await ExecuteAsync(message, stoppingToken);
                await _queueMessageDeleter.Delete(message, CancellationToken.None);

                _logger.LogInformation("Finished executing message with id: '{MessageId}'", message.MessageId);
            }
            finally
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    _hostApplicationLifetime.StopApplication();
                }
            }
        }

        private async Task ExecuteAsync(QueueMessage message, CancellationToken stoppingToken)
        {
            try
            {
                await _executor.Execute(message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing message '{MessageId}' failed", message.MessageId);
                await _queueMessageExceptionHandler.Handle(message, stoppingToken);
                throw;
            }
        }
    }
}
