using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dequeueable.AzureQueueStorage.Services.Hosts
{
    internal sealed class QueueListenerHost : BackgroundService
    {
        private readonly IHostExecutor _hostHandler;
        private readonly ILogger<QueueListenerHost> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public QueueListenerHost(IHostExecutor hostHandler, ILogger<QueueListenerHost> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostHandler = hostHandler;
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _hostHandler.HandleAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _hostApplicationLifetime.StopApplication();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception occurred, shutting down the host");
                    _hostApplicationLifetime.StopApplication();
                }
            }
        }
    }
}
